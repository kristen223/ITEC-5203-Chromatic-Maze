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

    //**When you place a colour, set the rule type int of the tile
    // When placing jump colour, check if that tile's jump bools are true

    private KruskalMaze.Maze maze;
    private Tile[] tiles; //array of the tile components of the tile game objects
    private List<MovementRules> mRules;
    private List<ColourRules> cRules;

    //may want to split this into two lists?
    private List<KeyValuePair<int, int>> includes; //first in is index of rule in RuleSet; second int is target colour of rule

    // Start is called before the first frame update
    void Start()
    {
        tiles = GenerateGrid.vertices;
        maze = GenerateGrid.maze;
        cRules = new List<ColourRules>();
        mRules = new List<MovementRules>();
    }

    //not in start because other script needs to finish first
    public void SetRules(List<MovementRules> mr, List<ColourRules> cr)
    {
        mRules = mr;
        cRules = cr;

        foreach (ColourRules rule in cRules)
        {
            if(rule.type == 8 || rule.type == 9) //an include or exclude rule
            {
                includes.Add(new KeyValuePair<int, int>(cRules.IndexOf(rule), rule.target));
            }
        }


        for (int t = 0; t < tiles.Length; t++) //change this to traverse maze in certain way
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
