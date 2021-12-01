using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColourAssigner : MonoBehaviour
{
    /*for each tile in the grid
     *      check which directions you can move (N S E W) within 1, 2 and 3 space 
     *      Based on the above, get list of possible rules to place down
     *      pick a random rule, except some rules will have a higher likelihood of being selected (generally and based on previously placed rules)
     *      if includeC rule exits, you can place down colour c2 anywhere if the rule colour has already been placed
     *      
     *      if its a dead end, either place a teleport colour or anythign else (doesn't matter)
     *          If teleport, tile doesn't count as a dead-end and won't cause a game over if no more undo's left
     *          
     *      everytime a colour is placed, check to see if any false inclusion.exclusion rule booleans need to be updated
     *      
     *      We only need to make sure they can step forward (including looped areas) not backwards
     */


    /* We could traverse along solution path first, placing colours, then go from each dead-end to the solution path, then from each dead-end-turned-cycle to the solution path
     * Or could traverse from root node to every child (until child rank == 0) < extra condition needed because of cycles
     */

    // When you place a colour, set the rule type, moveRule bool, colour int, and rule of the tile
    // When placing jump colour, check if that tile's jump bools are true

    private static KruskalMaze.Maze maze;
    public static List<MovementRule> mRules; //movement rules
    public static List<ColourRule> cRules; //colour rules
    //private static List<KeyValuePair<int, Type>> ruleTypes; //each rule index and its type

    //Lists of current rule types and their indexes (list indexes will match up)
    private static List<int> identifiers; //the indexes
    private static List<Type> ruleTypes;

    public static List<int> includeRules; //list of indexes of CheckPathInc rules of cRules list
    public static List<int> excludeRules; //list of indexes of CheckPathExc rules of cRules list
    private static List<Material> colours;

    // Start is called before the first frame update
    void Start()
    {        
        maze = GenerateGrid.maze;
        cRules = new List<ColourRule>();
        mRules = new List<MovementRule>();
        identifiers = new List<int>();
        ruleTypes = new List<Type>();

        colours = new List<Material>
        {
            (Material)Resources.Load("Materials/Red"),
            (Material)Resources.Load("Materials/Orange"),
            (Material)Resources.Load("Materials/Yellow"),
            (Material)Resources.Load("Materials/Green"),
            (Material)Resources.Load("Materials/Blue"),
            (Material)Resources.Load("Materials/Purple")
        };
    }

    //not in start because other script needs to finish first
    public static void SetRules(List<MovementRule> mr, List<ColourRule> cr)
    {
        mRules = mr;
        cRules = cr;

        //Get list of CheckPath rule indexes
        foreach (ColourRule rule in cRules)
        {
            identifiers.Add(rule.index);
            ruleTypes.Add(rule.type);
            if (rule.type == Type.checkPathInc) //check path include
            {
                includeRules.Add(cRules.IndexOf(rule));
            }
            else if(rule.type == Type.checkPathExc) //check path exclude
            {
                excludeRules.Add(cRules.IndexOf(rule));
            }
        }

        foreach(MovementRule rule in mRules)
        {
            identifiers.Add(rule.index);
            ruleTypes.Add(rule.type);
        }

        //AssignRulestoColours(mRules, cRules); //Every rule now has an assigned colour (src)
        RoundOne();
        RoundTwo();
        RoundThree();
    }

    /* Priority of rule assignments:
     * 1. include and exclude
     * 2. warm and cool
     * 3. rest
     * This is written under the assumption one colour can have multiple rules
     */
    //private static void AssignRulestoColours(List<MovementRule> mList, List<ColourRule> cList)
    //{
    //    List<Colour> warmColours = new List<Colour>();
    //    List<Colour> coolColours = new List<Colour>();
    //    warmColours.Add(Colour.Red);
    //    warmColours.Add(Colour.Orange);
    //    warmColours.Add(Colour.Yellow);
    //    coolColours.Add(Colour.Green);
    //    coolColours.Add(Colour.Blue);
    //    coolColours.Add(Colour.Purple);

    //    foreach (ColourRule c in cList)
    //    {
    //        if (c.type == Type.include || c.type == Type.exclude)
    //        {
    //            if (warmColours.Contains(c.src))
    //            {
    //                warmColours.Remove(c.src); //will always exist
    //            }
    //            else
    //            {
    //                coolColours.Remove(c.src);
    //            }
    //        }
    //    }
    //    for (int i = 0; i < cList.Count; i++)
    //    {
    //        ColourRule c = cList[i];

    //        if (c.type == Type.warm) //assign the source colour to a warm colour still in the list (if any)
    //        {
    //            if (warmColours.Count > 0)
    //            {
    //                c.src = warmColours[0];
    //                warmColours.RemoveAt(0);
    //                cList[i] = c;
    //            }
    //            else
    //            {
    //                List<Colour> extra = new List<Colour>();
    //                extra.Add(Colour.Red);
    //                extra.Add(Colour.Orange);
    //                extra.Add(Colour.Yellow);
    //                c.src = extra[UnityEngine.Random.Range(0, 3)]; //asign random warm colour
    //                cList[i] = c;
    //            }
    //        }
    //        if (c.type == Type.cool) //assign the source colour to a warm colour still in the list (if any)
    //        {
    //            if (coolColours.Count > 0)
    //            {
    //                c.src = coolColours[0];
    //                coolColours.RemoveAt(0);
    //                cList[i] = c;
    //            }
    //            else
    //            {
    //                List<Colour> extra = new List<Colour>();
    //                extra.Add(Colour.Green);
    //                extra.Add(Colour.Blue);
    //                extra.Add(Colour.Purple);
    //                c.src = extra[UnityEngine.Random.Range(0, 3)]; //asign random warm colour
    //                cList[i] = c;
    //            }
    //        }
    //    }

    //    for (int i = 0; i < mList.Count; i++)
    //    {
    //        MovementRule m = mList[i];
    //        if (coolColours.Count > 0)
    //        {
    //            m.src = coolColours[0];
    //            coolColours.RemoveAt(0);
    //            mList[i] = m;
    //        }
    //        else if (warmColours.Count > 0)
    //        {
    //            m.src = warmColours[0];
    //            warmColours.RemoveAt(0);
    //            mList[i] = m;
    //        }
    //        else //all colours used at least once
    //        {
    //            List<Colour> extra = new List<Colour>();
    //            extra.Add(Colour.Red);
    //            extra.Add(Colour.Orange);
    //            extra.Add(Colour.Yellow);
    //            extra.Add(Colour.Green);
    //            extra.Add(Colour.Blue);
    //            extra.Add(Colour.Purple);
    //            m.src = extra[UnityEngine.Random.Range(0, 6)]; //assign random colour
    //            mList[i] = m;
    //        }
    //    }

    //    for (int i = 0; i < cList.Count; i++)
    //    {
    //        ColourRule c = cList[i];
    //        if (c.type != Type.include && c.type != Type.exclude && c.type != Type.cool && c.type != Type.warm)
    //        {
    //            if (coolColours.Count > 0)
    //            {
    //                c.src = coolColours[0];
    //                coolColours.RemoveAt(0);
    //                cList[i] = c;
    //            }
    //            else if (warmColours.Count > 0)
    //            {
    //                c.src = warmColours[0];
    //                warmColours.RemoveAt(0);
    //                cList[i] = c;
    //            }
    //            else //all colours used at least once
    //            {
    //                List<Colour> extra = new List<Colour>
    //                {
    //                    Colour.Red,
    //                    Colour.Orange,
    //                    Colour.Yellow,
    //                    Colour.Green,
    //                    Colour.Blue,
    //                    Colour.Purple
    //                };
    //                c.src = extra[UnityEngine.Random.Range(0, 6)];
    //                cList[i] = c;
    //            }
    //        }
    //    }

    //    RoundOne();
    //    RoundTwo();
    //}

    private static void AssignByMRule(Tile t, MovementRule rule)
    {
        t.assigned = true;
        t.mRule = rule;
        t.ruleType = rule.type;
        t.moveRule = true;
        t.colour = rule.src;

        SpriteRenderer sr = t.GetComponent<SpriteRenderer>();

        foreach(Material mat in colours)
        {
            if(mat.name == t.colour.ToString())
            {
                sr.material.shader = mat.shader;
                sr.material.color = mat.color;
                break;
            }
        }
    }

    private static void AssignByCRule(Tile t, ColourRule rule)
    {
        t.assigned = true;
        t.cRule = rule;
        t.ruleType = rule.type;
        t.moveRule = true;
        t.colour = rule.src;

        SpriteRenderer sr = t.GetComponent<SpriteRenderer>();

        foreach (Material mat in colours)
        {
            if (mat.name == t.colour.ToString())
            {
                sr.material.shader = mat.shader;
                sr.material.color = mat.color;
                break;
            }
        }
    }

    //Used to set parents/children of a tile, when the tile's target colour is predetermined (colour rules)
    private static void AssignByColour(Tile t, Colour c)
    {

        //***You cannot assign a parent/child a Tmove or blank rule. If that's the option, the given maze + set fo rules doesn't work (or you have to restart)
        //These rules need to have specific child/parent relationships which you can't gaurantee

        List<int> indexes = new List<int>(); //list of rule indexes with source colour c


        if(c == Colour.Warm)
        {
            foreach (MovementRule rule in mRules)
            {
                if ((rule.src == Colour.Red || rule.src == Colour.Orange || rule.src == Colour.Yellow) && rule.type != Type.blank && rule.type != Type.Tmove)
                {
                    indexes.Add(rule.index);
                }
            }
            foreach (ColourRule rule in cRules)
            {
                if ((rule.src == Colour.Red || rule.src == Colour.Orange || rule.src == Colour.Yellow) && rule.type != Type.blank && rule.type != Type.Tmove)
                {
                    indexes.Add(rule.index);
                }
            }
        }
        else if(c == Colour.Cool)
        {
            foreach (MovementRule rule in mRules)
            {
                if ((rule.src == Colour.Blue || rule.src == Colour.Green || rule.src == Colour.Purple) && rule.type != Type.blank && rule.type != Type.Tmove)
                {
                    indexes.Add(rule.index);
                }
            }
            foreach (ColourRule rule in cRules)
            {
                if ((rule.src == Colour.Blue || rule.src == Colour.Green || rule.src == Colour.Purple) && rule.type != Type.blank && rule.type != Type.Tmove)
                {
                    indexes.Add(rule.index);
                }
            }
        }
        else //c is a specific colour
        {
            foreach (MovementRule rule in mRules)
            {
                if (rule.src == c && rule.type != Type.blank && rule.type != Type.Tmove)
                {
                    indexes.Add(rule.index);
                }
            }
            foreach (ColourRule rule in cRules)
            {
                if (rule.src == c && rule.type != Type.blank && rule.type != Type.Tmove)
                {
                    indexes.Add(rule.index);
                }
            }
        }


        //Gets a random rule with colour c set as its source and set that rule to t
        //DON'T GET A RANDOM, based on the index, try to pick some rules types over others (avoid teleport and jumps, and ones that specify a target colour)

        int rand = UnityEngine.Random.Range(0, indexes.Count);
        bool assigned = false;

        foreach (MovementRule rule in mRules) //see if random rule is a mRule
        {
            if (rule.index == rand)
            {
                AssignByMRule(t, rule); //assign rule to t
                assigned = true;

                //check if rule specifies a target colour. If it does, set tile's parent and children to that colour and recurse until done
                //Teleport may be unavoidable to place down, inthi scase it's the same as include
                if (rule.type == Type.teleport) 
                {
                    Colour targ = rule.target;
                    String s = "canBe" + targ;
                    if ((bool)t.parent.GetType().GetField(s).GetValue(t.parent) == true) //attempt to set parent to target colour
                    {
                        //if there's more than one rule for this colour, choose the rule that isnt' a Tmove, Blank, or specifies target
                        //if target is a blank or Tmove and parent doesn't fit with it, nothing will work and this part of the maze become untraversable

                        if (t.parent.assigned == false) //worse fitness if this and the above if statement aren't true
                        {
                            AssignByColour(t.parent, rule.target);
                        }
                    }
                    else
                    {
                        //increment something here for the fitness function
                        //the more this happens the worse it is
                        //maze may be traversable depending on cycles
                        //not being bal eot assign the paretn is much worse than children because children don't affect reaching the exit
                    }
                    foreach (Tile child in t.children) //attempt to set children to target colour
                    {
                        if ((bool)child.GetType().GetField(s).GetValue(child) == true)
                        {
                            if (child.assigned == false)
                            {
                                AssignByColour(child, rule.target);
                            }
                        }
                        else
                        {
                            //update something for fitness function but not as much as the above
                        }
                    }
                    return;
                }
                else if(rule.type == Type.warm)
                {
                    t.parent.canBeGreen = false;
                    t.parent.canBeBlue = false;
                    t.parent.canBePurple = false;

                    foreach (Tile child in t.children)
                    {
                        child.canBeGreen = false;
                        child.canBeBlue = false;
                        child.canBePurple = false;
                    }
                }
                else if(rule.type == Type.cool)
                {
                    t.parent.canBeRed = false;
                    t.parent.canBeOrange = false;
                    t.parent.canBeYellow = false;

                    foreach (Tile child in t.children)
                    {
                        child.canBeRed = false;
                        child.canBeOrange = false;
                        child.canBeYellow = false;
                    }
                }
            }
        }

        if(assigned == false) //if random rule wasn't an mRule, find the cRule
        {
            foreach (ColourRule rule in cRules)
            {
                if (rule.index == rand)
                {
                    AssignByCRule(t, rule); //assign rule to t

                    if (rule.type == Type.include)
                    {
                        Colour targ = rule.target;
                        String s = "canBe" + targ;
                        if ((bool)t.parent.GetType().GetField(s).GetValue(t.parent) == true) //attempt to set parent to target colour
                        {
                            if(t.parent.assigned == false)
                            {
                                AssignByColour(t.parent, rule.target);
                            }
                        }
                        else
                        {
                            //increment something here for the fitness function
                            //the more this happens the worse it is
                            //maze may be traversable depending on cycles
                            //not being bal eot assign the paretn is much worse than children because children don't affect reaching the exit
                        }
                        foreach (Tile child in t.children) //attempt to set children to target colour
                        {
                            if ((bool)child.GetType().GetField(s).GetValue(child) == true)
                            {
                                if (child.assigned == false)
                                {
                                    AssignByColour(child, rule.target);
                                }
                            }
                            else
                            {
                                //update something for fitness function but not as much as the above
                            }
                        }
                        return;
                    }
                    else if(rule.type == Type.exclude || rule.type == Type.block)  //if exclude and block you have to make sure to not put down a certain colour next
                    {
                        Colour targ = rule.target;
                        String s = "canBe" + targ;

                        t.parent.GetType().GetField(s).SetValue(t.parent, false);
                        foreach (Tile child in t.children)
                        {
                            child.GetType().GetField(s).SetValue(t.parent, false);
                        }
                    }
                }
            }
        }
    }

    /* Round 1
     *Place all Tmove and Blank rules
     */
    private static void RoundOne()
    {
        foreach(MovementRule rule in mRules)
        {
            if(rule.type == Type.blank)
            {
                foreach (Tile t in maze.tiles)
                {
                    if (t.children.Count + 1 == 4) //Cross section piece
                    {
                        AssignByMRule(t, rule);
                    }
                }
            }
            else if(rule.type == Type.Tmove)
            {
                foreach (Tile t in maze.tiles)
                {
                    if (t.children.Count + 1 == 3) //T piece
                    {
                        bool north = false;
                        bool south = false;
                        bool east = false;
                        bool west = false;
                        int tile = Array.IndexOf(maze.tiles, t);
                        int adjOne = Array.IndexOf(maze.tiles, t.children[0]);
                        int adjTwo = Array.IndexOf(maze.tiles, t.children[1]);
                        int adjThree;
                        if (t == maze.LP.exit) //if tile is root, has no parent
                        {
                            adjThree = Array.IndexOf(maze.tiles, t.children[2]);
                        }
                        else
                        {
                            adjThree = Array.IndexOf(maze.tiles, t.parent);
                        }
                        int[] neighbours = new int[3]{adjOne, adjTwo, adjThree};

                        for (int i = 0; i <3; i++)
                        {
                            if(neighbours[i] == tile + 1)
                            {
                                east = true;
                            }
                            else if (neighbours[i] == tile - 1)
                            {
                                west = true;
                            }
                            else if (neighbours[i] == tile + maze.w)
                            {
                                north = true;
                            }
                            else if (neighbours[i] == tile - maze.w)
                            {
                                south = true;
                            }
                        }

                        if(north == false)
                        {
                            if (rule.index == 13)
                            {
                                AssignByMRule(t, rule);
                            }
                        }
                        else if (south == false)
                        {
                            if (rule.index == 0)
                            {
                                AssignByMRule(t, rule);
                            }
                        }
                        else if (east == false)
                        {
                            if (rule.index == 15)
                            {
                                AssignByMRule(t, rule);
                            }
                        }
                        else if (west == false)
                        {
                            if (rule.index == 14)
                            {
                                AssignByMRule(t, rule);
                            }
                        }

                    }
                }
            }
        }
    }

    /* Round 2
     * Traverse the maze from entrance to exit, colouring the solution path
     * Will colour children of SP not on SP if necessary
     */
    private static void RoundTwo()
    {
        Tile child = maze.LP.entrance;
        int[] used = new int[identifiers.Count]; //number of times a rule was used (indexes of array line up with ruleTypes and identifiers lists)

        for(int i = 0; i < used.Length; i++)
        {
            used[i] = 0;
        }

        while (child.parent != child) //not root node
        {
            if (child.assigned == false)
            {
                int rand = UnityEngine.Random.Range(0, identifiers.Count); //random index

                if (identifiers[rand] <= 14) //MovementRule
                {
                    MovementRule r = Rules.GetMRule(identifiers[rand]); //assign current tile
                    AssignByMRule(child, r);
                    if (r.type == Type.warm || r.type == Type.cool) //assign children/parent if necessary
                    {
                        AssignByColour(child.parent, r.target);
                        foreach (Tile c in child.children)
                        {
                            if(c.assigned == false) //assign all children except previosu one on solution path
                            {
                                AssignByColour(c, r.target);
                            }
                        }
                    }
                }
                else
                {
                    ColourRule r = Rules.GetCRule(identifiers[rand]); //assign current tile
                    AssignByCRule(child, r);
                    if (r.type == Type.warm || r.type == Type.cool)
                    {
                        AssignByColour(child.parent, r.target);
                        foreach (Tile c in child.children)
                        {
                            if (c.assigned == false)
                            {
                                AssignByColour(c, r.target);
                            }
                        }
                    }
                }
                used[rand]++;
            }

            child = child.parent;
        }
    }

    /* Round 3
     * Traverse the maze from tiles of rank 0 to SP, colouring rest of maze
     * Must be checking if any parent or child is already assigned to assure one-way traversability 
     */
    private static void RoundThree()
    {

        //update kruskal so maze has a list of old dead ends now cycles to make one big list



        //Check if parent is assigned (only relevant in Round 3)
    //    if (child.parent.assigned == true) //set rule whose target colour is parent's source colour
    //    {
    //        bool assigned = false;
    //        if (child.parent.moveRule == true)
    //        {
    //            foreach (MovementRule rule in mRules)
    //            {
    //                if (rule.target == child.parent.mRule.src)
    //                {
    //                    AssignByMRule(child, rule);


    //                    //set the children by colour if they aren't already assigned (4 times)


    //                    assigned = true;
    //                    break;
    //                }
    //            }

    //            if (assigned == false)
    //            {
    //                foreach (ColourRule rule in cRules)
    //                {
    //                    if (rule.target == child.parent.mRule.src)
    //                    {
    //                        AssignByCRule(child, rule);
    //                        break;
    //                    }
    //                }
    //            }
    //        }
    //        else
    //        {
    //            foreach (MovementRule rule in mRules)
    //            {
    //                if (rule.target == child.parent.cRule.src)
    //                {
    //                    AssignByMRule(child, rule);
    //                    assigned = true;
    //                    break;
    //                }
    //            }
    //            if (assigned == false)
    //            {
    //                foreach (ColourRule rule in cRules)
    //                {
    //                    if (rule.target == child.parent.cRule.src)
    //                    {
    //                        AssignByCRule(child, rule);
    //                        break;
    //                    }
    //                }
    //            }
    //        }
    //    }
    }
}

//avoid rules with target = teleport rule source

/* Corridor or corner piece
 * Order of Priority:
 *  - Include
 *  - warm/cool
 *  - jumps (if you can stay on sp)
 *  - block/exclude
 */

/* T-piece (that don't work with active Tmove rules)
* Order of Priority:
*  - Exclude
*  - block
*  - warm/cool
*  - Include
*  - jumps
*/

