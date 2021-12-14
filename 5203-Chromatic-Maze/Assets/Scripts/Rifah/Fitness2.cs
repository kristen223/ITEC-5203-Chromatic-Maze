using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fitness2 : MonoBehaviour
{


    //num of deadEnds
    //num of checkers
    //min num of moves (dynamic algo?)
    //actual num of moves
    //if you want to make it more difficult => maximise the #of moves
    public struct ColouredMaze
    {
        public KruskalMaze.Maze maze;
        public Dictionary<int, int> used; //rule index and amount of times rule was used
        public int checkers; //number of checkers
        public bool properExit; //true if exit was assigned properly, false if exit was assigned to be traversable via solution path, but violates rule(s) with other adjacent tiles (not added to onPathViolations because it doesn't wreck the path)
        public int onPathViolations; //number of tiles that violate rule(s) of one or more adjacent tiles on solution path (not including CyclesAdded count)
        public int offPathViolations; //number of tiles that violate rule(s) of one or more adjacent tiles off solution path (not including CyclesAdded count)
    }



    //public static void fitness2(ColourAssigner.ColouredMaze colouredMaze)
    public static List<Chromosome> fitness2a(List<Chromosome> mc)
    {
        //PART 3 : CHECKING BADNESS WITH COLORS : "target color of teleport should not be src color of include/exclude"
        //ICollection ruleIdxs = d.Keys;
        int fit = 0;
        List<Chromosome> chosenChr2 = new List<Chromosome>();

        for (int k = 0; k < mc.Count; k++)
        //foreach (Chromosome r in mc)
        {
            List<NewRules> rr = new List<NewRules>() { mc[k].r1, mc[k].r2, mc[k].r3, mc[k].r4, mc[k].r5, mc[k].r6, mc[k].r7, mc[k].r8 };
            foreach (NewRules rule in rr)
            {
                if (rule.type == Type.teleport)
                {
                    foreach (NewRules rule2 in rr)
                    {
                        if (rule2.type == Type.include || rule2.type == Type.exclude)
                        {
                            if (rule.target == rule2.src)
                            {
                                fit = fit - 10;
                            }
                        }
                    }
                }
            }
            //PART 4 : WARM/COOL BALANCE for src only
            int wrm = 0;
            int cld = 0;

            for(int j = 0; j < rr.Count; j++)
            //foreach (NewRules rule in rr)
            {
                
                if (rr[j].src == Colour.Warm || rr[j].src == Colour.Red || rr[j].src == Colour.Orange || rr[j].src == Colour.Pink || rr[j].src == Colour.Yellow || rr[j].target == Colour.Warm || rr[j].target == Colour.Red || rr[j].target == Colour.Orange || rr[j].target == Colour.Pink || rr[j].target == Colour.Yellow)
                {
                    wrm++;
                }
                if (rr[j].src == Colour.Cool || rr[j].src == Colour.Blue || rr[j].src == Colour.Green || rr[j].src == Colour.Teal || rr[j].src == Colour.Teal || rr[j].target == Colour.Cool || rr[j].target == Colour.Blue || rr[j].target == Colour.Teal || rr[j].target == Colour.Green || rr[j].target == Colour.Purple)
                {
                    cld++;
                }

            }

            if (wrm == cld)
            {
                fit = fit + 5; //most fit = 8+2 = 10
            }
            if (wrm != cld)
            {
                fit = fit - (Mathf.Abs(wrm - cld));
            }

            Chromosome c = mc[k];
            c.fit = fit;
            mc[k] = c;
        }
        Debug.Log("Done adding all the fitnesses");

        List<int> fitvals2a = new List<int>();
        foreach(Chromosome r in mc){
            fitvals2a.Add(r.fit);
        }
        Debug.Log("added the fitness values to a list for sorting");
        fitvals2a.Sort();
        fitvals2a.Reverse();
        List<int> chosenfits = new List<int>();
        for (int h = 0; h < (int)Math.Ceiling(fitvals2a.Count*0.2); h++)
        {
            chosenfits.Add(fitvals2a[h]);
            
        }
        Debug.Log("chosenfits now has the fittest values gone through fitness2a with length"+chosenfits.Count);
        int count = 0;
        foreach (Chromosome r in mc)
        {

            for (int i = 0; i < chosenfits.Count; i++)
            {
                if (r.fit == chosenfits[i])
                {
                    Debug.Log(chosenfits[count] + "==" + r.fit);
                    chosenChr2.Add(r);
                    Debug.Log("chosenChr2 length is " + chosenChr2.Count);
                }
            }
            //if (chosenfits[count] == r.fit )
            //{
            //    Debug.Log(chosenfits[count] + "==" + r.fit);
            //    chosenChr2.Add(r);
            //    Debug.Log("chosenChr2 length is " + chosenChr2.Count);
                
            //}
            //count++;
        }
        Debug.Log("ChosenChr2 created and passed back from fitness2a");
        return chosenChr2;
    }
    
   
}
