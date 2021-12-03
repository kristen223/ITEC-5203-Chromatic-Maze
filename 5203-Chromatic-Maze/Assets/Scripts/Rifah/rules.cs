using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//structs cannot inehrit , used for small groups of variables


//ADDED ENUMS AND UPDATED TWO STRUCTS
public enum Type {Tmove, blank, teleport, jump1, jump2, warm, cool,include, exclude, block, checkPathInc, checkPathExc };
public enum Direction {North, South, East, West, All};
public enum Colour {Red, Orange, Yellow, Green, Blue,Pink,Teal, Purple, Warm, Cool, All}; //colour of tapped tile

public struct MovementRule
{
    public int index;
    public int distance;
    public Type type;
    public Direction direction;
    public Colour src; //set this later
    public Colour target;

 

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
     public static int index=0;
    //I UPDATED/ADDED CHECK PATH RULES AND EXCLUDE/INCLUDE RULES
    //public static MovementRule Tmove;
   /* public static MovementRule TmoveN;
    public static MovementRule TmoveS;
    public static MovementRule TmoveE;
    public static MovementRule TmoveW;
    public static MovementRule blank;
    public static MovementRule teleport;
    /*public static MovementRule teleportB;
    public static MovementRule teleportP;
    public static MovementRule teleportG;
    public static MovementRule teleportR;
    public static MovementRule teleportO;
    public static MovementRule teleportY;
    
    public static MovementRule jumpOne;
    public static MovementRule jumpTwo;
    public static MovementRule warmTemp;
    public static MovementRule coldTemp;
    public static ColourRule include;
    /*public static ColourRule includeRB;
    public static ColourRule includeOG;
    public static ColourRule includeYP;
    public static ColourRule includeGR;
    public static ColourRule includeBY;
    public static ColourRule includePR;
    public static ColourRule exclude;
    /* public static ColourRule excludeRP;
     public static ColourRule excludeOB;
     public static ColourRule excludeYG;
     public static ColourRule excludeGY;
     public static ColourRule excludeBR;
     public static ColourRule excludePO;
    public static ColourRule block;
    public static ColourRule blockR;
    public static ColourRule blockO;
    public static ColourRule blockY;
    public static ColourRule blockG;
    public static ColourRule blockB;
    public static ColourRule blockP;
    public static ColourRule checkPathInclude;
    public static ColourRule checkPathIncludeYG;
    public static ColourRule checkPathIncludeOP;
    public static ColourRule checkPathIncludeBR;
    public static ColourRule checkPathExclude;
    /*public static ColourRule checkPathExcludeGO;
    public static ColourRule checkPathExcludePB;
    public static ColourRule checkPathExcludeRY;*/

    public static List<MovementRule> movementRuleSets = new List<MovementRule>();
    public static List<ColourRule> colourRuleSets = new List<ColourRule>();

    public int popSize;


    void Start()
    {
       
        defineRules();
        selectChromosomes(movementRuleSets,colourRuleSets,popSize); //mutate and fitness are nested inside

    }

  
    //I UPDATED ALL OF THE RULE DEFINITIONS TO USE THE ENUMS
    //I ADDED MORE DEFINTIONS SO THE NUMBER OF UNIQUE INDEXES HAS CHANGED
    //I ADDED THE COLOUR RULES TO THE COLOUR RULE LIST

    public MovementRule createMRule(Direction dir, int dis, Colour target,Colour src, Type type)
    {
        MovementRule n = new MovementRule(); 
        n.index = index++;
        n.direction = dir;
        n.distance = dis;
        n.target = target;
        n.src = src;
        n.type = type;
        movementRuleSets.Add(n);
        return n;
    }
    //public ColourRule createCRule(Colour target, Colour src, Type type)
    //{
    //    ColourRule c=new ColourRule();
    //    c.index = index++;
    //    c.target = target;
    //    c.src = src;
    //    c.type = type;

    //    colourRuleSets.Add(c);
    //    return c;
    //}
    public void createCRule(Colour target, Colour src, Type type)//no need for the colour params
    {
        List<Colour> col = new List<Colour>();
        col.Add(Colour.Blue);
        col.Add(Colour.Red);
        col.Add(Colour.Yellow);
        col.Add(Colour.Teal);
        col.Add(Colour.Orange);
        col.Add(Colour.Green);
        col.Add(Colour.Pink);
        col.Add(Colour.Purple);

        foreach (Colour i in col)
        {
            foreach(Colour j in col)//skip the first index
            {
                ColourRule r=new ColourRule();
                r.index=index++;
                r.src = i;
                r.target = j;
                r.type = type;
                colourRuleSets.Add(r);
            }

        }   


    }

    public void defineRules()
    {

        MovementRule TmoveS = createMRule( Direction.South, 1, Colour.All,Colour.All, Type.Tmove);
        MovementRule TmoveN = createMRule(Direction.North, 1, Colour.All, Colour.All, Type.Tmove);
        MovementRule TmoveW = createMRule(Direction.West, 1, Colour.All, Colour.All, Type.Tmove);
        MovementRule TmoveE = createMRule(Direction.East, 1, Colour.All, Colour.All, Type.Tmove);
        MovementRule blank = createMRule(Direction.All, 1, Colour.All, Colour.All, Type.blank);
        MovementRule jumpOne = createMRule(Direction.All,2, Colour.All,Colour.All, Type.jump1);
        MovementRule jumpTwo = createMRule(Direction.All, 3, Colour.All, Colour.All, Type.jump2);
        MovementRule warmTemp = createMRule(Direction.All, 1, Colour.Warm, Colour.All, Type.warm); //SRC not specified here. should be specified?
        MovementRule coldTemp = createMRule(Direction.All, 1, Colour.Cool, Colour.All, Type.cool);
        MovementRule teleport = createMRule(Direction.All, 0, Colour.All, Colour.All, Type.teleport); //make multiple of these here or in assign colors?
        createCRule(Colour.All, Colour.All, Type.include);
        createCRule(Colour.All, Colour.All, Type.exclude);
        





        //Colour Rules

        //includeBY.index = 16;
        //includeBY.src = Colour.Blue;
        //includeBY.target = Colour.Yellow;
        //includeBY.type = Type.include;
        //colourRuleSets.Add(includeBY);

        ////I ADDED THE BELOW INCLUDE DEFINITIONS
        //includePR.index = 17;
        //includePR.src = Colour.Purple;
        //includePR.target = Colour.Red;
        //includePR.type = Type.include;
        //colourRuleSets.Add(includePR);

        //includeOG.index = 18;
        //includeOG.src = Colour.Orange;
        //includeOG.target = Colour.Green;
        //includeOG.type = Type.include;
        //colourRuleSets.Add(includeOG);

        //includeGR.index = 19;
        //includeGR.src = Colour.Green;
        //includeGR.target = Colour.Blue;
        //includeGR.type = Type.include;
        //colourRuleSets.Add(includeGR);

        //includeRB.index = 20;
        //includeRB.src = Colour.Red;
        //includeRB.target = Colour.Blue;
        //includeRB.type = Type.include;
        //colourRuleSets.Add(includeRB);

        //excludeRP.index = 21;
        //excludeRP.src = Colour.Red;
        //excludeRP.target = Colour.Purple;
        //excludeRP.type = Type.exclude;
        //colourRuleSets.Add(excludeRP);

        //excludeOB.index = 22;
        //excludeOB.src = Colour.Orange;
        //excludeOB.target = Colour.Blue;
        //excludeOB.type = Type.exclude;
        //colourRuleSets.Add(excludeOB);

        //excludeYG.index = 23;
        //excludeYG.src = Colour.Yellow;
        //excludeYG.target = Colour.Green;
        //excludeYG.type = Type.exclude;
        //colourRuleSets.Add(excludeYG);

        //excludeBR.index = 24;
        //excludeBR.src = Colour.Blue;
        //excludeBR.target = Colour.Red;
        //excludeBR.type = Type.exclude;
        //colourRuleSets.Add(excludeBR);

        //excludeGY.index = 25;
        //excludeGY.src = Colour.Green;
        //excludeGY.target = Colour.Yellow;
        //excludeGY.type = Type.exclude;
        //colourRuleSets.Add(excludeGY);

        //excludePO.index = 26;
        //excludePO.src = Colour.Purple;
        //excludePO.target = Colour.Orange;
        //excludePO.type = Type.exclude;
        //colourRuleSets.Add(excludePO);

        //blockR.index = 27;
        //blockR.target = Colour.Red;
        //blockR.type = Type.block;
        //colourRuleSets.Add(blockR);

        //blockO.index = 28;
        //blockO.target = Colour.Orange;
        //blockO.type = Type.block;
        //colourRuleSets.Add(blockO);

        //blockY.index = 29;
        //blockY.target = Colour.Yellow;
        //blockY.type = Type.block;
        //colourRuleSets.Add(blockY);

        //blockG.index = 30;
        //blockG.target = Colour.Green;
        //blockG.type = Type.block;
        //colourRuleSets.Add(blockG);

        //blockB.index = 31;
        //blockB.target = Colour.Blue;
        //blockB.type = Type.block;
        //colourRuleSets.Add(blockB);

        //blockP.index = 32;
        //blockP.target = Colour.Purple;
        //blockP.type = Type.block;
        //colourRuleSets.Add(blockP);

        //checkPathIncludeYG.index = 33;
        //checkPathIncludeYG.src = Colour.Yellow;
        //checkPathIncludeYG.target = Colour.Green;
        //checkPathIncludeYG.inclusion = false;
        //checkPathIncludeYG.type = Type.checkPathInc;
        //colourRuleSets.Add(checkPathIncludeYG);

        //checkPathIncludeOP.index = 34;
        //checkPathIncludeOP.src = Colour.Orange;
        //checkPathIncludeOP.target = Colour.Purple;
        //checkPathIncludeOP.inclusion = false;
        //checkPathIncludeOP.type = Type.checkPathInc;
        //colourRuleSets.Add(checkPathIncludeOP);

        //checkPathIncludeBR.index = 35;
        //checkPathIncludeBR.src = Colour.Blue;
        //checkPathIncludeBR.target = Colour.Red;
        //checkPathIncludeBR.inclusion = false;
        //checkPathIncludeBR.type = Type.checkPathInc;
        //colourRuleSets.Add(checkPathIncludeBR);

        //checkPathExcludeGO.index = 36;
        //checkPathExcludeGO.src = Colour.Green;
        //checkPathExcludeGO.target = Colour.Orange;
        //checkPathExcludeGO.inclusion = false;
        //checkPathExcludeGO.type = Type.checkPathExc;
        //colourRuleSets.Add(checkPathExcludeGO);

        //checkPathExcludePB.index = 37;
        //checkPathExcludePB.src = Colour.Purple;
        //checkPathExcludePB.target = Colour.Blue;
        //checkPathExcludePB.inclusion = false;
        //checkPathExcludePB.type = Type.checkPathExc;
        //colourRuleSets.Add(checkPathExcludePB);

        //checkPathExcludeRY.index = 15;
        //checkPathExcludeRY.src = Colour.Red;
        //checkPathExcludeRY.target = Colour.Yellow;
        //checkPathExcludeRY.inclusion = false;
        //checkPathExcludeRY.type = Type.checkPathExc;
        //colourRuleSets.Add(checkPathExcludeRY);
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

        List<Dictionary<int,Type>> ChrsDict = new List<Dictionary<int, Type>>(pop);
        Dictionary<int, Type> chr = new Dictionary<int, Type>();
        //Hashtable chr = new Hashtable();


        for (int i = 0; i < pop; i++)
        {

            //List<ArrayList> chr = new List<ArrayList>(7);

            
            List<int> usedIdx = new List<int>();
         
            for (int j = 0; j < 8; j++)   //filling up each chromosome with rule types
            {
                int idx = randNum.Next(0, allList.Count);
                usedIdx.Add(idx); //avoiding duplicate rules in a chromosome

                if (!usedIdx.Contains(idx))
                {
                    chr.Add(idx,allList[idx]); //adding the rule index and rule type to the chromosome
                   // chr[j]=allList[idx];
                }
            }
            ChrsDict.Add(chr);
        }
        Fitness1.fitnessOne(ChrsDict, pop, movementRuleSets,colourRuleSets);

       // fitnessOne(ChrsDict,pop);


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
    /*
     -------------------THESE METHODS ARE CREATED SEPERATELY IN DIFFERENT C# SCRIPTS-------------------------------
    public void fitnessOne(List<Dictionary<int,Type>> cList, int pop)
    {
       // List<int> fitVals = new List<int>(cList.Count);
        //int[] fitvals = new int[pop];
        Hashtable fitVals = new Hashtable();
        for (int i = 0; i < cList.Count; i++)//clist contains all chromosomes so checking for each chromosome
        {

            int fit = 0;
            Debug.Log(sizeof(Type));

            List<Type> uniqueTypes = new List<Type>(); //new unique list

            //old unique array:
            //int[] uniqueTypes = new int[sizeof(Type)]; //12 rule types "zero initiatilization"
            //for (int k = 0; k < uniqueTypes.Length; k++)
            //{
            //    uniqueTypes[k] = 0;
            //}

            for (int j = 0; j < cList.Count; j++) //check the variation in types
            {
                /*foreach (int key in cList[j].Keys) //clist[j] is a hashtable
                {
                    x=cList[key];
                   
                }*/

        /*        ICollection valueColl = cList[j].Values; //types are stored in cList.Values
                foreach (Type v in valueColl)
                {
                    // uniqueTypes[v]++; //incrementing the unique array with each rule type in a chromosome (old)
                    if (!uniqueTypes.Contains(v)) //types are added if they dont already exist in the list
                    {
                        uniqueTypes.Add(v);
                        print(v);
                    }

                    
                }
            }
            //new fitness function with list length
            if (uniqueTypes.Count == 7) //length is 7 if all types are unique, from line 515
            {
                fit = 1;
                fitVals.Add(i, fit);
                print("fitness of"+i+"is"+fit);
            }
            else
            {
                fit = 7 - uniqueTypes.Count + 1;  //if 2 same types among 7, then fit=2
                fitVals.Add(i, fit);
                print("fitness of" + i + "is" + fit);
            }                                     //if 3 same types among 7, then fit = 3

            //old fitness funct with unique array: 
            //for (int z = 0; z < uniqueTypes.Count; z++)
            //{
            //    Debug.Log(uniqueTypes[z]);
            //    if (uniqueTypes[z] != 0)
            //    {
            //        fit = fit * uniqueTypes[z];

            //        fitVals.Add(i, fit);
            //        print("fitness is"+fit);
            //    }
            //}
        }


        Debug.Log("fitvals" + fitVals.Count);
        //printing the ranks here:
        print(fitVals.Values.ToString());
        ArrayList ranks = new ArrayList(fitVals.Values); //changed "allfitvals" to "ranks"
        foreach (int v in fitVals.Values)
        {
            ranks.Add(v);
        }
        print(fitVals.Keys.ToString());
        ranks.Sort();
        print("The fitness values of the chromosomes ranked are:");
       // print(allfitvals[2]);
        foreach(int x in ranks) 
        {
            Debug.Log("dfsdlfjfjdsklfjdsklf");
            print("fitness value: "+ x);//ranks
          
        }

        int finalFitness = (int)ranks[index: 0]; //the first value in the sorted arraylist is the best fit
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


    

    public void finalRules(Dictionary<int,Type> c) //get the indexes (keys) of this hashtable
    {
       
        int[] finalIdxs = new int[7];
        int r = 0;
        foreach (int i in c.Keys)
        {
            finalIdxs[r] = (int)i;
            r++;
        }
        //all the final indexes are set in finalIdxs

        List<MovementRule> mr = new List<MovementRule>();
        List<ColourRule> cr = new List<ColourRule>();

        
        foreach(KeyValuePair<int,Type> kvp in c)
        {
            if (kvp.Value.Equals("Tmove") | kvp.Value.Equals("blank") | kvp.Value.Equals("teleport") | kvp.Value.Equals("jump1") | kvp.Value.Equals("jump2") | kvp.Value.Equals("warm") | kvp.Value.Equals("cool")) 
            {
                mr.Add(movementRuleSets.Find(x => x.index.Equals(finalIdxs[kvp.Key])));
            }
            else
            {
                cr.Add(colourRuleSets.Find(y => y.index.Equals(finalIdxs[kvp.Key])));
            }
        }


        //foreach (Type t in c.Values)
        //{
            
        //    if (t.Equals("Tmove") | t.Equals("blank") | t.Equals("teleport") | t.Equals("jump1") | t.Equals("jump2") | t.Equals("warm") | t.Equals("cool")) 
        //    {
        //        mr.Add(movementRuleSets.Find(x => x.index.Equals(finalIdxs[c.Keys])));
        //    }
        //    else
        //    {

        //    }

        //}



        //for (int i = 0; i < finalIdxs.Length; i++)
        //{
        //    if (finalIdxs[i] <= 15) //first 15 were movement rules , will be changed ENUMS
        //    {
        //        mr.Add(movementRuleSets.Find(x => x.index.Equals(finalIdxs[i])));
        //    }
        //    else
        //    {
        //        cr.Add(colourRuleSets.Find(y => y.index.Equals(finalIdxs[i])));
        //    }
            
          

        //}
        ColourAssigner.SetRules(mr,cr);
        



    }*/
    //I dont think we need the codes below bec i already did this work in line 615 and 619 (Rifah)
   /* public static MovementRule GetMRule(int index)
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
    }*/

}






