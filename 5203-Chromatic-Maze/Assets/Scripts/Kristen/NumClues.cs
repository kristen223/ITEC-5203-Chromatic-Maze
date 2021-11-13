using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NumClues : MonoBehaviour
{

    [HideInInspector] private static int width;
    [HideInInspector] private static int height;

    void Start()
    {
        width = GenerateGrid.wdth;
        height = GenerateGrid.hght;
    }

    public static void SetClues(Tile[] tiles)
    {
        for(int j = 1; j <= height; j++) // j = column number
        {
            int checkers = 0;
            for (int i = 0; i < height; i += width) //i = tile number
            {
                if (tiles[i].tag == "checker")
                {
                    checkers++;
                }
            }
            GameObject.Find("TopWall-" + j).GetComponentInChildren<Text>().text = checkers.ToString();
            //get wall prefab and set its text to checkers
        }

        for (int j = 1; j <= width; j++) // j = column number
        {
            int checkers = 0;
            for (int i = 0; i < width; i += height) //i = tile number
            {
                if (tiles[i].tag == "checker")
                {
                    checkers++;
                }
            }
            GameObject.Find("LeftWall-" + j).GetComponentInChildren<Text>().text = checkers.ToString(); ;
            //get wall prefab and set its text to checkers
        }
    }

}
