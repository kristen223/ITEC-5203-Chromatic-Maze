using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TraverseMaze : MonoBehaviour
{

    static SolutionPaths sPaths;

    public struct SolutionPaths
    {
        public List<List<Tile>> allPaths; //all possible solution paths
        public int shortest; //length of shortest path (0 if no paths)
        public int longest; //length of longest path (0 if no paths)
        public List<Tile> shortestPath; //shortest path
        public List<Tile> longestPath; //shortest path
    }

    // Start is called before the first frame update
    void Start()
    {
        sPaths = new SolutionPaths();
        sPaths.allPaths = new List<List<Tile>>();
        sPaths.shortest = 0;
        sPaths.longest = 0;
        sPaths.shortestPath = new List<Tile>();
        sPaths.longestPath = new List<Tile>();
    }

    private static void GetShortestPath()
    {
        sPaths.shortestPath = sPaths.allPaths[0];
        foreach (List<Tile> path in sPaths.allPaths)
        {
            if (path.Count < sPaths.shortestPath.Count)
            {
                sPaths.shortestPath = path;
            }
        }
        sPaths.shortest = sPaths.shortestPath.Count;
    }

    private static void GetLongestPath()
    {
        sPaths.longestPath = sPaths.allPaths[0];
        foreach (List<Tile> path in sPaths.allPaths)
        {
            if (path.Count > sPaths.longestPath.Count)
            {
                sPaths.longestPath = path;
            }
        }
        sPaths.longest = sPaths.longestPath.Count;
    }

    public static SolutionPaths GetPathsFromEntrance(ColourAssigner.ColouredMaze cmaze)
    {
        List<Tile> path = new List<Tile>();
        GetPaths(cmaze.maze.LP.entrance, cmaze.maze.LP.exit, path, cmaze); //updates allPaths variable

        if(sPaths.allPaths.Count > 0)
        {
            GetShortestPath();
            GetLongestPath();
        }

        String debugs = "Shortest path: ";
        foreach (Tile t in sPaths.shortestPath)
        {
            debugs += t.name + ", ";
        }
        Debug.Log(debugs);

        String debug = "Longest path: ";
        foreach (Tile t in sPaths.longestPath)
        {
            debug += t.name + ", ";
        }
        Debug.Log(debug);

        return sPaths;
        //returns the final found path a lot (like it replaces all others)
    }

    //Gets list of paths from given tile to exit
    public static void GetPaths(Tile start, Tile end, List<Tile> path, ColourAssigner.ColouredMaze cmaze)
    {
        if(end.ruleType == Type.wall)
        {
            return;
        }

        List<KeyValuePair<List<Tile>, Tile>> starts = new List<KeyValuePair<List<Tile>, Tile>>(); //branching off tile and path at that point
        Tile current = start;
        bool valid = true;

        while(current != end && valid == true)
        {
            if(path.Contains(current)) //it's looping, so stop and discard this path
            {
                break;
            }
            else if(current.ruleType == Type.wall) //hit a wall
            {
                break;
            }

            path.Add(current); //add current tile to current path

            MovementRule mr = current.mRule;
            ColourRule cr = current.cRule;
            List<Tile> destinations = new List<Tile>();
            if (current.moveRule == true) //current is on a movement rule
            {
                switch (mr.type)
                {
                    case Type.Tmove:
                    case Type.blank:
                        foreach (Tile c in current.children)
                        {
                            List<Tile> p = new List<Tile>();
                            foreach (Tile t in path)
                            {
                                p.Add(t);
                            }
                            if (c.mRule.type != Type.wall)
                            {
                                starts.Add(new KeyValuePair<List<Tile>, Tile>(p, c));
                            }
                        }
                        current = current.parent;
                        break;

                    case Type.warm:
                        Colour pc = current.parent.colour;
                        if (pc == Colour.Red || pc == Colour.Orange || pc == Colour.Yellow || pc == Colour.Pink)
                        {
                            destinations.Add(current.parent);
                        }
                        foreach (Tile c in current.children)
                        {
                            Colour cc = c.colour;
                            if (cc == Colour.Red || cc == Colour.Orange || cc == Colour.Yellow || cc == Colour.Pink)
                            {
                                destinations.Add(c);
                            }
                        }

                        if (destinations.Count > 0)
                        {
                            current = destinations[0];
                            destinations.RemoveAt(0);

                            foreach (Tile d in destinations)
                            {
                                List<Tile> p = new List<Tile>();
                                foreach (Tile t in path)
                                {
                                    p.Add(t);
                                }
                                starts.Add(new KeyValuePair<List<Tile>, Tile>(p, d));
                            }
                        }
                        else
                        {
                            valid = false;
                        }
                        break;

                    case Type.cool: //getting wrong tiles, not children/parent
                        Colour pcol = current.parent.colour;
                        if (pcol == Colour.Blue || pcol == Colour.Green || pcol == Colour.Purple || pcol == Colour.Teal)
                        {
                            destinations.Add(current.parent);
                        }
                        foreach (Tile c in current.children)
                        {
                            Colour ccol = c.colour;
                            if (ccol == Colour.Blue || ccol == Colour.Green || ccol == Colour.Purple || ccol == Colour.Teal)
                            {
                                destinations.Add(c);
                            }
                        }

                        if (destinations.Count > 0)
                        {
                            current = destinations[0];
                            destinations.RemoveAt(0);

                            foreach (Tile d in destinations)
                            {
                                List<Tile> p = new List<Tile>();
                                foreach (Tile t in path)
                                {
                                    p.Add(t);
                                }
                                starts.Add(new KeyValuePair<List<Tile>, Tile>(p, d));
                            }
                        }
                        else
                        {
                            valid = false;
                        }
                        break;
                    case Type.teleport:
                        foreach(Tile t in cmaze.maze.tiles)
                        {
                            if(t.colour == mr.target)
                            {
                                destinations.Add(t);
                            }
                        }
                        if(destinations.Count > 0)
                        {
                            current = destinations[0];
                            destinations.RemoveAt(0);

                            foreach(Tile d in destinations)
                            {
                                List<Tile> p = new List<Tile>();
                                foreach (Tile t in path)
                                {
                                    p.Add(t);
                                }
                                if (d.mRule.type != Type.wall)
                                {
                                    starts.Add(new KeyValuePair<List<Tile>, Tile>(p, d));
                                }
                            }
                        }
                        else
                        {
                            valid = false;
                        }
                        break;

                    case Type.jump1:
                        if (current.jumpN == true) //if tile exists
                        {
                            destinations.Add(cmaze.maze.tiles[Array.IndexOf(cmaze.maze.tiles, current) + 2*cmaze.maze.w]);
                        }
                        if (current.jumpS == true) //if tile exists
                        {
                            destinations.Add(cmaze.maze.tiles[Array.IndexOf(cmaze.maze.tiles, current) - 2 * cmaze.maze.w]);
                        }
                        if (current.jumpE == true) //if tile exists
                        {
                            destinations.Add(cmaze.maze.tiles[Array.IndexOf(cmaze.maze.tiles, current) + 2]);
                        }
                        if (current.jumpW == true) //if tile exists
                        {
                            destinations.Add(cmaze.maze.tiles[Array.IndexOf(cmaze.maze.tiles, current) - 2]);
                        }

                        if (destinations.Count > 0)
                        {
                            current = destinations[0];
                            destinations.RemoveAt(0);

                            foreach (Tile d in destinations)
                            {
                                List<Tile> p = new List<Tile>();
                                foreach (Tile t in path)
                                {
                                    p.Add(t);
                                }
                                if (d.mRule.type != Type.wall)
                                {
                                    starts.Add(new KeyValuePair<List<Tile>, Tile>(p, d));
                                }
                            }
                        }
                        else
                        {
                            valid = false;
                        }

                        break;

                    case Type.jump2:
                        if (current.jumpTwoN == true) //if tile exists
                        {
                            destinations.Add(cmaze.maze.tiles[Array.IndexOf(cmaze.maze.tiles, current) + 3 * cmaze.maze.w]);
                        }
                        if (current.jumpTwoS == true) //if tile exists
                        {
                            destinations.Add(cmaze.maze.tiles[Array.IndexOf(cmaze.maze.tiles, current) - 3 * cmaze.maze.w]);
                        }
                        if (current.jumpTwoE == true) //if tile exists
                        {
                            destinations.Add(cmaze.maze.tiles[Array.IndexOf(cmaze.maze.tiles, current) + 3]);
                        }
                        if (current.jumpTwoW == true) //if tile exists
                        {
                            destinations.Add(cmaze.maze.tiles[Array.IndexOf(cmaze.maze.tiles, current) - 3]);
                        }

                        if (destinations.Count > 0)
                        {
                            current = destinations[0];
                            destinations.RemoveAt(0);

                            foreach (Tile d in destinations)
                            {
                                List<Tile> p = new List<Tile>();
                                foreach (Tile t in path)
                                {
                                    p.Add(t);
                                }
                                if (d.mRule.type != Type.wall)
                                {
                                    starts.Add(new KeyValuePair<List<Tile>, Tile>(p, d));
                                }
                            }
                        }
                        else
                        {
                            valid = false;
                        }
                        break;
                }
            }
            else //current is on a colour rule
            {
                switch (cr.type)
                {
                    case Type.include:
                        if (current.parent.colour == current.cRule.target)
                        {
                            destinations.Add(current.parent);
                        }
                        foreach (Tile c in current.children)
                        {
                            if (c.colour == current.cRule.target)
                            {
                                destinations.Add(c);
                            }
                        }

                        if (destinations.Count > 0)
                        {
                            current = destinations[0];
                            destinations.RemoveAt(0);

                            foreach (Tile d in destinations)
                            {
                                List<Tile> p = new List<Tile>();
                                foreach (Tile t in path)
                                {
                                    p.Add(t);
                                }
                                starts.Add(new KeyValuePair<List<Tile>, Tile>(p, d));
                            }
                        }
                        else
                        {
                            valid = false;
                        }
                        break;

                    case Type.exclude:
                        if (current.parent.colour != current.cRule.target && current.parent.colour != Colour.Black)
                        {
                            destinations.Add(current.parent);
                        }
                        foreach (Tile c in current.children)
                        {
                            if (c.colour != current.cRule.target)
                            {
                                destinations.Add(c);
                            }
                        }

                        if (destinations.Count > 0)
                        {
                            current = destinations[0];
                            destinations.RemoveAt(0);

                            foreach (Tile d in destinations)
                            {
                                List<Tile> p = new List<Tile>();
                                foreach(Tile t in path)
                                {
                                    p.Add(t);
                                }
                                starts.Add(new KeyValuePair<List<Tile>, Tile>(p, d));
                            }
                        }
                        else
                        {
                            valid = false;
                        }
                        break;
                }
            }
        }

        if(current == end) //if path made it to end, add it to list
        {
            path.Add(end);
            sPaths.allPaths.Add(path);
        }

        foreach (KeyValuePair<List<Tile>, Tile> kp in starts)
        {
            GetPaths(kp.Value, end, kp.Key, cmaze);
        }
    }
}