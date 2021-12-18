using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InstructionsText : MonoBehaviour
{

    private static Text ruleText;
    private static string instructions;

    private static string red;
    private static string orange;
    private static string yellow;
    private static string green;
    private static string blue;
    private static string purple;
    private static string teal;
    private static string pink;

    void Awake()
    {
        instructions = "All rules imply orthogonal movement by one, unless otherwise specified." + "\n" + "\n";
        ruleText = GameObject.Find("ruleText").GetComponent<Text>();
    }

    public static void SetInstructions(List<MovementRule> mRules, List<ColourRule> cRules)
    {
        foreach(MovementRule mr in mRules) //Tmove, blank, teleport, jump1, jump2, warm, cool,include, exclude
        {
            string s = "";
            switch(mr.type)
            {
                case Type.Tmove:
                    s += mr.src + ": Can't move " + mr.direction.ToString().ToLower() + "\n";
                    break;
                case Type.blank:
                    s += mr.src + ": Can move in any direction by one" + "\n";
                    break;
                case Type.teleport:
                    s += mr.src + ": Can teleport to any " + mr.target.ToString().ToLower() + " tile on grid" + "\n";
                    break;
                case Type.warm:
                    s += mr.src + ": Can move to any warm colour" + "\n";
                    break;
                case Type.cool:
                    s += mr.src + ": Can move to any cool colour" + "\n";
                    break;
                case Type.jump1:
                    s += mr.src + ": Can jump over one tile in any direction" + "\n";
                    break;
                case Type.jump2:
                    s += mr.src + ": Can jump over two tiles in any direction" + "\n";
                    break;
            }
            switch(mr.src)
            {
                case Colour.Red:
                    red = s;
                    break;
                case Colour.Orange:
                    orange = s;
                    break;
                case Colour.Yellow:
                    yellow = s;
                    break;
                case Colour.Green:
                    green = s;
                    break;
                case Colour.Blue:
                    blue = s;
                    break;
                case Colour.Purple:
                    purple = s;
                    break;
                case Colour.Pink:
                    pink = s;
                    break;
                case Colour.Teal:
                    teal = s;
                    break;
            }
            
        }

        foreach (ColourRule cr in cRules)
        {
            string s = "";
            switch (cr.type)
            {
                case Type.include:
                    s += cr.src + ": Can only move onto " + cr.target.ToString().ToLower() + "\n";
                    break;
                case Type.exclude:
                    s += cr.src + ": Can move to any colour except " + cr.target.ToString().ToLower() + "\n";
                    break;
            }
            switch (cr.src)
            {
                case Colour.Red:
                    red = s;
                    break;
                case Colour.Orange:
                    orange = s;
                    break;
                case Colour.Yellow:
                    yellow = s;
                    break;
                case Colour.Green:
                    green = s;
                    break;
                case Colour.Blue:
                    blue = s;
                    break;
                case Colour.Purple:
                    purple = s;
                    break;
                case Colour.Pink:
                    pink = s;
                    break;
                case Colour.Teal:
                    teal = s;
                    break;
            }

        }
        ruleText.text = instructions + pink + red + orange + yellow + green + teal + blue + purple;
    }
}
