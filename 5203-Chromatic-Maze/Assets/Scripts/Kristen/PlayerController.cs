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
    public Stack<Tile> previous;
    private List<Tile> solutionP;

    public static int checkerCount; //SET THIS ONCE YOU PICK A MAZE

    //game object to represent player (liek the checker) (you can remove the checker really)
    //maybe highlioght the tile somehow instead? (give it an outline, illusion like it's bumped outward? some sort of gradient from centre?)
    private int cCount;
    private int sCount;
    private int bCount;

    private Text cText;
    private Text sText;
    private Text bText;
    private Text message;

    private Text endMessage;
    //private GameObject rButton;
    private GameObject gameOver;

    void Start()
    {
        checkerCount = 0;
        maze = GenerateGrid.maze;
        solutionP = new List<Tile>(maze.LP.path);
        player = maze.LP.entrance;
        player.transform.Find("border").gameObject.GetComponent<SpriteRenderer>().enabled = true;

        cText = GameObject.Find("CCount").GetComponent<Text>();
        cCount = 0;

        sText = GameObject.Find("SCount").GetComponent<Text>();
        sCount = maze.LP.length - 1; //no extra steps
        sText.text = sCount.ToString();

        bText = GameObject.Find("BCount").GetComponent<Text>();

        //Can't accurately set this until the colour assigner is properly set up
        if(sCount > 25)
        {
            bCount = (int)Math.Ceiling(sCount * .2f);
        }
        else if(sCount > 15)
        {
            bCount = (int)Math.Ceiling(sCount * .3f);
        }
        else
        {
            bCount = (int)Math.Ceiling(sCount * .4f);
        }

        bText.text = bCount.ToString();

        gameOver = GameObject.Find("GameOver");
        endMessage = GameObject.Find("AlertMessage").GetComponent<Text>();
        message = GameObject.Find("Alert").GetComponent<Text>();
        //rButton = GameObject.Find("Button");
        gameOver.SetActive(false);

        previous = new Stack<Tile>();
        tiles = GenerateGrid.vertices;
    }

    private void GameOver()
    {
        gameOver.SetActive(true);
        endMessage.text = "Out of Moves - Game Over";
    }

    void Update()
    {
        //Won
        if (player == maze.LP.exit)
        {
            gameOver.SetActive(true);
            endMessage.text = "Winner!";
        }

        if(bCount==0) //assuming teleport target colour exists
        {
            switch (player.ruleType)
            {
                case Type.jump1:
                    if(player.jumpN == true) //using two if statements because the tile doesn't exist unless this is true
                    {
                        if(maze.tiles[Array.IndexOf(maze.tiles, player) + maze.w * 2].assigned == true){
                            break; //possible to move
                        }
                    }
                    if (player.jumpS == true){
                        if (maze.tiles[Array.IndexOf(maze.tiles, player) - maze.w * 2].assigned == true){
                            break; //possible to move
                        }
                    }
                    if (player.jumpE == true){
                        if (maze.tiles[Array.IndexOf(maze.tiles, player) + 2].assigned == true){
                            break; //possible to move
                        }
                    }
                    if (player.jumpW == true){
                        if (maze.tiles[Array.IndexOf(maze.tiles, player) - 2].assigned == true){
                            break; //possible to move
                        }
                    }
                    GameOver(); //can't jump anywhere
                    break;

                case Type.jump2:
                    if (player.jumpTwoN == true){
                        if (maze.tiles[Array.IndexOf(maze.tiles, player) + maze.w * 3].assigned == true){
                            break; //possible to move
                        }
                    }
                    if (player.jumpTwoS == true){
                        if (maze.tiles[Array.IndexOf(maze.tiles, player) - maze.w * 3].assigned == true){
                            break; //possible to move
                        }
                    }
                    if (player.jumpTwoE == true){
                        if (maze.tiles[Array.IndexOf(maze.tiles, player) + 3].assigned == true){
                            break; //possible to move
                        }
                    }
                    if (player.jumpTwoW == true){
                        if (maze.tiles[Array.IndexOf(maze.tiles, player) - 3].assigned == true){
                            break; //possible to move
                        }
                    }
                    GameOver(); //can't jump anywhere
                    break;

                case Type.warm:
                    bool okayW = false;
                    if(player.parent != previous.Peek() && (player.parent.colour == Colour.Red || player.parent.colour == Colour.Orange || player.parent.colour == Colour.Yellow || player.parent.colour == Colour.Pink))
                    {
                        break;
                    }
                    foreach(Tile c in player.children)
                    {
                        if(c != previous.Peek() && (c.colour == Colour.Red || c.colour == Colour.Orange || c.colour == Colour.Yellow || c.colour == Colour.Pink))
                        {
                            okayW = true;
                            Debug.Log("child is fine");
                            break;
                        }
                    }
                    if(okayW == false)
                    {
                        Debug.Log("hmm");
                        GameOver();
                        break;
                    }
                    break;
                case Type.cool:
                    bool okayC = false;
                    if (player.parent != previous.Peek() && (player.parent.colour == Colour.Blue || player.parent.colour == Colour.Green || player.parent.colour == Colour.Purple || player.parent.colour == Colour.Teal))
                    {
                        break;
                    }
                    foreach (Tile c in player.children)
                    {
                        if (c != previous.Peek() && (c.colour == Colour.Blue || c.colour == Colour.Green || c.colour == Colour.Purple || c.colour == Colour.Teal))
                        {
                            okayC = true;
                            Debug.Log("cool child is fine");
                            break;
                        }
                        Debug.Log("cool child not fine");
                    }
                    if (okayC == false)
                    {
                        Debug.Log("huh");
                        GameOver();
                        break;
                    }
                    break;
                case Type.include:
                    Colour targ;
                    if(player.moveRule == true)
                    {
                        targ = player.mRule.target;
                    }
                    else
                    {
                        targ = player.cRule.target;
                    }
                    if (player.parent != previous.Peek() && player.parent.colour == targ)
                    {
                        break;
                    }
                    bool okayI = false;
                    foreach (Tile c in player.children)
                    {
                        if (c != previous.Peek() && c.colour == targ)
                        {
                            okayI = true;
                            break;
                        }
                    }
                    if(okayI == false)
                    {
                        GameOver();
                        break;
                    }
                    break;

                case Type.exclude:
                    Colour targEx;
                    if (player.moveRule == true)
                    {
                        targEx = player.mRule.target;
                    }
                    else
                    {
                        targEx = player.cRule.target;
                    }
                    if (player.parent != previous.Peek() && player.parent.colour != targEx && player.parent.assigned == true)
                    {
                        break;
                    }
                    bool okayE = false;
                    foreach (Tile c in player.children)
                    {
                        if (c != previous.Peek() && c.colour != targEx && c.assigned == true)
                        {
                            okayE = true;
                            break;
                        }
                    }
                    if(okayE == false)
                    {
                        GameOver();
                        break;
                    }
                    break;

                case Type.Tmove:
                    if (player.parent != previous.Peek() && player.parent.colour != Colour.Black)
                    {
                        break;
                    }
                    bool okayT = false;
                    foreach (Tile c in player.children)
                    {
                        if (c != previous.Peek() && c.colour != Colour.Black)
                        {
                            okayT = true;
                            break;
                        }
                    }
                    if(okayT == false)
                    {
                        GameOver();
                        break;
                    }
                    break;

                case Type.blank:
                    if (player.parent != previous.Peek() && player.parent.colour != Colour.Black)
                    {
                        break;
                    }
                    bool okayB = false;
                    foreach (Tile c in player.children)
                    {
                        if (c != previous.Peek() && c.colour != Colour.Black)
                        {
                            okayB = true;
                            break;
                        }
                    }
                    if (okayB == false)
                    {
                        GameOver();
                        break;
                    }
                    break;
            }
        }

        //Game Over - Out of Moves and no undo's
        if(sCount == 0 && bCount == 0 && player != maze.LP.exit)
        {
            GameOver();
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
                    player.passedChecker--;
                    if (player.passedChecker == 0)
                    {
                        player.transform.Find("Checker").gameObject.GetComponent<SpriteRenderer>().enabled = false;
                        cCount--;
                    }
                    
                    cText.text = cCount.ToString() + "/" + checkerCount.ToString();
                }
                previous.Pop(); //remove player from path

                //Go through previous list and check if include/exclude bools still apply
                //bool pExclude = false;
                //foreach (int exc in ColourAssigner.excludeRules) //Check if player was on exclude source colour
                //{
                //    if (player.colour == ColourAssigner.cRules[exc].src)
                //    {
                //        pExclude = true; //we potentially want to change this rule's bool back to false
                //        break;
                //    }
                //}

                //Colour colExc = player.colour;
                //bool excSrcExists = false;
                //if (pExclude == true) //if player was an exclude colour, check if that colour exists somewhere else on the player's path
                //{
                //    foreach (int exc in ColourAssigner.excludeRules) //Check inclusion bools and update
                //    {
                //        if (ColourAssigner.cRules[exc].src == colExc)
                //        {
                //            foreach (Tile t in previous)
                //            {
                //                if (t.colour == colExc)
                //                {
                //                    //the boolean can stay true
                //                    excSrcExists = true;
                //                    break;
                //                }
                //            }
                //        }
                //        if (excSrcExists == true)
                //        {
                //            break;
                //        }
                //    }
                //}

                //if (excSrcExists == false)
                //{
                //    foreach (int exc in ColourAssigner.excludeRules) //Check inclusion bools and update
                //    {
                //        if (ColourAssigner.cRules[exc].src == colExc)
                //        {
                //            ColourRule c = ColourAssigner.cRules[exc];
                //            c.inclusion = false;
                //            ColourAssigner.cRules[exc] = c;
                //        }
                //    }
                //}


                //bool pInclude = false;
                //foreach (int inc in ColourAssigner.includeRules) //Check if player was on exclude source colour
                //{
                //    if (player.colour == ColourAssigner.cRules[inc].src)
                //    {
                //        pInclude = true; //we potentially want to change this rule's bool back to false
                //        break;
                //    }
                //}

                //Colour colInc = player.colour;
                //bool incSrcExists = false;
                //if (pInclude == true) //if player was an exclude colour, check if that colour exists somewhere else on the player's path
                //{
                //    foreach (int inc in ColourAssigner.includeRules) //Check inclusion bools and update
                //    {
                //        if (ColourAssigner.cRules[inc].src == colInc)
                //        {
                //            foreach (Tile t in previous)
                //            {
                //                if (t.colour == colInc)
                //                {
                //                    //the boolean can stay true
                //                    incSrcExists = true;
                //                    break;
                //                }
                //            }
                //        }
                //        if (incSrcExists == true)
                //        {
                //            break;
                //        }
                //    }
                //}

                //if (incSrcExists == false)
                //{
                //    foreach (int inc in ColourAssigner.includeRules) //Check inclusion bools and update
                //    {
                //        if (ColourAssigner.cRules[inc].src == colInc)
                //        {
                //            ColourRule c = ColourAssigner.cRules[inc];
                //            c.inclusion = false;
                //            ColourAssigner.cRules[inc] = c;
                //        }
                //    }
                //}


                //update current position and path
                player.transform.Find("border").gameObject.GetComponent<SpriteRenderer>().enabled = false;
                player = tapped;
                player.transform.Find("border").gameObject.GetComponent<SpriteRenderer>().enabled = true; //comment out this line
                SpriteRenderer sr = player.gameObject.GetComponent<SpriteRenderer>();
                float r = sr.material.color.r + .3f;
                float b = sr.material.color.b + .3f;
                float g = sr.material.color.g + .3f;
                sr.material.color = new Color(r, g, b, 1);

                //update step count
                sCount++;
                sText.text = sCount.ToString();
            }
        }


        //Start here
        if (sCount > 0 && tapped != player && (previous.Count == 0 || previous.Peek() != tapped) && tapped.failedToAssign == false) //check that player can move and is not backtracking
        {
            MovementRule mr = player.mRule;
            ColourRule cr = player.cRule;
            if (player.moveRule == true) //player is on a movement rule
            {
                int pindex = Array.IndexOf(tiles, player); //index of player
                int tindex = Array.IndexOf(tiles, tapped);
                switch (mr.type)
                {
                    case Type.Tmove:
                        if (tapped == player.parent || player.children.Contains(tapped))
                        {
                            Move(tapped);
                        }
                        break;

                    case Type.blank:
                        if (tapped == player.parent || player.children.Contains(tapped)) //can move to any parent/child
                        {
                            Move(tapped);
                        }
                        break;

                    case Type.teleport:
                        if (mr.target == tapped.colour) //can teleport there
                        {
                            Move(tapped);
                        }
                        break;

                    case Type.jump1:
                        if (tindex == pindex + 2 * maze.w) //if tapped on tile is two spaces North
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

                    case Type.jump2:
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

                    case Type.warm:
                        if ((tapped == player.parent || player.children.Contains(tapped)) && (tapped.colour == Colour.Red || tapped.colour == Colour.Orange || tapped.colour == Colour.Yellow || tapped.colour == Colour.Pink)) //adjacent warm colour
                        {
                            Move(tapped);
                        }
                        break;

                    case Type.cool:
                        if ((tapped == player.parent || player.children.Contains(tapped)) && (tapped.colour == Colour.Blue || tapped.colour == Colour.Green || tapped.colour == Colour.Purple || tapped.colour == Colour.Teal)) //adjacent cool colour
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
                    case Type.include:
                        if ((tapped == player.parent || player.children.Contains(tapped)) && (tapped.colour == player.cRule.target)) //can only move to adjacent tile that's target colour
                        {
                            Move(tapped);
                        }
                        break;

                    case Type.exclude:
                        if ((tapped == player.parent || player.children.Contains(tapped)) && (tapped.colour != player.cRule.target)) //move to any adjacent tile except tile with target colour
                        {
                            Move(tapped);
                        }
                        break;
                }
            }
        }


        //Old Player Controller
        //if (sCount > 0 && tapped != player && (previous.Count == 0 || previous.Peek() != tapped) && (tapped == player.parent || player.children.Contains(tapped)) && previous.Contains(tapped) == false) //Can move here
        //{
        //    //check if checker is on tile
        //    if (tapped.tag == "checker")
        //    {
        //        if(tapped.passedChecker == 0)
        //        {
        //            cCount++;
        //            cText.text = cCount.ToString() + "/" + Shinro.checkerCount.ToString();
        //            tapped.transform.Find("Checker").gameObject.GetComponent<SpriteRenderer>().enabled = true;
        //        }
        //        tapped.passedChecker++;
        //    }

        //    previous.Push(player);

        //    //update current position
        //    SpriteRenderer sr = player.gameObject.GetComponent<SpriteRenderer>();
        //    float r = sr.material.color.r + .20f;
        //    float b = sr.material.color.b + .20f;
        //    float g = sr.material.color.g + .20f;
        //    sr.material.color = new Color(r, g, b, 1);
        //    player.transform.Find("border").gameObject.GetComponent<SpriteRenderer>().enabled = false;

        //    player = tapped; //update player tile
        //    player.transform.Find("border").gameObject.GetComponent<SpriteRenderer>().enabled = true;

        //    //update step count
        //    sCount--;
        //    sText.text = sCount.ToString();
        //}

    }

    private void Move(Tile tapped)
    {
        //Check inclusion bools and update
        //foreach (int inc in ColourAssigner.includeRules) //for every include rule
        //{
        //    if (tapped.colour == ColourAssigner.cRules[inc].src)
        //    {
        //        ColourRule c = ColourAssigner.cRules[inc];
        //        c.inclusion = true;
        //        ColourAssigner.cRules[inc] = c;
        //    }
        //    if (tapped.colour == ColourAssigner.cRules[inc].target && ColourAssigner.cRules[inc].inclusion == false) //if source has not been visited yet
        //    {
        //        message.text = "Cannot move onto " + ColourAssigner.cRules[inc].target + ", you have not passed " + ColourAssigner.cRules[inc].src + " yet!";
        //        return;
        //    }
        //}

        ////Check exclusion bools and update
        //foreach (int exc in ColourAssigner.excludeRules)
        //{
        //    if (tapped.colour == ColourAssigner.cRules[exc].src)
        //    {
        //        ColourRule c = ColourAssigner.cRules[exc];
        //        c.inclusion = true;
        //        ColourAssigner.cRules[exc] = c;
        //    }
        //    if (tapped.colour == ColourAssigner.cRules[exc].target && ColourAssigner.cRules[exc].inclusion == true)
        //    {
        //        message.text = "Cannot move onto " + ColourAssigner.cRules[exc].target.ToString().ToLower() + ", you have already passed " + ColourAssigner.cRules[exc].src.ToString().ToLower() + "!";
        //        return;
        //    }
        //}

        //check if checker is on tile
        if (tapped.tag == "checker")
        {
            cCount++;
            cText.text = cCount + "/" + checkerCount;
        }

        previous.Push(player);
        SpriteRenderer sr = player.gameObject.GetComponent<SpriteRenderer>();
        float r = sr.material.color.r - .3f;
        float b = sr.material.color.b - .3f;
        float g = sr.material.color.g - .3f;
        sr.material.color = new Color(r, g, b, 1);

        //update current position
        player.transform.Find("border").gameObject.GetComponent<SpriteRenderer>().enabled = false; //comment out this line
        player = tapped;
        player.transform.Find("border").gameObject.GetComponent<SpriteRenderer>().enabled = true;

        //update step count
        sCount--;
        sText.text = sCount.ToString();
    }
}
