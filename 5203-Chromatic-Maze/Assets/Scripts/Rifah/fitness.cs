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
}