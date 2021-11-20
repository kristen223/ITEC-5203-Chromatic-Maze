using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// A tile represents a vertex, a node in the tree
public class Tile : MonoBehaviour
{
    //The Vertex
    public GameObject tilepfab;
    public Material colour;

    public Tile parent;
    public Tile child; //may change to list later
    public int rank;
    public bool border;

    //This find the root noed of the subset of t
    //Equivalent to Kruskal's Find Function
    public static Tile GetRootParent(Tile t)
    {
        if (t.parent == t) { //like setting the parent to -1 in the example
            return t;
        }

        else{
            return GetRootParent(t.parent); //Goes up the tree until it can't anymore
        }
    }
}

/*
 * Current state of player will have the current colour they're on
 * When they tap on a tile:
 *      - Check if its orthogonally adjecent
 *      - Get the colour of the tapped tile
 *      - Based on rule system, check if move i sacceptable
 */