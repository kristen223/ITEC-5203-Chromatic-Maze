using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shinro : MonoBehaviour
{

    public static KruskalMaze.Maze maze;
    private Tile[] path;

    // Start is called before the first frame update
    void Start()
    {
        maze = GenerateGrid.maze;
        //Debug.Log(maze.deadends);

        path = GetPath(maze.LP.entrance, maze.LP.length);
        PlaceCheckers(path);
    }

    public Tile[] GetPath(Tile startPoint, int length)
    {
        Tile[] pathh = new Tile[length];

        for(int i = 0; i < length; i++)
        {
            pathh[i] = startPoint;
            startPoint = startPoint.parent;
        }

        return pathh;
    }

    public void PlaceCheckers(Tile[] path)
    {
        int count = Mathf.RoundToInt((path.Length - 2)*.3f);

        //place a checker on a random tile on the solution path that IS NOT the entrance or exit
        if (count < path.Length - 2) //if number of checkers fits on the path
        {
            for (int i = 0; i < count; i++)
            {
                path[Random.Range(1, path.Length - 1)].transform.Find("Checker").gameObject.GetComponent<SpriteRenderer>().enabled = true;
            }
        }
        else
        {

        }
    }

    //private static void Print(Tile[] path)
    //{
    //    for (int i = 0; i < path.Length; ++i)
    //    {
    //        Debug.Log(path[i]);
    //    }
    //}
}
