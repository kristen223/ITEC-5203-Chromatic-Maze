using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InstructionsText : MonoBehaviour
{

    private static Text ruleText;
    private static string instructions;

    void Awake()
    {
        instructions = "Rules: " + "\n";
        ruleText = GameObject.Find("ruleText").GetComponent<Text>();
    }

    public static void SetInstructions(List<MovementRule> mRules, List<ColourRule> cRules)
    {
        int count = 1;
        foreach(MovementRule mr in mRules) //Tmove, blank, teleport, jump1, jump2, warm, cool,include, exclude
        {
            switch(mr.type)
            {
                case Type.Tmove:
                    instructions += count.ToString() + ". " + mr.src + ": Can't move " + mr.direction.ToString().ToLower() + "\n";
                    break;
                case Type.blank:
                    instructions += count.ToString() + ". " + mr.src + ": Can move in any direction by one" + "\n";
                    break;
                case Type.teleport:
                    instructions += count.ToString() + ". " + mr.src + ": Can teleport to any " + mr.target.ToString().ToLower() + " tile" + "\n";
                    break;
                case Type.warm:
                    instructions += count.ToString() + ". " + mr.src + ": Can move to any warm-coloured tile" + "\n";
                    break;
                case Type.cool:
                    instructions += count.ToString() + ". " + mr.src + ": Can move to any cool-coloured tile" + "\n";
                    break;
                case Type.jump1:
                    instructions += count.ToString() + ". " + mr.src + ": Can jump over one tile in any direction" + "\n";
                    break;
                case Type.jump2:
                    instructions += count.ToString() + ". " + mr.src + ": Can jump over two tiles in any direction" + "\n";
                    break;
            }
            count++;
        }

        foreach (ColourRule cr in cRules)
        {
            switch (cr.type)
            {
                case Type.include:
                    instructions += count.ToString() + ". " + cr.src + ": Can only move onto adjacent " + cr.target.ToString().ToLower() + " tiles" + "\n";
                    break;
                case Type.exclude:
                    instructions += count.ToString() + ". " + cr.src + ": Can move onto any adjacent tile except " + cr.target.ToString().ToLower() + "\n";
                    break;
            }
            count++;
        }
        ruleText.text = instructions;
    }
}
