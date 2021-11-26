using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//structs cannot inehrit , used for small groups of variables

public struct MovementRules
{
    public int index;
    public int direction; //N,S,E,W=1111,  dont care=-1
    public int distance; //dont care = -1
    public int colour; //-1 means dont care , 1=yellow, 2=orange, 3=red, 4=green, 5=blue, 6=purple
    public int type; //assigning not done

    public static implicit operator MovementRules(List<MovementRules> v)
    {
        throw new NotImplementedException();
    }
}

public struct ColourRules  
{
    public int index;
    public bool inclusion;
    public int src;
    public int target;
    public int notTarget;
    public int type;//assigning not done 

    public static implicit operator ColourRules(List<ColourRules> v)
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
    public MovementRules TmoveU;
    public MovementRules TmoveD;
    public MovementRules TmoveL;
    public MovementRules TmoveR;
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
    public ColourRules includeBY; //if this is applied, other B? cant be applied.Make sure this rule is not the same as temperature. need to NOT have the temperature thing here.
    public ColourRules includePR;//same as above
    public ColourRules includeOG;
    public ColourRules includeGR;
    public ColourRules includeRB;
    public ColourRules includeYP;//more combination possible
    public ColourRules excludeR;
    public ColourRules excludeO;
    public ColourRules excludeY;
    public ColourRules excludeB;
    public ColourRules excludeG;
    public ColourRules excludeP;
    public ColourRules blockY;
    public ColourRules blockO;
    public ColourRules blockR;
    public ColourRules blockG;
    public ColourRules blockB;
    public ColourRules blockP;
    public ColourRules checkPathYG;
    public ColourRules checkPathOP;
    public ColourRules checkPathBR;
  
    public List<MovementRules> movementRuleSets = new List<MovementRules>();
    public List<ColourRules> colourRuleSets = new List<ColourRules>();

    public int popSize;


    void Start()
    {
       
        defineRules();
        selectChromosomes(movementRuleSets,colourRuleSets,popSize); //mutate and fitness are nested inside

    }

  

    public void defineRules()
    {
        TmoveU.index = 0;
        TmoveU.direction = 1011;
        TmoveU.distance = 1;
        TmoveU.colour = -1;
        TmoveU.type = 0;
        movementRuleSets.Add(TmoveU);

        blank.index = 1;
        blank.direction = 1111;
        blank.distance = 1;
        blank.colour = -1;
        blank.type = 1;
        movementRuleSets.Add(blank);

        teleportB.index = 3;
        teleportB.direction = -1;
        teleportB.distance = -1;
        teleportB.colour = 5;
        teleportB.type = 2;
        movementRuleSets.Add(teleportB);

        teleportP.index = 4;
        teleportP.direction = -1;
        teleportP.distance = -1;
        teleportP.colour = 6;
        teleportP.type = 2;
        movementRuleSets.Add(teleportP);

        teleportG.index = 5;
        teleportG.direction = -1;
        teleportG.distance = -1;
        teleportG.colour = 4;
        teleportG.type = 2;
        movementRuleSets.Add(teleportG);

        teleportR.index = 6;
        teleportR.direction = -1;
        teleportR.distance = -1;
        teleportR.colour = 3;
        teleportR.type = 2;
        movementRuleSets.Add(teleportR);

        teleportO.index = 7;
        teleportO.direction = -1;
        teleportO.distance = -1;
        teleportO.colour = 2;
        teleportO.type = 2;
        movementRuleSets.Add(teleportO);

        teleportY.index = 8;
        teleportY.direction = -1;
        teleportY.distance = -1;
        teleportY.colour = 1;
        teleportY.type = 2;
        movementRuleSets.Add(teleportY);

        jumpOne.index = 9;
        jumpOne.direction = 1111;
        jumpOne.distance = 2;
        jumpOne.colour = -1;
        jumpOne.type = 3;
        movementRuleSets.Add(jumpOne);

        jumpTwo.index = 10;
        jumpTwo.direction = 1111;
        jumpTwo.distance = 3;
        jumpTwo.colour = -1;
        jumpTwo.type = 4;
        movementRuleSets.Add(jumpTwo);

        warmTemp.index = 11;
        warmTemp.direction = 1111;
        warmTemp.distance = 1;
        warmTemp.colour = 1 | 2 | 3; //does this work or should it be seperate?
        warmTemp.type = 5;
        movementRuleSets.Add(warmTemp);

        coldTemp.index = 12;
        coldTemp.direction = 1111;
        coldTemp.distance = 1;
        coldTemp.colour = 4 | 5 | 6;
        coldTemp.type = 6;
        movementRuleSets.Add(coldTemp);

        TmoveD.index = 13;
        TmoveD.direction = 0111;
        TmoveD.distance = 1;
        TmoveD.colour = -1;
        TmoveD.type = 0;
        movementRuleSets.Add(TmoveD);

        TmoveL.index = 14;
        TmoveL.direction = 1101;
        TmoveL.distance = 1;
        TmoveL.colour = -1;
        TmoveL.type = 0;
        movementRuleSets.Add(TmoveL);

        TmoveR.index = 15;
        TmoveR.direction = 1110;
        TmoveR.distance = 1;
        TmoveR.colour = -1;
        TmoveR.type = 0;
        movementRuleSets.Add(TmoveR);

        //ColourRules includeBY;// if blue, goes to yellow
        // int currentpos = 9999;//take from the maze/how to keep track of prevs position when maze is not built?
        //if(currentpos.colour==1)
        //{
        includeBY.index = 16;
        includeBY.src = 5;
        includeBY.target = 1;
        includeBY.type = 7;
        //create more of these
        includeBY.inclusion = false; //not using this anywhere

        excludeR.index = 17;
        excludeR.src = 3;
        excludeR.notTarget = 3;
        excludeR.inclusion = false;
        excludeR.type = 8;
        //add to the list

        excludeO.index = 18;
      
        excludeO.notTarget = 2;
        excludeO.inclusion = false;
        excludeO.type = 8;

        excludeY.index = 19;
      
        excludeY.notTarget = 1;
        excludeY.inclusion = false;
        excludeY.type = 8;

        excludeB.index = 20;
        excludeB.notTarget = 5;
        excludeB.inclusion = false;
        excludeB.type = 8;

        excludeG.index = 21;
        excludeG.notTarget = 4;
        excludeG.inclusion = false;
        excludeG.type = 8;

        excludeP.index = 22;
        excludeP.notTarget = 6;
        excludeP.inclusion = false;
        excludeP.type = 8;

        blockY.index = 23;
        blockY.notTarget = 1;
        blockY.type = 9;

        blockO.index = 24;
        blockO.notTarget = 2;
        blockO.type = 9;

        blockR.index = 25;
        blockR.notTarget = 3;
        blockR.type = 9;

        blockG.index = 26;
        blockG.notTarget= 4;
        blockG.type = 9;

        blockB.index = 27;
        blockB.notTarget= 5;
        blockB.type = 9;

        blockP.index = 28;
        blockP.notTarget =6; //src and inclusion not defined yet
        blockP.type = 9;

        

        checkPathYG.index = 29;
        checkPathYG.src = 1;
        checkPathYG.target = 4;

        checkPathOP.index = 30;
        checkPathOP.src = 2;
        checkPathOP.target = 6;

        checkPathBR.index = 31;
        checkPathBR.src = 5;
        checkPathBR.target = 3;

    }


    public void selectChromosomes(List<MovementRules> m, List<ColourRules> c, int pop) //add each chromosome size param
    {
        System.Random randNum = new System.Random();


        // List<object> c1 = new List<object>();
        //List<object> c2 = new List<object>();
        //List<object> allList = movementRuleSets.Cast<object>().Concat(colourRuleSets).ToList();

        //int[] c1 = new int[7];
        //int[] c2 = new int[7]; //7 rules each time

        //global list to keep track of all chromosomes

        //list containing all possible rules:
        List<int> allList = new List<int>();

        foreach (var item in movementRuleSets)
        {
            allList.Add(item.type);
        }
        foreach (var item in colourRuleSets)
        {
            allList.Add(item.type);
        }

        List<Hashtable> ChrsList = new List<Hashtable>(pop);
        Hashtable chr = new Hashtable();


        for (int i = 0; i < pop; i++)
        {

            //List<ArrayList> chr = new List<ArrayList>(7);

            
            List<int> usedIdx = new List<int>();
         
            for (int j = 0; j < chr.Count; j++)   //filling up each chromosome with rule types
            {
                int idx = randNum.Next(0, allList.Count);
                usedIdx.Add(idx); //avoiding duplicate rules in a chromosome

                if (!usedIdx.Contains(idx))
                {
                    chr.Add(idx,allList[idx]); //adding the rule index and rule type to the chromosome
                   // chr[j]=allList[idx];
                }
            }
            ChrsList.Add(chr);
        }

        fitnessOne(ChrsList,pop);


        //List<int> c1 = new List<int>(7); //just adding the types to chromosomes
        //List<int> c2 = new List<int>(7);
        //List<int> usedIdx1 = new List<int>();
        //List<int> usedIdx2 = new List<int>();


        ////creating c1 and c2
        //for (int i = 0; i < c1.Count; i++)   //how long do we want each chromosome to be? a subset of allList
        //{
        //    int idx1 = randNum.Next(0,allList.Count); 
        //    usedIdx1.Add(idx1); //avoiding duplicate rules in a chromosome

        //    if (!usedIdx1.Contains(idx1))
        //    {
        //        c1.Add(allList[idx1]);
        //    }
        //}

        //for (int i = 0; i < c2.Count; i++)   
        //{
        //    int idx2 = randNum.Next(0, allList.Count);
        //    usedIdx2.Add(idx2); 

        //    if (!usedIdx2.Contains(idx2))
        //    {
        //        c2.Add(allList[idx2]);
        //    }
        //}

        //crossover(c1, c2);
        //fitnessOne(c1, c2);
    }





    //public void crossover(List<int> c1, List<int> c2)
    //{
    //    //crossover in the first half of c1 and c2
    //   // int[] chromosomes = new int[c1.Count + c2.Count]; 
    //    for (int i = 0; i < c1.Count/2; i++)
    //    {
    //        int x = c1[i];
    //        c1[i] = c2[i];
    //        c2[i] = x;
    //    }

    //    fitnessOne(c1, c2);


    // }


    public void fitnessOne(List<Hashtable> cList, int pop)
    {
       // List<int> fitVals = new List<int>(cList.Count);
        //int[] fitvals = new int[pop];
        Hashtable fitVals = new Hashtable();
        for (int i = 0; i < cList.Count; i++)//clist contains all chromosomes
        {

            int fit = 1;
            int[] uniqueTypes = new int[10]; //10 rule types "zero initiatilization"
            for (int j = 0; j < cList.Count; j++) //check the variation in types
            {
                /*foreach (int key in cList[j].Keys) //clist[j] is a hashtable
                {
                    x=cList[key];
                   
                }*/

                ICollection valueColl = cList[j].Values; //types are stored in cList.Values
                foreach (int v in valueColl)
                {
                    uniqueTypes[v]++; //incrementing the unique array with each rule type in a chromosome
                    print(v);
                }
            }

            for (int z = 0; z < uniqueTypes.Length; z++)
            {
                if (uniqueTypes[z] != 0)
                {
                    fit = fit * uniqueTypes[z];

                    fitVals.Add(i, fit);
                    print("fitness is"+fit);
                }
            }
        }



        //printing the ranks here:
        print(fitVals.Values.ToString());
        ArrayList allfitvals = new ArrayList(fitVals.Values);
        foreach (var v in fitVals.Values)
        {
            allfitvals.Add(v);
        }
        print(fitVals.Keys.ToString());
        allfitvals.Sort();
        print("The fitness values of the chromosomes ranked are:");
       // print(allfitvals[2]);
        foreach(int x in allfitvals) 
        {
            print("fitness value: "+ x);//ranks
            //if (x == 1)
            //{
                //pass to maze
                // fitVals.a
                //finalRules(cList[i]);
            //}
        }

        int finalFitness = (int)allfitvals[index: 0]; //the first value in the sorted arraylist is the best fit
        print(finalFitness);
        int min = 1;
        int finalCIdx=0;//chromosome index, not rule!
        foreach (int y in fitVals.Values)
        {
            if (y == min)
            {
                finalCIdx = y;
                print(finalCIdx);
                break;
            }
        }

        finalRules(cList[finalCIdx]); //passing the fit chromosome




    }

    public void finalRules(Hashtable c) //get the indexes (keys) of this hashtable
    {
        
        ICollection indexs = c.Keys;
        int[] finalIdxs = new int[7];
        int r = 0;
        foreach (int i in indexs)
        {
            finalIdxs[r] = (int)i;
            r++;
        }
        //all the final indexes are set in finalIdxs

        List<MovementRules> mr = new List<MovementRules>();
        List<ColourRules> cr = new List<ColourRules>();

        for (int i = 0; i < finalIdxs.Length; i++)
        {
            if (finalIdxs[i] <= 15) //first 15 were movement rules , will be changed ENUMS
            {
                mr.Add(movementRuleSets.Find(x => x.index.Equals(finalIdxs[i])));
            }
            else
            {
                cr.Add(colourRuleSets.Find(y => y.index.Equals(finalIdxs[i])));
            }
            
          

        }
        AssignColour.SetRules(mr,cr);



    }

}






