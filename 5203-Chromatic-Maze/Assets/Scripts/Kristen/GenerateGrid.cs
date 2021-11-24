using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// This maze implementation is based on this Got Hub Project https://github.com/willy1989/kruskal
//The original doesn't take weight into account, have tiles with parents and ranks, and the origin and destination vertices for edges weren't set up properly
//I mainly borrowed their grid generation method, but had to update all teh kruskal-related stuff so a minimum spanning tree was outputed

public class GenerateGrid : MonoBehaviour
{

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
    [HideInInspector] public GameObject[] tiles;
    [HideInInspector] public static Tile[] vertices;
    private Object[] colours;

    private GameObject BorderFolder;
    private GameObject EdgeFolder;
    private GameObject TileFolder;

    [HideInInspector] public static KruskalMaze.Maze maze;

    public void Awake()
    {
        hght = height;
        wdth = width;
        //To organize the Unity scene
        BorderFolder = GameObject.Find("Border");
        EdgeFolder = GameObject.Find("Edges");
        TileFolder = GameObject.Find("Tiles");

        //Create grid of tiles (vertices)
        tiles = new GameObject[width * height];

        //TEMPORARY - DELETE LATER
        colours = new Object[7];
        colours = Resources.LoadAll("Materials", typeof(Material));


        //This creates all of the coloured tiles
        int counter = 0;
        for (int y = 1; y <= height; y++)
        {
            for (int x = 1; x <= width; x++)
            {

                GameObject tile = Instantiate(tilePrefab, new Vector3((x * 20) - 20f, (y * 20) - 10f, 0), Quaternion.Euler(0, 0, 0));
                tile.name = "Tile-" + (counter + 1);
                tile.GetComponent<Tile>().rank = 0;
                tile.GetComponentInChildren<Text>().text = "0";
                tile.GetComponent<Tile>().parent = tile.GetComponent<Tile>();
                tile.GetComponentInChildren<Text>().text = tile.GetComponent<Tile>().rank.ToString();

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

                //TEMPORARY - DELETE LATER
                Material mat = (Material)colours[Random.Range(0, colours.Length)];
                tile.GetComponent<SpriteRenderer>().material.shader = mat.shader;
                tile.GetComponent<SpriteRenderer>().material.color = mat.color;

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

                edges.Add(wall.GetComponent<Wall>());

                EdgeIndex++;
                count++;
            }

        }
    }
}