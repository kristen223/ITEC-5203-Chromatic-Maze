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

    public static ColourAssigner.ColouredMaze GetFinalMaze(List<ColourAssigner.ColouredMaze> cmazes, List<GameObject> prefabs)
    {
        //1. Get rid of invalid mazes
        List<ColourAssigner.ColouredMaze> toRemove = new List<ColourAssigner.ColouredMaze>();
        List<GameObject> toRemoveP = new List<GameObject>();
        foreach (ColourAssigner.ColouredMaze cm in cmazes)
        {
            if(!(cm.spaths.allPaths.Count > 0)) //check if a solution path doesn't exist
            {
                toRemove.Add(cm);
                toRemoveP.Add(prefabs[cmazes.IndexOf(cm)]);
            }
            else //check if teleport target colour exists in grid
            {
                List<Colour> telTargets = new List<Colour>(); //list of teleport rule target colours
                foreach (MovementRule r in cm.mr)
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
                    foreach (Tile t in cm.maze.tiles)
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
                        toRemove.Add(cm);
                        toRemoveP.Add(prefabs[cmazes.IndexOf(cm)]); //teleport target colour does not exist in maze, so remove maze from list
                    }
                }
            }            
        }
        
        foreach (ColourAssigner.ColouredMaze c in toRemove) //remove invalid mazes
        {
            cmazes.Remove(c); //when you destory them, the indexes change
        }
        foreach (GameObject p in toRemoveP) //remove invalid mazes
        {
            prefabs.Remove(p);
            GameObject.Destroy(p);
        }  

        ColourAssigner.ColouredMaze finalMaze = new ColourAssigner.ColouredMaze();
        if (cmazes.Count == 0)
        {
            Debug.Log("NO VALID MAZE CREATED");
            return finalMaze;
        }

        /*2. Compare left over mazes
         *   Dictionary of cmaze and its fitness value (smaller the better)
        */
        Dictionary<ColourAssigner.ColouredMaze, int> fitnessValues = new Dictionary<ColourAssigner.ColouredMaze, int>();
        Dictionary<ColourAssigner.ColouredMaze, int> sPathLengths = new Dictionary<ColourAssigner.ColouredMaze, int>();
        Dictionary<ColourAssigner.ColouredMaze, int> NumOfWalls = new Dictionary<ColourAssigner.ColouredMaze, int>();

        foreach (ColourAssigner.ColouredMaze cm in cmazes)
        {
            fitnessValues.Add(cm, 0); //all mazes start at 0
            sPathLengths.Add(cm, cm.spaths.allPaths.Count);
            NumOfWalls.Add(cm, cm.PathViolations);
        }

        //1. COMPARE NUMBER OF SOLUTION PATHS (more = better)
        var pathOrder = sPathLengths.OrderBy(pair => pair.Value); //IEnumerable<KeyValuePair<coloured maze, int>> of mazes by number of solution paths (highest first)
        int increment = -1; //amount fitness is incremented by
        int lastLength = -1;
        foreach (KeyValuePair<ColourAssigner.ColouredMaze, int> mazeSP in pathOrder)
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
        foreach (KeyValuePair<ColourAssigner.ColouredMaze, int> mazeWalls in wallOrder)
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
        foreach (ColourAssigner.ColouredMaze cm in cmazes)
        {
            List<Colour> telTargets = new List<Colour>(); //list of teleport rule target colours
            foreach (MovementRule r in cm.mr)
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
            if (telTargets.Contains(cm.maze.LP.exit.colour)) //if exit tile is a teleport target colour
            {
                fitnessValues[cm]+= cmazes.Count/2; //the more mazes, the more this increments (which is parallel to parts 1 and 2)
            }

            foreach(Tile child in cm.maze.LP.exit.children)
            {
                if (telTargets.Contains(child.colour)) //if exit tile is a teleport target colour
                {
                    fitnessValues[cm] += Mathf.CeilToInt(cmazes.Count/3f); //num mazes/3 rounded up. 1-2 less than the above
                }
            }

            //4.
            Dictionary<ColourAssigner.ColouredMaze, int> percentTeleport = new Dictionary<ColourAssigner.ColouredMaze, int>();
            int count = 0;
            foreach(Tile t in cm.maze.tiles)
            {
                if(telTargets.Contains(t.colour))
                {
                    count++;
                }
            }

            float percent = count * 100 / (cm.maze.w * cm.maze.h); //percentage of grid that is coloured with teleport target colours
            percentTeleport.Add(cm, (int)percent);

            foreach(KeyValuePair<ColourAssigner.ColouredMaze, int> cmRatio in percentTeleport)
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
        Dictionary<ColourAssigner.ColouredMaze, int> RuleDistribution = new Dictionary<ColourAssigner.ColouredMaze, int>();

        foreach (ColourAssigner.ColouredMaze cm in cmazes)
        {
            int lowest = cm.used.ElementAt(0).Value;
            int highest = cm.used.ElementAt(0).Value;
            foreach (KeyValuePair<int, int> used in cm.used)
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
            RuleDistribution.Add(cm, difference);
        }

        var distributions = RuleDistribution.OrderByDescending(pair => pair.Value); //list of mazes by distribution of rules (most even distributions first)
        increment = -1;
        int lastdist = -1;
        foreach (KeyValuePair<ColourAssigner.ColouredMaze, int> cmDist in distributions)
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
        foreach(KeyValuePair<ColourAssigner.ColouredMaze, int> kvp in fitnessValues)
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
