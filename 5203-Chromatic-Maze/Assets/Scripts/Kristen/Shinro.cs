using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shinro : MonoBehaviour
{
    [HideInInspector] public Tile[] tiles;
    public static KruskalMaze.Maze maze;
    private Tile[] path;

    // Start is called before the first frame update
    void Start()
    {
        tiles = GenerateGrid.vertices;
        maze = GenerateGrid.maze;
        //Debug.Log(maze.deadends);

        path = GetPath(maze.LP.entrance, maze.LP.length);
        PlaceCheckers(path);
        NumClues.SetClues(tiles);
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
        int count = Mathf.RoundToInt((path.Length - 2)*.3f); //place checkers on 30% of path length minus entrance and exit

        //place a checker on a random tile on the solution path that IS NOT the entrance or exit
        for (int i = 0; i < count; i++)
        {
            int rand = Random.Range(1, path.Length - 1);
            path[rand].transform.Find("Checker").gameObject.GetComponent<SpriteRenderer>().enabled = true;
            path[rand].tag = "checker";
        }
    }
}
