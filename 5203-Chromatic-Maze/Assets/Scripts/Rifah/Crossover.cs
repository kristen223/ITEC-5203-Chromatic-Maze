using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crossover : MonoBehaviour
{
    // Start is called before the first frame update

    public Crossover()
    {

    }
    public static Dictionary<int,int> crossover(Dictionary<int,int> chosenChr)
    {
        Dictionary<int, int> newChrs = new Dictionary<int, int>();


        foreach(var idx in chosenChr.Values) //key=>fit, value=>idx
        {
            //get the rule of each index in chosenChr (get rules method)
            //each index has 8 rules.
            //make 5 more chromosomes with the same 8 rules. so just copy.
            //change the colors.
        }

        return newChrs;
    }
}
