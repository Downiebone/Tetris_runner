using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridEditor : MonoBehaviour
{
    [Header("EDITOR")]
    [SerializeField] private bool shouldStartByLoadingLevel = false;
    [Space]
    [Space]


    [Tooltip("Y,X (for some reason hehe)")]
    public Cell[,] Grid;

    [SerializeField] private GameObject cell_piece_prefab;

    [SerializeField] private Sprite emptyCellSprite;

    [SerializeField] private Sprite[] cellSprites;

    [SerializeField] private Color[] cellColors;

    [SerializeField] private bool fillRandom = false;

    [Header("Map_Gen")]
    
    public int gridHeight = 15;
    public int gridLength = 500;

    [Header("MAP SAVE")]

    public string MapName = "temp_name";
    public int gridHeight_SAVE = 15;
    public int gridLength_SAVE = 15;
    public int MapDifficulity = 0;
    public int MapRarity = 0;

    [Space]

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
        if (shouldStartByLoadingLevel)
        {

        }
        else //Generate perlin map
        {
            updatePerlinOffset();

            fillGridColors();

            fillGridShape();

            populateGrid();
        }


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
                Grid[r, c].color_index = colorInd;
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
                //-------------------------------PERLIN LEVEL PART
                //else if(r < currMaxHeight)
                //{
                //    Grid[r, c].isActive = getPerlinCord(r, c);
                //}
            }
        }
    }

    public void Fill_In_Loaded_grid(Cell[,] Grid_loaded, int load_from_pos_x)
    {
        for (int i = 0; i < Grid_loaded.GetLength(0); i++) //height
        {
            for (int j = 0; j < Grid_loaded.GetLength(1); j++) //width
            {
                Cell grid_cell = Grid[i, j + load_from_pos_x];
                Cell loaded_cell = Grid_loaded[i, j];
                grid_cell.isActive = loaded_cell.isActive;
                grid_cell.type = loaded_cell.type;
                grid_cell.color_index = loaded_cell.color_index;

                Color set_color = loaded_cell.type == Cell.Cell_type.Ground ? cellColors[loaded_cell.color_index] : Color.white;

                grid_cell.cellColor = set_color;
                
                grid_cell.sprite_rend.color = set_color;
                if (!grid_cell.isActive)
                {
                    grid_cell.sprite_rend.sprite = null;
                }
                else
                {
                    grid_cell.sprite_rend.sprite = cellSprites[(int)loaded_cell.type];
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
        if(pos.y >= gridHeight) { return false; }

        Cell cur_cell = Grid[pos.y, pos.x];

        return cur_cell.type == Cell.Cell_type.Ground && cur_cell.isActive == true;
    }
    public Cell Cell_on_position(Vector2Int pos)
    {
        if (pos.y >= gridHeight) { //return "empty"/"off" cell if we check aboce the ceiling
            Cell tempcell = new Cell();
            tempcell.isActive = false;
            return tempcell;
        }

        return Grid[pos.y, pos.x];
    }
    public void placeTile(Vector2Int pos, Color placeTileColor, Cell.Cell_type typa_cell = Cell.Cell_type.Ground, int color_index = 0)
    {
        Cell cell = Grid[pos.y, pos.x];
        //if (cell.isActive && cell.type == typa_cell)
        //{
        //    return;
        //}

        cell.type = typa_cell;
        

        cell.isActive = true;

        if(typa_cell != Cell.Cell_type.Ground)
        {
            placeTileColor = Color.white;
        }

        cell.cellColor = placeTileColor;
        cell.sprite_rend.color = placeTileColor;
        cell.color_index = color_index;

        cell.sprite_rend.sprite = cellSprites[(int)cell.type];
    }
    public void place_fullCell(Vector2Int pos, Cell newCell)
    {
        //Debug.Log("placing full cell: active: " + newCell.isActive + " | x: " + pos.x + " | y: " + pos.y);
        Cell cell = Grid[pos.y, pos.x];
        //if (cell.isActive && cell.type == typa_cell)
        //{
        //    return;
        //}

        cell.type = newCell.type;

        cell.isActive = newCell.isActive;

        Color placeTileColor = newCell.cellColor;

        if (newCell.type != Cell.Cell_type.Ground)
        {
            placeTileColor = Color.white;
        }

        cell.cellColor = placeTileColor;
        cell.sprite_rend.color = placeTileColor;
        cell.color_index = newCell.color_index;

        cell.sprite_rend.sprite = newCell.isActive == true? cellSprites[(int)cell.type] : null;
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

    public Cell getCellAtPoint(Vector2Int point)
    {
        return Grid[point.y, point.x];
    }

    public Color GainRandomColor()
    {
        return cellColors[Random.Range(0, cellColors.Length)];
    }
}
