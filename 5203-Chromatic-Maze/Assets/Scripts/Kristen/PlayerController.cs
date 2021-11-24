using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{

    public KruskalMaze.Maze maze;
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
    }

    private void tappedOn(Collider2D tap)
    {
        Tile tapped = tap.gameObject.GetComponent<Tile>();
        bool oneJump = false;
        bool twoJump = false;

        if(player.ruleType == 4)
        {
            oneJump = true;
        }
        else if (player.ruleType == 5)
        {
            twoJump = true;
        }

        //Back Tracking
        if (previous.Count > 0 && previous.Peek() == tapped) //they want to back track
        {
            if(bCount > 0)
            {
                bCount--;
                bText.text = bCount.ToString();

                if(player.tag == "checker") //remove checker if applicable
                {
                    cCount--;
                    cText.text = cCount.ToString() + "/" + Shinro.checkerCount.ToString();
                }
                previous.Pop();
                //update current position
                player.transform.Find("border").gameObject.GetComponent<SpriteRenderer>().enabled = false;
                player = tapped;
                player.transform.Find("border").gameObject.GetComponent<SpriteRenderer>().enabled = true; //comment out this line

                //update step count
                sCount++;
                sText.text = sCount.ToString();
            }
        }

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

        //check rule type, direction, colour variables etc. of player tile and compare tapped on tile to rule conditions
        //MovementRules mr = player.mRule;
        //ColorRules cr = player.cRule;
        //if (mr != null) //a movement rule
        //{
        //    if (mr.type == 1) //Tmove
        //    {
        //        int first = mr.direction / 1000;
        //        int second = mr.direction  % 1000 / 100;
        //        int third = mr.direction % 100 / 10;
        //        int fourth = mr.direction % 10;

        //        if(first == 0) //Can't move North
        //        {
        //            //put long if statement here
        //            Move(tapped);
        //        }
        //        else if(second == 0) //Can't move South
        //        {
        //            //put long if statement here
        //            Move(tapped);
        //        }
        //        else if (third == 0) //Can't move East
        //        {
        //            //put long if statement here
        //            Move(tapped);
        //        }
        //        else if (fourth == 0)//Can't move West
        //        {
        //            //put long if statement here
        //            Move(tapped);
        //        }

        //        //Other option
        //        int index;
        //        foreach(int i in mr.direction)
        //        {
        //            if(i == 0)
        //            {
        //                index = mr.direction.IndexOf(i);
        //                break;
        //            }

        //        }

        //        switch (index)
        //        {
        //            case 0: //Can't move North
        //                //put long if statement here
        //                Move(tapped);
        //                break;
        //            case 1: //Can't move South
        //                //put long if statement here
        //                Move(tapped);
        //                break;
        //            case 2: //Can't move East
        //                //put long if statement here
        //                Move(tapped);
        //                break;
        //            case 3: //Can't move West
        //                //put long if statement here
        //                Move(tapped);
        //                break;
        //        }

        //    }
        //    if(mr.type == 2) //blank
        //    {
        //        //same as above if statement
        //    }

        //}
        //I may want to update the parent/child stuff so it keeps track of whic tile is directly N S E or W of a given tile (or I just get them from their names in the scene)
    }

    //private void Move(Tile tapped)
    //{
    //    //check if checker is on tile
    //    if (tapped.tag == "checker")
    //    {
    //        cCount++;
    //        cText.text = cCount.ToString() + "/" + Shinro.checkerCount.ToString();
    //    }

    //    previous.Push(player);

    //    //update current position
    //    player.transform.Find("border").gameObject.GetComponent<SpriteRenderer>().enabled = false; //comment out this line
    //    player = tapped;
    //    player.transform.Find("border").gameObject.GetComponent<SpriteRenderer>().enabled = true;

    //    //update step count
    //    sCount--;
    //    sText.text = sCount.ToString();
    //}
}
