using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridEditor : MonoBehaviour
{
    [Header("EDITOR")]
    [SerializeField] private bool shouldStartByLoadingLevel = false;

    [SerializeField] private int x_value_to_start_loading = 10;
    [SerializeField] private int x_value_offset_from_player_to_load = 30;

    [SerializeField] private GridSaver Save_script;
    [SerializeField] private level_play_system play_system;

    private Transform player_trans;

    [SerializeField] private string current_from_str = "Bot";
    [SerializeField] private string current_to_str = "Bot";
    [SerializeField] private int current_difficulity = 1;

    private string[] to_save_options = new string[3]
    {
        "Bot",
        "Mid",
        "Top"
    };

    private int[] difficulity_slider = { //how likely each difficulity is to get
        1,
        1,
        1,
        2,
        2,
        2,
        3,
        3,
        4
    };

    [Space]
    [Space]


    

    [SerializeField] private GameObject cell_piece_prefab;

    [Tooltip("Y,X (for some reason hehe)")]
    public Cell[,] Grid;

    public Cell[,] Grid_2;

    public GameObject Grid_1_Parent;
    public GameObject Grid_2_Parent;

    //private bool move_grid_forward = false;
    /// <summary>
    /// 1: will move @Grid next, 2: will move @Grid_2 next
    /// </summary>
    private int next_grid_to_move = 2;
    private int number_of_times_grid_moved = 0;

    // sets constantly from the player script
    private int player_x_val = 1;
    public void player_x_val_set(int new_x)
    {
        player_x_val = new_x;
    }

    [SerializeField] private Sprite emptyCellSprite;

    [SerializeField] private Sprite[] cellSprites;

    [SerializeField] private Color[] cellColors;

    //[SerializeField] private bool fillRandom = false;

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
        Grid_2 = new Cell[gridHeight, gridLength];
    }

    public Cell getCellAtPoint(Vector2Int point)
    {
        if (point.y >= gridHeight || point.y < 0)
        {
            return new Cell();
        }

        return getCellAtPoint(point.y, point.x);
    }

    public Cell getCellAtPoint(int y_, int x_)
    {
        if (y_ >= gridHeight || y_ < 0 || x_ < 0)
        {
            return new Cell();
        }

        int offset_x = x_;

        if(x_ >= (gridLength * number_of_times_grid_moved)) //on latest_grid_moved
        {
            if(number_of_times_grid_moved > 0)
            {
                offset_x = x_ % (gridLength * number_of_times_grid_moved);
            }
            
            return get_latest_moved_grid_cell(offset_x, y_);
        }
        else
        {
            if ((number_of_times_grid_moved - 1) > 0)
            {
                offset_x = x_ % (gridLength * (number_of_times_grid_moved - 1));
            }

            return get_oldest_moved_grid_cell(offset_x, y_);
        }

        
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
            updatePerlinOffset();

            fillGridColors();

            fillGridShape();

            populateGrid();

            player_trans = GameObject.FindGameObjectWithTag("Player").transform;
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
                //fill grid_1
                Grid[r, c] = new Cell();

                GameObject GO = Instantiate(cell_piece_prefab, new Vector3(c + Grid_1_Parent.transform.position.x, r + Grid_2_Parent.transform.position.y, 0), Quaternion.identity, Grid_1_Parent.transform);
                Grid[r, c].sprite_rend = GO.GetComponent<SpriteRenderer>();

                int colorInd = Random.Range(0, cellColors.Length);
                Grid[r, c].color_index = colorInd;
                Grid[r, c].cellColor = cellColors[colorInd];
                Grid[r, c].sprite_rend.color = Grid[r, c].cellColor;


                //fill grid_2
                Grid_2[r, c] = new Cell();

                GameObject GO_2 = Instantiate(cell_piece_prefab, new Vector3(c + Grid_2_Parent.transform.position.x, r + Grid_2_Parent.transform.position.y, 0), Quaternion.identity, Grid_2_Parent.transform);
                Grid_2[r, c].sprite_rend = GO_2.GetComponent<SpriteRenderer>();
                 
                int colorInd_2 = Random.Range(0, cellColors.Length);
                Grid_2[r, c].color_index = colorInd_2;
                Grid_2[r, c].cellColor = cellColors[colorInd_2];
                Grid_2[r, c].sprite_rend.color = Grid_2[r, c].cellColor;
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
                    Grid_2[r, c].isActive = true;
                }
                //-------------------------------PERLIN LEVEL PART
                //else if(r < currMaxHeight)
                //{
                //    Grid[r, c].isActive = getPerlinCord(r, c);
                //}
            }
        }
    }

    public void Fill_In_Loaded_grid(Cell[,] Grid_loaded)
    {

        for (int i = 0; i < Grid_loaded.GetLength(0); i++) //height
        {
            for (int j = 0; j < Grid_loaded.GetLength(1); j++) //width
            {
                Cell grid_cell = getCellAtPoint(i, j + x_value_to_start_loading); //check
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

        x_value_to_start_loading += Grid_loaded.GetLength(1);
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
    public bool Cell_is_active_type(Vector2Int pos, Cell.Cell_type typ)
    {
        if(pos.y >= gridHeight || pos.y < 0) { return false; }

        Cell cur_cell = getCellAtPoint(pos.y, pos.x); //check

        return cur_cell.type == typ && cur_cell.isActive == true;
    }
    public void placeTile(Vector2Int pos, Color placeTileColor, Cell.Cell_type typa_cell = Cell.Cell_type.Ground, int color_index = 0)
    {
        Cell cell = getCellAtPoint(pos.y, pos.x);
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
        Cell cell = getCellAtPoint(pos.y, pos.x);
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
    public void replace_active_Tile(Vector2Int pos, Cell.Cell_type replace_to_type)
    {
        if (pos.y < 1) //dont replace bottom
        {
            return;
        }

        Cell cell = getCellAtPoint(pos.y, pos.x);
        if (!cell.isActive)
        {
            return;
        }
        cell.type = replace_to_type;
        cell.sprite_rend.color = Color.white;
        cell.sprite_rend.sprite = cellSprites[(int)cell.type];
    }
    public void deleteTile(Vector2Int pos)
    {
        Cell cell = getCellAtPoint(pos.y, pos.x);
        if (!cell.isActive)
        {
            return;
        }
        cell.isActive = false;
        cell.sprite_rend.sprite = null;
    }
    public void deleteTile_exeptFloor(Vector2Int pos)
    {
        if(pos.y < 1) //dont destroy floor
        {
            return;
        }

        Cell cell = getCellAtPoint(pos.y, pos.x);
        if (!cell.isActive)
        {
            return;
        }
        cell.isActive = false;
        cell.sprite_rend.sprite = null;
    }
    public void bombTile(Vector2Int pos)
    {
        if (pos.y < 1) //dont destroy floor
        {
            return;
        }

        Cell cell = getCellAtPoint(pos.y, pos.x);
        if (!cell.isActive)
        {
            return;
        }

        if(cell.type == Cell.Cell_type.collectable_coin) // maybe become an item?
        {
            play_system.spawn_coin_referance(pos, player_trans);
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
                Cell curCell_2 = Grid_2[r, c];
                if (curCell.isActive == true)
                {
                    curCell.sprite_rend.sprite = cellSprites[(int)curCell.type];
                    curCell_2.sprite_rend.sprite = cellSprites[(int)curCell.type];
                }
                else
                {
                    curCell.sprite_rend.sprite = null;
                    curCell_2.sprite_rend.sprite = null;
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

    public Color GainRandomColor()
    {
        return cellColors[Random.Range(0, cellColors.Length)];
    }

    private bool can_load = true;

    private void Move_grids()
    {
        number_of_times_grid_moved++;

        if(next_grid_to_move == 1)
        {
            Grid_1_Parent.transform.position = new Vector2((gridLength * number_of_times_grid_moved), 0);
            next_grid_to_move = 2;
        }
        else
        {
            Grid_2_Parent.transform.position = new Vector2((gridLength * number_of_times_grid_moved), 0);
            next_grid_to_move = 1;
        }
    }

    private Cell get_latest_moved_grid_cell(int x__, int y__)
    {
        if(next_grid_to_move == 1) //if we next will move the first grid, that means we latest moved the second
        {
            return Grid_2[y__, x__];
        }
        else
        {
            return Grid[y__, x__];
        }
    }
    private Cell get_oldest_moved_grid_cell(int x__, int y__)
    {
        if (next_grid_to_move == 1) //if we next will move the first grid, that means we latest moved the second
        {
            return Grid[y__, x__];
        }
        else
        {
            return Grid_2[y__, x__];
        }
    }

    private void Update()
    {

        //if (Input.GetKeyDown(KeyCode.L))
        //{
        //    current_to_str = to_save_options[Random.Range(0, 3)];
        //    current_difficulity = difficulity_slider[Random.Range(0, difficulity_slider.Length)];

        //    StartCoroutine(Save_script.LoadCells_at_random_fromResources(current_from_str, current_to_str, current_difficulity));

        //    current_from_str = current_to_str;
        //}
        //return;

        if (!shouldStartByLoadingLevel) { return; }

        if(player_x_val == (gridLength/2) + gridLength * number_of_times_grid_moved)
        {
            Debug.Log("MOVING GRIDS");
            Move_grids();
        }

        if(can_load && (x_value_to_start_loading < (player_trans.position.x + x_value_offset_from_player_to_load)))
        {
            Debug.Log("------------LOADING NEXT!!--------------");

            can_load = false;

            load_next_level();
        }
        
    }


    public void load_next_level()
    {
        //if (reached_end_of_world) { return; }


        current_to_str = to_save_options[Random.Range(0, 3)];
        current_difficulity = difficulity_slider[Random.Range(0, difficulity_slider.Length)];

        StartCoroutine(CountingCoroutine(current_from_str, current_to_str, current_difficulity));
    }

    IEnumerator CountingCoroutine(string from_load, string to_load, int difficulity_load)
    {
        //var task = Save_script.LoadCells_at_random(from_load, to_load, difficulity_load);

        //yield return new WaitUntil(() => task.IsCompleted);

        //if (task.Exception != null)
        //{
        //    Debug.LogError("Error in LoadCells: " + task.Exception);
        //    yield break;
        //}

        yield return Save_script.LoadCells_at_random_fromResources(from_load, to_load, difficulity_load);

        //Debug.Log("Loaded x: " + Save_script.grid_return.GetLength(1));

        Fill_In_Loaded_grid(Save_script.grid_return);
        //can_load = true;

        //set next in chain, so the world links up
        current_from_str = current_to_str;
        can_load = true;
        //Debug.Log("LoadCells finished successfully!");
    }
}
