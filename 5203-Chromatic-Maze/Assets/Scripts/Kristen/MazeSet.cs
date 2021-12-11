using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeSet : MonoBehaviour
{

    

        //private GameObject mazePrefab; //put this at the top

        //void Start()
        //{
        //mazePrefab = (GameObject)Resources.Load("ColourAssigner"); //put this in Start method
        //}

        ////add this once you have final list of chromosomes
        ////create list of prefabs and their coloured mazes for each chromosome
        //List<GameObject> prefabs = new List<GameObject>();
        //List<ColourAssigner.ColouredMaze> cmazes = new List<ColourAssigner.ColouredMaze>(); //list of coloured mazes, one for each chromosome
        //foreach (Chromosome r in ChromosomesList)
        //{
        //    GameObject maze = Instantiate(mazePrefab, new Vector3(0, 0, 0), Quaternion.Euler(0, 0, 0));
        //    prefabs.Add(maze);
        //    maze.GetComponent<ColourAssigner>().SetRules(mr, cr); //set the maze rules to this chromosome's rules
        //    cmazes.Add(maze.GetComponent<ColourAssigner>().ColourMaze()); //colour the maze and add it to the list
        //}

        ////compare mazes and pick one (fitness 2)

        ////delete all prefabs except the chosen one

        ////temp variable to represent the chosen maze
        //ColourAssigner.ColouredMaze chosen = new ColourAssigner.ColouredMaze();

        ////set Tile components of tile game objects in grid
        //for(int i = 0; i < GenerateGrid.tiles.Length; i++)
        //{
        //    Component chosenComponent = chosen.maze.tiles[i].GetComponent<Tile>();

        //    System.Type type = chosenComponent.GetType();

        //    System.Reflection.FieldInfo[] fields = type.GetFields();
        //    foreach (System.Reflection.FieldInfo field in fields)
        //    {
        //        field.SetValue(GenerateGrid.tiles[i].GetComponent<Tile>(), field.GetValue(chosenComponent));
        //    }
        //}

        ////Set all of the player controller variables and texts here

}
