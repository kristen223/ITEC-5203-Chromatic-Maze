using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{

    public KruskalMaze.Maze maze;
    private Tile[] tiles;
    private Tile player;
    private Stack<Tile> previous;
    private List<Tile> solutionP;

    //game object to represent player (liek the checker) (you can remove the checker really)
    //maybe highlioght the tile somehow instead? (give it an outline, illusion like it's bumped outward? some sort of gradient from centre?)
    private int cCount;
    private int sCount;
    private int bCount;

    private Text cText;
    private Text sText;
    private Text bText;

    private Text message;
    //private GameObject rButton;
    private GameObject gameOver;

    void Start()
    {
        
        maze = GenerateGrid.maze;
        solutionP = new List<Tile>(maze.LP.path);
        player = maze.LP.entrance;
        player.transform.Find("border").gameObject.GetComponent<SpriteRenderer>().enabled = true;

        cText = GameObject.Find("CCount").GetComponent<Text>();
        cCount = 0;
        cText.text = cCount.ToString() + "/" + Shinro.checkerCount.ToString();

        sText = GameObject.Find("SCount").GetComponent<Text>();
        sCount = maze.LP.length - 1; //no extra steps
        sText.text = sCount.ToString();

        bText = GameObject.Find("BCount").GetComponent<Text>();
        bCount = 5;
        bText.text = bCount.ToString();

        gameOver = GameObject.Find("GameOver"); 
        message = GameObject.Find("AlertMessage").GetComponent<Text>();
        //rButton = GameObject.Find("Button");
        gameOver.SetActive(false);

        previous = new Stack<Tile>();
        tiles = GenerateGrid.vertices;
    }

    void Update()
    {
        //Won
        if (player == maze.LP.exit)
        {
            gameOver.SetActive(true);
            message.text = "Winner!";
        }

        //Game Over - reached dead end, no undo's left, and is not a teleport tile
        //**Add condition: if player.ruleType != 3 (is not teleport rule)
        if (bCount == 0 && maze.deadends.Contains(player))
        {
            gameOver.SetActive(true);
            message.text = "Out of Moves - Game Over";
        }

        //Game Over - Out of Moves and no undo's
        if(sCount == 0 && bCount == 0 && player != maze.LP.exit)
        {
            gameOver.SetActive(true);
            message.text = "Out of Moves - Game Over";
        }

        //Desktop Input
        if (Input.GetMouseButtonDown(0)){
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

            if (hit.collider != null){
                Collider2D tap = hit.collider;
                tappedOn(tap);
            }
        }

        //Mobile Input
        if ((Input.touchCount > 0) && (Input.GetTouch(0).phase == TouchPhase.Began)){
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position), Vector2.zero);

            if (hit.collider != null)
            {
                Collider2D tap = hit.collider;
                tappedOn(tap);
            }
        }
    } //checkPath

    private void tappedOn(Collider2D tap)
    {
        Tile tapped = tap.gameObject.GetComponent<Tile>();

        //Back Tracking
        if (previous.Count > 0 && previous.Peek() == tapped) //they want to back track
        {
            if(bCount > 0)
            {
                bCount--;
                bText.text = bCount.ToString();

                if (player.tag == "checker") //remove checker if applicable
                {
                    cCount--;
                    cText.text = cCount.ToString() + "/" + Shinro.checkerCount.ToString();
                }
                previous.Pop();
                //go through previous list and check if include/exclude bools still apply


                //update current position
                player.transform.Find("border").gameObject.GetComponent<SpriteRenderer>().enabled = false;
                player = tapped;
                player.transform.Find("border").gameObject.GetComponent<SpriteRenderer>().enabled = true; //comment out this line

                //update step count
                sCount++;
                sText.text = sCount.ToString();
            }
        }

        //Delete later
        //if they didn't tap on player, and this is their first move or they're not backtracking, and they tapped on an adjacent tile (must update with jump moves)
        if (sCount > 0 && tapped != player && (previous.Count == 0 || previous.Peek() != tapped) && (tapped == player.parent || player.children.Contains(tapped))) //Can move here
        {
            //check if checker is on tile
            if (tapped.tag == "checker")
            {
                cCount++;
                cText.text = cCount.ToString() + "/" + Shinro.checkerCount.ToString();
            }

            previous.Push(player);
            
            //update current position
            player.transform.Find("border").gameObject.GetComponent<SpriteRenderer>().enabled = false; //comment out this line
            player = tapped;
            player.transform.Find("border").gameObject.GetComponent<SpriteRenderer>().enabled = true;

            //update step count
            sCount--;
            sText.text = sCount.ToString();
        }



        //Start here
        if (sCount > 0 && tapped != player && (previous.Count == 0 || previous.Peek() != tapped)) //check that player can move and is not backtracking
        {
            MovementRules mr = player.mRule;
            colourRules cr = player.cRule;
            if (tapped.moveRule == true) //player is on a movement rule
            {
                int pindex = Array.IndexOf(tiles, player); //index of player
                int tindex = Array.IndexOf(tiles, tapped);
                switch (mr.type)
                {
                    case 0: //TMOVE
                            //Getting which direction they can't move
                        int first = mr.direction / 1000;
                        int second = mr.direction % 1000 / 100;
                        int third = mr.direction % 100 / 10;
                        int fourth = mr.direction % 10;
                        if (first == 0) //Can't move North
                        {
                            int n = pindex + maze.w;
                            if (tindex != n && (tapped == player.parent || player.children.Contains(tapped))) //Can move here
                            {
                                Move(tapped);
                            }
                        }
                        else if (second == 0) //Can't move South
                        {
                            int s = pindex - maze.w;
                            if (tindex != s && (tapped == player.parent || player.children.Contains(tapped)))
                            {
                                Move(tapped);
                            }
                        }
                        else if (third == 0) //Can't move East
                        {
                            int e = pindex + 1;
                            if (e + 1 % maze.w == 0) //east tile does not exist so can move ot any parent/child
                            {
                                if (tapped == player.parent || player.children.Contains(tapped))
                                {
                                    Move(tapped);
                                }
                            }
                            else if (tindex != e && (tapped == player.parent || player.children.Contains(tapped)))
                            {
                                Move(tapped);
                            }
                        }
                        else if (fourth == 0)//Can't move West
                        {
                            int w = pindex - 1;
                            if (w % maze.w == 0) //west tile does not exist so can move ot any parent/child
                            {
                                if (tapped == player.parent || player.children.Contains(tapped))
                                {
                                    Move(tapped);
                                }
                            }
                            else if (tindex != w && (tapped == player.parent || player.children.Contains(tapped)))
                            {
                                Move(tapped);
                            }
                        }
                        break;

                    case 1: //BLANK
                        if (tapped == player.parent || player.children.Contains(tapped)) //can move to any parent/child
                        {
                            Move(tapped);
                        }
                        break;

                    case 2: //TELEPORT
                        if (cr.inclusion == true) //can teleport there
                        {
                            Move(tapped);
                        }
                        break;

                    case 3: //JUMP 1
                        if (tindex == pindex + 2*maze.w) //if tapped on tile is two spaces North
                        {
                            if (player.jumpN == true) //if tile exists
                            {
                                Move(tapped);
                                break;
                            }
                        }
                        else if (tindex == pindex - 2 * maze.w) //if tapped on tile is two spaces South
                        {
                            if (player.jumpS == true) //if tile exists
                            {
                                Move(tapped);
                                break;
                            }
                        }
                        else if (tindex == pindex + 2) //if tapped on tile is two spaces East
                        {
                            if (player.jumpE == true) //if tile exists
                            {
                                Move(tapped);
                                break;
                            }
                        }
                        else if (tindex == pindex - 2) //if tapped on tile is two spaces West
                        {
                            if (player.jumpW == true) //if tile exists
                            {
                                Move(tapped);
                                break;
                            }
                        }
                        break;

                    case 4: //JUMP 2
                        if (tindex == pindex + 3 * maze.w) //if tapped on tile is three spaces North
                        {
                            if (player.jumpTwoN == true) //if tile exists
                            {
                                Move(tapped);
                                break;
                            }
                        }
                        else if (tindex == pindex - 3 * maze.w) //if tapped on tile is three spaces South
                        {
                            if (player.jumpTwoS == true) //if tile exists
                            {
                                Move(tapped);
                                break;
                            }
                        }
                        else if (tindex == pindex + 3) //if tapped on tile is three spaces East
                        {
                            if (player.jumpTwoE == true) //if tile exists
                            {
                                Move(tapped);
                                break;
                            }
                        }
                        else if (tindex == pindex - 3) //if tapped on tile is three spaces West
                        {
                            if (player.jumpTwoW == true) //if tile exists
                            {
                                Move(tapped);
                                break;
                            }
                        }
                        break;
                         
                    case 5: //WARM
                        if ((tapped == player.parent || player.children.Contains(tapped)) && (tapped.colour == 1 || tapped.colour == 2 || tapped.colour == 3)) //adjacent warm colour
                        {
                            Move(tapped);
                        }
                        break;

                    case 6: //COOL
                        if ((tapped == player.parent || player.children.Contains(tapped)) && (tapped.colour == 4 || tapped.colour == 5 || tapped.colour == 6)) //adjacent cool colour
                        {
                            Move(tapped);
                        }
                        break;
                }
            }
            else //player is on a colour rule
            {
                switch (cr.type)
                {
                    case 7: //INCLUDE
                        //if ((tapped == player.parent || player.children.Contains(tapped)) && (tapped.colour == player.cRule.target)) //can only move to adjecent tile that's target colour of rule
                        //{
                        //    Move(tapped);
                        //}
                        break;

                    case 8: //EXCLUDE
                        //if ((tapped == player.parent || player.children.Contains(tapped)) && (tapped.colour != player.cRule.src)) //move to any adjecent colour except rule source colour
                        //{
                        //    Move(tapped);
                        //}
                        break;

                    case 9: //BLOCK

                        break;
                }
                //I may want to update the parent/child stuff so it keeps track of whic tile is directly N S E or W of a given tile (or I just get them from their names in the scene)
            }
        }

    }

    private void Move(Tile tapped)
    {
        //Check inclusion bools and update
        foreach (KeyValuePair<int, int> kv in AssignColour.includeRules) //for every include rule
        {
            if (tapped.colour == AssignColour.cRules[kv.Key].src)
            {
                AssignColour.cRules[kv.Key].inclusion = true;
            }
            if (tapped.colour == AssignColour.cRules[kv.Key].target && AssignColour.cRules[kv.Key].inclusion == false) //if source has not been visited yet
            {
                return; //can't move onto this tile at the moment
            }
        }

        //Check exclusion bools and update
        foreach (KeyValuePair<int, int> kv in AssignColour.excludeRules)
        {
            if (tapped.colour == AssignColour.cRules[kv.Key].src)
            {
                AssignColour.cRules[kv.Key].inclusion = true;
            }
            if (tapped.colour == AssignColour.cRules[kv.Key].target && AssignColour.cRules[kv.Key].inclusion == true)
            {
                return; //can't move onto this because source colour was visited
            }
        }

        //check if checker is on tile
        if (tapped.tag == "checker")
        {
            cCount++;
            cText.text = cCount.ToString() + "/" + Shinro.checkerCount.ToString();
        }

        previous.Push(player);

        //update current position
        player.transform.Find("border").gameObject.GetComponent<SpriteRenderer>().enabled = false; //comment out this line
        player = tapped;
        player.transform.Find("border").gameObject.GetComponent<SpriteRenderer>().enabled = true;

        //update step count
        sCount--;
        sText.text = sCount.ToString();
    }
}
