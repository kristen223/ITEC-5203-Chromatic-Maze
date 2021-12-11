using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeCreation : MonoBehaviour
{
    // Start is called before the first frame update
    private GameObject mazePrefab; //put this at the top

    void Start()
    {
        mazePrefab = (GameObject)Resources.Load("ColourAssigner"); //put this in Start method
    }


    public static void getFinalRules(Dictionary<int, int> chosenChr, Dictionary<int, Dictionary<int, Type>> clist, List<MovementRule> m, List<ColourRule> c)
    {
        //int[][] chromosomes = new int[2][];


        
        foreach (KeyValuePair<int, int> k in chosenChr)
        {



            List<MovementRule> mr = new List<MovementRule>();
            List<ColourRule> cr = new List<ColourRule>();
            if (clist.ContainsKey(k.Key)) //chosenChr.key = clist.key
            {
                foreach (Dictionary<int, Type> d in clist.Values)
                {
                    foreach (KeyValuePair<int, Type> kvp in d)
                    {
                        if (kvp.Value == Type.exclude || kvp.Value == Type.include)
                        {
                            cr.Add(Fitness1.GetCRule(kvp.Key, c));
                        }
                        else
                        {
                            mr.Add(Fitness1.GetMRule(kvp.Key, m));
                        }
                        // ChosenRulesIdx.Add(kvp.Key);


                    }
                }

            }

            //CALL SETRULES HERE , have mr,cr
            setrules(mr, cr);
            //chromosomes.Add(mr);
            //chromosomes.Add(cr);

        }
        //MazeCreation.mazeC(chromosomes);

    }





    //add this once you have final list of chromosomes
    //create list of prefabs and their coloured mazes for each chromosome

    public static void mazeC(ArrayList chromosomes)   //an arraylist of chromosomes. its like : chromosomes=>[mr,cr,mr,cr,mr,cr,mr,cr.....]
    {
        List<GameObject> prefabs = new List<GameObject>();
        List<ColourAssigner.ColouredMaze> cmazes = new List<ColourAssigner.ColouredMaze>(); //list of coloured mazes, one for each chromosome
        foreach (var item in chromosomes)
        {
            GameObject maze = Instantiate(mazePrefab, new Vector3(0, 0, 0), Quaternion.Euler(0, 0, 0));
            prefabs.Add(maze);



            List<MovementRule> mr = new List<MovementRule>();
            List<ColourRule> cr = new List<ColourRule>();
            

            maze.GetComponent<ColourAssigner>().SetRules(mr, cr); //set the maze rules to this chromosome's rules
            cmazes.Add(maze.GetComponent<ColourAssigner>().ColourMaze()); //colour the maze and add it to the list
        }

        //compare mazes and pick one (fitness 2)

        //delete all prefabs except the chosen one

        //temp variable to represent the chosen maze
        ColourAssigner.ColouredMaze chosen = new ColourAssigner.ColouredMaze();

        //set Tile components of tile game objects in grid
        for (int i = 0; i < GenerateGrid.tiles.Length; i++)
        {
            Component chosenComponent = chosen.maze.tiles[i].GetComponent<Tile>();

            System.Type type = chosenComponent.GetType();

            System.Reflection.FieldInfo[] fields = type.GetFields();
            foreach (System.Reflection.FieldInfo field in fields)
            {
                field.SetValue(GenerateGrid.tiles[i].GetComponent<Tile>(), field.GetValue(chosenComponent));
            }
        }

        //Set all of the player controller variables and texts here (I’ll do this)
    }
}
   
