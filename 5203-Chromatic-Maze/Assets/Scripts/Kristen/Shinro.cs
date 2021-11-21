using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shinro : MonoBehaviour
{
    [HideInInspector] public Tile[] tiles;
    public static KruskalMaze.Maze maze;
    private Tile[] solutionPath;
    private  int chance; //likelihood checker is placed (percentage)
    public static int checkerCount;

    void Awake()
    {
        tiles = GenerateGrid.vertices;
        maze = GenerateGrid.maze;
        chance = 10;

        checkerCount = Mathf.RoundToInt((maze.LP.length - 1) * .3f);
        PlaceCheckers(maze.LP.entrance, maze.LP.length, checkerCount);
        NumClues.SetClues(tiles);
    }

    public void PlaceCheckers(Tile startPoint, int length, int count)
    {
        for (int i = 0; i < length-1; i++)
        {
            Tile current = startPoint.parent;

            if(i == length - count - 1) //if the number of checkers left to place is equal to the number of tiles left in path traversal
            {//must add checkers to rest of path
                current.transform.Find("Checker").gameObject.GetComponent<SpriteRenderer>().enabled = true;
                current.tag = "checker";
                count--;
                chance = 10;
            }
            else
            {
                if (Random.Range(1, 101) <= chance)
                {
                    current.transform.Find("Checker").gameObject.GetComponent<SpriteRenderer>().enabled = true;
                    current.tag = "checker";
                    count--;
                    chance = 10;
                }
                else //no checker added
                {
                    chance += 10;
                }

                if (count == 0)
                {
                    break;
                }
            }
            startPoint = startPoint.parent;
        }
    }

    //Old way of placing checkers that places tehm randomly

    //public Tile[] GetPath(Tile startPoint, int length)
    //{
    //    Tile[] pathh = new Tile[length];

    //    for (int i = 0; i < length; i++)
    //    {
    //        pathh[i] = startPoint;
    //        startPoint = startPoint.parent;
    //    }

    //    return pathh;
    //}

    //public void PlaceCheckers(Tile[] path)
    //{
    //    int count = Mathf.RoundToInt((path.Length - 2)*.3f); //place checkers on 30% of path length minus entrance and exit

    //    //place a checker on a random tile on the solution path that IS NOT the entrance or exit
    //    for (int i = 0; i < count; i++)
    //    {
    //        int rand = Random.Range(1, path.Length - 1);
    //        path[rand].transform.Find("Checker").gameObject.GetComponent<SpriteRenderer>().enabled = true;
    //        path[rand].tag = "checker";
    //    }
    //}
}

//In Shinro puzzles, the size is similar but there's way more checkers placed whic amkes it less trivial

//go along path, each time you don't place a checker, the likelihood of you placing a checker on teh next tile increases
//likelihood lowers when you place a checker