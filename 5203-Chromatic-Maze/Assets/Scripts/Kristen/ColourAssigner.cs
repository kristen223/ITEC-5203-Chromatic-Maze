using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColourAssigner : MonoBehaviour
{
    /*for each tile in the grid
     *      check which directions you can move (N S E W) within 1, 2 and 3 space 
     *      Based on the above, get list of possible rules to place down
     *      pick a random rule, except some rules will have a higher likelihood of being selected (generally and based on previously placed rules)
     *      if includeC rule exits, you can place down colour c2 anywhere if the rule colour has already been placed
     *      
     *      if its a dead end, either place a teleport colour or anythign else (doesn't matter)
     *          If teleport, tile doesn't count as a dead-end and won't cause a game over if no more undo's left
     *          
     *      everytime a colour is placed, check to see if any false inclusion.exclusion rule booleans need to be updated
     *      
     *      We only need to make sure they can step forward (including looped areas) not backwards
     */


    /* We could traverse along solution path first, placing colours, then go from each dead-end to the solution path, then from each dead-end-turned-cycle to the solution path
     * Or could traverse from root node to every child (until child rank == 0) < extra condition needed because of cycles
     */

    //**When you place a colour, set the rule type, moveRule bool, colour int, and rule of the tile
    // When placing jump colour, check if that tile's jump bools are true
    //blank spaces can only be placed at cross sections

    private static KruskalMaze.Maze maze;
    public static List<MovementRule> mRules;
    public static List<ColourRule> cRules;

    public static List<int> includeRules; //list of indexes of CheckPathInc rules of cRules list
    public static List<int> excludeRules; //list of indexes of CheckPathExc rules of cRules list

    //private static Material Red;
    //private static Material Orange;
    //private static Material Yellow;
    //private static Material Green;
    //private static Material Blue;
    //private static Material Purple;

    private Object[] materials;
    private List<Material> colours;

    // Start is called before the first frame update
    void Start()
    {        
        maze = GenerateGrid.maze;
        cRules = new List<ColourRule>();
        mRules = new List<MovementRule>();

        materials = new Object[6];
        materials = Resources.LoadAll("Materials", typeof(Material));
        //colours = new List<Material>(materials);

        //Red = (Material)Resources.Load("Materials/Red");
        //Orange = (Material)Resources.Load("Materials/Orange");
        //Yellow = (Material)Resources.Load("Materials/Yellow");
        //Green = (Material)Resources.Load("Materials/Green");
        //Blue = (Material)Resources.Load("Materials/Blue");
        //Purple = (Material)Resources.Load("Materials/Purple");
    }

    //not in start because other script needs to finish first
    public static void SetRules(List<MovementRule> mr, List<ColourRule> cr)
    {
        mRules = mr;
        cRules = cr;

        //Get list of CheckPath rule indexes
        foreach (ColourRule rule in cRules)
        {
            if(rule.type == Type.checkPathInc) //check path include
            {
                includeRules.Add(cRules.IndexOf(rule));
            }
            else if(rule.type == Type.checkPathExc) //check path exclude
            {
                excludeRules.Add(cRules.IndexOf(rule));
            }
        }

        AssignRulestoColours(mRules, cRules); //Every rule now has an assigned colour (src)
    }

    /* Priority of rule assignments:
     * 1. include and exclude
     * 2. warm and cool
     * 3. rest
     * This is written under the assumption one colour can have multiple rules
     */
    private static void AssignRulestoColours(List<MovementRule> mList, List<ColourRule> cList)
    {
        List<Colour> warmColours = new List<Colour>();
        List<Colour> coolColours = new List<Colour>();
        warmColours.Add(Colour.Red);
        warmColours.Add(Colour.Orange);
        warmColours.Add(Colour.Yellow);
        coolColours.Add(Colour.Green);
        coolColours.Add(Colour.Blue);
        coolColours.Add(Colour.Purple);

        foreach (ColourRule c in cList)
        {
            if (c.type == Type.include || c.type == Type.exclude)
            {
                if (warmColours.Contains(c.src))
                {
                    warmColours.Remove(c.src); //does this work?
                }
                else
                {
                    coolColours.Remove(c.src);
                }
            }
        }
        for (int i = 0; i < cList.Count; i++)
        {
            ColourRule c = cList[i];

            if (c.type == Type.warm) //assign the source colour to a warm colour still in the list (if any)
            {
                if (warmColours.Count > 0)
                {
                    c.src = warmColours[0];
                    warmColours.RemoveAt(0);
                    cList[i] = c;
                }
                else
                {
                    List<Colour> extra = new List<Colour>();
                    extra.Add(Colour.Red);
                    extra.Add(Colour.Orange);
                    extra.Add(Colour.Yellow);
                    c.src = extra[Random.Range(0, 3)]; //asign random warm colour
                    cList[i] = c;
                }
            }
            if (c.type == Type.cool) //assign the source colour to a warm colour still in the list (if any)
            {
                if (coolColours.Count > 0)
                {
                    c.src = coolColours[0];
                    coolColours.RemoveAt(0);
                    cList[i] = c;
                }
                else
                {
                    List<Colour> extra = new List<Colour>();
                    extra.Add(Colour.Green);
                    extra.Add(Colour.Blue);
                    extra.Add(Colour.Purple);
                    c.src = extra[Random.Range(0, 3)]; //asign random warm colour
                    cList[i] = c;
                }
            }
        }

        for (int i = 0; i < mList.Count; i++)
        {
            MovementRule m = mList[i];
            if (coolColours.Count > 0)
            {
                m.src = coolColours[0];
                coolColours.RemoveAt(0);
                mList[i] = m;
            }
            else if (warmColours.Count > 0)
            {
                m.src = warmColours[0];
                warmColours.RemoveAt(0);
                mList[i] = m;
            }
            else //all colours used at least once
            {
                List<Colour> extra = new List<Colour>();
                extra.Add(Colour.Red);
                extra.Add(Colour.Orange);
                extra.Add(Colour.Yellow);
                extra.Add(Colour.Green);
                extra.Add(Colour.Blue);
                extra.Add(Colour.Purple);
                m.src = extra[Random.Range(0, 6)]; //assign random colour
                mList[i] = m;
            }
        }

        for (int i = 0; i < cList.Count; i++)
        {
            ColourRule c = cList[i];
            if (c.type != Type.include && c.type != Type.exclude && c.type != Type.cool && c.type != Type.warm)
            {
                if (coolColours.Count > 0)
                {
                    c.src = coolColours[0];
                    coolColours.RemoveAt(0);
                    cList[i] = c;
                }
                else if (warmColours.Count > 0)
                {
                    c.src = warmColours[0];
                    warmColours.RemoveAt(0);
                    cList[i] = c;
                }
                else //all colours used at least once
                {
                    List<Colour> extra = new List<Colour>();
                    extra.Add(Colour.Red);
                    extra.Add(Colour.Orange);
                    extra.Add(Colour.Yellow);
                    extra.Add(Colour.Green);
                    extra.Add(Colour.Blue);
                    extra.Add(Colour.Purple);
                    c.src = extra[Random.Range(0, 6)];
                    cList[i] = c;
                }
            }
        }
    }

    private void AssignColourM(Tile t, MovementRule rule)
    {
        t.mRule = rule;
        t.ruleType = rule.type;
        t.moveRule = true;
        t.colour = rule.src;

        SpriteRenderer sr = t.GetComponent<SpriteRenderer>();

        foreach(Material mat in colours)
        {
            if(mat.name == t.colour.ToString())
            {
                sr.material.shader = mat.shader;
                sr.material.color = mat.color;
                break;
            }
        }
    }

    private void AssignColourC(Tile t, ColourRule rule)
    {
        t.cRule = rule;
        t.ruleType = rule.type;
        t.moveRule = true;
        t.colour = rule.src;

        SpriteRenderer sr = t.GetComponent<SpriteRenderer>();

        foreach (Material mat in colours)
        {
            if (mat.name == t.colour.ToString())
            {
                sr.material.shader = mat.shader;
                sr.material.color = mat.color;
                break;
            }
        }
    }

    /* Round 1
     *Place all Tmove and Blank rules
     *
     */
    private static void RoundOne()
    {
        foreach(Tile t in maze.tiles)
        {
            if(t.children.Count + 1 == 4) //Cross section piece
            {

            }
            else if (t.children.Count + 1 == 3) //T piece
            {

            }
        }
    }
}
