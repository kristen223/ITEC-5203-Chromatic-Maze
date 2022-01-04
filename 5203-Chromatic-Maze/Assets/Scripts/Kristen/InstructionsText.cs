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

    static bool r;
    static bool y;
    static bool o;
    static bool pi;
    static bool b;
    static bool g;
    static bool t;
    static bool pu;

    void Awake()
    {
        instructions = "All rules imply orthogonal movement by one, unless otherwise specified." + "\n" + "\n";
        ruleText = GameObject.Find("ruleText").GetComponent<Text>();
    }

    public static void SetInstructions(List<MovementRule> mRules, List<ColourRule> cRules)
    {
        foreach (Tile tile in GenerateGrid.maze.tiles)
        {
            switch (tile.colour)
            {
                case Colour.Red:
                    r = true;
                    break;
                case Colour.Yellow:
                    y = true;
                    break;
                case Colour.Orange:
                    o = true;
                    break;
                case Colour.Green:
                    g = true;
                    break;
                case Colour.Blue:
                    b = true;
                    break;
                case Colour.Purple:
                    pu = true;
                    break;
                case Colour.Pink:
                    pi = true;
                    break;
                case Colour.Teal:
                    t = true;
                    break;

            }
        }

        foreach(MovementRule mr in mRules) //Tmove, blank, teleport, jump1, jump2, warm, cool,include, exclude
        {
            bool check = true;
            string colour = mr.src.ToString();
            if(colour == "Red")
            {
                if(!r)
                {
                    check = false;
                }
            }
            else if(colour == "Orange")
            {
                if (!o)
                {
                    check = false;
                }
            }
            else if (colour == "Yellow")
            {
                if (!y)
                {
                    check = false;
                }
            }
            else if (colour == "Green")
            {
                if (!g)
                {
                    check = false;
                }
            }
            else if (colour == "Blue")
            {
                if (!b)
                {
                    check = false;
                }
            }
            else if (colour == "Purple")
            {
                if (!pu)
                {
                    check = false;
                }
            }
            else if (colour == "Pink")
            {
                if (!pi)
                {
                    check = false;
                }
            }
            else if (colour == "Teal")
            {
                if (!t)
                {
                    check = false;
                }
            }

            if(check)
            {
                string s = "";
                switch (mr.type)
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
                switch (mr.src)
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

                ruleText.text = instructions + pink + red + orange + yellow + green + teal + blue + purple;
            }
            
        }

        foreach (ColourRule cr in cRules)
        {
            bool check = true;
            string colour = cr.src.ToString();
            if (colour == "Red")
            {
                if (!r)
                {
                    check = false;
                }
            }
            else if (colour == "Orange")
            {
                if (!o)
                {
                    check = false;
                }
            }
            else if (colour == "Yellow")
            {
                if (!y)
                {
                    check = false;
                }
            }
            else if (colour == "Green")
            {
                if (!g)
                {
                    check = false;
                }
            }
            else if (colour == "Blue")
            {
                if (!b)
                {
                    check = false;
                }
            }
            else if (colour == "Purple")
            {
                if (!pu)
                {
                    check = false;
                }
            }
            else if (colour == "Pink")
            {
                if (!pi)
                {
                    check = false;
                }
            }
            else if (colour == "Teal")
            {
                if (!t)
                {
                    check = false;
                }
            }

            if (check)
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

                ruleText.text = instructions + pink + red + orange + yellow + green + teal + blue + purple;
            }

        }
        
    }
}
