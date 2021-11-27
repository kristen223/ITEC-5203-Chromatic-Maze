using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//structs cannot inehrit , used for small groups of variables


//ADDED ENUMS AND UPDATED TWO STRUCTS
public enum Type {Tmove, blank, teleport, jump1, jump2, warm, cool,include, exclude, block, checkPathInc, checkPathExc };
public enum Direction {North, South, East, West, All};
public enum Colour {Red, Orange, Yellow, Green, Blue, Purple, Warm, Cool, All}; //colour of tapped tile

public struct MovementRules
{
    public int index;
    public int distance;
    public Type type;
    public Direction direction;
    public Colour colour;

    //public int direction; //N,S,E,W=1111,  dont care=-1
    //public int colour; //-1 means dont care , 1=yellow, 2=orange, 3=red, 4=green, 5=blue, 6=purple
    //public int type;

    public static implicit operator MovementRules(List<MovementRules> v)
    {
        throw new NotImplementedException();
    }
}

public struct ColourRules  
{
    public int index;
    public bool inclusion; //true if the src colour was passed for both CheckPath rule types
    public Type type;
    public Colour src;
    public Colour target; //for CheckPath rules, can only move onto this colour if inclusion == true or false depending
    
    //public int src; 
    //public int target; 
    //public int notTarget; 
    //public int type;

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
    public MovementRules TmoveN;
    public MovementRules TmoveS;
    public MovementRules TmoveE;
    public MovementRules TmoveW;
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
    public ColourRules blockR;
    public ColourRules blockO;
    public ColourRules blockY;
    public ColourRules blockG;
    public ColourRules blockB;
    public ColourRules blockP;

    //I UPDATED/ADDED THESE. NOT SURE WHICH COLOUR COMBOS ARE BEST SO YOU MAY WANT TO UPDATE THESE NAMES AND THEIR RULE DEFINITIONS BELOW
    public ColourRules checkPathIncludeYG;
    public ColourRules checkPathIncludeOP;
    public ColourRules checkPathIncludeBR;
    public ColourRules checkPathExcludeGO;
    public ColourRules checkPathExcludePB;
    public ColourRules checkPathExcludeRY;

    public List<MovementRules> movementRuleSets = new List<MovementRules>();
    public List<ColourRules> colourRuleSets = new List<ColourRules>();

    public int popSize;


    void Start()
    {
       
        defineRules();
        selectChromosomes(movementRuleSets,colourRuleSets,popSize); //mutate and fitness are nested inside

    }

  
    //I UPDATED ALL OF THE RULE DEFINITIONS TO USE THE ENUMS
    //I ADDED MORE DEFINTIONS SO THE NUMBER OF UNIQUE INDEXES HAS CHANGED
    //I ADDED THE COLOUR RULES TO THE COLOUR RULE LIST
    public void defineRules()
    {
        TmoveS.index = 0;
        TmoveS.direction = Direction.South;
        TmoveS.distance = 1;
        TmoveS.colour = Colour.All;
        TmoveS.type = Type.Tmove;
        movementRuleSets.Add(TmoveS);

        TmoveN.index = 13; //I MOVED THESE TMOVE RULE DEFINITIONS FROM BOTTOM SO THAT CHANGES ORDER OF ITEMS IN LIST
        TmoveN.direction = Direction.North;
        TmoveN.distance = 1;
        TmoveN.colour = Colour.All;
        TmoveN.type = Type.Tmove;
        movementRuleSets.Add(TmoveN);

        TmoveW.index = 14;
        TmoveW.direction = Direction.West;
        TmoveW.distance = 1;
        TmoveW.colour = Colour.All;
        TmoveW.type = Type.Tmove;
        movementRuleSets.Add(TmoveW);

        TmoveE.index = 15;
        TmoveE.direction = Direction.East;
        TmoveE.distance = 1;
        TmoveE.colour = Colour.All;
        TmoveE.type = Type.Tmove;
        movementRuleSets.Add(TmoveE);

        blank.index = 1;
        blank.direction = Direction.All;
        blank.distance = 1;
        blank.colour = Colour.All;
        blank.type = Type.blank;
        movementRuleSets.Add(blank);

        teleportB.index = 3;
        teleportB.direction = Direction.All;
        teleportB.distance = -1;
        teleportB.colour = Colour.Blue;
        teleportB.type = Type.teleport;
        movementRuleSets.Add(teleportB);

        teleportP.index = 4;
        teleportP.direction = Direction.All;
        teleportP.distance = -1;
        teleportP.colour = Colour.Purple;
        teleportP.type = Type.teleport;
        movementRuleSets.Add(teleportP);

        teleportG.index = 5;
        teleportG.direction = Direction.All;
        teleportG.distance = -1;
        teleportG.colour = Colour.Green;
        teleportG.type = Type.teleport;
        movementRuleSets.Add(teleportG);

        teleportR.index = 6;
        teleportR.direction = Direction.All;
        teleportR.distance = -1;
        teleportR.colour = Colour.Red;
        teleportR.type = Type.teleport;
        movementRuleSets.Add(teleportR);

        teleportO.index = 7;
        teleportO.direction = Direction.All;
        teleportO.distance = -1;
        teleportO.colour = Colour.Orange;
        teleportO.type = Type.teleport;
        movementRuleSets.Add(teleportO);

        teleportY.index = 8;
        teleportY.direction = Direction.All;
        teleportY.distance = -1;
        teleportY.colour = Colour.Yellow;
        teleportY.type = Type.teleport;
        movementRuleSets.Add(teleportY);

        jumpOne.index = 9;
        jumpOne.direction = Direction.All;
        jumpOne.distance = 2;
        jumpOne.colour = Colour.All;
        jumpOne.type = Type.jump1;
        movementRuleSets.Add(jumpOne);

        jumpTwo.index = 10;
        jumpTwo.direction = Direction.All;
        jumpTwo.distance = 3;
        jumpTwo.colour = Colour.All;
        jumpTwo.type = Type.jump2;
        movementRuleSets.Add(jumpTwo);

        warmTemp.index = 11;
        warmTemp.direction = Direction.All;
        warmTemp.distance = 1;
        warmTemp.colour = Colour.Warm; //does this work or should it be seperate?
        warmTemp.type = Type.warm;
        movementRuleSets.Add(warmTemp);

        coldTemp.index = 12;
        coldTemp.direction = Direction.All;
        coldTemp.distance = 1;
        coldTemp.colour = Colour.Cool;
        coldTemp.type = Type.cool;
        movementRuleSets.Add(coldTemp);


        //Colour Rules

        includeBY.index = 16;
        includeBY.src = Colour.Blue;
        includeBY.target = Colour.Yellow;
        includeBY.type = Type.include;
        colourRuleSets.Add(includeBY);

        //I ADDED THE BELOW INCLUDE DEFINITIONS
        includePR.index = 17;
        includePR.src = Colour.Purple;
        includePR.target = Colour.Red;
        includePR.type = Type.include;
        colourRuleSets.Add(includePR);

        includeOG.index = 18;
        includeOG.src = Colour.Orange;
        includeOG.target = Colour.Green;
        includeOG.type = Type.include;
        colourRuleSets.Add(includeOG);

        includeGR.index = 19;
        includeGR.src = Colour.Green;
        includeGR.target = Colour.Blue;
        includeGR.type = Type.include;
        colourRuleSets.Add(includeGR);

        includeRB.index = 20;
        includeRB.src = Colour.Red;
        includeRB.target = Colour.Blue;
        includeRB.type = Type.include;
        colourRuleSets.Add(includeRB);

        excludeR.index = 21;
        excludeR.target = Colour.Red;
        excludeR.type = Type.exclude;
        colourRuleSets.Add(excludeR);

        excludeO.index = 22;
        excludeO.target = Colour.Orange;
        excludeO.type = Type.exclude;
        colourRuleSets.Add(excludeO);

        excludeY.index = 23;
        excludeY.target = Colour.Yellow;
        excludeY.type = Type.exclude;
        colourRuleSets.Add(excludeY);

        excludeB.index = 24;
        excludeB.target = Colour.Blue;
        excludeB.type = Type.exclude;
        colourRuleSets.Add(excludeB);

        excludeG.index = 25;
        excludeG.target = Colour.Green;
        excludeG.type = Type.exclude;
        colourRuleSets.Add(excludeG);

        excludeP.index = 26;
        excludeP.target = Colour.Purple;
        excludeP.type = Type.exclude;
        colourRuleSets.Add(excludeP);

        blockY.index = 27;
        blockY.target = Colour.Yellow;
        blockY.type = Type.block;
        colourRuleSets.Add(blockY);

        blockO.index = 28;
        blockO.target = Colour.Orange;
        blockO.type = Type.block;
        colourRuleSets.Add(blockO);

        blockR.index = 29;
        blockR.target = Colour.Red;
        blockR.type = Type.block;
        colourRuleSets.Add(blockR);

        blockG.index = 30;
        blockG.target = Colour.Green;
        blockG.type = Type.block;
        colourRuleSets.Add(blockG);

        blockB.index = 31;
        blockB.target = Colour.Blue;
        blockB.type = Type.block;
        colourRuleSets.Add(blockB);

        blockP.index = 32;
        blockP.target = Colour.Purple;
        blockP.type = Type.block;
        colourRuleSets.Add(blockP);

        checkPathIncludeYG.index = 33;
        checkPathIncludeYG.src = Colour.Yellow;
        checkPathIncludeYG.target = Colour.Green;
        checkPathIncludeYG.inclusion = false;
        checkPathIncludeYG.type = Type.checkPathInc;
        colourRuleSets.Add(checkPathIncludeYG);

        checkPathIncludeOP.index = 34;
        checkPathIncludeOP.src = Colour.Orange;
        checkPathIncludeOP.target = Colour.Purple;
        checkPathIncludeOP.inclusion = false;
        checkPathIncludeOP.type = Type.checkPathInc;
        colourRuleSets.Add(checkPathIncludeOP);

        checkPathIncludeBR.index = 35;
        checkPathIncludeBR.src = Colour.Blue;
        checkPathIncludeBR.target = Colour.Red;
        checkPathIncludeBR.inclusion = false;
        checkPathIncludeBR.type = Type.checkPathInc;
        colourRuleSets.Add(checkPathIncludeBR);

        checkPathExcludeGO.index = 36;
        checkPathExcludeGO.src = Colour.Green;
        checkPathExcludeGO.target = Colour.Orange;
        checkPathExcludeGO.inclusion = false;
        checkPathExcludeGO.type = Type.checkPathExc;
        colourRuleSets.Add(checkPathExcludeGO);

        checkPathExcludePB.index = 36;
        checkPathExcludePB.src = Colour.Purple;
        checkPathExcludePB.target = Colour.Blue;
        checkPathExcludePB.inclusion = false;
        checkPathExcludePB.type = Type.checkPathExc;
        colourRuleSets.Add(checkPathExcludePB);

        checkPathExcludeRY.index = 36;
        checkPathExcludeRY.src = Colour.Red;
        checkPathExcludeRY.target = Colour.Yellow;
        checkPathExcludeRY.inclusion = false;
        checkPathExcludeRY.type = Type.checkPathExc;
        colourRuleSets.Add(checkPathExcludeRY);
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
        //I CHANGED THE LIST FROM INT TO TYPE
        List<Type> allList = new List<Type>();

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






