using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ColourAssigner : MonoBehaviour
{
    public KruskalMaze.Maze maze;
    public List<MovementRule> RoundOneRules; //Tmove and blank rules only
    public List<MovementRule> mRules; //movement rules (Tmoves and blanks removed from this list part way through process)
    public List<ColourRule> cRules; //colour rules

    //Lists of current rule types, their indexes, and amloutn fo times each rules is used (list indexes will match up)
    private List<int> identifiers; //the indexes
    private List<Type> ruleTypes;
    public Dictionary<int, int> used; //rule index and amount of times used
    private int unassigned;
    private int pathUnassigned;
    private bool exitAssigned;

    public List<int> includeRules; //list of indexes of CheckPathInc rules of cRules list
    public List<int> excludeRules; //list of indexes of CheckPathExc rules of cRules list
    private static List<Material> colours;

    public struct ColouredMaze
    {
        public TraverseMaze.SolutionPaths spaths;
        public KruskalMaze.Maze maze;
        public Dictionary<int, int> used; //rule index and amount of times rule was used
        public int checkers; //number of checkers
        public bool properExit; //true if exit was assigned properly, false if exit was assigned to be traversable via solution path, but violates rule(s) with other adjacent tiles (not added to onPathViolations because it doesn't wreck the path)
        public int PathViolations; //number of walls

        public List<MovementRule> mr;
        public List<ColourRule> cr;
    }

    // Start is called before the first frame update
    void Awake()
    {
        colours = new List<Material>
        {
            (Material)Resources.Load("Materials/Red"),
            (Material)Resources.Load("Materials/Orange"),
            (Material)Resources.Load("Materials/Yellow"),
            (Material)Resources.Load("Materials/Green"),
            (Material)Resources.Load("Materials/Blue"),
            (Material)Resources.Load("Materials/Purple"),
            (Material)Resources.Load("Materials/Pink"),
            (Material)Resources.Load("Materials/Teal")
        };
    }


    public ColouredMaze ColourMaze()
    {
        //Other script needs to call SetRules first
        RoundOne(); //Tmove and blank rules will be removed from mRules after this point
        RoundTwo();
        //RoundThree();

        ColouredMaze cmaze = new ColouredMaze()
        {
            maze = maze,
            used = used,
            PathViolations = pathUnassigned + unassigned,
            properExit = exitAssigned
        };

        cmaze.mr = new List<MovementRule>();
        foreach(MovementRule m in RoundOneRules)
        {
            cmaze.mr.Add(m);
        }
        foreach (MovementRule m in mRules)
        {
            cmaze.mr.Add(m);
        }
        cmaze.cr = new List<ColourRule>();
        foreach (ColourRule c in cRules)
        {
            cmaze.cr.Add(c);
        }

        cmaze.spaths = new TraverseMaze.SolutionPaths();
        cmaze.spaths = GetComponent<TraverseMaze>().GetPathsFromEntrance(cmaze);

        if (cmaze.spaths.longest > 0) //if solution paths exist
        {
            //Shinro.PlaceCheckers(cmaze.spaths.shortestPath, cmaze, .5f);
            Shinro.PlaceCheckers(cmaze.spaths.mediumPath, cmaze, .3f);
            cmaze.checkers = Mathf.RoundToInt((cmaze.spaths.mediumPath.Count - 1) * .3f);
            NumClues.SetClues(maze.tiles);
        }
        return cmaze;
    }

    public MovementRule GetMRule(int index)
    {

        foreach(MovementRule rule in mRules)
        {
            if(rule.index == index)
            {
                return rule;
            }
        }

        Debug.Log("ERROR: RETURNED WRONG RULE");
        return mRules[0]; //this should never happen, not sure what to put here
    }

    public ColourRule GetCRule(int index)
    {
        foreach (ColourRule rule in cRules)
        {
            if (rule.index == index)
            {
                return rule;
            }
        }

        Debug.Log("ERROR: RETURNED WRONG RULE");
        return cRules[0]; //this should never happen
    }

    //not in start because other script needs to finish first
    public void SetRules(List<MovementRule> mr, List<ColourRule> cr)
    {
        //copy over the maze to create all new references
        maze = new KruskalMaze.Maze();
        maze.w = GenerateGrid.maze.w;
        maze.h = GenerateGrid.maze.h;
        maze.LP = GenerateGrid.maze.LP;
        maze.tree = GenerateGrid.maze.tree;

        maze.deadends = new List<Tile>();
        foreach (Tile t in GenerateGrid.maze.deadends)
        {
            maze.deadends.Add(t);
        }
        maze.rankZero = new List<Tile>();
        foreach (Tile t in GenerateGrid.maze.rankZero)
        {
            maze.rankZero.Add(t);
        }

        maze.tileList = new List<Tile>();
        foreach (Tile t in GenerateGrid.maze.tileList)
        {
            maze.tileList.Add(t);
        }

        maze.tiles = new Tile[GenerateGrid.maze.tiles.Length];
        for (int i = 0; i < maze.tiles.Length; i++)
        {
            maze.tiles[i] = GenerateGrid.maze.tiles[i];
        }

        cRules = new List<ColourRule>();
        mRules = new List<MovementRule>();
        RoundOneRules = new List<MovementRule>();
        identifiers = new List<int>();
        ruleTypes = new List<Type>();
        used = new Dictionary<int, int>();
        pathUnassigned = 0;
        unassigned = 0;
        exitAssigned = true;

        mRules = mr;
        cRules = cr;
        //Debug.Log(mr.Count);
        //Debug.Log(cr.Count);

        //Get list of CheckPath rule indexes
        foreach (ColourRule rule in cRules)
        {
            identifiers.Add(rule.index);
            ruleTypes.Add(rule.type);

            used.Add(rule.index, 0); //ERROR the same rule index is being added (same key value) ********************************************************
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
            used.Add(rule.index, 0);

            if(rule.type == Type.Tmove || rule.type == Type.blank)
            {
                RoundOneRules.Add(rule);
            }
        }
    }

    private void AssignByMRule(Tile t, MovementRule rule)
    {
        t.assigned = true;
        t.mRule = rule;
        t.ruleType = rule.type;
        t.moveRule = true;
        t.colour = rule.src;
        t.index = rule.index;
        used[rule.index]++;

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

        List<Tile> wallNeighbours = getAllWallTiles(t); //assigned and unassigned adjacent tiles on other side of wall

        if (rule.type == Type.warm) //|| r.type == Type.teleport) *******I'm not setting the parnet/children of Teleport tiles meaning if one is placed anywhere but a deadend, it'll alter the paths
        {
            t.parent.canBe[Colour.Green] = false;
            t.parent.canBe[Colour.Blue] = false;
            t.parent.canBe[Colour.Purple] = false;
            t.parent.canBe[Colour.Teal] = false;
            foreach (Tile c in t.children)
            {
                c.canBe[Colour.Green] = false;
                c.canBe[Colour.Blue] = false;
                c.canBe[Colour.Purple] = false;
                c.canBe[Colour.Teal] = false;
            }

            //Set the can be bools of tiles on other side of walls
            for (int i = 0; i < wallNeighbours.Count; i++)
            {
                wallNeighbours[i].canBe[Colour.Red] = false;
                wallNeighbours[i].canBe[Colour.Orange] = false;
                wallNeighbours[i].canBe[Colour.Yellow] = false;
                wallNeighbours[i].canBe[Colour.Pink] = false;
            }
        }
        else if (rule.type == Type.cool)
        {
            t.parent.canBe[Colour.Red] = false;
            t.parent.canBe[Colour.Orange] = false;
            t.parent.canBe[Colour.Yellow] = false;
            t.parent.canBe[Colour.Pink] = false;
            foreach (Tile c in t.children)
            {
                c.canBe[Colour.Red] = false;
                c.canBe[Colour.Orange] = false;
                c.canBe[Colour.Yellow] = false;
                t.parent.canBe[Colour.Pink] = false;
            }

            //Set the can be bools of tiles on other side of walls
            for (int i = 0; i < wallNeighbours.Count; i++)
            {
                wallNeighbours[i].canBe[Colour.Blue] = false;
                wallNeighbours[i].canBe[Colour.Green] = false;
                wallNeighbours[i].canBe[Colour.Purple] = false;
                wallNeighbours[i].canBe[Colour.Teal] = false;
            }
        }
    }

    private void AssignByCRule(Tile t, ColourRule rule)
    {
        t.assigned = true;
        t.cRule = rule;
        t.ruleType = rule.type;
        t.moveRule = false;
        t.colour = rule.src;
        t.index = rule.index;
        used[rule.index]++;

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

        List<Tile> wallNeighbours = getAllWallTiles(t); //assigned and unassigned adjacent tiles on other side of wall

        if (rule.type == Type.include) //may not need these after updating Round 2 method
        {
            //Set the can be bools of tiles on other side of walls
            for (int i = 0; i < wallNeighbours.Count; i++)
            {
                wallNeighbours[i].canBe[rule.target] = false;
            }

            //Set the parent and children as the ruel of the target colour
            if (t.parent.canBe[rule.target] == true && t.parent.assigned == false)
            {
                AssignByColour(t.parent, rule.target);
            }
            foreach (Tile c in t.children)
            {
                if (c.canBe[rule.target] == true && c.assigned == false)
                {
                    AssignByColour(c, rule.target);
                }
            }
        }
        else if (rule.type == Type.exclude)
        {
            t.parent.canBe[rule.target] = false;
            foreach (Tile c in t.children)
            {
                c.canBe[rule.target] = false;
            }

            //Assign exclude target colour to all tiels on other side of walls
            for (int i = 0; i < wallNeighbours.Count; i++)
            {
                AssignByColour(wallNeighbours[i], rule.target);
            }
        }
    }

    //Used to set parents/children of a tile, when the tile's target colour is predetermined (colour rules)
    //The colour could be "warm" "cool" or a specific colour meaning multiple rule options potentially
    private void AssignByColour(Tile t, Colour c)
    {

        //***You cannot assign a parent/child a Tmove or blank rule. If that's the option, the given maze + set fo rules doesn't work (or you have to restart)
        //These rules need to have specific child/parent relationships which you can't gaurantee

        List<int> indexes = new List<int>(); //list of rule indexes with source colour c


        if(c == Colour.Warm)
        {
            foreach (MovementRule rule in mRules)
            {
                if ((rule.src == Colour.Red || rule.src == Colour.Orange || rule.src == Colour.Yellow || rule.src == Colour.Pink) && rule.type != Type.blank && rule.type != Type.Tmove)
                {
                    indexes.Add(rule.index);
                }
            }
            foreach (ColourRule rule in cRules)
            {
                if ((rule.src == Colour.Red || rule.src == Colour.Orange || rule.src == Colour.Yellow || rule.src == Colour.Pink) && rule.type != Type.blank && rule.type != Type.Tmove)
                {
                    indexes.Add(rule.index);
                }
            }
        }
        else if(c == Colour.Cool)
        {
            foreach (MovementRule rule in mRules)
            {
                if ((rule.src == Colour.Blue || rule.src == Colour.Green || rule.src == Colour.Purple || rule.src == Colour.Teal) && rule.type != Type.blank && rule.type != Type.Tmove)
                {
                    indexes.Add(rule.index);
                }
            }
            foreach (ColourRule rule in cRules)
            {
                if ((rule.src == Colour.Blue || rule.src == Colour.Green || rule.src == Colour.Purple || rule.src == Colour.Teal) && rule.type != Type.blank && rule.type != Type.Tmove)
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
        if(indexes.Count > 0)
        {
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

            if (assigned == false) //if random rule wasn't an mRule, find the cRule
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
        else
        {
            //This means the only rules with the colour is a Tmove and blank rule which we can't assign
            //Therefore, colour not possible to put down
            foreach (MovementRule rule in RoundOneRules)
            {
                if (rule.src == c)
                {
                    t.failedToAssign = true;
                    t.ruleType = Type.wall;
                    t.colour = Colour.Black;
                    SpriteRenderer sr = t.GetComponent<SpriteRenderer>();
                    Material black = (Material)Resources.Load("Black");
                    sr.material.shader = black.shader;
                    sr.material.color = black.color;
                    Debug.Log("Failed to assigned " + t.name);

                    if (maze.LP.path.Contains(t) == true)
                    {
                        pathUnassigned++;
                    }
                    else
                    {
                        unassigned++;
                    }
                }
            }
        }
    }

    /* Round 1
     *Place all Tmove and Blank rules
     */
    private void RoundOne()
    {
        foreach (MovementRule rule in mRules)
        {
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
                            if(rule.direction == Direction.North)
                            {
                                AssignByMRule(t, rule);
                            }
                        }
                        else if (south == false)
                        {
                            if (rule.direction == Direction.South)
                            {
                                AssignByMRule(t, rule);
                            }
                        }
                        else if (east == false)
                        {
                            if (rule.direction == Direction.East)
                            {
                                AssignByMRule(t, rule);
                            }
                        }
                        else if (west == false)
                        {
                            if (rule.direction == Direction.West)
                            {
                                AssignByMRule(t, rule);
                            }
                        }

                    }
                }
            }
            if (rule.type == Type.blank)
            {
                foreach (Tile t in maze.tiles)
                {
                    if(t.assigned == false)
                    {
                        if (t.children.Count + 1 == 3 && t.border == true && t != maze.LP.exit)
                        {
                            AssignByMRule(t, rule); //this includes the exit where its parent is itself
                        }

                        else if (t.children.Count + 1 == 4) //Cross section piece
                        {
                            AssignByMRule(t, rule); //this includes the exit where its parent is itself
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
    private void RoundTwo()
    {
        Tile tile = maze.LP.entrance;
        Tile lastTile = maze.LP.exit; //this will be set to the previously visited tile
        List<Tile> subtrees = new List<Tile>();
        bool root = false;

        while (root == false) //not root node
        {
            if (tile.assigned == false && tile.failedToAssign == false)
            {
                bool check = false;

                //get list of all adjacent tiles that are not parent/children and are assigned
                List<Tile> wallNeighbours = getAdjacentWallTiles(tile);

                List<Tile> relatedNeighbours = new List<Tile>(); //list of all assigned children and parent except pevious tile

                if (tile.parent.assigned == true && tile.parent != lastTile) //not last tile because tile is gauranteed to be part of the solution path, it won't be down a subtree
                {
                    relatedNeighbours.Add(tile.parent);
                }

                foreach (Tile c in tile.children)
                {
                    if (c.assigned == true && c != lastTile)
                    {
                        relatedNeighbours.Add(c);
                    }
                }

                if (relatedNeighbours.Count > 0 || wallNeighbours.Count > 0) //at least one adjacent tile is assigned already
                {
                    check = AdjacentAssigned(tile, relatedNeighbours, wallNeighbours, check);
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

                        if (identifiers[rand] < (mRules.Count + RoundOneRules.Count)) //MovementRule
                        {
                            MovementRule r = GetMRule(identifiers[rand]); //assign current tile (DOESN'T GET A RULE, GETS A TMOVE OLD RULE WITH WRONG SOURCE COLOUR??)

                            if (tile.canBe[r.src] == true)
                            {
                                check = true;

                                AssignByMRule(tile, r);
                            }

                        }
                        else
                        {
                            ColourRule r = GetCRule(identifiers[rand]); //assign current tile

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
                    tile.ruleType = Type.wall;
                    tile.colour = Colour.Black;
                    SpriteRenderer sr = tile.GetComponent<SpriteRenderer>();
                    Material black = (Material)Resources.Load("Black");
                    sr.material.shader = black.shader;
                    sr.material.color = black.color;
                    Debug.Log("Failed to assigned " + tile.name);

                    if(maze.LP.path.Contains(tile) == true)
                    {
                        pathUnassigned++;
                    }
                    else
                    {
                        unassigned++;
                    }
                    //no colour could be assigned to this tile
                    //currently not doing anything when this happens
                    //this will happen if a tile is next to a warm and cool tile (all colours bools will be set to false)
                }
            }

            foreach(Tile c in tile.children)
            {
                if (c != lastTile)
                {
                    subtrees.Add(c);
                    //ParentToChild(c);
                }

            }

            if(tile.parent != tile)
            {
                lastTile = tile;
                tile = tile.parent;
            }
            else{
                root = true;
            }
        }

        foreach(Tile t in subtrees)
        {
            ParentToChild(t);
        }
    }

    /* Unlike the solution path, the rest of the maze must be traversable both ways
     * I had to split into two because we can assign from root to leafs
     *
     * This traverses down children until tile of rank 0 is becomes previosu tile
     */
    private void ParentToChild(Tile tile)
    {

        //player must be able to traverse to parent and every child of tile

        if(tile.assigned == false && tile.failedToAssign == false)
        {
            bool check = false;

            //get list of all adjacent tiles that are not parent/children and are assigned
            List<Tile> wallNeighbours = getAdjacentWallTiles(tile);

            List<Tile> relatedNeighbours = new List<Tile>(); //list of all assigned children and parent
            if (tile.parent.assigned == true)
            {
                relatedNeighbours.Add(tile.parent);
            }
            foreach (Tile c in tile.children)
            {
                if (c.assigned == true)
                {
                    relatedNeighbours.Add(c);
                }
            }

            if (relatedNeighbours.Count > 0|| wallNeighbours.Count > 0)
            {
                check = AdjacentAssigned(tile, relatedNeighbours, wallNeighbours, check);
            }
            else //This will only happen if previosu tile couldn't be assigned
            {
                List<int> randIndex = new List<int>(); //list of list indexes of possible rules

                for (int i = 0; i < identifiers.Count; i++)
                {
                    randIndex.Add(i); //add indexes 0 to unchecked rule count - 1 *********
                }

                while (!check && randIndex.Count > 0)
                {
                    int randListItem = UnityEngine.Random.Range(0, randIndex.Count); //get a random index
                    int rand = randIndex[randListItem];
                    randIndex.RemoveAt(randListItem); //remove this index so rule does not get checked again

                    if (identifiers[rand] < (mRules.Count + RoundOneRules.Count)) //MovementRule
                    {
                        MovementRule r = GetMRule(identifiers[rand]); //assign current tile

                        if (tile.canBe[r.src] == true)
                        {
                            check = true;
                            AssignByMRule(tile, r);
                        }

                    }
                    else
                    {
                        ColourRule r = GetCRule(identifiers[rand]); //assign current tile

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
                tile.colour = Colour.Black;
                tile.ruleType = Type.wall;
                SpriteRenderer sr = tile.GetComponent<SpriteRenderer>();
                Material black = (Material)Resources.Load("Black");
                sr.material.shader = black.shader;
                sr.material.color = black.color;
                Debug.Log("Failed to assigned " + tile.name);

                if (maze.LP.path.Contains(tile) == true)
                {
                    pathUnassigned++;
                }
                else
                {
                    unassigned++;
                }
            }
        }


        if (tile.rank != 0)
        {
            foreach (Tile c in tile.children)
            {
                if(c.assigned == true && c.moveRule == true)
                {
                    if(c.mRule.type == Type.Tmove || c.mRule.type == Type.blank)
                    {
                        ParentToChild(c);
                    }
                }
                else if (c.assigned == false) //condition so you don't infinitely go between two cycled tiles
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
    private bool AdjacentAssigned(Tile tile, List<Tile> adjacent, List<Tile> walls, bool check)
    {
        foreach (MovementRule m in mRules) //first try to assign a warm or cool rule
        {
            if (m.type == Type.warm)
            {
                bool warm = true;
                foreach(Tile a in adjacent)
                {
                    if(a.colour != Colour.Red && a.colour != Colour.Orange && a.colour != Colour.Yellow && a.colour != Colour.Pink)
                    {
                        warm = false;
                        break;
                    }
                }
                if(warm == true)
                {
                    foreach (Tile w in walls)
                    {
                        if (w.colour == Colour.Red || w.colour == Colour.Orange || w.colour == Colour.Yellow || w.colour == Colour.Pink)
                        {
                            warm = false;
                            break;
                        }
                    }
                }

                if (warm == true && tile.canBe[m.src] == true) //all adjacent tiles are a warm colour
                {
                    check = true;
                    //Debug.Log("Assigned warm to " + tile.name + " Adjacent walls: " + walls.Count + "Neighbours: " + adjacent.Count);

                    AssignByMRule(tile, m);
                }
            }
            else if (m.type == Type.cool)
            {
                bool cool = true;
                foreach (Tile a in adjacent)
                {
                    if (a.colour != Colour.Blue && a.colour != Colour.Green && a.colour != Colour.Purple && a.colour != Colour.Teal)
                    {
                        cool = false;
                        break;
                    }
                }
                if(cool == true)
                {
                    foreach (Tile w in walls)
                    {
                        if (w.colour == Colour.Blue || w.colour == Colour.Green || w.colour == Colour.Purple || w.colour == Colour.Teal)
                        {
                            cool = false;
                            break;
                        }
                    }
                }

                if (cool == true && tile.canBe[m.src] == true)
                {
                    check = true;
                    AssignByMRule(tile, m);
                }
            }
        }
        if (!check) //then try to assign a random jump or teleport rule
        {
            List<MovementRule> otherMRules = new List<MovementRule>();
            foreach (MovementRule m in mRules)
            {
                if (m.type == Type.jump1 || m.type == Type.jump2 || m.type == Type.teleport)
                {
                    otherMRules.Add(m);
                }
            }

            List<int> randIndex = new List<int>(); //list of list indexes of possible rules
            for (int i = 0; i < otherMRules.Count; i++)
            {
                randIndex.Add(i); //add indexes 0 to otherMRules count - 1
            }
            while (randIndex.Count > 0)
            {
                int randListItem = UnityEngine.Random.Range(0, randIndex.Count); //get a random index
                int rand = randIndex[randListItem];
                randIndex.RemoveAt(randListItem);
                MovementRule m = mRules[rand];

                if (tile.canBe[m.src] == true)
                {
                    check = true;
                    AssignByMRule(tile, m);
                    break;
                }
            }
        }
        if (!check) //then try to assign exclude or block
        {
            foreach (ColourRule c in cRules)
            {

                if (c.type == Type.exclude)
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
                    if(canPlace == true)
                    {
                        foreach (Tile w in walls)
                        {
                            if (w.colour != c.target)
                            {
                                canPlace = false;
                                break;
                            }
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
                    if(canPlace == true)
                    {
                        foreach (Tile w in walls)
                        {
                            if (w.colour == c.target)
                            {
                                canPlace = false;
                                break;
                            }
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

    private List<Tile> getAdjacentWallTiles(Tile tile) //returns assigned adjacent tiles on other side of wall
    {

        List<Tile> wallNeighbours = new List<Tile>();
        int tileNum = Array.IndexOf(maze.tiles, tile); //tile index


        if (tileNum + 1 != maze.w * maze.h) //not top right tile
        {
            bool child = false;
            Tile east = maze.tiles[tileNum + 1];
            if ((tileNum + 1) % maze.w != 0) //east tile exists
            {
                if (east.assigned == true)
                {
                    if (east != tile.parent) //if eastward tile is assigned and not child/parent, add to list
                    {
                        foreach (Tile c in tile.children)
                        {
                            if (c == east)
                            {
                                child = true;
                                break;
                            }
                        }
                        if (child == false)
                        {
                            wallNeighbours.Add(east);
                        }
                    }
                }
            }
        }

        if(tileNum != 0) //not bottom left tile
        {
            bool child = false;
            Tile west = maze.tiles[tileNum - 1];
            if (tileNum % maze.w != 0) //west tile exists
            {
                if (west.assigned == true)
                {
                    if (west != tile.parent) //if westward tile is assigned and not child/parent, add to list
                    {
                        foreach (Tile c in tile.children)
                        {
                            if (c == west)
                            {
                                child = true;
                                break;
                            }
                        }
                        if (child == false)
                        {
                            wallNeighbours.Add(west);
                        }
                    }
                }
            }
        }

        if (tileNum + maze.w < maze.w * maze.h) //north tile exists
        {
            bool child = false;
            Tile north = maze.tiles[tileNum + maze.w];
            if (north.assigned == true)
            {
                if (north != tile.parent) //if northward tile is assigned and not child/parent, add to list
                {
                    foreach (Tile c in tile.children)
                    {
                        if (c == north)
                        {
                            child = true;
                            break;
                        }
                    }
                    if (child == false)
                    {
                        wallNeighbours.Add(north);
                    }
                }
            }
        }
        if (tileNum - maze.w >= 0) //south tile exists
        {
            bool child = false;
            Tile south = maze.tiles[tileNum - maze.w];
            if (south.assigned == true)
            {
                if (south != tile.parent) //if southward tile is assigned and not child/parent, add to list
                {
                    foreach (Tile c in tile.children)
                    {
                        if (c == south)
                        {
                            child = true;
                            break;
                        }
                    }
                    if (child == false)
                    {
                        wallNeighbours.Add(south);
                    }
                }
            }
        }

        return wallNeighbours;
    }

    private List<Tile> getAllWallTiles(Tile tile) //returns assigned and unassigned adjacent tiles on other side of wall
    {

        List<Tile> wallNeighbours = new List<Tile>();
        int tileNum = Array.IndexOf(maze.tiles, tile); //tile index

        if (tileNum + 1 != maze.w * maze.h) //not top right tile
        {
            bool child = false;
            Tile east = maze.tiles[tileNum + 1];
            if ((tileNum + 1) % maze.w != 0) //east tile exists
            {
                if (east != tile.parent) //if eastward tile is assigned and not child/parent, add to list
                {
                    foreach (Tile c in tile.children)
                    {
                        if (c == east)
                        {
                            child = true;
                            break;
                        }
                    }
                    if (child == false)
                    {
                        wallNeighbours.Add(east);
                    }
                }
            }
        }

        if (tileNum != 0) //not bottom left tile
        {
            bool child = false;
            Tile west = maze.tiles[tileNum - 1];
            if (tileNum % maze.w != 0) //west tile exists
            {
                if (west != tile.parent) //if westward tile is assigned and not child/parent, add to list
                {
                    foreach (Tile c in tile.children)
                    {
                        if (c == west)
                        {
                            child = true;
                            break;
                        }
                    }
                    if (child == false)
                    {
                        wallNeighbours.Add(west);
                    }
                }
            }
        }

        if (tileNum + maze.w < maze.w * maze.h) //north tile exists
        {
            bool child = false;
            Tile north = maze.tiles[tileNum + maze.w];

            if (north != tile.parent) //if northward tile is assigned and not child/parent, add to list
            {
                foreach (Tile c in tile.children)
                {
                    if (c == north)
                    {
                        child = true;
                        break;
                    }
                }
                if (child == false)
                {
                    wallNeighbours.Add(north);
                }
            }
        }
        if (tileNum - maze.w >= 0) //south tile exists
        {
            bool child = false;
            Tile south = maze.tiles[tileNum - maze.w];

            if (south != tile.parent) //if southward tile is assigned and not child/parent, add to list
            {
                foreach (Tile c in tile.children)
                {
                    if (c == south)
                    {
                        child = true;
                        break;
                    }
                }
                if (child == false)
                {
                    wallNeighbours.Add(south);
                }
            }
        }

        return wallNeighbours;
    }


    /* Round 3
     * - Ensure the exit tile is coloured
     * - Colour the tiles that are only reachable by going past exit
     * - try not to colour them with a teleport colour
     */
    private void RoundThree()
    {
        //1. Ensure exit tile is coloured
        if (maze.LP.exit.assigned == false)
        {
            Debug.Log("Exit tile assigned via Round 3");
            Tile exit = maze.LP.exit;
            foreach (Tile c in exit.children)
            {
                if (maze.LP.path.Contains(c))
                {
                    List<Colour> teleports = new List<Colour>(); //List of teleport rule target colours. Don't give exit these colours if possible.
                    List<Colour> others = new List<Colour>(); //All other colours not including Tmove and blank source colours

                    foreach (MovementRule r in mRules)
                    {
                        if (r.type == Type.teleport)
                        {
                            teleports.Add(r.target);
                        }
                        others.Add(r.src);
                    }

                    for (int i = 0; i < others.Count; i++)
                    {
                        if (teleports.Contains(others[i]))
                        {
                            others.Remove(others[i]);
                            i--;
                        }
                    }

                    Colour col; //get target colour of child on SP
                    if (c.moveRule)
                    {
                        col = c.mRule.target;
                    }
                    else
                    {
                        col = c.cRule.target;
                    }

                    SpriteRenderer sr = exit.GetComponent<SpriteRenderer>();
                    if (col == Colour.All)
                    {
                        if (others.Count > 0)
                        {
                            foreach(Colour colour in others)
                            {
                                if(exit.canBe[colour] == true)
                                {
                                    AssignByColour(exit, others[0]);
                                }
                            }
                        }

                        if(exit.assigned == false)
                        {
                            foreach (Colour colour in teleports)
                            {
                                if (exit.canBe[colour] == true)
                                {
                                    AssignByColour(exit, teleports[0]);
                                }
                            }
                        }
                    }
                    else if (col == Colour.Warm)
                    {
                        bool coloured = false;
                        foreach (Colour cc in others)
                        {
                            if (cc == Colour.Red || cc == Colour.Orange || cc == Colour.Yellow || cc == Colour.Pink)
                            {
                                if (exit.canBe[cc] == true)
                                {
                                    AssignByColour(exit, cc);
                                    coloured = true;
                                    break;
                                }
                            }
                        }
                        if (coloured == false)
                        {
                            foreach (Colour cc in teleports)
                            {
                                if (cc == Colour.Red || cc == Colour.Orange || cc == Colour.Yellow || cc == Colour.Pink)
                                {
                                    if (exit.canBe[cc] == true)
                                    {
                                        AssignByColour(exit, cc);
                                        coloured = true;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    else if (col == Colour.Cool)
                    {
                        bool coloured = false;
                        foreach (Colour cc in others)
                        {
                            if (cc == Colour.Blue || cc == Colour.Green || cc == Colour.Purple || cc == Colour.Teal)
                            {
                                if (exit.canBe[cc] == true)
                                {
                                    AssignByColour(exit, cc);
                                    coloured = true;
                                    break;
                                }
                            }
                        }
                        if (coloured == false)
                        {
                            foreach (Colour cc in teleports)
                            {
                                if (cc == Colour.Blue || cc == Colour.Green || cc == Colour.Purple || cc == Colour.Teal)
                                {
                                    if (exit.canBe[cc] == true)
                                    {
                                        AssignByColour(exit, cc);
                                        coloured = true;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    else //target is specific colour
                    {
                        //Even if target can't be this colour due to a diff. adjacent tile,
                        //I'm still setting it to this to make the solution path traversable
                        if (exit.canBe[col] == true)
                        {
                            AssignByColour(exit, col);
                        }
                        else
                        {
                            Debug.Log("Failed to assigned exit tile properly");
                            exitAssigned = false; //not adding to any unassigned count because path is still traversable as normal so it shouldnt hinder fitness in that way
                            AssignByColour(exit, col); //try to assign colour anyway
                            if(exit.assigned == false)
                            {
                                exit.colour = col; //last resort, give it a colour so the SP is fully traversable
                            }
                        }
                    }
                    break;
                }
            }
        }

        //2. Colour other side of exit tile
        foreach (Tile c in maze.LP.exit.children)
        {
            if (maze.LP.path.Contains(c) == false)
            {
                if(c.assigned == true && c.moveRule == true)
                {
                    if(c.mRule.type == Type.Tmove || c.mRule.type == Type.blank)
                    {
                        ParentToChild(c);
                    }
                }
                else if(c.assigned == false)
                {
                    ParentToChild(c);
                }
            }
        }
    }
}
