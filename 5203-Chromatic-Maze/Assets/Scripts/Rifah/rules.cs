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
    public int type; //assigning not done

    public static implicit operator MovementRules(List<MovementRules> v)
    {
        throw new NotImplementedException();
    }
}

public struct ColorRules  
{
    public bool inclusion;
    public int src;
    public int target;
    public int type;//assigning not done 

    public static implicit operator ColorRules(List<ColorRules> v)
    {
        throw new NotImplementedException();
    }
}

public struct RuleTypes //chromosomes (total 10 types of rules)
{
    string name;
    int type;
}

public class Rules : MonoBehaviour
{

    //global variables => rules 
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
    public ColorRules includeBY; //if this is applied, other B? cant be applied.Make sure this rule is not the same as temperature. need to NOT have the temperature thing here.
    public ColorRules includePR;//same as above
    public ColorRules includeOG;
    public ColorRules includeGR;
    public ColorRules includeRB;
    public ColorRules includeYP;//more combination possible
    public ColorRules excludeR;
    public ColorRules excludeO;
    public ColorRules excludeY;
    public ColorRules excludeB;
    public ColorRules excludeG;
    public ColorRules excludeP;
    public ColorRules blockY;
    public ColorRules blockO;
    public ColorRules blockR;
    public ColorRules blockG;
    public ColorRules blockB;
    public ColorRules blockP;

    public List<MovementRules> movementRuleSets = new List<MovementRules>();
    public List<ColorRules> colorRuleSets = new List<ColorRules>();

    void Start()
    {
       
        defineRules();
        selectChromosomes(movementRuleSets,colorRuleSets); //mutate and fitness are nested in crossover

    }

  

    public void defineRules()
    {
        
        Tmove.direction = 1011;
        Tmove.distance = 1;
        Tmove.color = -1;
        Tmove.type = 1;
        movementRuleSets.Add(Tmove);


        blank.direction = 1111;
        blank.distance = 1;
        blank.color = -1;
        blank.type = 2;
        movementRuleSets.Add(blank);
        

        teleportB.direction = -1;
        teleportB.distance = -1;
        teleportB.color = 5;
        teleportB.type = 3;
        movementRuleSets.Add(teleportB);


        teleportP.direction = -1;
        teleportP.distance = -1;
        teleportP.color = 6;
        teleportP.type = 3;
        movementRuleSets.Add(teleportP);

        teleportG.direction = -1;
        teleportG.distance = -1;
        teleportG.color = 4;
        teleportG.type = 3;
        movementRuleSets.Add(teleportG);


        teleportR.direction = -1;
        teleportR.distance = -1;
        teleportR.color = 3;
        teleportR.type = 3;
        movementRuleSets.Add(teleportR);

        teleportO.direction = -1;
        teleportO.distance = -1;
        teleportO.color = 2;
        teleportO.type = 3;
        movementRuleSets.Add(teleportO);


        teleportY.direction = -1;
        teleportY.distance = -1;
        teleportY.color = 1;
        teleportY.type = 3;
        movementRuleSets.Add(teleportY);


        jumpOne.direction = 1111;
        jumpOne.distance = 2;
        jumpOne.color = -1;
        jumpOne.type = 4;
        movementRuleSets.Add(jumpOne);


        jumpTwo.direction = 1111;
        jumpTwo.distance = 3;
        jumpTwo.color = -1;
        jumpTwo.type = 5;
        movementRuleSets.Add(jumpTwo);

        warmTemp.direction = 1111;
        warmTemp.distance = 1;
        warmTemp.color = 1 | 2 | 3; //does this work or should it be seperate?
        warmTemp.type = 6;
        movementRuleSets.Add(warmTemp);


        coldTemp.direction = 1111;
        coldTemp.distance = 1;
        coldTemp.color = 4 | 5 | 6;
        coldTemp.type = 7;
        movementRuleSets.Add(coldTemp);

        ColorRules includeBY;// if blue, goes to yellow
        // int currentpos = 9999;//take from the maze/how to keep track of prevs position when maze is not built?
        //if(currentpos.color==1)
        //{
        includeBY.src = 5;
        includeBY.target = 1;
        includeBY.type = 8;
        //create more of these
        includeBY.inclusion = true; //not using this anywhere

        excludeR.src = 3;
        excludeR.target = 1 | 2 | 4 | 5 | 6;
        excludeR.inclusion = false;
        excludeR.type = 9;
        //add to the list

        excludeO.src = 2;
        excludeO.target = 1 | 3 | 4 | 5 | 6;
        excludeO.inclusion = false;
        excludeO.type = 9;

        excludeY.src = 1;
        excludeY.target = 3 | 2 | 4 | 5 | 6;
        excludeY.inclusion = false;
        excludeY.type = 9;


        excludeB.src = 5;
        excludeB.target = 1 | 2 | 4 | 3 | 6;
        excludeB.inclusion = false;
        excludeB.type = 9;

        excludeG.src = 4;
        excludeG.target = 1 | 2 | 3 | 5 | 6;
        excludeG.inclusion = false;
        excludeG.type = 9;


        excludeP.src = 6;
        excludeP.target = 1 | 2 | 4 | 5 | 3;
        excludeP.inclusion = false;
        excludeP.type = 9;

        blockY.target=2 | 3 | 4 | 5 | 6;
        blockY.type = 10;

        blockO.target = 1 | 3 | 4 | 5 | 6;
        blockO.type = 10;

        blockR.target = 1 | 2 | 4 | 5 | 6;
        blockR.type = 10;

        blockG.target= 1 | 2 | 3 | 5 | 6;
        blockG.type = 10;

        blockB.target= 1 | 2 | 3 | 4 | 6;
        blockB.type = 10;

        blockP.target = 1 | 2 | 3 | 4 | 5; //src and inclusion not defined yet
        blockP.type = 10;


    }


    public void selectChromosomes(List<MovementRules> m, List<ColorRules> c)
    {
        System.Random randNum = new System.Random();


        // List<object> c1 = new List<object>();
        //List<object> c2 = new List<object>();
        //List<object> allList = movementRuleSets.Cast<object>().Concat(colorRuleSets).ToList();

        //int[] c1 = new int[7];
        //int[] c2 = new int[7]; //7 rules each time

        List<int> c1 = new List<int>(7); //just adding the types to chromosomes
        List<int> c2 = new List<int>(7);
        List<int> allList = new List<int>();

        foreach (var item in movementRuleSets)
         {
           allList.Add(item.type);
         }
         foreach (var item in colorRuleSets)
         {
            allList.Add(item.type);
         }
        
        
        List<int> usedIdx1 = new List<int>();
        List<int> usedIdx2 = new List<int>();

        //creating c1 and c2
        for (int i = 0; i < c1.Count; i++)   //how long do we want each chromosome to be? a subset of allList
        {
            int idx1 = randNum.Next(0,allList.Count); 
            usedIdx1.Add(idx1); //avoiding duplicate rules in a chromosome

            if (!usedIdx1.Contains(idx1))
            {
                c1.Add(allList[idx1]);
            }
        }

        for (int i = 0; i < c2.Count; i++)   
        {
            int idx2 = randNum.Next(0, allList.Count);
            usedIdx2.Add(idx2); 

            if (!usedIdx2.Contains(idx2))
            {
                c2.Add(allList[idx2]);
            }
        }
      
        crossover(c1, c2);
        


    }

    public void crossover(List<int> c1, List<int> c2)
    {
        //crossover in the first half of c1 and c2
       // int[] chromosomes = new int[c1.Count + c2.Count]; 
        for (int i = 0; i < c1.Count/2; i++)
        {
            int x = c1[i];
            c1[i] = c2[i];
            c2[i] = x;
        }

        fitnessOne(c1, c2);
      

    }


    public void fitnessOne(List<int> c1, List<int> c2)
    {
        //chromosome that has more different types of rules is a better fit rule.

        //fitness metrics : 1)variation in types , 2)too much or too less of only color rules/movement rules

        //fitness = [chromosome[1] for chromosome in population ]
        int fitc1 = 1;
        int fitc2 = 1;
        
        int t=0;
        int[] uniqueTypesc1 = new int[11]; //rule types , not using index 0, want to use index 1-10
        for (int i = 0; i < c1.Count; i++) //check the variation in types
        {
            t = c1[i];
            uniqueTypesc1[t]++; //incrementing the rule types using rule types as index
        }

        for (int i = 0; i < uniqueTypesc1.Length; i++)
        {
            if (uniqueTypesc1[i] != 0)
            {
                fitc1 = fitc1 * uniqueTypesc1[i];
            }
        }




        print("fitness 1 : all rule types are used only once");
        print("fitness value 1-2 is good. Higher value means same type of rule is being repeated more than twice, which is not good.");
        //print("fitness value is " + fit1);



        int s = 0;
        int[] uniqueTypesc2 = new int[11]; //rule types , not using index 0, want to use index 1-10
        for (int i = 0; i < c2.Count; i++) //check the variation in types
        {
            s = c2[i];
            uniqueTypesc2[s]++; //incrementing the rule types using rule types as index
        }

        for (int i = 0; i < uniqueTypesc2.Length; i++)
        {

            if (uniqueTypesc2[i] != 0)
            {
                fitc2 = fitc2 * uniqueTypesc2[i];
            }
        }

        if (fitc1 < fitc2)
        {
            if (fitc1 <= 2)
            {
                print("c1 selected with fitness value "+ fitc1);
                //pass c1 to maze
            }
            else
            {
                selectChromosomes(movementRuleSets, colorRuleSets);
            }
        }
        else
        {
            if (fitc2 <= 2)
            {
                print("c2 selected with fitness value " + fitc2);
                //pass c2 to maze
            }
            else
            {
                selectChromosomes(movementRuleSets, colorRuleSets);
            }
        }

        
    }
}





