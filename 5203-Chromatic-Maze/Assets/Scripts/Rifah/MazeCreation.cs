using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeCreation : MonoBehaviour
{
    // Start is called before the first frame update
    private static GameObject mazePrefab; //put this at the top
    //private static List<GameObject> prefabs; //list of colour assigner prefabs
    //private static List<ColourAssigner.ColouredMaze> cmazes; //list of coloured mazes, one for each chromosome
    private static Dictionary<GameObject, ColourAssigner.ColouredMaze> cmazes;
    public static Colour[] allcolors = new Colour[8] { Colour.Red, Colour.Orange, Colour.Yellow, Colour.Pink, Colour.Teal, Colour.Blue, Colour.Purple, Colour.Green };
    private static List<Tile[]> unassignedTiles;
    private static GameObject gameover;
    void Awake()
    {
        gameover = GameObject.Find("GameOver2");
        gameover.SetActive(false);
        mazePrefab = (GameObject)Resources.Load("ColourAssigner"); //put this in Start method
        //prefabs = new List<GameObject>();
        cmazes = new Dictionary<GameObject, ColourAssigner.ColouredMaze>();
        unassignedTiles = new List<Tile[]>();
    }

    public static void seperateRules(List<Chromosome> mc)
    {
        Debug.Log("count: " + mc.Count);
        int counter = 1;
        foreach (Chromosome s in mc)
        {
            Debug.Log("chromosome " + counter);
            List<MovementRule> mr = new List<MovementRule>();
            List<ColourRule> cr = new List<ColourRule>();


            List<NewRules> rr = new List<NewRules>() { s.r1, s.r2, s.r3, s.r4, s.r5, s.r6, s.r7, s.r8 };

            foreach(NewRules rule in rr)
            {
                Debug.Log("rule " + rule.type);
                if(rule.type == Type.blank || rule.type == Type.cool || rule.type == Type.warm || rule.type == Type.jump1 || rule.type == Type.jump2 || rule.type == Type.teleport || rule.type == Type.Tmove){
                    MovementRule m = new MovementRule();
                    m.direction = rule.direction;
                    m.distance = rule.distance;
                    m.type = rule.type;
                    m.src = rule.src;
                    m.target = rule.target;
                    mr.Add(m);
                }
                else
                {
                    ColourRule m = new ColourRule();
                    m.inclusion = rule.inclusion;
                    m.type = rule.type;
                    m.src = rule.src;
                    m.target = rule.target;
                    cr.Add(m);
                  
                }
            
            }

      /*      if (s.r1.type == Type.blank || s.r1.type == Type.cool || s.r1.type == Type.warm || s.r1.type == Type.jump1 || s.r1.type == Type.jump2 || s.r1.type == Type.teleport || s.r1.type == Type.teleport)
            {
                MovementRule m = new MovementRule();
                m.direction = s.r1.direction;
                m.distance = s.r1.distance;
                m.type = s.r1.type;
                m.src = s.r1.src;
                m.target = s.r1.target;
                mr.Add(m);
            }
            if (s.r1.type == Type.include || s.r1.type == Type.exclude)
            {
                ColourRule m = new ColourRule();
                m.inclusion = s.r1.inclusion;
                m.type = s.r1.type;
                m.src = s.r1.src;
                m.target = s.r1.target;
                cr.Add(m);
            }
            if (s.r2.type == Type.blank || s.r2.type == Type.cool || s.r2.type == Type.warm || s.r2.type == Type.jump1 || s.r2.type == Type.jump2 || s.r2.type == Type.teleport || s.r2.type == Type.teleport)
            {
                MovementRule m = new MovementRule();
                m.direction = s.r2.direction;
                m.distance = s.r2.distance;
                m.type = s.r2.type;
                m.src = s.r2.src;
                m.target = s.r2.target;
                mr.Add(m);
            }
            if (s.r2.type == Type.include || s.r2.type == Type.exclude)
            {
                ColourRule m = new ColourRule();
                m.inclusion = s.r2.inclusion;
                m.type = s.r2.type;
                m.src = s.r2.src;
                m.target = s.r2.target;
                cr.Add(m);
            }
            if (s.r3.type == Type.blank || s.r3.type == Type.cool || s.r3.type == Type.warm || s.r3.type == Type.jump1 || s.r3.type == Type.jump2 || s.r3.type == Type.teleport || s.r3.type == Type.teleport)
            {
                MovementRule m = new MovementRule();
                m.direction = s.r3.direction;
                m.distance = s.r3.distance;
                m.type = s.r3.type;
                m.src = s.r3.src;
                m.target = s.r3.target;
                mr.Add(m);
            }
            if (s.r3.type == Type.include || s.r3.type == Type.exclude)
            {
                ColourRule m = new ColourRule();
                m.inclusion = s.r3.inclusion;
                m.type = s.r3.type;
                m.src = s.r3.src;
                m.target = s.r3.target;
                cr.Add(m);
            }
            if (s.r4.type == Type.blank || s.r4.type == Type.cool || s.r4.type == Type.warm || s.r4.type == Type.jump1 || s.r4.type == Type.jump2 || s.r4.type == Type.teleport || s.r4.type == Type.teleport)
            {
                MovementRule m = new MovementRule();
                m.direction = s.r4.direction;
                m.distance = s.r4.distance;
                m.type = s.r4.type;
                m.src = s.r4.src;
                m.target = s.r4.target;
                mr.Add(m);
            }
            if (s.r4.type == Type.include || s.r4.type == Type.exclude)
            {
                ColourRule m = new ColourRule();
                m.inclusion = s.r4.inclusion;
                m.type = s.r4.type;
                m.src = s.r4.src;
                m.target = s.r4.target;
                cr.Add(m);
            }
            if (s.r5.type == Type.blank || s.r5.type == Type.cool || s.r5.type == Type.warm || s.r5.type == Type.jump1 || s.r5.type == Type.jump2 || s.r5.type == Type.teleport || s.r5.type == Type.teleport)
            {
                MovementRule m = new MovementRule();
                m.direction = s.r5.direction;
                m.distance = s.r5.distance;
                m.type = s.r5.type;
                m.src = s.r5.src;
                m.target = s.r5.target;
                mr.Add(m);
            }
            if (s.r5.type == Type.include || s.r5.type == Type.exclude)
            {
                ColourRule m = new ColourRule();
                m.inclusion = s.r5.inclusion;
                m.type = s.r5.type;
                m.src = s.r5.src;
                m.target = s.r5.target;
                cr.Add(m);
            }
            if (s.r6.type == Type.blank || s.r6.type == Type.cool || s.r6.type == Type.warm || s.r6.type == Type.jump1 || s.r6.type == Type.jump2 || s.r6.type == Type.teleport || s.r6.type == Type.teleport)
            {
                MovementRule m = new MovementRule();
                m.direction = s.r6.direction;
                m.distance = s.r6.distance;
                m.type = s.r6.type;
                m.src = s.r6.src;
                m.target = s.r6.target;
                mr.Add(m);
            }
            if (s.r6.type == Type.include || s.r6.type == Type.exclude)
            {
                ColourRule m = new ColourRule();
                m.inclusion = s.r6.inclusion;
                m.type = s.r6.type;
                m.src = s.r6.src;
                m.target = s.r6.target;
                cr.Add(m);
            }
            if (s.r7.type == Type.blank || s.r7.type == Type.cool || s.r7.type == Type.warm || s.r7.type == Type.jump1 || s.r7.type == Type.jump2 || s.r7.type == Type.teleport || s.r7.type == Type.teleport)
            {
                MovementRule m = new MovementRule();
                m.direction = s.r7.direction;
                m.distance = s.r7.distance;
                m.type = s.r7.type;
                m.src = s.r7.src;
                m.target = s.r7.target;
                mr.Add(m);
            }
            if (s.r7.type == Type.include || s.r7.type == Type.exclude)
            {
                ColourRule m = new ColourRule();
                m.inclusion = s.r7.inclusion;
                m.type = s.r7.type;
                m.src = s.r7.src;
                m.target = s.r7.target;
                cr.Add(m);
            }
            if (s.r8.type == Type.blank || s.r8.type == Type.cool || s.r8.type == Type.warm || s.r8.type == Type.jump1 || s.r8.type == Type.jump2 || s.r8.type == Type.teleport || s.r8.type == Type.teleport)
            {
                MovementRule m = new MovementRule();
                m.direction = s.r8.direction;
                m.distance = s.r8.distance;
                m.type = s.r8.type;
                m.src = s.r8.src;
                m.target = s.r8.target;
                mr.Add(m);
            }
            if (s.r8.type == Type.include || s.r8.type == Type.exclude)
            {
                ColourRule m = new ColourRule();
                m.inclusion = s.r8.inclusion;
                m.type = s.r8.type;
                m.src = s.r8.src;
                m.target = s.r8.target;
                cr.Add(m);
            }
      */
            //Kristen's Code

            Debug.Log("mr count "  + mr.Count);
            Debug.Log("cr count " + cr.Count);

            //reset index values
            for (int ind = 0; ind < mr.Count; ind++)
            {
                MovementRule newm = mr[ind];
                newm.index = ind;
                mr[ind] = newm;
            }

            int cruleCount = 0;
            for (int ind = mr.Count; ind < cr.Count + mr.Count; ind++)
            {
                ColourRule newc = cr[cruleCount];
                newc.index = ind;
                cr[cruleCount] = newc;
                cruleCount++;
            }


            //Make one maze per chromosome
            GameObject temp = Instantiate(new GameObject(), new Vector3(0, 0, 0), Quaternion.Euler(0, 0, 0));
            temp.name = "Maze-" + counter;

            GameObject maze = Instantiate(mazePrefab, new Vector3(0, 0, 0), Quaternion.Euler(0, 0, 0));
            maze.name = "Prefab-" + counter;
            counter++;

            maze.transform.parent = temp.transform; //set prefab as child of empty game object temp
                                                    //prefabs.Add(temp);

            Debug.Log("Begin maze");
            maze.GetComponent<ColourAssigner>().SetRules(mr, cr); //set the maze rules to the current chromosome's rules
            cmazes.Add(temp, maze.GetComponent<ColourAssigner>().ColourMaze()); //colour the maze and add it to the list
            Debug.Log("finished maze");


        }
        LastStep();
    }

    public static void getFinalRules(Dictionary<int, int> chosenChr, Dictionary<int, Dictionary<int, Type>> clist, List<MovementRule> m, List<ColourRule> c)

    {
        int counter = 1;

        //int[][] chromosomes = new int[2][];
        foreach (KeyValuePair<int, int> k in chosenChr) //suppose to run (20% of popsize) times
        {
            foreach (KeyValuePair<int, Dictionary<int, Type>> x in clist)
            {

                if (k.Key == x.Key)
                {
                    List<MovementRule> mr = new List<MovementRule>();
                    List<ColourRule> cr = new List<ColourRule>();
                    //foreach (Dictionary<int, Type> d in clist.Values) //suppose to
                    //{
                    int i = 0;
                    foreach (KeyValuePair<int, Type> kvp in x.Value) //supposed to run 8 times
                    {
                        if (kvp.Value == Type.exclude || kvp.Value == Type.include)
                        {

                            ColourRule z = Fitness1.GetCRule(kvp.Key, c);
                            //Debug.Log("original color was---------------------------------" + z.src);
                            //z.src = allcolors[i];
                            //i++;
                            //Debug.Log("new color is------------------------- " + z.src);
                            cr.Add(z);
                            //Debug.Log("src color is-----" + z.src);
                        }
                        else
                        {
                            MovementRule y = Fitness1.GetMRule(kvp.Key, m);
                            //Debug.Log("original color was---------------------------------" + y.src);
                            //y.src = allcolors[i];
                           // i++;
                           // Debug.Log("new color is------------------------- " + y.src);
                            mr.Add(y);
                            //Debug.Log("src color is-----" + y.src);
                        }
                        // ChosenRulesIdx.Add(kvp.Key);

                    }
                }
            }
        }
    }

    private static void LastStep()
    {
        //FITNESS 2
        GameObject finalMazePrefab = PickMaze.GetFinalMaze(cmazes); //prefab Key

        if (finalMazePrefab == null)
        {
            gameover.SetActive(true);
            return; //no valid mazes created (debug statement somewhere else)
        }

        ColourAssigner.ColouredMaze finalMaze = cmazes[finalMazePrefab];

        //DELETE ALL GAME OBJECTS EXCEPT CHOSEN MAZE
        foreach (KeyValuePair<GameObject, ColourAssigner.ColouredMaze> kvp in cmazes)
        {
            if (kvp.Key != finalMazePrefab)
            {
                kvp.Key.SetActive(false); //better to destroy
                Destroy(kvp.Key);
            }
        }

        if (finalMaze.spaths.allPaths != null) //if maze was chosen
        {
            //TEMP DEBUG STUFF
            string ss = "final rules: ";
            foreach (MovementRule g in finalMaze.mr)
            {
                ss += g.type + "-" + g.src + ", ";
            }
            foreach (ColourRule h in finalMaze.cr)
            {
                ss += h.type + "-" + h.src + ", ";
            }
            Debug.Log(ss);
            Debug.Log("number of paths: " + finalMaze.spaths.allPaths.Count);

            string debugs = "Shortest path: ";
            foreach (Tile t in finalMaze.spaths.shortestPath)
            {
                debugs += t.name + ", ";
            }
            Debug.Log(debugs);

            string debugsss = "Medium path: ";
            foreach (Tile t in finalMaze.spaths.mediumPath)
            {
                debugsss += t.name + ", ";
            }
            Debug.Log(debugsss);

            string debug = "Longest path: ";
            foreach (Tile t in finalMaze.spaths.longestPath)
            {
                debug += t.name + ", ";
            }
            Debug.Log(debug);

            //SET INSTRUCTIONS TEXT
            InstructionsText.SetInstructions(finalMaze.mr, finalMaze.cr);

            //SET TILE GRID TO CORRECT MAZE'S TILES AND RESET CHECKERS
            //for (int i = 0; i < GenerateGrid.tiles.Length; i++)
            //{
            //    GenerateGrid.tiles[i].tag = "Untagged";
            //    GenerateGrid.tiles[i].transform.Find("Checker").GetComponent<SpriteRenderer>().enabled = false;

            //    Component chosenComponent = finalMaze.maze.tiles[i].GetComponent<Tile>();
            //    Debug.Log("Tile-" + (i + 1) + "colour = " + finalMaze.maze.tiles[i].GetComponent<Tile>().colour + ", rule type = " + finalMaze.maze.tiles[i].GetComponent<Tile>().ruleType);

            //    System.Type type = chosenComponent.GetType();

            //    System.Reflection.FieldInfo[] fields = type.GetFields();
            //    foreach (System.Reflection.FieldInfo field in fields)
            //    {
            //        field.SetValue(GenerateGrid.tiles[i].GetComponent<Tile>(), field.GetValue(chosenComponent));
            //    }
            //}


            //Debug.Log("Start Here " + finalMaze.mRuleAssignments.Count);
            //foreach (KeyValuePair<Tile, MovementRule> mrAssignment in finalMaze.mRuleAssignments)
            //{
            //    Debug.Log(mrAssignment.Key.name + "x  " + mrAssignment.Value);
            //}
            //foreach (KeyValuePair<Tile, ColourRule> mrAssignment in finalMaze.cRuleAssignments)
            //{
            //    Debug.Log(mrAssignment.Key.name + "x  " + mrAssignment.Value);
            //}


            //REASSIGN AND COLOUR TILES
            finalMazePrefab.GetComponentInChildren<ColourAssigner>().ResetGrid(finalMaze.maze.tiles);
            foreach (KeyValuePair<Tile, MovementRule> mrAssignment in finalMaze.mRuleAssignments)
            {
                finalMazePrefab.GetComponentInChildren<ColourAssigner>().AssignMRule(mrAssignment.Key, mrAssignment.Value);
            }
            foreach (KeyValuePair<Tile, ColourRule> crAssignment in finalMaze.cRuleAssignments)
            {
                finalMazePrefab.GetComponentInChildren<ColourAssigner>().AssignCRule(crAssignment.Key, crAssignment.Value);
            }
            foreach (Tile t in finalMaze.wallAssignments)
            {
                t.failedToAssign = true;
                t.ruleType = Type.wall;
                t.colour = Colour.Black;
                SpriteRenderer sr = t.GetComponent<SpriteRenderer>();
                Material black = (Material)Resources.Load("Black");
                sr.material.shader = black.shader;
                sr.material.color = black.color;
            }


            //ADD CHECKERS AND NUMBER CLUES
            Shinro.PlaceCheckers(finalMaze.spaths.mediumPath, finalMaze, .3f);
            NumClues.SetClues(finalMaze.maze.tiles);

            //SET UP PLAYER CONTROLLER (steps, undos, etc.)
            PlayerController.SetupPlayerController(finalMaze);
        }

        //Ending
    }
}
