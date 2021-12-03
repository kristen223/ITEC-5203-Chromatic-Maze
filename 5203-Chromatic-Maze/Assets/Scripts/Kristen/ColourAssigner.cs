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

    //Lists of current rule types, their indexes, and amloutn fo times each rules is used (list indexes will match up)
    private static List<int> identifiers; //the indexes
    private static List<Type> ruleTypes;
    //private static int[] used;

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

        Test();
    }


    private void Test()
    {
        MovementRule TmoveS = new MovementRule();
        TmoveS.index = 0;
        TmoveS.direction = Direction.South;
        TmoveS.distance = 1;
        TmoveS.src = Colour.Green;
        TmoveS.target = Colour.All;
        TmoveS.type = Type.Tmove;

        MovementRule teleportB = new MovementRule();
        teleportB.index = 3;
        teleportB.direction = Direction.All;
        teleportB.distance = -1;
        teleportB.src = Colour.Red;
        teleportB.target = Colour.Blue;
        teleportB.type = Type.teleport;

        MovementRule jumpOne = new MovementRule();
        jumpOne.index = 9;
        jumpOne.direction = Direction.All;
        jumpOne.distance = 2;
        jumpOne.src = Colour.Orange;
        jumpOne.target = Colour.All;
        jumpOne.type = Type.jump1;

        MovementRule blank = new MovementRule();
        blank.index = 1;
        blank.direction = Direction.All;
        blank.distance = 1;
        blank.src = Colour.Yellow;
        blank.target = Colour.All;
        blank.type = Type.blank;

        MovementRule coldTemp = new MovementRule();
        coldTemp.index = 12;
        coldTemp.direction = Direction.All;
        coldTemp.distance = 1;
        coldTemp.src = Colour.Purple;
        coldTemp.target = Colour.Cool;
        coldTemp.type = Type.cool;

        ColourRule excludeBR = new ColourRule();
        excludeBR.index = 24;
        excludeBR.src = Colour.Blue;
        excludeBR.target = Colour.Yellow;
        excludeBR.type = Type.exclude;


        List<MovementRule> m = new List<MovementRule>()
        {
            {TmoveS},
            {teleportB},
            {jumpOne},
            {blank},
            {coldTemp},
        };
        List<ColourRule> c = new List<ColourRule>()
        {
            {excludeBR }
        };

        SetRules(m, c);
    }

    //not in start because other script needs to finish first
    public static void SetRules(List<MovementRule> mr, List<ColourRule> cr)
    {
        Debug.Log(cr[0]);
        Debug.Log("mr:  " + mr.Count + "   cr: " + cr.Count);
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
            else if (rule.type == Type.checkPathExc) //check path exclude
            {
                excludeRules.Add(cRules.IndexOf(rule));
            }
        }

        foreach (MovementRule rule in mRules)
        {
            identifiers.Add(rule.index);
            ruleTypes.Add(rule.type);
        }

        Debug.Log(identifiers.Count);

        //used = new int[identifiers.Count];
        //for (int i = 0; i < used.Length; i++)
        //{
        //    used[i] = 0;
        //}


        RoundOne(); //Tmove and blank rules will be removed from mRules after this point
        RoundTwo();
    }

    private static void AssignByMRule(Tile t, MovementRule rule)
    {
        t.assigned = true;
        t.mRule = rule;
        t.ruleType = rule.type;
        t.moveRule = true;
        t.colour = rule.src;
        t.index = rule.index;
        Debug.Log(t.name + " index " + rule.index);

        int index = identifiers.IndexOf(rule.index);
        //used[index]++;

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

        if (rule.type == Type.warm) //|| r.type == Type.teleport) *******I'm not setting the parnet/children of Teleport tiles meaning if one is placed anywhere but a deadend, it'll alter the paths
        {
            t.parent.canBe[Colour.Green] = false;
            t.parent.canBe[Colour.Blue] = false;
            t.parent.canBe[Colour.Purple] = false;
            foreach (Tile c in t.children)
            {
                c.canBe[Colour.Green] = false;
                c.canBe[Colour.Blue] = false;
                c.canBe[Colour.Purple] = false;
            }


            //Alternative - set the parent/child rules isntead of just the bools

            //if ((t.parent.canBeRed || t.parent.canBeOrange || t.parent.canBeYellow) && t.parent.assigned == false)
            //{
            //    AssignByColour(t.parent, rule.target);
            //} 
            //foreach (Tile c in t.children)
            //{
            //    if ((t.parent.canBeRed || t.parent.canBeOrange || t.parent.canBeYellow) && t.parent.assigned == false)
            //    {
            //        AssignByColour(c, rule.target);
            //    }
            //}
        }
        else if(rule.type == Type.cool)
        {
            t.parent.canBe[Colour.Red] = false;
            t.parent.canBe[Colour.Orange] = false;
            t.parent.canBe[Colour.Yellow] = false;
            foreach (Tile c in t.children)
            {
                c.canBe[Colour.Red] = false;
                c.canBe[Colour.Orange] = false;
                c.canBe[Colour.Yellow] = false;
            }

            //if ((t.parent.canBeBlue || t.parent.canBeGreen|| t.parent.canBePurple) && t.parent.assigned == false)
            //{
            //    AssignByColour(t.parent, rule.target);
            //}
            //foreach (Tile c in t.children)
            //{
            //    if ((t.parent.canBeBlue || t.parent.canBeGreen|| t.parent.canBePurple) && t.parent.assigned == false)
            //    {
            //        AssignByColour(c, rule.target);
            //    }
            //}
        }

        /*if (rule.type == Type.teleport)
        {
            Colour targ = rule.target;
            String s = "canBe" + targ;
            if ((bool)t.parent.GetType().GetField(s).GetValue(t.parent) && t.parent.assigned == false) //attempt to set parent to target colour
            {
                AssignByColour(t.parent, rule.target); //worse fitness if the above if statement isn't true
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
                if ((bool)child.GetType().GetField(s).GetValue(child) && child.assigned == false)
                {
                    AssignByColour(child, rule.target);
                }
                else
                {
                    //update something for fitness function but not as much as the above
                }
            }
            return;
        }*/
    }

    private static void AssignByCRule(Tile t, ColourRule rule)
    {
        t.assigned = true;
        t.cRule = rule;
        t.ruleType = rule.type;
        t.moveRule = true;
        t.colour = rule.src;
        t.index = rule.index;

        Debug.Log(t.name + " index " + rule.index);

        int index = identifiers.IndexOf(rule.index);
        //used[index]++;

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

        if (rule.type == Type.include) //may not need these after updating Round 2 method
        {
            if(t.parent.canBe[rule.target] == true && t.parent.assigned == false)
            {
                AssignByColour(t.parent, rule.target);
            }
            else //tile cannot go to parent - very bad
            {
                //I don't think this will occur here (because I'll detect this earlier)

                //increment something here for the fitness function
                //the more this happens the worse it is
                //maze may be traversable depending on cycles
                //not being bal eot assign the paretn is much worse than children because children don't affect reaching the exit
            }
            foreach (Tile c in t.children)
            {
                if (c.canBe[rule.target] == true && c.assigned == false)
                {
                    AssignByColour(c, rule.target);
                }
                else //tile cannot go to this child
                {
                    //update something for fitness function but not as much as the above
                    
                }
            }
        }
        else if (rule.type == Type.exclude || rule.type == Type.block)
        {
            t.parent.canBe[rule.target] = false;
            foreach (Tile c in t.children)
            {
                c.canBe[rule.target] = false;
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

        int rand = UnityEngine.Random.Range(0, indexes.Count); //Used to get index of a random rule that has c as its source colour
        bool assigned = false;

        foreach (MovementRule rule in mRules) //see if random rule is a mRule
        {
            if (rule.index == indexes[rand])
            {
                AssignByMRule(t, rule); //assign rule to t
                assigned = true;
            }
        }

        if(assigned == false) //if random rule wasn't an mRule, find the cRule
        {
            foreach (ColourRule rule in cRules)
            {
                if (rule.index == rand)
                {
                    AssignByCRule(t, rule); //assign rule to t
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
            if (rule.type == Type.blank)
            {
                foreach (Tile t in maze.tiles)
                {
                    if (t.children.Count + 1 == 4) //Cross section piece
                    {
                        Debug.Log("cross");
                        AssignByMRule(t, rule); //this includes the exit where its parent is itself
                    }
                }
            }
            if (rule.type == Type.Tmove)
            {
                foreach (Tile t in maze.tiles)
                {
                    if (t.children.Count + 1 == 3 && t.parent != t) //T piece
                    {
                        bool north = false;
                        bool south = false;
                        bool east = false;
                        bool west = false;
                        int tile = Array.IndexOf(maze.tiles, t);
                        int adjOne = Array.IndexOf(maze.tiles, t.children[0]);
                        int adjTwo = Array.IndexOf(maze.tiles, t.children[1]);
                        int adjThree = Array.IndexOf(maze.tiles, t.parent);
   
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


        for(int i = 0; i < mRules.Count; i++)
        {
            if (mRules[i].type == Type.Tmove || mRules[i].type == Type.blank)
            {
                mRules.Remove(mRules[i]);
                i--; 
            }
        }
        for (int i = 0; i < ruleTypes.Count; i++)
        {
            if (ruleTypes[i] == Type.Tmove || ruleTypes[i] == Type.blank)
            {
                ruleTypes.Remove(ruleTypes[i]);
                identifiers.Remove(identifiers[i]);
                i--;
            }
        }
    }

    /* Round 2
     * Traverse the maze from entrance to exit, colouring the solution path
     * Going backwards on the solution path isn't gaurenteed to help ensure successful coloru assignment 
     * 
     * Go from entrace to exit following up the parents. For each child go down their child list until you hit a dead end, you you pass through cycle back to SP
     */
    private static void RoundTwo()
    {
        Tile tile = maze.LP.entrance;
        Tile lastTile = maze.LP.exit; //this will be set to the previously visited tile

        while (tile.parent != tile) //not root node
        {
            if (tile.assigned == false)
            {
                bool check = false;

                List<Tile> adjacent = new List<Tile>(); //list of all assigned children and parent except pevious tile

                if (tile.parent.assigned == true && tile.parent != lastTile)
                {
                    adjacent.Add(tile.parent);
                }

                foreach (Tile c in tile.children)
                {
                    if (c.assigned == true && c != lastTile)
                    {
                        adjacent.Add(c);
                    }
                }

                if (adjacent.Count > 0) //at least one adjacent tile is assigned already
                {
                    check = AdjacentAssigned(tile, adjacent, check);
                }
                else //parent and children unassigned, pick a random rule besides Tmove and Blank
                {
                    List<int> randIndex = new List<int>(); //list of list indexes of possible rules

                    for (int i = 0; i < identifiers.Count; i++)
                    {
                        randIndex.Add(i); //add indexes 0 to unchecked rule count - 1
                    }

                    while (!check && randIndex.Count > 0)
                    {
                        int randListItem = UnityEngine.Random.Range(0, randIndex.Count); //get a random index
                        int rand = randIndex[randListItem];
                        randIndex.RemoveAt(randListItem); //remove this index so rule does not get checked again

                        if (identifiers[rand] <= 14) //MovementRule
                        {
                            MovementRule r = Rules.GetMRule(identifiers[rand]); //assign current tile (DOESN'T GET A RULE, GETS A TMOVE OLD RULE WITH WRONG SOURCE COLOUR??)

                            if (tile.canBe[r.src] == true)
                            {
                                check = true;
                                AssignByMRule(tile, r);
                            }

                        }
                        else
                        {
                            ColourRule r = Rules.GetCRule(identifiers[rand]); //assign current tile

                            if (tile.canBe[r.src] == true)
                            {
                                check = true;
                                AssignByCRule(tile, r);
                            }
                        }
                    }
                }

                if(!check)
                {
                    tile.failedToAssign = true;
                    Debug.Log("Failed to assigned " + tile.name);
                    //no colour could be assigned to this tile
                    //currently not doing anything when this happens

                    //this will happen if a tile is next to a warm and cool tile (all colours bools will be set to false)
                }
            }

            foreach(Tile c in tile.children)
            {
                if (c != lastTile)
                {
                    ParentToChild(c);
                }
                
            }

            lastTile = tile;
            tile = tile.parent;
        }
    }

    /* Unlike the solution path, the rest of the maze must be traversable both ways
     * I had to split into two because we can assign from root to leafs
     * 
     * This traverses down children until tile of rank 0 is becomes previosu tile
     */
    private static void ParentToChild(Tile tile)
    {

        //player must be able to traverse to parent and every child of tile

        if(tile.assigned == false)
        {
            bool check = false;

            List<Tile> adjacent = new List<Tile>(); //list of all assigned children and parent

            if (tile.parent.assigned == true)
            {
                adjacent.Add(tile.parent);
            }

            foreach (Tile c in tile.children)
            {
                if (c.assigned == true)
                {
                    adjacent.Add(c);
                }
            }


            if (adjacent.Count > 0)
            {
                check = AdjacentAssigned(tile, adjacent, check);
            }
            else //This will only happen if previosu tile couldn't be assigned
            {
                List<int> randIndex = new List<int>(); //list of list indexes of possible rules

                for (int i = 0; i < identifiers.Count; i++)
                {
                    randIndex.Add(i); //add indexes 0 to unchecked rule count - 1
                }

                while (!check && randIndex.Count > 0)
                {
                    int randListItem = UnityEngine.Random.Range(0, randIndex.Count); //get a random index
                    int rand = randIndex[randListItem];
                    randIndex.RemoveAt(randListItem); //remove this index so rule does not get checked again

                    if (identifiers[rand] <= 14) //MovementRule
                    {
                        MovementRule r = Rules.GetMRule(identifiers[rand]); //assign current tile

                        if (tile.canBe[r.src] == true)
                        {
                            check = true;
                            AssignByMRule(tile, r);
                        }

                    }
                    else
                    {
                        ColourRule r = Rules.GetCRule(identifiers[rand]); //assign current tile

                        if (tile.canBe[r.src] == true)
                        {
                            check = true;
                            AssignByCRule(tile, r);
                        }
                    }
                }  
            }
            if (!check)
            {
                tile.failedToAssign = true;
                Debug.Log("Failed to assigned " + tile.name);
                //no colour could be assigned to this tile
                //currently not doing anything when this happens
            }
        }

       
        if (tile.rank != 0)
        {
            foreach (Tile c in tile.children)
            {
                if (c.assigned == false) //condition so you don't inifnetly go between two cycled tiles
                {
                    ParentToChild(c); //go down each child until you reach a dead end or cycle
                }
            }
        }

    }

    /* If parent and or child is already assigned:
    *  - try the warm.cool rules
    *  - if thsoe don't exist, place teleport or jump rule
    *  - if those don't exist, try block and exclude
    *  - if none of that is possible try include
    *  - if none of the above worked, no rules work
    */
    private static bool AdjacentAssigned(Tile tile, List<Tile> adjacent, bool check)
    {
        foreach (MovementRule m in mRules) //first try to assign a warm or cool rule
        {
            if (m.type == Type.warm)
            {
                bool warm = true;
                foreach(Tile a in adjacent)
                {
                    if(a.colour != Colour.Red && a.colour != Colour.Orange && a.colour != Colour.Yellow)
                    {
                        warm = false;
                        break;
                    }
                }

                if(warm == true && tile.canBe[m.src] == true) //all adjacent tiles are a warm colour
                {
                    check = true;
                    AssignByMRule(tile, m);
                }
            }
            else if (m.type == Type.cool)
            {
                bool cool = true;
                foreach (Tile a in adjacent)
                {
                    if (a.colour != Colour.Blue && a.colour != Colour.Green && a.colour != Colour.Purple)
                    {
                        cool = false;
                        break;
                    }
                }

                if (cool == true && tile.canBe[m.src] == true)
                {
                    check = true;
                    AssignByMRule(tile, m);
                }
            }
        }
        if (!check) //then try to assign a jump or teleport rule
        {
            foreach (MovementRule m in mRules)
            {
                if (m.type == Type.jump1 || m.type == Type.jump2 || m.type == Type.teleport)
                {
                    if (tile.canBe[m.src] == true)
                    {
                        check = true;
                        AssignByMRule(tile, m);
                        break;
                    }
                }
            }
        }
        if (!check) //then try to assign exclude or block
        {
            foreach (ColourRule c in cRules)
            {

                if (c.type == Type.exclude || c.type == Type.block)
                {

                    bool canPlace = true;
                    foreach (Tile a in adjacent)
                    {
                        if(a.colour == c.target)
                        {
                            canPlace = false;
                            break;
                        }
                    }

                    if(canPlace == true && tile.canBe[c.src] == true)
                    {
                        check = true;
                        AssignByCRule(tile, c);
                        break;
                    }
                }
            }
        }
        if (!check) //Last, try to assign an include rule
        {
            foreach (ColourRule c in cRules)
            {
                if (c.type == Type.include) // && tile.parent.colour == c.target)
                {
                    bool canPlace = true;
                    foreach (Tile a in adjacent)
                    {
                        if (a.colour != c.target)
                        {
                            canPlace = false;
                            break;
                        }
                    }

                    if (canPlace == true && tile.canBe[c.src] == true)
                    {
                        check = true;
                        AssignByCRule(tile, c);
                        break;
                    }
                }
            }
        }

        return check;

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

