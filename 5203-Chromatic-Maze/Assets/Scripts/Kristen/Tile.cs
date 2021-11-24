using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// A tile represents a vertex, a node in the tree
public class Tile : MonoBehaviour
{
    //The Vertex
    public GameObject tilepfab;
    public Material colour;

    //one will be null
    public MovementRules mRule;
    public ColorRules cRule;

    public int ruleType;
    public bool jumpN;
    public bool jumpS;
    public bool jumpE;
    public bool jumpW;
    public bool jumpTwoN;
    public bool jumpTwoS;
    public bool jumpTwoE;
    public bool jumpTwoW;

    public Tile parent;
    public List<Tile> children; //may change to list later
    public int rank;
    public bool border;


    private void Awake()
    {
        jumpN = false;
        jumpS = false;
        jumpE = false;
        jumpW = false;
        jumpTwoN = false;
        jumpTwoS = false;
        jumpTwoE = false;
        jumpTwoW = false;
    }

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