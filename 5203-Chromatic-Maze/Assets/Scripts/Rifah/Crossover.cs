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
            nr.target = tcolors[(idx + 1) % tcolors.Length]; //circular array concept
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
            //usedIdx.Add(idx);
            //for (int i = 0; i < zz.mr.Count; i++)
            //{
            //    Colour c1 = scolors[idx];
            //    zz.mr[i].src =c1 ;
            //}

            //}
            //if (!usedIdx.Contains(idx))
            //{



            //mc[i].r1.src = scolors[idx];
            //zz.r1.target = setTarget(zz.r1, idx);

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

            NewRules rule3 = mc[i].r3;
            rule3.src = scolors[idx];
            rule3.target = setTarget(rule3, idx);
            idx++;
            if (idx >= 8)
            {
                idx = 0;
            }

            NewRules rule4 = mc[i].r4;
            rule4.src = scolors[idx];
            rule4.target = setTarget(rule4, idx);
            idx++;
            if (idx >= 8)
            {
                idx = 0;
            }

            NewRules rule5 = mc[i].r5;
            rule5.src = scolors[idx];
            rule5.target = setTarget(rule5, idx);
            idx++;
            if (idx >= 8)
            {
                idx = 0;
            }

            NewRules rule6 = mc[i].r6;
            rule6.src = scolors[idx];
            rule6.target = setTarget(rule6, idx);
            idx++;
            if (idx >= 8)
            {
                idx = 0;
            }

            NewRules rule7 = mc[i].r7;
            rule7.src = scolors[idx];
            rule7.target = setTarget(rule7, idx);
            idx++;
            if (idx >= 8)
            {
                idx = 0;
            }

            NewRules rule8 = mc[i].r8;
            rule8.src = scolors[idx];
            rule8.target = setTarget(rule8, idx);
            idx++;
            if (idx >= 8)
            {
                idx = 0;
            }


        }
        return mc;
    }


    public static void makeCopies(Chromosome xx,int pop)
    {
        int count = (int)System.Math.Ceiling(0.2 * pop);
        for (int i = 0; i < count; i++)
        {
            Chromosome yy = xx;
            Debug.Log("adding to mc" + yy.ToString());
            mc.Add(yy);
        }
    }
    
    public Crossover()
    {

    }
    public static void crossovers(Dictionary<int, int> chosenChr, Dictionary<int, Dictionary<int, Type>> clist, List<MovementRule> m, List<ColourRule> c, int pop)
    {
        //Dictionary<int, int> newChrs = new Dictionary<int, int>();
        Debug.Log("at crossover and length of chosenChr is "+chosenChr.Count );

        
        foreach (KeyValuePair<int, int> k in chosenChr) //suppose to run (20% of popsize) times = 4
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
            xx.fit = -100;

            //chromosome<NewRules> xx = new chromosome<NewRules>();
            GameObject g = Instantiate(new GameObject(), new Vector3(0,0,0), Quaternion.Euler(0, 0, 0));

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

                foreach (int i in ruleIndexs)
                {
                    List<NewRules> r = new List<NewRules> { xx.r1, xx.r2, xx.r3, xx.r4, xx.r5, xx.r6, xx.r7, xx.r8 };
                    Debug.Log("ruleindex originals: " + i);
                    if (i < m.Count) //its a movement rule
                    {
                        MovementRule mm = new MovementRule();
                        mm = Fitness1.GetMRule(i, m);
                        Debug.Log("made a movement rule and src is " + mm.src);
                        Debug.Log("type is"+mm.type);


                        //CANNOT modify rx if youre traversing through r
                        //foreach (NewRules rx in r)
                        //{
                        //    Debug.Log("reached forloop");
                        //    rx.type = mm.type;
                        //    rx.distance = mm.distance;
                        //    rx.direction = mm.direction;
                        //}


                        for (int j = 0; j < r.Count; j++)
                        {
                            //xx[j].r.type = mm.type;
                            //xx
                            //r[0].type = Type.blank;

                            //not sure if this is what you want or not
                            NewRules rule = r[j];
                            rule.type = Type.blank;
                            r[j] = rule;
                        }
                    }
                    else
                    {
                        ColourRule cc = new ColourRule();
                        cc = Fitness1.GetCRule(i, c);
                        Debug.Log("made a color rule and src is " + cc.src);
                        Debug.Log("type is" + cc.type);

                        for (int j = 0; j < r.Count; j++)
                        {
                            NewRules rule = r[j];
                            rule.type = cc.type;
                            rule.inclusion = cc.inclusion;
                            r[j] = rule;
                        }

                        //    foreach (NewRules rx in r)
                        //{
                        //    rx.type = cc.type;
                        //    rx.inclusion = cc.inclusion;

                        //}
                    }
                }
                    //switch (count)
                    //{

                    //    case 1:
                    //        xx.r1.type = mm.type;
                    //        xx.r1.distance = mm.distance;
                    //        xx.r1.direction = mm.direction;
                    //        count++;
                    //        break;
                    //    case 2:
                    //        xx.r2.type = mm.type;
                    //        xx.r2.distance = mm.distance;
                    //        xx.r2.direction = mm.direction;
                    //        count++;
                    //        break;
                    //    case 3:
                    //        xx.r3.type = mm.type;
                    //        xx.r3.distance = mm.distance;
                    //        xx.r3.direction = mm.direction;
                    //        count++;
                    //        break;
                    //    case 4:
                    //        xx.r4.type = mm.type;
                    //        xx.r4.distance = mm.distance;
                    //        xx.r4.direction = mm.direction;
                    //        count++;
                    //        break;
                    //    case 5:
                    //        xx.r5.type = mm.type;
                    //        xx.r5.distance = mm.distance;
                    //        xx.r5.direction = mm.direction;
                    //        count++;
                    //        break;
                    //    case 6:
                    //        xx.r6.type = mm.type;
                    //        xx.r6.distance = mm.distance;
                    //        xx.r6.direction = mm.direction;
                    //        count++;
                    //        break;
                    //    case 7:
                    //        xx.r7.type = mm.type;
                    //        xx.r7.distance = mm.distance;
                    //        xx.r7.direction = mm.direction;
                    //        count++;
                    //        break;
                    //    case 8:
                    //        xx.r8.type = mm.type;
                    //        xx.r8.distance = mm.distance;
                    //        xx.r8.direction = mm.direction;
                    //        count++;
                    //        break;
                    //    default:
                    //        Debug.Log("Your input in default case is out of range");
                    //        break;
                    //}

                    Debug.Log("for loop ended");






            }
            Debug.Log("make copies now");
            makeCopies(xx,pop);

        }
        List<Chromosome> chrList = assignUniqueColors(mc);
        Debug.Log("length of mc is "+chrList.Count);
        List< Chromosome> chrList2a = new List<Chromosome>();
        chrList2a = Fitness2.fitness2a(chrList);
        Debug.Log("seperate rules now");
        MazeCreation.seperateRules(chrList2a);
        //MazeCreation.

    }
}
















            //if (k.Key == x.Key)
            //{
            //    List<MovementRule> mr = new List<MovementRule>();
            //    List<ColourRule> cr = new List<ColourRule>();
            //    //foreach (Dictionary<int, Type> d in clist.Values) //suppose to
            //    //{
            //    int i = 0;
            //    List<Colour> uniqueColors = new List<Colour>();

            //    foreach (KeyValuePair<int, Type> kvp in x.Value) //supposed to run 8 times
            //    {

            //        kvp.k


            //            if (kvp.Value == Type.exclude || kvp.Value == Type.include)
            //        {

            //            ColourRule z = Fitness1.GetCRule(kvp.Key, c);
            //            Debug.Log("original color was---------------------------------" + z.src);

            //            z.src = allcolors[i];
            //            i++;
            //            Debug.Log("new color is------------------------- " + z.src);
            //            cr.Add(z);
            //        }
            //        else
            //        {
            //            MovementRule y = Fitness1.GetMRule(kvp.Key, m);
            //            Debug.Log("original color was---------------------------------" + y.src);

            //            y.src = allcolors[i];


            //            i++;
            //            Debug.Log("new color is------------------------- " + y.src);
            //            mr.Add(y);
            //        }


            //    }
              //  MazeCreation.getFinalRules(chosenChr, clist, m, c);


                //get the rule of each index in chosenChr (get rules method)
                //each index has 8 rules.
                //make 5 more chromosomes with the same 8 rules. so just copy.
                //change the colors.
            

            // return newChrs;
   


    /*public static void createMCopies(MovementRule mm, int idx)
    {
        int copyCount = (int)System.Math.Ceiling(0.2 * pop); //avoiding zero with ceiling
        for (int i = 0; i < copyCount; i++)
        {
            NewRules newM = new NewRules();
            newM.index = newIdx++;
            newM.type = mm.type;
            newM.src = scolors[idx];//find whats idx here???
            if (newM.type == Type.warm)
            {
                newM.target = Colour.Warm;
            }
            if(newM.type == Type.cool)
            {
                newM.target = Colour.Cool;
            }
            if(newM.type == Type.blank || newM.type == Type.Tmove || newM.type == Type.jump1 || newM.type == Type.jump2)
            {
                newM.target = Colour.All;
                newM.direction = mm.direction;//null exception? if happens, just set a value in their direction in rules.cs
                newM.distance = mm.distance;  //null exception?
            }
            if (newM.type == Type.teleport)
            {
                newM.target = tcolors[(idx+1)%tcolors.Length]; //circular array concept
            }

            NewRuleslist.Add(newM);

        }

    }
    public static void createCCopies(ColourRule cc, int idx)
    {
        int copyCount = (int)System.Math.Ceiling(0.2 * pop); //avoiding zero with ceiling
        for (int i = 0; i < copyCount; i++)
        {
            NewRules newC = new NewRules();
            newC.index = newIdx++;
            newC.type = cc.type;
            newC.src = scolors[idx];//find whats idx here???firgurw out later
            if (newC.type == Type.include)
            {
                newC.target = scolors[(idx + 1) % tcolors.Length];//scolors bec cant be warm,cool,all
            }
            if (newC.type == Type.exclude)
            {
                newC.target = scolors[(idx + 1) % tcolors.Length]; ;
            }
            
            

            NewRuleslist.Add(newC);

        }
}*/


    


 