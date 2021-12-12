using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeCreation : MonoBehaviour
{
    // Start is called before the first frame update
    private static GameObject mazePrefab; //put this at the top
    private static List<GameObject> prefabs; //list of colour assigner prefabs
    private static List<ColourAssigner.ColouredMaze> cmazes; //list of coloured mazes, one for each chromosome

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

                    foreach (KeyValuePair<int, Type> kvp in x.Value) //supposed to run 8 times
                    {
                        if (kvp.Value == Type.exclude || kvp.Value == Type.include)
                        {
                            ColourRule z = Fitness1.GetCRule(kvp.Key, c);
                            cr.Add(z);
                        }
                        else
                        {
                            mr.Add(Fitness1.GetMRule(kvp.Key, m));
                        }
                        // ChosenRulesIdx.Add(kvp.Key);

                    }
                    //Debug.Log("total mr : " + mr.Count);
                    //Debug.Log("total cr : " + cr.Count);
                    GameObject maze = Instantiate(mazePrefab, new Vector3(0, 0, 0), Quaternion.Euler(0, 0, 0));
                    maze.name = "Prefab-" + counter;
                    counter++;
                    prefabs.Add(maze);

                    maze.GetComponent<ColourAssigner>().SetRules(mr, cr); //set the maze rules to the current chromosome's rules
                    cmazes.Add(maze.GetComponent<ColourAssigner>().ColourMaze()); //colour the maze and add it to the list

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

            //Set the instruction text
            ColourAssigner.ColouredMaze finalMaze = cmazes[5];
            InstructionsText.SetInstructions(finalMaze.mr, finalMaze.cr);

            for (int i = 0; i < GenerateGrid.tiles.Length; i++)
            {
                Component chosenComponent = finalMaze.maze.tiles[i].GetComponent<Tile>();

                System.Type type = chosenComponent.GetType();

                System.Reflection.FieldInfo[] fields = type.GetFields();
                foreach (System.Reflection.FieldInfo field in fields)
                {
                    field.SetValue(GenerateGrid.tiles[i].GetComponent<Tile>(), field.GetValue(chosenComponent));
                }
            }


         Debug.Log("checkers " + finalMaze.checkers);
        
    }
    

    /*

    List of coloured mazes now exists
    1.  Call fitness 2 here (with list of coloured mazes and prefabs a parameter) and return a single coloured maze
        **inside fitness two, once final maze is picked, delete all other prefabs and coloured mazes from lists

        CODE: ColourAssigner.ColouredMaze finalMaze = fitnessc(cmazes, prefabs);



    2. Set Tile components of tile game objects in grid

        for (int i = 0; i < GenerateGrid.tiles.Length; i++)
        {
            Component chosenComponent = finalMaze.maze.tiles[i].GetComponent<Tile>();

            System.Type type = chosenComponent.GetType();

            System.Reflection.FieldInfo[] fields = type.GetFields();
            foreach (System.Reflection.FieldInfo field in fields)
            {
                field.SetValue(GenerateGrid.tiles[i].GetComponent<Tile>(), field.GetValue(chosenComponent));
            }
        }

    3. Set all of the player controller variables and texts according to finalMaze

    */
}




//    public static void getFinalRules(Dictionary<int, int> chosenChr, Dictionary<int, Dictionary<int, Type>> clist, List<MovementRule> m, List<ColourRule> c)
//    {
//        //int[][] chromosomes = new int[2][];
//        Debug.Log("reached get final rules");
//        foreach (KeyValuePair<int, int> k in chosenChr) //suppose to run (20% of popsize) times
//        {


//            foreach(KeyValuePair<int, Dictionary<int, Type>> x in clist)
//            {
//                if (k.Key == x.Key)
//                {
//                    List<MovementRule> mr = new List<MovementRule>();
//                    List<ColourRule> cr = new List<ColourRule>();
//                    foreach (Dictionary<int, Type> d in clist.Values) //suppose to
//                    {

//                        foreach (KeyValuePair<int, Type> kvp in d) //supposed to run 8 times
//                        {
//                            if (kvp.Value == Type.exclude || kvp.Value == Type.include)
//                            {
//                                Debug.Log(kvp.Value + "so adding to cr");
//                                ColourRule z = Fitness1.GetCRule(kvp.Key, c);
//                                Debug.Log("this rule " + z.index);
//                                cr.Add(z);
//                            }
//                            else
//                            {
//                                mr.Add(Fitness1.GetMRule(kvp.Key, m));
//                            }
//                            // ChosenRulesIdx.Add(kvp.Key);
//                        }
//                    }
//                    Debug.Log("total mr : " + mr.Count);
//                    Debug.Log("total cr : " + cr.Count);
//                }


//            }
//        }

//    }
//}

