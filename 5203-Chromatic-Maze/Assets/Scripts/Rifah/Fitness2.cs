using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fitness2 : MonoBehaviour
{


    //num of deadEnds
    //num of checkers
    //min num of moves (dynamic algo?)
    //actual num of moves
    //if you want to make it more difficult => maximise the #of moves
    public struct ColouredMaze
    {
        public KruskalMaze.Maze maze;
        public Dictionary<int, int> used; //rule index and amount of times rule was used
        public int checkers; //number of checkers
        public bool properExit; //true if exit was assigned properly, false if exit was assigned to be traversable via solution path, but violates rule(s) with other adjacent tiles (not added to onPathViolations because it doesn't wreck the path)
        public int onPathViolations; //number of tiles that violate rule(s) of one or more adjacent tiles on solution path (not including CyclesAdded count)
        public int offPathViolations; //number of tiles that violate rule(s) of one or more adjacent tiles off solution path (not including CyclesAdded count)
    }



    public static void fitness2(ColourAssigner.ColouredMaze colouredMaze)
    {
        
    }
}
