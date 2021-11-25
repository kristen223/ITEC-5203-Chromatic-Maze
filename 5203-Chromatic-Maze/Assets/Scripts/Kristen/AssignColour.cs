using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssignColour : MonoBehaviour
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
    public static List<MovementRules> mRules;
    public static List<ColourRules> cRules;

    //may want to split this into two lists?
    public static List<int> includeRules; //first int is index of rule in RuleSet; second int is the rule's type
    public static List<int> excludeRules;

    // Start is called before the first frame update
    void Start()
    {
        maze = GenerateGrid.maze;
        cRules = new List<ColourRules>();
        mRules = new List<MovementRules>();
    }

    //not in start because other script needs to finish first
    public static void SetRules(List<MovementRules> mr, List<ColourRules> cr)
    {
        mRules = mr;
        cRules = cr;


        //INCLUDE: once you've landed on the src colour, boolean becomes true and you can move onto target colour
        //EXCLUDE: once you've landed on the src colour, boolean becomes true and you cannot move onto the target colour

        foreach (ColourRules rule in cRules)
        {
            if(rule.type == 10) //check path include
            {
                includeRules.Add(cRules.IndexOf(rule));
            }
            else if(rule.type == 11) //check path exclude
            {
                excludeRules.Add(cRules.IndexOf(rule));
            }
        }


        for (int t = 0; t < maze.tiles.Length; t++) //change this to traverse maze in certain way
        {




            int chance = Random.Range(0, 2);

            if(chance == 0) //try a movement rule
            {

            }
            else //try a colour rule
            {

            }
        }

        //check the rule set
        //for each include and exclude rule, set a boolean that is true if the colour has been placed down
    }
}
