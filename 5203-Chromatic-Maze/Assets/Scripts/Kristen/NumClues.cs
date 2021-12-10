using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NumClues : MonoBehaviour
{
    
    [HideInInspector] private static int width;
    [HideInInspector] private static int height;
    [HideInInspector] public static int checkerCount;

    private void Start()
    {
        checkerCount = 0;
    }

    public static void SetClues(Tile[] tiles)
    {
        width = GenerateGrid.wdth;
        height = GenerateGrid.hght;

        //Checking columns
        for (int j = 0; j < width; j++) // j = column number
        {
            int checkers = 0;
            for (int i = 0; i < height; i++) //i = tile number
            {
                if (tiles[j+(i*width)].tag == "checker")
                {
                    checkers++;
                    checkerCount++;
                }
            }
            GameObject.Find("TopWall-" + (j+1)).GetComponentInChildren<Text>().text = checkers.ToString();
            //get wall prefab and set its text to checkers
        }

        //Checking rows
        for (int j = 0; j < height; j++) // j = column number
        {
            int checkers = 0;
            for (int i = 1; i <= width; i++) //i = tile number
            {
                if (tiles[i + (j*width) - 1].tag == "checker")
                {
                    checkers++;
                }
            }
            GameObject.Find("LeftWall-" + (j+1)).GetComponentInChildren<Text>().text = checkers.ToString(); ;
        }
    }

}
