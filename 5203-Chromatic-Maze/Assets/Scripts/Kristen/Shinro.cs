using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shinro : MonoBehaviour
{

    public float percentOfLongest;
    public float percentOfShortest;
    public static float percentL;
    public static float percentS;
    private static int chance; //likelihood checker is placed (percentage)
      
    void Start()
    {
        chance = 10;
        percentL = percentOfLongest;
        if (percentL <= 0)
        {
            percentL = .3f; //30% of longest path
        }

        percentS = percentOfShortest;
        if (percentS <= 0)
        {
            percentS = .3f; //30% of longest path
        }

    }

    public static void PlaceCheckers(List<Tile> path, ColourAssigner.ColouredMaze cmaze, float percent)
    {
        int length = path.Count;
        int checkerCount = Mathf.RoundToInt((length - 1) * percent);
        Tile previous = path[0]; //skip the entrance by making it previous

        for (int i = 0; i < path.Count-1; i++)
        {
            Tile current = previous.parent;

            if(i == length - checkerCount - 1) //if the number of checkers left to place is equal to the number of tiles left in path traversal
            {//must add checkers to rest of path
                if(current.tag != "checker") //may already be placed here
                {
                    current.transform.Find("Checker").gameObject.GetComponent<SpriteRenderer>().enabled = true;
                    current.tag = "checker";
                }
                checkerCount--; //want to subtract count regardless
                chance = 10;
            }
            else
            {   //don't place checker after jump or teleport
                if (Random.Range(1, 101) <= chance && previous.ruleType != Type.jump1 && previous.ruleType != Type.jump2 && previous.ruleType != Type.teleport)
                {
                    if (current.tag != "checker")
                    {
                        current.transform.Find("Checker").gameObject.GetComponent<SpriteRenderer>().enabled = true;
                        current.tag = "checker";
                    }
                    checkerCount--;
                    chance = 10;
                }
                else //no checker added
                {
                    chance += 15;
                }

                if (checkerCount == 0)
                {
                    break;
                }
            }
            previous = previous.parent;
        }
    }
}