using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// This maze implementation is based on this Got Hub Project https://github.com/willy1989/kruskal
//The original doesn't take weight into account, have tiles with parents and ranks, and the origin and destination vertices for edges weren't set up properly
//I mainly borrowed their grid generation method, but had to update all teh kruskal-related stuff so a minimum spanning tree was outputed

public class GenerateGrid : MonoBehaviour
{
    GameObject map;
    public GameObject wallPrefab;
    public GameObject leftWallPrefab;
    public GameObject topWallPrefab;
    public GameObject tilePrefab;
    public int width;
    public static int wdth;
    public int height;
    public static int hght;
    public int cycles;

    [HideInInspector] public List<Wall> edges;
    [HideInInspector] public int EdgeIndex = 0;
    [HideInInspector] public static GameObject[] tiles;
    [HideInInspector] public static Tile[] vertices;

    private GameObject BorderFolder;
    private GameObject EdgeFolder;
    private GameObject TileFolder;

    [HideInInspector] public static KruskalMaze.Maze maze;

    public void Awake()
    {
        width = setUpScene.width;
        height = setUpScene.height;
        hght = setUpScene.height;
        wdth = setUpScene.width;
        //To organize the Unity scene
        BorderFolder = GameObject.Find("Border");
        EdgeFolder = GameObject.Find("Edges");
        TileFolder = GameObject.Find("Tiles");
        map = GameObject.Find("map");

        //Create grid of tiles (vertices)
        tiles = new GameObject[width * height];

        //This creates all of the coloured tiles
        int counter = 0;
        for (int y = 1; y <= height; y++)
        {
            for (int x = 1; x <= width; x++)
            {

                GameObject tile = Instantiate(tilePrefab, new Vector3((x * 20) - 20f, (y * 20) - 10f, 0), Quaternion.Euler(0, 0, 0));
                tile.name = "Tile-" + (counter + 1);
                //tile.GetComponentInChildren<Text>().text = "0";
                tile.GetComponent<Tile>().parent = tile.GetComponent<Tile>();
                

                if (y == 1 || y == height || x == 1 || x == width)
                {
                    tile.GetComponent<Tile>().border = true;
                }
                else
                {
                    tile.GetComponent<Tile>().border = false;
                }

                //Setting Jump Bools
                if(y <= height -2)
                {
                    tile.GetComponent<Tile>().jumpN = true;
                }
                if (y >= 3)
                {
                    tile.GetComponent<Tile>().jumpS = true;
                }
                if (x <= width - 2)
                {
                    tile.GetComponent<Tile>().jumpE = true;
                }
                if (x >= 3)
                {
                    tile.GetComponent<Tile>().jumpW = true;
                }
                if (y <= height - 3)
                {
                    tile.GetComponent<Tile>().jumpTwoN = true;
                }
                if (y >= 4)
                {
                    tile.GetComponent<Tile>().jumpTwoS = true;
                }
                if (x <= width - 3)
                {
                    tile.GetComponent<Tile>().jumpTwoE = true;
                }
                if (x >= 4)
                {
                    tile.GetComponent<Tile>().jumpTwoW = true;
                }

                tiles[counter] = tile;

                tile.transform.SetParent(TileFolder.transform);

                counter++;
            }
        }

        edges = new List<Wall>();

        spawnLeftRightBoundaries();
        spawnUpDownBoundaries();
        spawnInnerEdgesLeftRight();
        spawnInnerEdgesUpDown();


        vertices = new Tile[tiles.Length];
        for(int i = 0; i < tiles.Length; i++)
        {
            vertices[i] = tiles[i].GetComponent<Tile>();
        }

        maze = GetComponent<KruskalMaze>().CreateMaze(KruskalMaze.CreateGraph(edges.ToArray(), vertices.Length), vertices, cycles);
        ResizeGrid(wdth, hght);
    }

    //OUTER BORDER (not part of tree)
    int sideCount = 1;
    bool left = true;
    public void spawnLeftRightBoundaries()
    {
        for (int y = 1; y <= height; y++)
        {
            for (int x = 0; x <= width; x++)
            {
                if (x == 0 || x == width)
                {
                    GameObject w;
                    if (left == true)
                    {
                        w = Instantiate(leftWallPrefab, new Vector3((x * 20) - 10f, (y * 20) - 10f, 0), Quaternion.Euler(0, 0, 0));
                        w.name = "LeftWall-" + sideCount.ToString();
                        left = false;
                        sideCount++;
                    }
                    else
                    {
                        w = Instantiate(wallPrefab, new Vector3((x * 20) - 10f, (y * 20) - 10f, 0), Quaternion.Euler(0, 0, 0));
                        w.name = "RightWall";
                        left = true;
                    }
                    Destroy(w.GetComponent<Wall>());
                    w.transform.SetParent(BorderFolder.transform);
                }

            }
        }

    }
    int topCount = 1;
    public void spawnUpDownBoundaries()
    {
        for (int y = 0; y <= height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (y == 0 || y == height)
                {
                    GameObject w;
                    if (topCount > width)
                    {
                        w = Instantiate(topWallPrefab, new Vector3((x * 20), (y * 20), 0), Quaternion.Euler(0, 0, 90));
                        w.name = "TopWall-" + (topCount - width);
                    }
                    else
                    {
                        w = Instantiate(wallPrefab, new Vector3((x * 20), (y * 20), 0), Quaternion.Euler(0, 0, 90));
                        w.name = "BottomWall";
                    }
                    Destroy(w.GetComponent<Wall>());
                    w.transform.SetParent(BorderFolder.transform);
                    topCount++;
                }

            }
        }

    }
    int count = 1;
    // GENERATING TREE EDGES (grid lines)
    public void spawnInnerEdgesLeftRight()
    {
        EdgeIndex = 0;
        
        for (int y = 1; y <= height; y++)
        {
            for (int x = 1; x < width; x++)
            {
                GameObject wall = Instantiate(wallPrefab, new Vector3((x * 20) -10f, (y * 20) -10f, 0), Quaternion.Euler(0, 0, 0));
                wall.name = "Edge-" + count;
                wall.GetComponent<Wall>().connectedTiles = new Tile[2];
                wall.GetComponent<Wall>().connectedTiles[0] = tiles[EdgeIndex].GetComponent<Tile>();
                wall.GetComponent<Wall>().connectedTiles[1] = tiles[EdgeIndex + 1].GetComponent<Tile>();
                wall.GetComponent<Wall>().origin = tiles[EdgeIndex].GetComponent<Tile>();
                wall.GetComponent<Wall>().destination = tiles[EdgeIndex + 1].GetComponent<Tile>();
                wall.GetComponent<Wall>().weight = Random.Range(0, 100);
                wall.transform.SetParent(EdgeFolder.transform);
                wall.GetComponent<SpriteRenderer>().enabled = false;

                edges.Add(wall.GetComponent<Wall>());

               EdgeIndex++;
               count++;
            }
            EdgeIndex++;
        }
    }

    public void spawnInnerEdgesUpDown()
    {
        EdgeIndex = 0;

        for (int y = 1; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                GameObject wall = Instantiate(wallPrefab, new Vector3((x * 20),(y * 20), 0), Quaternion.Euler(0, 0, 90));
                wall.name = "Edge-" + count;
                wall.GetComponent<Wall>().connectedTiles = new Tile[2];
                wall.GetComponent<Wall>().connectedTiles[0] = tiles[EdgeIndex].GetComponent<Tile>();
                wall.GetComponent<Wall>().connectedTiles[1] = tiles[EdgeIndex + width].GetComponent<Tile>(); //no?
                wall.GetComponent<Wall>().origin = tiles[EdgeIndex].GetComponent<Tile>();
                wall.GetComponent<Wall>().destination = tiles[EdgeIndex + width].GetComponent<Tile>(); //changed
                wall.GetComponent<Wall>().weight = Random.Range(0, 100);
                wall.transform.SetParent(EdgeFolder.transform);
                wall.GetComponent<SpriteRenderer>().enabled = false;

                edges.Add(wall.GetComponent<Wall>());

                EdgeIndex++;
                count++;
            }

        }
    }

    void ResizeGrid(int w, int h)
    {
        if(w == 10 || h == 10)
        {
            return;

        }
        else if(w == 9 || h == 9)
        {
            map.transform.position = new Vector3(-4.5f, map.transform.position.y, map.transform.position.z);
            map.transform.localScale = new Vector3(1.0752f, 1.0752f, 1.0752f);
        }
        else if (w == 8 || h == 8)
        {
            map.transform.position = new Vector3(-1.3f, map.transform.position.y, map.transform.position.z);
            map.transform.localScale = new Vector3(1.143f, 1.143f, 1.143f);
        }
        else if (w == 7 || h == 7)
        {
            map.transform.position = new Vector3(3.2f, map.transform.position.y, map.transform.position.z);
            map.transform.localScale = new Vector3(1.346797f, 1.346797f, 1.346797f);
        }
        else if (w == 6 || h == 6)
        {
            map.transform.position = new Vector3(7.5f, map.transform.position.y, map.transform.position.z);
            map.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
        }
        else if (w == 5 || h == 5)
        {
            map.transform.position = new Vector3(17f, map.transform.position.y, map.transform.position.z);
            map.transform.localScale = new Vector3(1.78f, 1.78f, 1.78f);

        }
        else if (w == 4 || h == 4)
        {
            map.transform.position = new Vector3(16.8f, map.transform.position.y, map.transform.position.z);
            map.transform.localScale = new Vector3(1.8f, 1.8f, 1.8f);
        }
        else if (w == 3 || h == 3)
        {
            map.transform.position = new Vector3(22.1f, 31.5f, map.transform.position.z);
            map.transform.localScale = new Vector3(2f, 2f, 2f);
        }
        else //2 X 2
        {
            map.transform.position = new Vector3(53.6f, 53.6f, map.transform.position.z);
            map.transform.localScale = new Vector3(1.9683f, 1.9683f, 1.9683f);
        }


    }
}