using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//structs cannot inehrit , used for small groups of variables

public struct MovementRules
{
    public int direction; //N,S,E,W=1111,  dont care=-1
    public int distance; //dont care = -1
    public int color; //-1 means dont care , 1=yellow, 2=orange, 3=red, 4=green, 5=blue, 6=purple
    public int w; //assigning not done

    public static implicit operator MovementRules(List<MovementRules> v)
    {
        throw new NotImplementedException();
    }
}

public struct ColorRules
{
    public bool inclusion;
    public int color;
    public int w;//assigning not done 

    public static implicit operator ColorRules(List<ColorRules> v)
    {
        throw new NotImplementedException();
    }
}
//two lists => same length,two chromosomes, genetic algo

public class Rules : MonoBehaviour
{

    //global variables => rules
    private MovementRules Tmove;
    private MovementRules blank;
    private MovementRules teleportB;
    private MovementRules teleportP;
    private MovementRules teleportG;
    private MovementRules teleportR;
    private MovementRules teleportO;
    private MovementRules teleportY;
    private MovementRules jumpOne;
    private MovementRules jumpTwo;
    private MovementRules warmTemp;
    private MovementRules coldTemp;
    private ColorRules excludeR;
    private ColorRules excludeO;
    private ColorRules excludeY;
    private ColorRules excludeB;
    private ColorRules excludeG;
    private ColorRules excludeP;
    private ColorRules block;

    public MovementRules movementRuleSets = new List<MovementRules>();
    public ColorRules colorRuleSets = new List<ColorRules>();

    void start()
    {
        defineRules();
        fitnessOne();
       

    }
    public void defineRules()
    {
        
        Tmove.direction = 1011;
        Tmove.distance = 1;
        Tmove.color = -1;
        movementRuleSets.Add(Tmove);


        blank.direction = 1111;
        blank.distance = 1;
        blank.color = -1;
        movementRuleSets.Add(blank);

        teleportB.direction = -1;
        teleportB.distance = -1;
        teleportB.color = 5;
        movementRuleSets.Add(teleportB);


        teleportP.direction = -1;
        teleportP.distance = -1;
        teleportP.color = 6;
        movementRuleSets.Add(teleportP);

        teleportG.direction = -1;
        teleportG.distance = -1;
        teleportG.color = 4;
        movementRuleSets.Add(teleportG);


        teleportR.direction = -1;
        teleportR.distance = -1;
        teleportR.color = 3;
        movementRuleSets.Add(teleportR);

        teleportO.direction = -1;
        teleportO.distance = -1;
        teleportO.color = 2;
        movementRuleSets.Add(teleportO);


        teleportY.direction = -1;
        teleportY.distance = -1;
        teleportY.color = 1;
        movementRuleSets.Add(teleportY);


        jumpOne.direction = 1111;
        jumpOne.distance = 2;
        jumpOne.color = -1;
        movementRuleSets.Add(jumpOne);


        jumpTwo.direction = 1111;
        jumpTwo.distance = 3;
        jumpTwo.color = -1;
        movementRuleSets.Add(jumpTwo);

        warmTemp.direction = 1111;
        warmTemp.distance = 1;
        warmTemp.color = 1 | 2 | 3; //does this work or should it be seperate?
        movementRuleSets.Add(warmTemp);


        coldTemp.direction = 1111;
        coldTemp.distance = 1;
        coldTemp.color = 4 | 5 | 6;
        movementRuleSets.Add(coldTemp);

        ColorRules includeY;// if blue, goes to yellow
        // int currentpos = 9999;//take from the maze/how to keep track of prevs position when maze is not built?
        //if(currentpos.color==1)
        //{
        includeY.color = 1;
        //currentpos.color + 1;
        includeY.inclusion = true;

        
        excludeR.color = 1 | 2 | 4 | 5 | 6;
        excludeR.inclusion = false;
        //add to the list
   
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

    public void fitnessOne(List<MovementRules> m, List<ColorRules> c) 
    {
        int w1, w2;

        for (int i = 0; i < m.Count; i++)
        {
            w1 = m[i].w;
            w2 = c[i].w;
            
           
        }

        //fitness = [chromosome[1] for chromosome in population ]




}






