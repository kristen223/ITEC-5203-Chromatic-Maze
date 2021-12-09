using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fitness1 : MonoBehaviour
{
    // Start is called before the first frame update
    public static void fitnessOne(List<Dictionary<int, Type>> cList,  List<MovementRule> m, List<ColourRule> c)
    {
        //SortedDict<<fit,index>>:
        SortedDictionary<int,int> fitVals = new SortedDictionary<int, int>(); //automatically sort by key  => fit
        for (int i = 0; i < cList.Count; i++)//clist contains all chromosomes so checking for each chromosome
        {

            int fit = 0;
            Debug.Log(sizeof(Type));

            List<Type> uniqueTypes = new List<Type>(); //new unique list


            for (int j = 0; j < cList.Count; j++) //check the variation in types, checking each chromosome
            {


                ICollection valueColl = cList[j].Values; //types are stored in cList.Values
                foreach (Type v in valueColl)
                {

                    if (!uniqueTypes.Contains(v)) //types are added if they dont already exist in the list
                    {
                        uniqueTypes.Add(v);
                        print(v);
                    }


                }
            }
            // PART 1 : CHECKING VARAITION IN TYPES 
            //new fitness function with list length
            if (uniqueTypes.Count == 8) //length is 7 if all types are unique, from line 515, then highest fitness is 8.
            {
                fit = 8;

            }
            else
            {
                //fit = 8 - uniqueTypes.Count + 1;  //if 2 same types among 7, then fit=2
                fit = uniqueTypes.Count;//best fit=8, second best = 7, so on
                //fitVals.Add(i, fit);
                //print("fitness of" + i + "is" + fit);
            }

            //PART 2 : CHECKING BADNESS WITH TMOVES,INC,EXC : "mutliple Tmoves aren't that bad, but multiple include/excludes are bad"
            int tmove = 0;
            int inc = 0;
            int exc = 0;


            foreach (MovementRule r in m)
            {
                if (r.type == Type.Tmove)
                {
                    tmove++;

                }
            }
            foreach (ColourRule r in c)
            {
                if (r.type == Type.include)
                {
                    inc++;
                }
                if (r.type == Type.exclude)
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
            if (inc == 0 || exc == 0)
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

            foreach (MovementRule r in m)
            {
                if (r.type == Type.teleport)
                {
                    foreach (ColourRule x in c)
                    {
                        if (x.type == Type.include || x.type == Type.exclude)
                        {
                            if (r.target == x.src)
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
            foreach (MovementRule r in m)
            {
                if (r.src == Colour.Warm || r.src == Colour.Red || r.src == Colour.Orange || r.src == Colour.Pink || r.src == Colour.Yellow || r.target == Colour.Warm || r.target == Colour.Red || r.target == Colour.Orange || r.target == Colour.Pink || r.target == Colour.Yellow)
                {
                    wrm++;
                }
                if (r.src == Colour.Cool || r.src == Colour.Blue || r.src == Colour.Green || r.src == Colour.Teal || r.src == Colour.Teal || r.target == Colour.Cool || r.target == Colour.Blue || r.target == Colour.Teal || r.target == Colour.Green || r.target == Colour.Purple)
                {
                    cld++;
                }
            }
            foreach (ColourRule r in c)
            {
                if (r.src == Colour.Warm || r.src == Colour.Red || r.src == Colour.Orange || r.src == Colour.Pink || r.src == Colour.Yellow || r.target == Colour.Warm || r.target == Colour.Red || r.target == Colour.Orange || r.target == Colour.Pink || r.target == Colour.Yellow)
                {
                    wrm++;
                }
                if (r.src == Colour.Cool || r.src == Colour.Blue || r.src == Colour.Green || r.src == Colour.Teal || r.src == Colour.Teal || r.target == Colour.Cool || r.target == Colour.Blue || r.target == Colour.Teal || r.target == Colour.Green || r.target == Colour.Purple)
                {
                    cld++;
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

            //FINAL STEP : ADDING THE FITNESS VALUE TO THE HASHTABLE ALONG WITH INDEX

            fitVals.Add(fit,i);
            print("fitness of" + i + "is" + fit);


        }





        //Debug.Log("fitvals" + fitVals.Count);
        ////printing the ranks here:
        //print(fitVals.Values.ToString());
        //ArrayList ranks = new ArrayList(fitVals.Values); //changed "allfitvals" to "ranks"
        //foreach (int v in fitVals.Values)
        //{
        //    ranks.Add(v);
        //}
        //print(fitVals.Keys.ToString());
        //ranks.Sort();
        //ranks.Reverse(); //greatest value is the most fit. 
        //print("The fitness values of the chromosomes ranked are:");
        //// print(allfitvals[2]);
        //foreach (int x in ranks)
        //{
        //    Debug.Log("fitness values coming:");
        //    print("fitness value: " + x);//ranks

        //}


        //int fittest = (int)ranks[index: 0]; //the first value in the sorted arraylist is the best fit
        //print(fittest);
       
        int chosensize = (int)System.Math.Ceiling(fitVals.Count * 0.2); //20% of the best fits
        Dictionary<int, int> chosenChr = new Dictionary<int, int>();
        int count = 0;
        foreach(KeyValuePair<int,int> pairs in fitVals)
        {
            if (count <= chosensize)
            {
                chosenChr.Add(pairs.Key,pairs.Value);
                count++;
            }
            
        }
        foreach(int v in chosenChr.Values) //passing the top 20% chromosomes
        {
            FinalRules.finalRules(cList[v], m, c);
        }




        
        //FinalRules.finalRules(cList[finalCIdx], m, c); //passing the fit chromosome


    }


}
