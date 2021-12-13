using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class newRules
{
    public int index;
    public int distance;
    public Type type;
    public Direction direction;
    public Colour src; //set this later
    public Colour target;
    public bool inclusion;
}
public class chromosome
{
    public newRules r1;
    public newRules r2;
    public newRules r3;
    public newRules r4;
    public newRules r5;
    public newRules r6;
    public newRules r7;
    public newRules r8;

    
}
public class Crossover : MonoBehaviour
{
    public static int pop = Rules.popSize;
    public static int newIdx = 0;
    public static List<chromosome> mc = new List<chromosome>();
    //public static List<newRules> newruleslist = new List<newRules>();
    public static Colour[] scolors = new Colour[8] { Colour.Red, Colour.Orange, Colour.Yellow, Colour.Pink, Colour.Teal, Colour.Blue, Colour.Purple, Colour.Green };
    public static Colour[] tcolors = new Colour[11] { Colour.Red, Colour.Orange, Colour.Yellow, Colour.Pink, Colour.Teal, Colour.Blue, Colour.Purple, Colour.Green, Colour.All, Colour.Warm, Colour.Cool };
    // Start is called before the first frame update

    public static Colour setTarget(newRules nr, int idx)
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
        if (nr.type == Type.teleport)
        {
            nr.target = tcolors[(idx + 1) % tcolors.Length]; //circular array concept
        }
        return nr.target;
    }

    public static void assignUniqueColors(List<chromosome> mc)
    {
        foreach (chromosome zz in mc)
        {
            List<int> usedIdx = new List<int>();
            System.Random rand = new System.Random();
            int idx = rand.Next(0, scolors.Length);
            //usedIdx.Add(idx);

            //if (!usedIdx.Contains(idx))
            //{
            zz.r1.src = scolors[idx];
            zz.r1.target = setTarget(zz.r1, idx);
            idx++;
            if (idx >= 8)
            {
                idx = 0;
            }
            zz.r2.src = scolors[idx];
            zz.r2.target = setTarget(zz.r2, idx);
            idx++;
            if (idx >= 8)
            {
                idx = 0;
            }
            zz.r3.src = scolors[idx];
            zz.r3.target = setTarget(zz.r3, idx);
            idx++;
            if (idx >= 8)
            {
                idx = 0;
            }
            zz.r4.src = scolors[idx];
            zz.r4.target = setTarget(zz.r4, idx);
            idx++;
            if (idx >= 8)
            {
                idx = 0;
            }
            zz.r5.src = scolors[idx];
            zz.r5.target = setTarget(zz.r5, idx);
            idx++;
            if (idx >= 8)
            {
                idx = 0;
            }
            zz.r6.src = scolors[idx];
            zz.r6.target = setTarget(zz.r6, idx);
            idx++;
            if (idx >= 8)
            {
                idx = 0;
            }
            zz.r7.src = scolors[idx];
            zz.r7.target = setTarget(zz.r7, idx);
            idx++;
            if (idx >= 8)
            {
                idx = 0;
            }
            zz.r8.src = scolors[idx];
            zz.r8.target = setTarget(zz.r8, idx);
            idx++;
            if (idx >= 8)
            {
                idx = 0;
            }





        }
    }


    public static void makeCopies(chromosome xx)
    {
        int count = (int)System.Math.Ceiling(0.2 * pop);
        for (int i = 0; i < count; i++)
        {
            chromosome yy = xx;
            mc.Add(yy);
        }
    }

    public Crossover()
    {

    }
    public static void crossover(Dictionary<int, int> chosenChr, Dictionary<int, Dictionary<int, Type>> clist, List<MovementRule> m, List<ColourRule> c)
    {
        //Dictionary<int, int> newChrs = new Dictionary<int, int>();



        foreach (KeyValuePair<int, int> k in chosenChr) //suppose to run (20% of popsize) times = 4
        {

            //List<Type>

            chromosome xx = new chromosome();
            foreach (KeyValuePair<int, Dictionary<int, Type>> x in clist)
            {
                Debug.Log("reached crossover" + k.Key + " ," + x.Key);

                //ONE CHROMOSOME:
                ICollection ruleIndexs = x.Value.Keys; //8 rules indexs
                                                       //ICollection ruleTypes= x.Value.Values;//8 rule types

                //chromosome mc = new chromosome();

                int count = 1;
                foreach (int i in ruleIndexs)
                {


                    if (i < m.Count) //its a movement rule
                    {
                        MovementRule mm = Fitness1.GetMRule(i, m);
                        switch (count)
                        {

                            case 1:
                                xx.r1.type = mm.type;
                                xx.r1.distance = mm.distance;
                                xx.r1.direction = mm.direction;
                                count++;
                                break;
                            case 2:
                                xx.r2.type = mm.type;
                                xx.r2.distance = mm.distance;
                                xx.r2.direction = mm.direction;
                                count++;
                                break;
                            case 3:
                                xx.r3.type = mm.type;
                                xx.r3.distance = mm.distance;
                                xx.r3.direction = mm.direction;
                                count++;
                                break;
                            case 4:
                                xx.r4.type = mm.type;
                                xx.r4.distance = mm.distance;
                                xx.r4.direction = mm.direction;
                                count++;
                                break;
                            case 5:
                                xx.r5.type = mm.type;
                                xx.r5.distance = mm.distance;
                                xx.r5.direction = mm.direction;
                                count++;
                                break;
                            case 6:
                                xx.r6.type = mm.type;
                                xx.r6.distance = mm.distance;
                                xx.r6.direction = mm.direction;
                                count++;
                                break;
                            case 7:
                                xx.r7.type = mm.type;
                                xx.r7.distance = mm.distance;
                                xx.r7.direction = mm.direction;
                                count++;
                                break;
                            case 8:
                                xx.r8.type = mm.type;
                                xx.r8.distance = mm.distance;
                                xx.r8.direction = mm.direction;
                                count++;
                                break;
                            default:
                                Debug.Log("Your input in default case is out of range");
                                break;
                        }


                    }



                    //createMCopies(mm,);

                    else
                    {
                        ColourRule mm = Fitness1.GetCRule(i, c);
                        //createCCopies(cc,);
                        switch (count)
                        {

                            case 1:
                                xx.r1.type = mm.type;
                                count++;
                                break;
                            case 2:
                                xx.r2.type = mm.type;
                                count++;
                                break;
                            case 3:
                                xx.r3.type = mm.type;
                                count++;
                                break;
                            case 4:
                                xx.r4.type = mm.type;
                                count++;
                                break;
                            case 5:
                                xx.r5.type = mm.type;
                                count++;
                                break;
                            case 6:
                                xx.r6.type = mm.type;
                                count++;
                                break;
                            case 7:
                                xx.r7.type = mm.type;
                                count++;
                                break;
                            case 8:
                                xx.r8.type = mm.type;
                                count++;
                                break;
                            default:
                                Debug.Log("Your input in default case is out of range");
                                break;
                        }


                    }
                }
            }
            makeCopies(xx);
        }
        assignUniqueColors(mc);

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
            newRules newM = new newRules();
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

            newruleslist.Add(newM);

        }

    }
    public static void createCCopies(ColourRule cc, int idx)
    {
        int copyCount = (int)System.Math.Ceiling(0.2 * pop); //avoiding zero with ceiling
        for (int i = 0; i < copyCount; i++)
        {
            newRules newC = new newRules();
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
            
            

            newruleslist.Add(newC);

        }
}*/


    


 