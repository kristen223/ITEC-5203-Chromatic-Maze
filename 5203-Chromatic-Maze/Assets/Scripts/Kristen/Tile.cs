using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// A tile represents a vertex, a node in the tree
public class Tile : MonoBehaviour
{
    //The Vertex
    //public GameObject tilepfab;

    //one will be null
    public MovementRule mRule;
    public ColourRule cRule;
    public bool moveRule;
    public int index;

    //these should be set to the same thing
    public Colour colour;
    public bool assigned;
    public bool failedToAssign;

    public Type ruleType;
    public bool jumpN;
    public bool jumpS;
    public bool jumpE;
    public bool jumpW;
    public bool jumpTwoN;
    public bool jumpTwoS;
    public bool jumpTwoE;
    public bool jumpTwoW;

    public Dictionary<Colour, bool> canBe;

    public Tile parent;
    public List<Tile> children; //may change to list later
    public int rank;
    public bool border;
    public int passedChecker;

    private void Awake()
    {
        assigned = false;
        failedToAssign = false;
        jumpN = false;
        jumpS = false;
        jumpE = false;
        jumpW = false;
        jumpTwoN = false;
        jumpTwoS = false;
        jumpTwoE = false;
        jumpTwoW = false;
        moveRule = false;

        canBe = new Dictionary<Colour, bool>()
        {
            {Colour.Red, true},
            {Colour.Orange, true},
            {Colour.Yellow, true},
            {Colour.Green, true},
            {Colour.Blue, true},
            {Colour.Purple, true},
        };

        passedChecker = 0;
        rank = 0;
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
