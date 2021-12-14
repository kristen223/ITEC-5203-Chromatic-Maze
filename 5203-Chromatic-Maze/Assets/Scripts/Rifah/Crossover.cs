using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct NewRules
{
    public int index;
    public int distance;
    public Type type;
    public Direction direction;
    public Colour src; //set this later
    public Colour target;
    public bool inclusion;
}

public struct Chromosome
{
    public NewRules r1;
    public NewRules r2;
    public NewRules r3;
    public NewRules r4;
    public NewRules r5;
    public NewRules r6;
    public NewRules r7;
    public NewRules r8;
    public int fit;

    public List<NewRules> NewRules;
}


public class Crossover : MonoBehaviour
{
    //public int pop = Rules.popSize;
    public static int newIdx = 0;

    public static List<Chromosome> mc = new List<Chromosome>();
    //public static List<NewRules> NewRuleslist = new List<NewRules>();
    public static Colour[] scolors = new Colour[8] { Colour.Red, Colour.Orange, Colour.Yellow, Colour.Pink, Colour.Teal, Colour.Blue, Colour.Purple, Colour.Green };
    public static Colour[] tcolors = new Colour[11] { Colour.Red, Colour.Orange, Colour.Yellow, Colour.Pink, Colour.Teal, Colour.Blue, Colour.Purple, Colour.Green, Colour.All, Colour.Warm, Colour.Cool };
    // Start is called before the first frame update

    public static Colour setTarget(NewRules nr, int idx)
    {
        if (nr.type == Type.warm)
        {
            nr.target = Colour.Warm;
        }

        if (nr.type == Type.cool)
        {
            nr.target = Colour.Cool;
        }
        if (nr.type == Type.blank || nr.type == Type.Tmove || nr.type == Type.jump1 || nr.type == Type.jump2)
        {
            nr.target = Colour.All;

        }
        if (nr.type == Type.teleport || nr.type==Type.include || nr.type == Type.exclude)
        {
            nr.target = scolors[(idx + 1) % scolors.Length]; //circular array concept
        }
        return nr.target;
    }

    public static List<Chromosome> assignUniqueColors(List<Chromosome> mc)
    {
        List<Chromosome> newMc = new List<Chromosome>();
       // foreach (Chromosome zz in mc)
       for (int i = 0; i < mc.Count; i++)
        {
            List<int> usedIdx = new List<int>();
            System.Random rand = new System.Random();
            int idx = rand.Next(0, scolors.Length);

            Chromosome c = mc[i];
            NewRules rule1 = mc[i].r1;
            rule1.src = scolors[idx];

            rule1.target = setTarget(rule1, idx);
            idx++; //going serially which could be bad
            if (idx >= 8)
            {
                idx = 0;
            }
            c.r1 = rule1;
            mc[i] = c;

            NewRules rule2 = mc[i].r2;
            rule2.src = scolors[idx];
            rule2.target = setTarget(rule2, idx);

            idx++;
            if (idx >= 8)
            {
                idx = 0;
            }
            c.r2 = rule2;

            NewRules rule3 = mc[i].r3;
            rule3.src = scolors[idx];
            rule3.target = setTarget(rule3, idx);
            idx++;
            if (idx >= 8)
            {
                idx = 0;
            }
            c.r3 = rule3;

            NewRules rule4 = mc[i].r4;
            rule4.src = scolors[idx];
            rule4.target = setTarget(rule4, idx);
            idx++;
            if (idx >= 8)
            {
                idx = 0;
            }
            c.r4 = rule4;

            NewRules rule5 = mc[i].r5;
            rule5.src = scolors[idx];
            rule5.target = setTarget(rule5, idx);
            idx++;
            if (idx >= 8)
            {
                idx = 0;
            }
            c.r5 = rule5;

            NewRules rule6 = mc[i].r6;
            rule6.src = scolors[idx];
            rule6.target = setTarget(rule6, idx);
            idx++;
            if (idx >= 8)
            {
                idx = 0;
            }
            c.r6 = rule6;

            NewRules rule7 = mc[i].r7;
            rule7.src = scolors[idx];
            rule7.target = setTarget(rule7, idx);
            idx++;
            if (idx >= 8)
            {
                idx = 0;
            }
            c.r7 = rule7;

            NewRules rule8 = mc[i].r8;
            rule8.src = scolors[idx];
            rule8.target = setTarget(rule8, idx);
            idx++;
            if (idx >= 8)
            {
                idx = 0;
            }
            c.r8 = rule8;

            mc[i] = c;


            Debug.Log("kk Start of Chromosome");
            Debug.Log("kk" + mc[i].r1.src + " " + mc[i].r1.target + " " + mc[i].r1.type);
            Debug.Log("kk" + mc[i].r2.src + " " + mc[i].r2.target + " " + mc[i].r2.type);
            Debug.Log("kk" + mc[i].r3.src + " " + mc[i].r3.target + " " + mc[i].r3.type);
            Debug.Log("kk" + mc[i].r4.src + " " + mc[i].r4.target + " " + mc[i].r4.type);
            Debug.Log("kk" + mc[i].r5.src + " " + mc[i].r5.target + " " + mc[i].r5.type);
            Debug.Log("kk" + mc[i].r6.src + " " + mc[i].r6.target + " " + mc[i].r6.type);
            Debug.Log("kk" + mc[i].r7.src + " " + mc[i].r7.target + " " + mc[i].r7.type);
            Debug.Log("kk" + mc[i].r8.src + " " + mc[i].r8.target + " " + mc[i].r8.type);
            Debug.Log("kk End");

        }
        return mc;
    }


    public static void makeCopies(Chromosome xx,int pop) //single chr getting in
    {
        
        //xx has ALL TMOVES

        List<Chromosome> copyChr = new List<Chromosome>();
        int count = (int)System.Math.Ceiling(0.2 * pop);
        for (int i = 0; i < count; i++)
        {
            Chromosome yy = xx;
            Debug.Log("adding to copychr" + yy.ToString());
            mc.Add(yy);
            //copyChr.Add(yy);
        }
        Debug.Log("LENGTH OF COPIEDchr " + copyChr.Count);
        //return copyChr;
    }
    
    public Crossover()
    {

    }
    public static void crossovers(Dictionary<int, int> chosenChr, Dictionary<int, Dictionary<int, Type>> clist, List<MovementRule> m, List<ColourRule> c, int pop)
    {
        //Dictionary<int, int> newChrs = new Dictionary<int, int>();
        Debug.Log("at crossover and length of chosenChr is "+chosenChr.Count );

        
        foreach (KeyValuePair<int, int> k in chosenChr) //suppose to run (20% of popsize) times => 4 IF POP=10
        {

            //NewRules 
            Chromosome xx = new Chromosome();
            xx.r1 = new NewRules();
            xx.r2 = new NewRules();
            xx.r3 = new NewRules();
            xx.r4 = new NewRules();
            xx.r5 = new NewRules();
            xx.r6 = new NewRules();
            xx.r7 = new NewRules();
            xx.r8 = new NewRules();
            xx.fit = 0;

            //chromosome<NewRules> xx = new chromosome<NewRules>();
           //GameObject g = Instantiate(new GameObject(), new Vector3(0,0,0), Quaternion.Euler(0, 0, 0));

           // g.AddComponent<Chromosome>(); //CHANGED THESE TWO LINES
            //Chromosome xx = g.GetComponent<Chromosome>(); //rest of your script still uses your old chromosome class.

            foreach (KeyValuePair<int, Dictionary<int, Type>> x in clist)
            {
                Debug.Log("reached crossover" + k.Key + " ," + x.Key);

                //ONE CHROMOSOME:
                ICollection ruleIndexs = x.Value.Keys; //8 rules indexs
                Debug.Log(ruleIndexs);                                       //ICollection ruleTypes= x.Value.Values;//8 rule types

                //chromosome mc = new chromosome();

                //int count = 1;
                //CONVERTING INTO LIST OF CHROMOSOME STRUCT:
                List<NewRules> r = new List<NewRules> { xx.r1, xx.r2, xx.r3, xx.r4, xx.r5, xx.r6, xx.r7, xx.r8 };
                int j = 0;
                foreach (int i in ruleIndexs)
                {
                    
                    Debug.Log("ruleindex originals: " + i);
                   
                    if (i < m.Count) //its a movement rule
                    {



                        // for (int j = 0; j < r.Count; j++)
                        //{

                        MovementRule mm = new MovementRule();
                        mm = Fitness1.GetMRule(i, m);
                        Debug.Log("made a movement rule src is " + mm.src);
                        Debug.Log("rule type is" + mm.type);
                        NewRules rule = r[j];
                        rule.type = mm.type;
                        rule.direction = mm.direction;
                        rule.distance = mm.distance;
                       

                        r[j] = rule;
                        
                        j++;

                        
                        
                        

                        //}
                    }
                    else
                    {


                        // for (int j = 0; j < r.Count; j++)
                        //{
                        ColourRule cc = new ColourRule();
                        cc = Fitness1.GetCRule(i, c);
                        Debug.Log("made a color rule and src is " + cc.src);
                        Debug.Log("type is" + cc.type);
                        NewRules rule = r[j];
                        rule.type = cc.type;
                        rule.inclusion = cc.inclusion;
                        r[j] = rule;
                        j++;
                        
                        

                    }

                    
                        
                    


                }
                foreach (NewRules g in r)
                {
                    Debug.Log("TYPE IS HERE-------------------" + g.type);

                }

                xx.r1 = r[0];
                xx.r2 = r[1];
                xx.r3 = r[2];
                xx.r4 = r[3];
                xx.r5 = r[4];
                xx.r6 = r[5];
                xx.r7 = r[6];
                xx.r8 = r[7];

                    

                    Debug.Log("for loop ended");






            }
            Debug.Log("make copies now");
            makeCopies(xx,pop);

        }
       



        List<Chromosome> chrList = assignUniqueColors(mc);
        
        Debug.Log("length of mc is "+chrList.Count);
        List< Chromosome> chrList2a = new List<Chromosome>();
        Debug.Log("hi");
        
        chrList2a = Fitness2.fitness2a(chrList);
        foreach (Chromosome ch in chrList2a)
        {
            Debug.Log("==================================");
            Debug.Log("all the rule types and target  are:" + ch.r1.type + "-"+ch.r1.target);
            Debug.Log("all the rule types are:" + ch.r2.type + "-" + ch.r2.target);
            Debug.Log("all the rule types are:" + ch.r3.type + "-" + ch.r3.target);
            Debug.Log("all the rule types are:" + ch.r4.type + "-" + ch.r4.target);
            Debug.Log("all the rule types are:" + ch.r5.type + "-" + ch.r5.target);
            Debug.Log("all the rule types are:" + ch.r6.type + "-" + ch.r6.target);
            Debug.Log("==================================");



        }
        Debug.Log("bye");
        Debug.Log("seperate rules now");
        MazeCreation.seperateRules(chrList2a);
       

    }
}






    


 