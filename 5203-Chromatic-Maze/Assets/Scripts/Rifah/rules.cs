using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//structs cannot inehrit , used for small groups of variables

public struct MovementRules
{
    public int direction; //N,S,E,W=1111,  dont care=-1
    public int distance; //dont care = -1
    public int color; //-1 means dont care , 1=yellow, 2=orange, 3=red, 4=green, 5=blue, 6=purple
}

public struct ColorRules
{
    public bool inclusion;
    public int color;
}

public class Rules : MonoBehaviour
{
    public MovementRules Tmove;
    public MovementRules blank;
    public MovementRules teleportB;
    public MovementRules teleportP;
    public MovementRules teleportG;
    public MovementRules teleportR;
    public MovementRules teleportO;
    public MovementRules teleportY;
    public MovementRules jumpOne;
    public MovementRules jumpTwo;
    public MovementRules warmTemp;
    public MovementRules coldTemp;
    public ColorRules excludeR;
    public ColorRules excludeO;
    public ColorRules excludeY;
    public ColorRules excludeB;
    public ColorRules excludeG;
    public ColorRules excludeP;
    public ColorRules block;

    void start()
    {
        defineRules();
       

    }
    public void defineRules()
    {
        
        Tmove.direction = 1011;
        Tmove.distance = 1;
        Tmove.color = -1;

       
        blank.direction = 1111;
        blank.distance = 1;
        blank.color = -1;

        
        teleportB.direction = -1;
        teleportB.distance = -1;
        teleportB.color = 5;

  
        teleportP.direction = -1;
        teleportP.distance = -1;
        teleportP.color = 6;

       
        teleportG.direction = -1;
        teleportG.distance = -1;
        teleportG.color = 4;

        
        teleportR.direction = -1;
        teleportR.distance = -1;
        teleportR.color = 3;

        
        teleportO.direction = -1;
        teleportO.distance = -1;
        teleportO.color = 2;

        
        teleportY.direction = -1;
        teleportY.distance = -1;
        teleportY.color = 1;

        
        jumpOne.direction = 1111;
        jumpOne.distance = 2;
        jumpOne.color = -1;

       
        jumpTwo.direction = 1111;
        jumpTwo.distance = 3;
        jumpTwo.color = -1;

        
        warmTemp.direction = 1111;
        warmTemp.distance = 1;
        warmTemp.color = 1 | 2 | 3; //does this work or should it be seperate?


       
        coldTemp.direction = 1111;
        coldTemp.distance = 1;
        coldTemp.color = 4 | 5 | 6;


        ColorRules includeY;// if blue, goes to yellow
        // int currentpos = 9999;//take from the maze/how to keep track of prevs position when maze is not built?
        //if(currentpos.color==1)
        //{
        includeY.color = 1;
        //currentpos.color + 1;
        includeY.inclusion = true;

        
        excludeR.color = 1 | 2 | 4 | 5 | 6;
        excludeR.inclusion = false;

   
        excludeO.color = 1 | 3 | 4 | 5 | 6;
        excludeO.inclusion = false;

     
        excludeY.color = 3 | 2 | 4 | 5 | 6;
        excludeY.inclusion = false;

        
        excludeB.color = 1 | 2 | 4 | 3 | 6;
        excludeB.inclusion = false;

        
        excludeG.color = 1 | 2 | 3 | 5 | 6;
        excludeG.inclusion = false;

      
        excludeP.color = 1 | 2 | 4 | 5 | 3;
        excludeP.inclusion = false;

        ColorRules block; //block is the same as exclude C if we dont keep track of prevs position

    }


}






