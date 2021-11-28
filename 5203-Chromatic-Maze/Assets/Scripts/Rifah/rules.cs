using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//structs cannot inehrit , used for small groups of variables


//ADDED ENUMS AND UPDATED TWO STRUCTS
public enum Type {Tmove, blank, teleport, jump1, jump2, warm, cool,include, exclude, block, checkPathInc, checkPathExc };
public enum Direction {North, South, East, West, All};
public enum Colour {Red, Orange, Yellow, Green, Blue, Purple, Warm, Cool, All}; //colour of tapped tile

public struct MovementRule
{
    public int index;
    public int distance;
    public Type type;
    public Direction direction;
    public Colour src; //set this later
    public Colour target;

    //public int direction; //N,S,E,W=1111,  dont care=-1
    //public int colour; //-1 means dont care , 1=yellow, 2=orange, 3=red, 4=green, 5=blue, 6=purple
    //public int type;

    public static implicit operator MovementRule(List<MovementRule> v)
    {
        throw new NotImplementedException();
    }
}

public struct ColourRule  
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

    public static implicit operator ColourRule(List<ColourRule> v)
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

    //I UPDATED/ADDED CHECK PATH RULES AND EXCLUDE/INCLUDE RULES
    public static MovementRule TmoveN;
    public static MovementRule TmoveS;
    public static MovementRule TmoveE;
    public static MovementRule TmoveW;
    public static MovementRule blank;
    public static MovementRule teleportB;
    public static MovementRule teleportP;
    public static MovementRule teleportG;
    public static MovementRule teleportR;
    public static MovementRule teleportO;
    public static MovementRule teleportY;
    public static MovementRule jumpOne;
    public static MovementRule jumpTwo;
    public static MovementRule warmTemp;
    public static MovementRule coldTemp;
    public static ColourRule includeRB;
    public static ColourRule includeOG;
    public static ColourRule includeYP;
    public static ColourRule includeGR;
    public static ColourRule includeBY;
    public static ColourRule includePR;
    public static ColourRule excludeRP;
    public static ColourRule excludeOB;
    public static ColourRule excludeYG;
    public static ColourRule excludeGY;
    public static ColourRule excludeBR;
    public static ColourRule excludePO;
    public static ColourRule blockR;
    public static ColourRule blockO;
    public static ColourRule blockY;
    public static ColourRule blockG;
    public static ColourRule blockB;
    public static ColourRule blockP;
    public static ColourRule checkPathIncludeYG;
    public static ColourRule checkPathIncludeOP;
    public static ColourRule checkPathIncludeBR;
    public static ColourRule checkPathExcludeGO;
    public static ColourRule checkPathExcludePB;
    public static ColourRule checkPathExcludeRY;

    public List<MovementRule> movementRuleSets = new List<MovementRule>();
    public List<ColourRule> colourRuleSets = new List<ColourRule>();

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
        TmoveS.target = Colour.All;
        TmoveS.type = Type.Tmove;
        movementRuleSets.Add(TmoveS);

        TmoveN.index = 13; //I MOVED THESE TMOVE RULE DEFINITIONS FROM BOTTOM SO THAT CHANGES ORDER OF ITEMS IN LIST
        TmoveN.direction = Direction.North;
        TmoveN.distance = 1;
        TmoveN.target = Colour.All;
        TmoveN.type = Type.Tmove;
        movementRuleSets.Add(TmoveN);

        TmoveW.index = 14;
        TmoveW.direction = Direction.West;
        TmoveW.distance = 1;
        TmoveW.target = Colour.All;
        TmoveW.type = Type.Tmove;
        movementRuleSets.Add(TmoveW);

        TmoveE.index = 2;
        TmoveE.direction = Direction.East;
        TmoveE.distance = 1;
        TmoveE.target = Colour.All;
        TmoveE.type = Type.Tmove;
        movementRuleSets.Add(TmoveE);

        blank.index = 1;
        blank.direction = Direction.All;
        blank.distance = 1;
        blank.target = Colour.All;
        blank.type = Type.blank;
        movementRuleSets.Add(blank);

        teleportB.index = 3;
        teleportB.direction = Direction.All;
        teleportB.distance = -1;
        teleportB.target = Colour.Blue;
        teleportB.type = Type.teleport;
        movementRuleSets.Add(teleportB);

        teleportP.index = 4;
        teleportP.direction = Direction.All;
        teleportP.distance = -1;
        teleportP.target = Colour.Purple;
        teleportP.type = Type.teleport;
        movementRuleSets.Add(teleportP);

        teleportG.index = 5;
        teleportG.direction = Direction.All;
        teleportG.distance = -1;
        teleportG.target = Colour.Green;
        teleportG.type = Type.teleport;
        movementRuleSets.Add(teleportG);

        teleportR.index = 6;
        teleportR.direction = Direction.All;
        teleportR.distance = -1;
        teleportR.target = Colour.Red;
        teleportR.type = Type.teleport;
        movementRuleSets.Add(teleportR);

        teleportO.index = 7;
        teleportO.direction = Direction.All;
        teleportO.distance = -1;
        teleportO.target = Colour.Orange;
        teleportO.type = Type.teleport;
        movementRuleSets.Add(teleportO);

        teleportY.index = 8;
        teleportY.direction = Direction.All;
        teleportY.distance = -1;
        teleportY.target = Colour.Yellow;
        teleportY.type = Type.teleport;
        movementRuleSets.Add(teleportY);

        jumpOne.index = 9;
        jumpOne.direction = Direction.All;
        jumpOne.distance = 2;
        jumpOne.target = Colour.All;
        jumpOne.type = Type.jump1;
        movementRuleSets.Add(jumpOne);

        jumpTwo.index = 10;
        jumpTwo.direction = Direction.All;
        jumpTwo.distance = 3;
        jumpTwo.target = Colour.All;
        jumpTwo.type = Type.jump2;
        movementRuleSets.Add(jumpTwo);

        warmTemp.index = 11;
        warmTemp.direction = Direction.All;
        warmTemp.distance = 1;
        warmTemp.target = Colour.Warm; //does this work or should it be seperate?
        warmTemp.type = Type.warm;
        movementRuleSets.Add(warmTemp);

        coldTemp.index = 12;
        coldTemp.direction = Direction.All;
        coldTemp.distance = 1;
        coldTemp.target = Colour.Cool;
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

        excludeRP.index = 21;
        excludeRP.src = Colour.Red;
        excludeRP.target = Colour.Purple;
        excludeRP.type = Type.exclude;
        colourRuleSets.Add(excludeRP);

        excludeOB.index = 22;
        excludeOB.src = Colour.Orange;
        excludeOB.target = Colour.Blue;
        excludeOB.type = Type.exclude;
        colourRuleSets.Add(excludeOB);

        excludeYG.index = 23;
        excludeYG.src = Colour.Yellow;
        excludeYG.target = Colour.Green;
        excludeYG.type = Type.exclude;
        colourRuleSets.Add(excludeYG);

        excludeBR.index = 24;
        excludeBR.src = Colour.Blue;
        excludeBR.target = Colour.Red;
        excludeBR.type = Type.exclude;
        colourRuleSets.Add(excludeBR);

        excludeGY.index = 25;
        excludeGY.src = Colour.Green;
        excludeGY.target = Colour.Yellow;
        excludeGY.type = Type.exclude;
        colourRuleSets.Add(excludeGY);

        excludePO.index = 26;
        excludePO.src = Colour.Purple;
        excludePO.target = Colour.Orange;
        excludePO.type = Type.exclude;
        colourRuleSets.Add(excludePO);

        blockR.index = 27;
        blockR.target = Colour.Red;
        blockR.type = Type.block;
        colourRuleSets.Add(blockR);

        blockO.index = 28;
        blockO.target = Colour.Orange;
        blockO.type = Type.block;
        colourRuleSets.Add(blockO);

        blockY.index = 29;
        blockY.target = Colour.Yellow;
        blockY.type = Type.block;
        colourRuleSets.Add(blockY);

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

        checkPathExcludePB.index = 37;
        checkPathExcludePB.src = Colour.Purple;
        checkPathExcludePB.target = Colour.Blue;
        checkPathExcludePB.inclusion = false;
        checkPathExcludePB.type = Type.checkPathExc;
        colourRuleSets.Add(checkPathExcludePB);

        checkPathExcludeRY.index = 15;
        checkPathExcludeRY.src = Colour.Red;
        checkPathExcludeRY.target = Colour.Yellow;
        checkPathExcludeRY.inclusion = false;
        checkPathExcludeRY.type = Type.checkPathExc;
        colourRuleSets.Add(checkPathExcludeRY);
    }


    public void selectChromosomes(List<MovementRule> m, List<ColourRule> c, int pop) //add each chromosome size param
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
                Debug.Log(uniqueTypes[z]);
                if (uniqueTypes[z] != 0)
                {
                    fit = fit * uniqueTypes[z];

                    fitVals.Add(i, fit);
                    print("fitness is"+fit);
                }
            }
        }


        Debug.Log("fitvals" + fitVals.Count);
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
            Debug.Log("dfsdlfjfjdsklfjdsklf");
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

        List<MovementRule> mr = new List<MovementRule>();
        List<ColourRule> cr = new List<ColourRule>();

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
        ColourAssigner.SetRules(mr,cr);



    }

    public static MovementRule GetMRule(int index)
    {
        switch(index)
        {
            case 0:
                return TmoveS;
            case 1:
                return blank;
            case 2:
                return TmoveE;
            case 3:
                return teleportB;
            case 4:
                return teleportP;
            case 5:
                return teleportG;
            case 6:
                return teleportR;
            case 7:
                return teleportO;
            case 8:
                return teleportY;
            case 9:
                return jumpOne;
            case 10:
                return jumpTwo;
            case 11:
                return warmTemp;
            case 12:
                return coldTemp;
            case 13:
                return TmoveN;
            case 14:
                return TmoveW;
        }

        Debug.Log("ERROR: returned blank by mistake");
        return blank; //will never happen

    }

    public static ColourRule GetCRule(int index)
    {
        switch (index)
        {
            case 15:
                return checkPathExcludeRY;
            case 16:
                return includeBY;
            case 17:
                return includePR;
            case 18:
                return includeOG;
            case 19:
                return includeGR;
            case 20:
                return includeRB;
            case 21:
                return excludeRP;
            case 22:
                return excludeOB;
            case 23:
                return excludeYG;
            case 24:
                return excludeBR;
            case 25:
                return excludeGY;
            case 26:
                return excludePO;
            case 27:
                return blockR;
            case 28:
                return blockO;
            case 29:
                return blockY;
            case 30:
                return blockG;
            case 31:
                return blockB;
            case 32:
                return blockP;
            case 33:
                return checkPathIncludeYG;
            case 34:
                return checkPathIncludeOP;
            case 35:
                return checkPathIncludeBR;
            case 36:
                return checkPathExcludeGO;
            case 37:
                return checkPathExcludePB;
        }

        Debug.LogError("ERROR: returned blockR by mistake");
        return blockR; //will never happen
    }

}






