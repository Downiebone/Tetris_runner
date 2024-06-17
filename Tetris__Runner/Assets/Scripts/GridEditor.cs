using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridEditor : MonoBehaviour
{
    private Cell[,] Grid;

    [SerializeField] private GameObject cell_piece_prefab;

    [SerializeField] private Sprite emptyCellSprite;

    [SerializeField] private Sprite[] cellSprites;

    [SerializeField] private Color[] cellColors;

    [SerializeField] private bool fillRandom = false;

    [Header("Map_Gen")]
    
    public int gridHeight = 15;
    public int gridLength = 500;

    [SerializeReference] private int startArea_size = 10;

    [Tooltip(">= 1")]
    [SerializeField] private int perlin_size = 32;

    [Tooltip(">= 1")]
    [SerializeField] private int Level_Min_Height = 2;
    [SerializeField] private int Level_Max_Height = 15;


    private float perlin_x_offset = 0;
    private float perlin_y_offset = 0;

    private void Awake()
    {
        Grid = new Cell[gridHeight, gridLength];
    }
    private void Start()
    {
        //fail_safe
        if(Level_Max_Height > gridHeight)
        {
            Level_Max_Height = gridHeight;
        }

        updatePerlinOffset();

        fillGridColors();

        fillGridShape();

        populateGrid();
    }

    

    // Update is called once per frame
    void fillGridColors()
    {
        for (int r = 0; r < gridHeight; r++)
        {
            for (int c = 0; c < gridLength; c++)
            {
                Grid[r, c] = new Cell();

                GameObject GO = Instantiate(cell_piece_prefab, new Vector3(c, r, 0), Quaternion.identity, gameObject.transform);
                Grid[r, c].sprite_rend = GO.GetComponent<SpriteRenderer>();

                int colorInd = Random.Range(0, cellColors.Length);
                Grid[r, c].cellColor = cellColors[colorInd];
                Grid[r, c].sprite_rend.color = Grid[r, c].cellColor;
            }
        }
    }

    void fillGridShape()
    {
        int grid_usable_height = (Level_Max_Height) - (Level_Min_Height);

        for (int c = 0; c < gridLength; c++)
        {
            int currMaxHeight = Mathf.RoundToInt(Level_Max_Height * ((float)Random.Range(Level_Min_Height + 1, Level_Max_Height - 1) / grid_usable_height));

            if(c < startArea_size)
            {
                currMaxHeight = Level_Min_Height;
            }

            //Debug.Log("X: " + );
            for (int r = 0; r < gridHeight; r++)
            {
                if(r < Level_Min_Height)
                {
                    Grid[r, c].isActive = true;
                }
                else if(r < currMaxHeight)
                {
                    Grid[r, c].isActive = getPerlinCord(r, c);
                }
            }
        }
    }
    void updatePerlinOffset()
    {
        perlin_x_offset = Random.Range(0f, 1f);
        perlin_y_offset = Random.Range(0f, 1f);
    }
    bool getPerlinCord(int x, int y)
    {
        float xCord = ((float)x / perlin_size) + perlin_x_offset;
        float yCord = ((float)y / perlin_size) + perlin_y_offset;

        return Mathf.RoundToInt(Mathf.PerlinNoise(xCord, yCord)) == 1;

    }
    public bool Cell_is_ground(Vector2Int pos)
    {
        Cell cur_cell = Grid[pos.y, pos.x];

        return cur_cell.type == Cell.Cell_type.Ground && cur_cell.isActive == true;
    }
    public Cell Cell_on_position(Vector2Int pos)
    {
        return Grid[pos.y, pos.x];
    }
    public void placeTile(Vector2Int pos)
    {
        Cell cell = Grid[pos.y, pos.x];
        if (cell.isActive)
        {
            return;
        }
        cell.isActive = true;
        cell.sprite_rend.sprite = cellSprites[(int)cell.type];
    }
    public void deleteTile(Vector2Int pos)
    {
        Cell cell = Grid[pos.y, pos.x];
        if (!cell.isActive)
        {
            return;
        }
        cell.isActive = false;
        cell.sprite_rend.sprite = null;
    }
    void populateGrid()
    {
        for (int r = 0; r < gridHeight; r++)
        {
            for (int c = 0; c < gridLength; c++)
            {
                Cell curCell = Grid[r, c];
                if (curCell.isActive == true)
                {
                    curCell.sprite_rend.sprite = cellSprites[(int)curCell.type];
                }
                else
                {
                    curCell.sprite_rend.sprite = null;
                }
            }
        }
    }

    public Vector3Int getClosestGridPos(Vector3 curPos)
    {
        Vector3Int newPos = Vector3Int.RoundToInt(curPos);
        if(newPos.y > gridHeight - 1 || newPos.y < 0 || newPos.x > gridLength - 1 || newPos.x < 0)
        {
            return new Vector3Int(-1,-1,0);
        }
        else
        {
            return new Vector3Int(newPos.x, newPos.y, 0);
        }
    }
}
