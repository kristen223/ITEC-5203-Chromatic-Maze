using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//structs cannot inehrit , used for small groups of variables


//ADDED ENUMS AND UPDATED TWO STRUCTS
public enum Type {Tmove, blank, teleport, jump1, jump2, warm, cool,include, exclude, wall, checkPathInc, checkPathExc };
public enum Direction {North, South, East, West, All};
public enum Colour {Red, Orange, Yellow, Green, Blue, Pink, Teal, Purple, Warm, Cool, All, Black}; //colour of tapped tile

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
  
    public void createCRule( Type type)//no need for the colour params
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
                if (j != i)
                {
                    ColourRule r = new ColourRule();
                    r.index = index++;
                    r.src = i;
                    r.target = j;
                    r.type = type;
                    colourRuleSets.Add(r);
                }
                
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
      
        createCRule(Type.include);
        createCRule(Type.exclude);
        





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
        Debug.Log("all rules : "+allList.Count);
        Dictionary<int, Dictionary<int,Type>> ChrsDict = new Dictionary<int, Dictionary<int,Type>>(pop); //dictionary of chr-idx and chr
        Dictionary<int, Type> chr = new Dictionary<int, Type>(); //dictionary of rule-idx and type
        //Hashtable chr = new Hashtable();


        for (int i = 0; i < pop; i++)
        {

            //List<ArrayList> chr = new List<ArrayList>(7);


            List<int> usedIdx = new List<int>();

            for (int j = 0; j < 8; j++)   //filling up each chromosome with rule types
            {
                int idx = randNum.Next(0, allList.Count);
                usedIdx.Add(idx); //avoiding duplicate rules in a chromosome
                Debug.Log(idx);
                if (!usedIdx.Contains(idx))
                {
                    chr.Add(idx, allList[idx]); //adding the rule index and rule type to the chromosome
                    Debug.Log("idx is " + idx + "type is " + allList[idx]);                            // chr[j]=allList[idx];
                }
            }
            ChrsDict.Add(i,chr); // num of chromosomes = pop size
            Debug.Log("chr types adding: "+chr.Values.ToString());
        }
        //foreach(var item in ChrsDict)
        //{
        //    Debug.Log("chrs dictionary is " + item.Values);
        //}
       
        Fitness1.fitnessOne(ChrsDict, movementRuleSets, colourRuleSets);

    }



      
    
   

}






