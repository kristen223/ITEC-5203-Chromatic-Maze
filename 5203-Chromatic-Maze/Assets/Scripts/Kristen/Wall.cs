using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour
{

    //The edge

    public Tile origin; //source vertex (tile at tiles[0])
    public Tile destination; //destination vertex (tile at tiles[1])
    public int weight; //weight of edge

    public Tile[] connectedTiles;



    public void disableEdge() //update
    {
        gameObject.GetComponent<SpriteRenderer>().enabled = false;
    }

    public void enableEdge() //update
    {
        gameObject.GetComponent<SpriteRenderer>().enabled = true;
    }


}
