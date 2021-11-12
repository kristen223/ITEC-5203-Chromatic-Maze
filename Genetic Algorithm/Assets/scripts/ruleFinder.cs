using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ruleFinder : MonoBehaviour
{
    //a list of rules
    //rules encoded for easier understanding (LudiGDL?)
    //a list of constraints (what cant be joined with what)
    //get a combination of rules
    //now it belongs to the infeasible population until it satisfies min constraints
    //if constraints satisfied(fitness function 1), it goes to feasible population
    //from the feasible population, the rule combinations are applied to the maze
    //check if the rules satisfy the MST without errors (fitness function 2) : if the possible path is traversible 

    //fitness function 1 : if the combinations satisfy constraints (unmatchable pairs, pathological pairs) ~ probability weights?
    //fitness function 2 : if the game generated with the rule is actually working without errors on the path.

    //string[] rules=new;
    List<string> rules= new List<string>();



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update() {
    }


    public void InitRules()
    {
        rules.Add("a");
        rules.Add("b");
        rules.Add("c");
        rules.Add("d");
        rules.Add("e");

        EncodeRules(rules);

    }

    public void EncodeRules(List<string>) //????
    {

    }

    public void getCombinations()
    {

    }

    public void infeasiblePop()
    {
        fitness1();
    }

    public void feasiblePop()
    {
        mazeGen();
        fitness2();
    }

    public void fitness1()
    {
        int fvalue = 0;
        if (fvalue > 0.5)
        {
            feasiblePop();
        }
    }
    public void fitness2()
    {

    }

    public void mazeGen()
    {
        //generate the maze with these set of rules r
        //can it build MST without error?
        fitness2();
    }
}
