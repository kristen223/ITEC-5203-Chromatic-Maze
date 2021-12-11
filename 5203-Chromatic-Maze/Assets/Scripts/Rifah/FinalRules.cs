using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinalRules : MonoBehaviour
{
    // Start is called before the first frame update
    public static void finalRules(Dictionary<int, Type> c, List<MovementRule> movementRuleSets, List<ColourRule> colourRuleSets) //get the indexes (keys) of this hashtable
    {

        int[] finalIdxs = new int[8];
        int r = 0;
        foreach (int i in c.Keys)
        {
            finalIdxs[r] = (int)i;
            r++;
        }
        //Debug.Log();
        //all the final indexes are set in finalIdxs

        List<MovementRule> mr = new List<MovementRule>();
        List<ColourRule> cr = new List<ColourRule>();


        foreach (KeyValuePair<int, Type> kvp in c)
        {
            if (kvp.Value.Equals("Tmove") | kvp.Value.Equals("blank") | kvp.Value.Equals("teleport") | kvp.Value.Equals("jump1") | kvp.Value.Equals("jump2") | kvp.Value.Equals("warm") | kvp.Value.Equals("cool"))
            {
                mr.Add(movementRuleSets.Find(x => x.index.Equals(kvp.Key)));
                Debug.Log("mr added is" + kvp.Key);
            }
            else
            {
                cr.Add(colourRuleSets.Find(y => y.index.Equals(kvp.Key)));
            }
        }


        Debug.Log("calling set rules");

        ColourAssigner.SetRules(mr, cr);

        

    }
}
