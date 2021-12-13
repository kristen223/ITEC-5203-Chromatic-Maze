using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Fitness1 : MonoBehaviour
{
   


    public static MovementRule GetMRule(int idx, List<MovementRule> m)
    {
        foreach(MovementRule x in m)
        {
            if (x.index == idx)
            {
                return x;
            }
            
        }
        Debug.Log("could NOT find the rule index");
        return null;
    }


    public static ColourRule GetCRule(int idx, List<ColourRule> c)
    {
        foreach (ColourRule y in c)
        {
            if (y.index == idx)
            {
                return y;
            }   
        }
        Debug.Log("could NOT find rule index");
        return null;
    }




    // Start is called before the first frame update
    public static void fitnessOne(Dictionary<int, Dictionary<int,Type>> cList,  List<MovementRule> m, List<ColourRule> c ,int pop)
    {
         Debug.Log("fitness1");
        //Debug.Log("clist: " + cList.Count);
        //Debug.Log("clist keys: " + cList.Keys.Count);
        //Debug.Log("clist val: " + cList.Values.Count);
        //SortedDict fitvals << rule-index, fit >>:
        SortedDictionary<int,int> fitVals = new SortedDictionary<int, int>(); //automatically sort by key  => fit
        
       // foreach (var item in cList)//clist contains all chromosomes so checking for each chromosome
        //{
          
            int chrCount = 0;
            
           

            List<Type> uniqueTypes = new List<Type>(); //new unique list

            foreach (Dictionary<int, Type> d in cList.Values) //inside each chromosome (rules)
            {
                int fit = 0;
                // PART 1 : CHECKING VARAITION IN TYPES

                // ICollection valueColl = d.Values; //types
                //Debug.Log("-------START of a single chr------------");
                //Debug.Log("this should be length 8 but it is : " + d.Count);
                foreach (Type v in d.Values) //looping in types (chr.value)
                {

                    //Debug.Log("uniquetypes starts");
                    if (!uniqueTypes.Contains(v)) //types are added if they dont already exist in the list
                    {
                        uniqueTypes.Add(v);
                        //Debug.Log("adding this to uniqetypes " + v);
                    }
                }
                //Debug.Log("uniquetypes length is : " + uniqueTypes.Count);
                if (uniqueTypes.Count == 8) //length is 8 if all types are unique
                {
                    fit = 8;

                }
                if (uniqueTypes.Count < 8)
                {
                    fit = uniqueTypes.Count;//best fit=8, second best = 7, so on

                }



                //PART 2 : CHECKING BADNESS WITH TMOVES,INC,EXC : "mutliple Tmoves aren't that bad, but multiple include/excludes are bad"
                int tmove = 0;
                int inc = 0;
                int exc = 0;


                foreach (Type t in d.Values)
                {
                    if (t == Type.Tmove)
                    {
                        tmove++;

                    }

                    if (t == Type.include)
                    {
                        inc++;
                    }
                    if (t == Type.exclude)
                    {
                        exc++;
                    }

                }
                if (tmove > 1)
                {
                    for (int h = 0; h < tmove; h++)
                    {
                        fit--;
                    }
                }
                if (inc > 1)
                {
                    for (int k = 0; k < inc; k++)
                    {
                        fit = fit - 2;
                    }
                }
                if (inc > 0 && exc > 0)
                {
                    fit = fit - 7;
                }
                if (exc > 1)
                {
                    for (int e = 0; e < exc; e++)
                    {
                        fit = fit - 2;
                    }
                }

                //PART 3 : CHECKING BADNESS WITH COLORS : "target color of teleport should not be src color of include/exclude"
                //ICollection ruleIdxs = d.Keys;


                foreach (KeyValuePair<int, Type> kvc in d)
                {
                    if (kvc.Value == Type.teleport) //teleport so its a movement rule
                    {
                        MovementRule r1 = GetMRule(kvc.Key, m);
                        foreach (KeyValuePair<int, Type> kvb in d)
                        {
                            if (kvb.Value == Type.include || kvb.Value == Type.exclude)
                            {
                                ColourRule r2 = GetCRule(kvb.Key, c);
                                if (r1.target == r2.src)
                                {
                                    fit = fit - 5;
                                }
                            }
                        }
                    }


                }

                //PART 4 : WARM/COOL BALANCE
                int wrm = 0;
                int cld = 0;

                foreach (KeyValuePair<int, Type> p in d)
                {
                    if (p.Value == Type.include || p.Value == Type.exclude)
                    {
                        ColourRule r = GetCRule(p.Key, c);
                        if (r.src == Colour.Warm || r.src == Colour.Red || r.src == Colour.Orange || r.src == Colour.Pink || r.src == Colour.Yellow || r.target == Colour.Warm || r.target == Colour.Red || r.target == Colour.Orange || r.target == Colour.Pink || r.target == Colour.Yellow)
                        {
                            wrm++;
                        }
                        if (r.src == Colour.Cool || r.src == Colour.Blue || r.src == Colour.Green || r.src == Colour.Teal || r.src == Colour.Teal || r.target == Colour.Cool || r.target == Colour.Blue || r.target == Colour.Teal || r.target == Colour.Green || r.target == Colour.Purple)
                        {
                            cld++;
                        }

                    }
                    else
                    {
                        MovementRule r = GetMRule(p.Key, m);
                        if (r.src == Colour.Warm || r.src == Colour.Red || r.src == Colour.Orange || r.src == Colour.Pink || r.src == Colour.Yellow || r.target == Colour.Warm || r.target == Colour.Red || r.target == Colour.Orange || r.target == Colour.Pink || r.target == Colour.Yellow)
                        {
                            wrm++;
                        }
                        if (r.src == Colour.Cool || r.src == Colour.Blue || r.src == Colour.Green || r.src == Colour.Teal || r.src == Colour.Teal || r.target == Colour.Cool || r.target == Colour.Blue || r.target == Colour.Teal || r.target == Colour.Green || r.target == Colour.Purple)
                        {
                            cld++;
                        }
                    }

                }

                if (wrm == cld)
                {
                    fit = fit + 2; //most fit = 8+2 = 10
                }
                if (wrm != cld)
                {
                    fit = fit - (Mathf.Abs(wrm - cld));
                }
            //existence of warm/cold rules, then check the exc/incl 's src colors to not be the same warm/cold

        /*    //PART 5 : CHECKING FOR ALL UNIQUE SRC COLORS
            List<Colour> uniqueColors = new List<Colour>();
 
            foreach (KeyValuePair<int, Type> kvc in d)
            {
                if(kvc.Value==Type.include || kvc.Value == Type.exclude)
                {
                    ColourRule rr = GetCRule(kvc.Key,c);
                   
                    if (uniqueColors.Contains(rr.src))
                    {
                        fit = fit - 50;
                    }
                    else
                    {
                        uniqueColors.Add(rr.src);
                    }
                    

                }
                else
                {
                    MovementRule rr = GetMRule(kvc.Key, m);

                    if (uniqueColors.Contains(rr.src))
                    {
                        fit = fit - 50;
                    }
                    else
                    {
                        uniqueColors.Add(rr.src);
                    }
                }
                



                }*/

                //FINAL STEP : ADDING THE FITNESS VALUE TO THE dict ALONG WITH INDEX




                //Debug.Log("-------END of a single chr------------");
               // Debug.Log("fitness of " + chrCount + "is " + fit);
                fitVals.Add(chrCount, fit);
                chrCount++;
               
            }


        int chosensize = (int)System.Math.Ceiling(fitVals.Count * 0.2); //20% of the best fits
        Debug.Log("chosen size should be " + chosensize);
        Dictionary<int, int> chosenChr = new Dictionary<int, int>(); //sort descending order, crossover to build new 80% or 100%, passs to f1 again
        int count = 0;
        foreach(KeyValuePair<int,int> pairs in fitVals.OrderByDescending(val => val.Value)) //descending order sorted
        {
            if (count <= chosensize)
            {
                chosenChr.Add(pairs.Key,pairs.Value);
                count++;
            }
            
        }
        Debug.Log("chosenChr size is "+chosenChr.Count);
        foreach(var item in chosenChr)
        {
            Debug.Log("chosen fitness are : "+item.Value);
        }
        Debug.Log("done");



        //CROSSOVER
        Crossover.crossover(chosenChr,cList,m,c,pop);
        //Dictionary<int, int> newSetOfChrs = new Dictionary<int, int>();
        //newSetOfChrs=Crossover.crossover(chosenChr);

//Crossover.crossover(chosenChr,cList,m,c);



        //got to this after crossover
        /*  foreach(int v in chosenChr.Values) //passing the top 20% chromosomes
          {
              FinalRules.finalRules(cList[v], m, c);
          }*/


        //FinalRules.finalRules(cList, m, c);
       
        MazeCreation.getFinalRules(chosenChr, cList, m, c);




    }
   

}
