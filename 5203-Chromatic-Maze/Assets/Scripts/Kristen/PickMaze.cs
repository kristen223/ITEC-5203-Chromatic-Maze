using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PickMaze : MonoBehaviour
{
    /* 1.Remove invlaid mazes from running
     *      - 0 solution paths
     *      - teleport target colour does not exist in grid
     * 2.Compare leftover mazes
     *      1. number of solution paths
     *      2. number of walls in grid
     *      3. check if exit/children of exit are teleport target colours
     *      4. even distribution of used list (rules assigned)
     *      5. percentage of grid with teleport target colour
    */

    public static GameObject GetFinalMaze(Dictionary<GameObject, ColourAssigner.ColouredMaze> cmazes)
    {
        //1. Get rid of invalid mazes
        List<GameObject> toRemove = new List<GameObject>();
        foreach (GameObject prefab in cmazes.Keys)
        {
            if(!(cmazes[prefab].spaths.allPaths.Count > 0)) //check if a solution path doesn't exist
            {
                toRemove.Add(prefab);
            }
            else //check if teleport target colour exists in grid
            {
                List<Colour> telTargets = new List<Colour>(); //list of teleport rule target colours
                foreach (MovementRule r in cmazes[prefab].mr)
                {
                    if (r.type == Type.teleport)
                    {
                        if (!telTargets.Contains(r.target))
                        {
                            telTargets.Add(r.target);
                        }
                    }
                }

                if (telTargets.Count > 0)
                {
                    foreach (Tile t in cmazes[prefab].maze.tiles)
                    {
                        if (telTargets.Contains(t.colour))
                        {
                            telTargets.Remove(t.colour);
                        }
                        if (telTargets.Count == 0)
                        {
                            break;
                        }
                    }
                    if (telTargets.Count > 0)
                    {
                        toRemove.Add(prefab);
                        //teleport target colour does not exist in maze, so remove maze from list
                    }
                }
            }            
        }
        
        foreach (GameObject g in toRemove) //remove invalid mazes
        {
            cmazes.Remove(g); //when you destory them, the indexes change
        }

        GameObject finalMaze = new GameObject();
        if (cmazes.Count == 0)
        {
            Debug.Log("NO VALID MAZE CREATED");
            return finalMaze;
        }

        /*2. Compare left over mazes
         *   Dictionary of cmaze and its fitness value (smaller the better)
        */
        Dictionary<GameObject, int> fitnessValues = new Dictionary<GameObject, int>();
        Dictionary<GameObject, int> sPathLengths = new Dictionary<GameObject, int>();
        Dictionary<GameObject, int> NumOfWalls = new Dictionary<GameObject, int>();

        foreach (KeyValuePair<GameObject, ColourAssigner.ColouredMaze> pair in cmazes)
        {
            fitnessValues.Add(pair.Key, 0); //all mazes start at 0
            sPathLengths.Add(pair.Key, pair.Value.spaths.allPaths.Count);
            NumOfWalls.Add(pair.Key, pair.Value.PathViolations);
        }

        //1. COMPARE NUMBER OF SOLUTION PATHS (more = better)
        var pathOrder = sPathLengths.OrderBy(pair => pair.Value); //IEnumerable<KeyValuePair<coloured maze, int>> of mazes by number of solution paths (highest first)
        int increment = -1; //amount fitness is incremented by
        int lastLength = -1;
        foreach (KeyValuePair<GameObject, int> mazeSP in pathOrder)
        {
            if (mazeSP.Value != lastLength) //if this solution path is longer than the last (so paths of equal value are incremented the same)
            {
                increment++;
            }

            fitnessValues[mazeSP.Key] += increment;
            lastLength = mazeSP.Value;
        }

        //2. COMPARE NUMBER OF WALLS IN GRID
        var wallOrder = NumOfWalls.OrderByDescending(pair => pair.Value); //list of mazes ordered by number of walls (fewer walls first)
        increment = -1; //amount fitness is incremented by
        int lastWallCount = -1;
        foreach (KeyValuePair<GameObject, int> mazeWalls in wallOrder)
        {
            if (mazeWalls.Value != lastWallCount) //if this wall count is greater than the last wall count in list (so paths of equal value are incremented the same)
            {
                increment++;
            }

            fitnessValues[mazeWalls.Key] += increment;
            lastLength = mazeWalls.Value;
        }

        //3. CHECK IF EXIT AND CHILDREN OF EXIT ARE A TELEPORT TARGET COLOUR
        //4. CHECK PERCENTAGE OF GRID WITH TELEPORT TARGET COLOURS (includes all teleport rules in one)
        foreach (KeyValuePair<GameObject, ColourAssigner.ColouredMaze> pair in cmazes)
        {
            List<Colour> telTargets = new List<Colour>(); //list of teleport rule target colours
            foreach (MovementRule r in pair.Value.mr)
            {
                if (r.type == Type.teleport)
                {
                    if (!telTargets.Contains(r.target))
                    {
                        telTargets.Add(r.target);
                    }
                }
            }

            //3.
            if (telTargets.Contains(pair.Value.maze.LP.exit.colour)) //if exit tile is a teleport target colour
            {
                fitnessValues[pair.Key] += cmazes.Count/2; //the more mazes, the more this increments (which is parallel to parts 1 and 2)
            }

            foreach(Tile child in pair.Value.maze.LP.exit.children)
            {
                if (telTargets.Contains(child.colour)) //if exit tile is a teleport target colour
                {
                    fitnessValues[pair.Key] += Mathf.CeilToInt(cmazes.Count/3f); //num mazes/3 rounded up. 1-2 less than the above
                }
            }

            //4.
            Dictionary<GameObject, int> percentTeleport = new Dictionary<GameObject, int>();
            int count = 0;
            foreach(Tile t in pair.Value.maze.tiles)
            {
                if(telTargets.Contains(t.colour))
                {
                    count++;
                }
            }

            float percent = count * 100 / (pair.Value.maze.w * pair.Value.maze.h); //percentage of grid that is coloured with teleport target colours
            percentTeleport.Add(pair.Key, (int)percent);

            foreach(KeyValuePair<GameObject, int> cmRatio in percentTeleport)
            {
                //<15, 15-25, 26-39, <=40
                //  2,   0,     1,    2
                if(cmRatio.Value < 15 || cmRatio.Value >= 40) //0-14% or 40-100%
                {
                    fitnessValues[cmRatio.Key] += 2;
                }
                else if (cmRatio.Value >= 26 && cmRatio.Value <= 39) //26-39%
                {
                    fitnessValues[cmRatio.Key] += 1;
                }
            }
        }

        /*5. CHECK RATIO OF RULES IN GRID
             Compare all values in 'used' dictionary for each maze
             Get highest and lowest values to find difference
             Make a new KVP list and sort cmazes by differences
             The larger the difference, the more the fitness is incremented
        */
        Dictionary<GameObject, int> RuleDistribution = new Dictionary<GameObject, int>();

        foreach (KeyValuePair<GameObject, ColourAssigner.ColouredMaze> pair in cmazes)
        {
            int lowest = pair.Value.used.ElementAt(0).Value;
            int highest = pair.Value.used.ElementAt(0).Value;
            foreach (KeyValuePair<int, int> used in pair.Value.used)
            {
                if(used.Value < lowest)
                {
                    lowest = used.Value;
                }
                if (used.Value > highest)
                {
                    highest = used.Value;
                }
            }

            int difference = highest - lowest;
            RuleDistribution.Add(pair.Key, difference);
        }

        var distributions = RuleDistribution.OrderByDescending(pair => pair.Value); //list of mazes by distribution of rules (most even distributions first)
        increment = -1;
        int lastdist = -1;
        foreach (KeyValuePair<GameObject, int> cmDist in distributions)
        {
            if (cmDist.Value != lastdist)
            {
                increment++;
            }

            fitnessValues[cmDist.Key] += increment;
            lastdist = cmDist.Value;
        }


        //6. RETURN BEST MAZE
        //Dictionary<ColourAssigner.ColouredMaze, int> fitnessValues
        int bestFit = 1000;
        foreach(KeyValuePair<GameObject, int> kvp in fitnessValues)
        {
            if(kvp.Value < bestFit)
            {
                finalMaze = kvp.Key;
                bestFit = kvp.Value;
            }
        }

        return finalMaze;
    }

}
