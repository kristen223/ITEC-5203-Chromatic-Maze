using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fitness : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    public void fitnessOne(List<Hashtable> cList, int pop)
    {
        // List<int> fitVals = new List<int>(cList.Count);
        //int[] fitvals = new int[pop];
        Hashtable fitVals = new Hashtable();
        for (int i = 0; i < cList.Count; i++)//clist contains all chromosomes
        {

            int fit = 1;
            int[] uniqueTypes = new int[10]; //10 rule types
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
                }
            }

            for (int z = 0; z < uniqueTypes.Length; z++)
            {
                if (uniqueTypes[z] != 0)
                {
                    fit = fit * uniqueTypes[z];

                    fitVals.Add(i, fit);
                }
            }
        }

        ArrayList allfitvals = new ArrayList(fitVals.Values);
        allfitvals.Sort();
        print("The fitness values of the chromosomes ranked are:");
        foreach (int x in allfitvals)
        {
            print(x);
            if (x == 1)
            {
                //pass to maze
                // fitVals.a
            }
        }
    }
    /*  foreach (int v in fv)
           {
               uniqueTypes[v]++; //incrementing the unique array with each rule type in a chromosome
           }



           fitVals.Add(fit);//check ranks from here
           if (fit == 1)
           {
               fitVals.Add(fit);
               finalRules(cList[i],allList);
           }
           else
           {
               selectChromosomes(movementRuleSets, colourRuleSets, pop);
           }

       }


       */
    //chromosome that has more different types of rules is a better fit rule.

    //fitness metrics : 1)variation in types , 2)too much or too less of only colour rules/movement rules

    //fitness = [chromosome[1] for chromosome in population ]
    //int fitc1 = 1;
    //int fitc2 = 1;

    //int t=0;
    //int[] uniqueTypesc1 = new int[11]; //rule types , not using index 0, want to use index 1-10
    //for (int i = 0; i < c1.Count; i++) //check the variation in types
    //{
    //    t = c1[i];
    //    uniqueTypesc1[t]++; //incrementing the rule types using rule types as index
    //}

    //for (int i = 0; i < uniqueTypesc1.Length; i++)
    //{
    //    if (uniqueTypesc1[i] != 0)
    //    {
    //        fitc1 = fitc1 * uniqueTypesc1[i];
    //    }
    //}




    // print("fitness 1 : all rule types are used only once");
    //print("fitness value 1-2 is good. Higher value means same type of rule is being repeated more than twice, which is not good.");
    //print("fitness value is " + fit1);



    //int s = 0;
    //int[] uniqueTypesc2 = new int[11]; //rule types , not using index 0, want to use index 1-10
    //for (int i = 0; i < c2.Count; i++) //check the variation in types
    //{
    //    s = c2[i];
    //    uniqueTypesc2[s]++; //incrementing the rule types using rule types as index
    //}

    //for (int i = 0; i < uniqueTypesc2.Length; i++)
    //{

    //    if (uniqueTypesc2[i] != 0)
    //    {
    //        fitc2 = fitc2 * uniqueTypesc2[i];
    //    }
    //}

    //if (fitc1 < fitc2)
    //{
    //    if (fitc1 <= 2)
    //    {
    //        print("c1 selected with fitness value "+ fitc1);
    //        //pass c1 to maze
    //    }
    //    else
    //    {
    //        selectChromosomes(movementRuleSets, colourRuleSets);
    //    }
    //}
    //else
    //{
    //    if (fitc2 <= 2)
    //    {
    //        print("c2 selected with fitness value " + fitc2);
    //        //pass c2 to maze
    //    }
    //    else
    //    {
    //        selectChromosomes(movementRuleSets, colourRuleSets);
    //    }
}