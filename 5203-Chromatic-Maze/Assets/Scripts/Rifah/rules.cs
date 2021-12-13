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

    public static int popSize;


    void Start()
    {
       
        defineRules();
        selectChromosomes(movementRuleSets,colourRuleSets,popSize); 

    }


    //I UPDATED ALL OF THE RULE DEFINITIONS TO USE THE ENUMS
    //I ADDED MORE DEFINTIONS SO THE NUMBER OF UNIQUE INDEXES HAS CHANGED
    //I ADDED THE COLOUR RULES TO THE COLOUR RULE LIST

    public void createMRule(Type type)
    {
        List<Colour> clrs = new List<Colour>();
        clrs.Add(Colour.Blue);
        clrs.Add(Colour.Red);
        clrs.Add(Colour.Yellow);
        clrs.Add(Colour.Teal);
        clrs.Add(Colour.Orange);
        clrs.Add(Colour.Green);
        clrs.Add(Colour.Pink);
        clrs.Add(Colour.Purple);

        List<Direction> dir = new List<Direction>();
        dir.Add(Direction.East);
        dir.Add(Direction.North);
        dir.Add(Direction.West);
        dir.Add(Direction.South);

        if (type == Type.Tmove)
        {
            foreach(Colour c in clrs)
            {
                foreach(Direction d in dir)
                {
                    MovementRule r = new MovementRule();
                    r.index = index++;
                    r.src = c;
                    r.target = Colour.All;
                    r.direction = d;
                    
                    r.type = type;
                    movementRuleSets.Add(r);
                }
            }
        }

        if (type == Type.warm)
        {
            foreach (Colour i in clrs)
            {
                MovementRule r = new MovementRule();
                r.index = index++;
                r.src = i;
                r.target = Colour.Warm;
                r.type = type;
                movementRuleSets.Add(r);
            }
        }
        if (type == Type.cool)
        {
            foreach (Colour i in clrs)
            {
                MovementRule r = new MovementRule();
                r.index = index++;
                r.src = i;
                r.target = Colour.Cool;
                r.type = type;
                movementRuleSets.Add(r);
            }
        }
        if (type == Type.blank)
        {
            foreach (Colour i in clrs)
            {
                MovementRule r = new MovementRule();
                r.index = index++;
                r.src = i;
                r.target = Colour.All;
                
                r.type = type;
                movementRuleSets.Add(r);
                
            }
        }
        if (type == Type.jump1 || type==Type.jump2)
        {
            foreach(Colour c in clrs)
            {
                MovementRule r = new MovementRule();
                r.index = index++;
                r.src = c;
                r.target = Colour.All;
                if (type == Type.jump1)
                {
                    r.distance = 2;
                }
                else
                {
                    r.distance = 3;
                }
                r.type = type;
                movementRuleSets.Add(r);
            }
        }
        else
        {
            foreach (Colour s in clrs)
            {
                foreach (Colour t in clrs)
                {
                    if (s != t)
                    {
                        MovementRule n = new MovementRule();
                        n.index = index++;
                        
                        n.target = t;
                        n.src = s;
                        n.type = type;
                        movementRuleSets.Add(n);

                    }

                }

            }



        }
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
            foreach (Colour j in col)//skip the first index
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

       createMRule(  Type.Tmove);
       
       createMRule(Type.blank);
       createMRule( Type.jump1);
       createMRule(Type.jump2);
       createMRule( Type.warm); 
       createMRule(Type.cool);
       createMRule(Type.teleport); 
      
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
        //Dictionary<int, Type> chr = new Dictionary<int, Type>(); //dictionary of rule-idx and type
        //Hashtable chr = new Hashtable();


        for (int i = 0; i < pop; i++)
        {

            //List<ArrayList> chr = new List<ArrayList>(7);


            //List<int> usedIdx = new List<int>();
            Dictionary<int, Type> chr = new Dictionary<int, Type>(); //dictionary of rule-idx and type
            //for (int j = 0; j < 8; j++)   //filling up each chromosome with rule types
            int count = 0;
            while(count<8)
            {
                int idx = randNum.Next(0, allList.Count);
                //avoiding duplicate rules in a chromosome
                
                //if (!usedIdx.Contains(idx))
                if(!chr.ContainsKey(idx))
                {
                    
                    chr.Add(idx, allList[idx]); //adding the rule index and rule type to the chromosome
                    //Debug.Log("idx is " + idx + "type is " + allList[idx]);                            // chr[j]=allList[idx];
                    count++;
                }
                
            }
            Debug.Log(chr.Count);
           
            ChrsDict.Add(i,chr); // num of chromosomes = pop size
            //Debug.Log("chr types adding: "+chr.Values.ToString());
        }
        //foreach(var item in ChrsDict)
        //{
        //    Debug.Log("chrs dictionary is " + item.Values);
        //}
       
        Fitness1.fitnessOne(ChrsDict, movementRuleSets, colourRuleSets);

    }



      
    
   

}






