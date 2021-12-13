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
    public static List<chromosome<newRules>> fitness2a(List<chromosome<newRules>> mc)
    {
        //PART 3 : CHECKING BADNESS WITH COLORS : "target color of teleport should not be src color of include/exclude"
        //ICollection ruleIdxs = d.Keys;
        int fit = 0;
        List<chromosome<newRules>> chosenChr2 = new List<chromosome<newRules>>();
        foreach (chromosome<newRules> r in mc)
        {
            List<newRules> rr = new List<newRules>() { r.r1, r.r2, r.r3, r.r4, r.r5, r.r6, r.r7, r.r8 };
            foreach (newRules rule in rr)
            {
                if (rule.type == Type.teleport)
                {
                    foreach (newRules rule2 in rr)
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

            foreach (newRules rule in rr)
            {
                
                if (rule.src == Colour.Warm || rule.src == Colour.Red || rule.src == Colour.Orange || rule.src == Colour.Pink || rule.src == Colour.Yellow || rule.target == Colour.Warm || rule.target == Colour.Red || rule.target == Colour.Orange || rule.target == Colour.Pink || rule.target == Colour.Yellow)
                {
                    wrm++;
                }
                if (rule.src == Colour.Cool || rule.src == Colour.Blue || rule.src == Colour.Green || rule.src == Colour.Teal || rule.src == Colour.Teal || rule.target == Colour.Cool || rule.target == Colour.Blue || rule.target == Colour.Teal || rule.target == Colour.Green || rule.target == Colour.Purple)
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
            r.fit = fit;

        }
        List<int> fitvals2a = new List<int>();
        foreach(chromosome<newRules> r in mc){
            fitvals2a.Add(r.fit);
        }
        fitvals2a.Sort();
        fitvals2a.Reverse();
        List<int> chosenfits = new List<int>();
        for (int k = 0; k < (int)Math.Ceiling(fitvals2a.Count*0.2); k++)
        {
            chosenfits.Add(fitvals2a[k]);
            
        }
        int i = 0;
        foreach (chromosome<newRules> r in mc)
        {
            if (chosenfits[i]==r.fit)
            {
                chosenChr2.Add(r);
                i++;
            }
        }
        return chosenChr2;
    }
    
   
}
