using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeCreation : MonoBehaviour
{
    // Start is called before the first frame update
    private static GameObject mazePrefab; //put this at the top
    private static List<GameObject> prefabs; //list of colour assigner prefabs
    private static List<ColourAssigner.ColouredMaze> cmazes; //list of coloured mazes, one for each chromosome
    public static Colour[] allcolors = new Colour[8] { Colour.Red, Colour.Orange, Colour.Yellow, Colour.Pink, Colour.Teal, Colour.Blue, Colour.Purple, Colour.Green };

    void Awake()
    {
        mazePrefab = (GameObject)Resources.Load("ColourAssigner"); //put this in Start method
        prefabs = new List<GameObject>();
        cmazes = new List<ColourAssigner.ColouredMaze>();
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
                            Debug.Log("src color is-----" + z.src);
                        }
                        else
                        {
                            MovementRule y = Fitness1.GetMRule(kvp.Key, m);
                            //Debug.Log("original color was---------------------------------" + y.src);
                            //y.src = allcolors[i];
                           // i++;
                           // Debug.Log("new color is------------------------- " + y.src);
                            mr.Add(y);
                            Debug.Log("src color is-----" + y.src);
                        }
                        // ChosenRulesIdx.Add(kvp.Key);

                    }
                    //Debug.Log("total mr : " + mr.Count);
                    //Debug.Log("total cr : " + cr.Count);

                    //resetting indexes
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

                    List<MovementRule> mrCopy = new List<MovementRule>();
                    foreach(MovementRule mcopy in mr)
                    {
                        mrCopy.Add(mcopy);
                    }
                    List<ColourRule> crCopy = new List<ColourRule>();
                    foreach (ColourRule ccopy in cr)
                    {
                        crCopy.Add(ccopy);
                    }

                    //Making two mazes per set of rules
                    GameObject maze = Instantiate(mazePrefab, new Vector3(0, 0, 0), Quaternion.Euler(0, 0, 0));
                    maze.name = "Prefab-" + counter;
                    counter++;
                    prefabs.Add(maze);

                    maze.GetComponent<ColourAssigner>().SetRules(mr, cr); //set the maze rules to the current chromosome's rules
                    cmazes.Add(maze.GetComponent<ColourAssigner>().ColourMaze()); //colour the maze and add it to the list

                    GameObject mazeTwo = Instantiate(mazePrefab, new Vector3(0, 0, 0), Quaternion.Euler(0, 0, 0));
                    mazeTwo.name = "Prefab-" + counter;
                    counter++;
                    prefabs.Add(mazeTwo);

                    mazeTwo.GetComponent<ColourAssigner>().SetRules(mrCopy, crCopy);
                    cmazes.Add(mazeTwo.GetComponent<ColourAssigner>().ColourMaze());
                }


                //List<MovementRule> mr = new List<MovementRule>();
                //List<ColourRule> cr = new List<ColourRule>();
                //if (clist.ContainsKey(k.Key)) //chosenChr.key = clist.key
                //{
                //    foreach (Dictionary<int, Type> d in clist.Values)
                //    {
                //        foreach (KeyValuePair<int, Type> kvp in d)
                //        {
                //            if (kvp.Value == Type.exclude || kvp.Value == Type.include)
                //            {
                //                cr.Add(Fitness1.GetCRule(kvp.Key, c));
                //            }
                //            else
                //            {
                //                mr.Add(Fitness1.GetMRule(kvp.Key, m));
                //            }
                //            // ChosenRulesIdx.Add(kvp.Key);
                //        }
                //    }
                //}

            }
        }

        //FITNESS 2
        ColourAssigner.ColouredMaze finalMaze = PickMaze.GetFinalMaze(cmazes, prefabs);

        if(finalMaze.spaths.allPaths != null) //if maze was chosen
        {
            //TEMP DEBUG STUFF
            string ss = "xxfinal rules: ";
            foreach (MovementRule g in finalMaze.mr)
            {
                ss += g.type + "-" + g.src + ", ";
            }
            foreach (ColourRule h in finalMaze.cr)
            {
                ss += h.type + "-" + h.src + ", ";
            }
            Debug.Log(ss);
            Debug.Log("xxnumber of paths: " + finalMaze.spaths.allPaths.Count);

            string debugs = "xxShortest path: ";
            foreach (Tile t in finalMaze.spaths.shortestPath)
            {
                debugs += t.name + ", ";
            }
            Debug.Log(debugs);

            string debugsss = "xxMedium path: ";
            foreach (Tile t in finalMaze.spaths.mediumPath)
            {
                debugsss += t.name + ", ";
            }
            Debug.Log(debugsss);

            string debug = "xxLongest path: ";
            foreach (Tile t in finalMaze.spaths.longestPath)
            {
                debug += t.name + ", ";
            }
            Debug.Log(debug);

            //SET INSTRUCTIONS TEXT
            InstructionsText.SetInstructions(finalMaze.mr, finalMaze.cr);

            //SET TILE GRID TO CORRECT MAZE'S TILES AND RESET CHECKERS
            for (int i = 0; i < GenerateGrid.tiles.Length; i++)
            {
                GenerateGrid.tiles[i].tag = "Untagged";
                GenerateGrid.tiles[i].transform.Find("Checker").GetComponent<SpriteRenderer>().enabled = false;

                Component chosenComponent = finalMaze.maze.tiles[i].GetComponent<Tile>();

                System.Type type = chosenComponent.GetType();

                System.Reflection.FieldInfo[] fields = type.GetFields();
                foreach (System.Reflection.FieldInfo field in fields)
                {
                    field.SetValue(GenerateGrid.tiles[i].GetComponent<Tile>(), field.GetValue(chosenComponent));
                }
            }

            //ADD CHECKERS
            Shinro.PlaceCheckers(finalMaze.spaths.mediumPath, finalMaze, .3f);

            //SET UP PLAYER CONTROLLER (steps, undos, etc.)
            PlayerController.SetupPlayerController(finalMaze);
        }




        //Ending
    }
}
