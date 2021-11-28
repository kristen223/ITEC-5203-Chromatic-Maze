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
    public static List<MovementRule> mRules; //movemnet rules
    public static List<ColourRule> cRules; //colour rules
    private static List<KeyValuePair<int, Type>> ruleTypes; //each rule index and its type

    public static List<int> includeRules; //list of indexes of CheckPathInc rules of cRules list
    public static List<int> excludeRules; //list of indexes of CheckPathExc rules of cRules list
    private static List<Material> colours;

    // Start is called before the first frame update
    void Start()
    {        
        maze = GenerateGrid.maze;
        cRules = new List<ColourRule>();
        mRules = new List<MovementRule>();
        ruleTypes = new List<KeyValuePair<int, Type>>();

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
            ruleTypes.Add(new KeyValuePair<int,Type>(rule.index, rule.type));
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
            ruleTypes.Add(new KeyValuePair<int, Type>(rule.index, rule.type));
        }

        AssignRulestoColours(mRules, cRules); //Every rule now has an assigned colour (src)
    }

    /* Priority of rule assignments:
     * 1. include and exclude
     * 2. warm and cool
     * 3. rest
     * This is written under the assumption one colour can have multiple rules
     */
    private static void AssignRulestoColours(List<MovementRule> mList, List<ColourRule> cList)
    {
        List<Colour> warmColours = new List<Colour>();
        List<Colour> coolColours = new List<Colour>();
        warmColours.Add(Colour.Red);
        warmColours.Add(Colour.Orange);
        warmColours.Add(Colour.Yellow);
        coolColours.Add(Colour.Green);
        coolColours.Add(Colour.Blue);
        coolColours.Add(Colour.Purple);

        foreach (ColourRule c in cList)
        {
            if (c.type == Type.include || c.type == Type.exclude)
            {
                if (warmColours.Contains(c.src))
                {
                    warmColours.Remove(c.src); //will always exist
                }
                else
                {
                    coolColours.Remove(c.src);
                }
            }
        }
        for (int i = 0; i < cList.Count; i++)
        {
            ColourRule c = cList[i];

            if (c.type == Type.warm) //assign the source colour to a warm colour still in the list (if any)
            {
                if (warmColours.Count > 0)
                {
                    c.src = warmColours[0];
                    warmColours.RemoveAt(0);
                    cList[i] = c;
                }
                else
                {
                    List<Colour> extra = new List<Colour>();
                    extra.Add(Colour.Red);
                    extra.Add(Colour.Orange);
                    extra.Add(Colour.Yellow);
                    c.src = extra[UnityEngine.Random.Range(0, 3)]; //asign random warm colour
                    cList[i] = c;
                }
            }
            if (c.type == Type.cool) //assign the source colour to a warm colour still in the list (if any)
            {
                if (coolColours.Count > 0)
                {
                    c.src = coolColours[0];
                    coolColours.RemoveAt(0);
                    cList[i] = c;
                }
                else
                {
                    List<Colour> extra = new List<Colour>();
                    extra.Add(Colour.Green);
                    extra.Add(Colour.Blue);
                    extra.Add(Colour.Purple);
                    c.src = extra[UnityEngine.Random.Range(0, 3)]; //asign random warm colour
                    cList[i] = c;
                }
            }
        }

        for (int i = 0; i < mList.Count; i++)
        {
            MovementRule m = mList[i];
            if (coolColours.Count > 0)
            {
                m.src = coolColours[0];
                coolColours.RemoveAt(0);
                mList[i] = m;
            }
            else if (warmColours.Count > 0)
            {
                m.src = warmColours[0];
                warmColours.RemoveAt(0);
                mList[i] = m;
            }
            else //all colours used at least once
            {
                List<Colour> extra = new List<Colour>();
                extra.Add(Colour.Red);
                extra.Add(Colour.Orange);
                extra.Add(Colour.Yellow);
                extra.Add(Colour.Green);
                extra.Add(Colour.Blue);
                extra.Add(Colour.Purple);
                m.src = extra[UnityEngine.Random.Range(0, 6)]; //assign random colour
                mList[i] = m;
            }
        }

        for (int i = 0; i < cList.Count; i++)
        {
            ColourRule c = cList[i];
            if (c.type != Type.include && c.type != Type.exclude && c.type != Type.cool && c.type != Type.warm)
            {
                if (coolColours.Count > 0)
                {
                    c.src = coolColours[0];
                    coolColours.RemoveAt(0);
                    cList[i] = c;
                }
                else if (warmColours.Count > 0)
                {
                    c.src = warmColours[0];
                    warmColours.RemoveAt(0);
                    cList[i] = c;
                }
                else //all colours used at least once
                {
                    List<Colour> extra = new List<Colour>();
                    extra.Add(Colour.Red);
                    extra.Add(Colour.Orange);
                    extra.Add(Colour.Yellow);
                    extra.Add(Colour.Green);
                    extra.Add(Colour.Blue);
                    extra.Add(Colour.Purple);
                    c.src = extra[UnityEngine.Random.Range(0, 6)];
                    cList[i] = c;
                }
            }
        }

        RoundOne();
        RoundTwo();
    }

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

    private static void AssignByColour(Tile t, Colour c)
    {


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
     * Traverse the maze from entrance to exit, only colouring the solution path
     */
    private static void RoundTwo()
    {
        Tile child = maze.LP.entrance;
        while (child.parent != child) //not root node
        {
            if(child.assigned == false)
            {
                /* Corridor or corner piece
                 * Order of Priority:
                 *  - Include
                 *  - warm/cool
                 *  - jumps (if you can stay on sp)
                 *  - block/exclude
                 */
                if (child.children.Count == 1) //number of directions they can traverse without back tracking
                {
                    //must check if parent is assigned
                    if(child.parent.assigned == true)
                    {
                        
                    }
                    else //extra steps
                    {

                        foreach(KeyValuePair<int, Type> type in ruleTypes)
                        {
                            if(type.Value == Type.include)
                            {
                                if(type.Key <= 14) //movement rule
                                {
                                    //AssignByMRule(child, );
                                    //AssignByColour(child.parent, );
                                }
                                else
                                {
                                    //AssignByCRule(child, );
                                    //AssignByColour(child.parent, );
                                }
                            }
                        }
                       
                    }

                }
                /* T-piece (that don't work with active Tmove rules)
                 * Order of Priority:
                 *  - Exclude
                 *  - block
                 *  - warm/cool
                 *  - Include
                 *  - jumps
                 */
                else if (child.children.Count == 2) //t piece
                {

                }
            }
            child = child.parent;
        }
    }
}
