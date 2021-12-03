using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fitness1 : MonoBehaviour
{
    // Start is called before the first frame update
    public static void fitnessOne(List<Dictionary<int, Type>> cList, int pop, List<MovementRule>m, List<ColourRule>c)
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

            for (int j = 0; j < cList.Count; j++) //check the variation in types, checking each chromosome
            {
                

                ICollection valueColl = cList[j].Values; //types are stored in cList.Values
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
                print("fitness of" + i + "is" + fit);
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
        foreach (int x in ranks)
        {
            Debug.Log("dfsdlfjfjdsklfjdsklf");
            print("fitness value: " + x);//ranks

        }

        int finalFitness = (int)ranks[index: 0]; //the first value in the sorted arraylist is the best fit
        print(finalFitness);
        int min = 1;
        int finalCIdx = 0;//chromosome index, not rule!
        foreach (int y in fitVals.Values)
        {
            if (y == min)
            {
                finalCIdx = y;
                print(finalCIdx);
                break;
            }
        }

        FinalRules.finalRules(cList[finalCIdx], m,c); //passing the fit chromosome




    }


}
