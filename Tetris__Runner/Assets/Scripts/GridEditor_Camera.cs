using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.UI;
using TMPro;


public class undo_cell_value
{
    public Vector2Int yx_vals = new Vector2Int();
    public Cell cell = new Cell();
}
public class GridEditor_Camera : MonoBehaviour
{
    [SerializeField] private float camSpeed;
    [SerializeField] private float camSpeed_multiplier;

    [SerializeField] private GameObject cell_highlight;

    [SerializeField] private GridEditor Grid_script;

    [SerializeField] private GameObject horizont_liner;
    [SerializeField] private GameObject vertical_liner;

    [SerializeField] private Image ui_sprite;

    [SerializeField] private TMP_Text placing_color_int;

    public GridSaver Save_Script;

    private Vector2Int last_placed_position = new Vector2Int(-8,-8);

    const int max_placing_types = 3;

    [SerializeField] private Sprite[] type_sprite_list;

    [SerializeField] private Cell.Cell_type type_cell_placing = Cell.Cell_type.Ground;
    private int current_type_placing = 0;

    private bool can_load = true;

    private bool using_random_colors = true;

    private int[] counter_vals = { //how many blocks you need to place before new color
        1,
        2,
        2,
        2,
        3,
        3,
        4,
    };

    [SerializeField] private Image[] Colors_images; //colors to place (grayscale during editor?)

    [SerializeField] private Image manual_automatic_button;

    private int current_color_index = 0; // 0,1,2,3,4 t colors??
    private int counter_before_colorChange = 0;

    private List<undo_cell_value> undo_cell_values = new List<undo_cell_value>();

    public void set_color_to_manual(int new_color)
    {
        Color tmp = manual_automatic_button.color;
        tmp.a = 0.5f;
        manual_automatic_button.color = tmp;

        using_random_colors = false;

        Colors_images[current_color_index].rectTransform.sizeDelta = new Vector2(35, 35);

        current_color_index = new_color;

        Colors_images[current_color_index].rectTransform.sizeDelta = new Vector2(35, 50);
    }

    public void switch_between_manual_and_automatic()
    {
        using_random_colors = !using_random_colors;

        Color tmp = manual_automatic_button.color;
        tmp.a = using_random_colors? 1f : 0.5f;
        manual_automatic_button.color = tmp;

        if (using_random_colors)
        {
            counter_before_colorChange = 0;
            update_counter_colorIndex();
        }
    }

    private void add_tile_to_undo_list(Vector2Int pos, Cell save_cell)
    {
        bool point_already_added = false;
        for (int i = 0; i < undo_cell_values.Count; i++)
        {
            if(undo_cell_values[i].yx_vals == pos)
            {
                point_already_added = true;
            }
        }
        if (!point_already_added)
        {
            undo_cell_value undo_val = new undo_cell_value();
            undo_val.yx_vals = pos;
            undo_val.cell.copyCell(save_cell);
            Debug.Log("save_cell: " + save_cell.isActive + " | new_cell: " + undo_val.cell.isActive);
            undo_cell_values.Add(undo_val);
        }
    }


    private void Start()
    {
        Save_Script.Setup_AreaSize(Grid_script, Grid_script.gridHeight, Grid_script.gridLength);

        update_counter_colorIndex();
    }

    // Update is called once per frame
    void Update()
    {
        float cam_multiplier = 1;

        if (Input.GetKey(KeyCode.LeftShift))
        {
            cam_multiplier = camSpeed_multiplier;
        }

        transform.Translate(Vector2.up * Input.GetAxis("Vertical") * camSpeed * cam_multiplier * Time.deltaTime);
        transform.Translate(Vector2.right * Input.GetAxis("Horizontal") * camSpeed * cam_multiplier * Time.deltaTime);

        horizont_liner.transform.position = new Vector3(Grid_script.gridLength_SAVE - 0.5f, (Grid_script.gridHeight_SAVE / 2f) - 0.5f, 0);
        vertical_liner.transform.position = new Vector3((Grid_script.gridLength_SAVE / 2f) - 0.5f, Grid_script.gridHeight_SAVE - 0.5f, 0);
        horizont_liner.transform.localScale = new Vector3(0.2f, Grid_script.gridHeight_SAVE, 1);
        vertical_liner.transform.localScale = new Vector3(Grid_script.gridLength_SAVE, 0.2f, 1);
    }

    private void LateUpdate()
    {
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2Int currentGridMousePos = new Vector2Int(Grid_script.getClosestGridPos(mouseWorldPos).x, Grid_script.getClosestGridPos(mouseWorldPos).y);
        cell_highlight.transform.position = Grid_script.getClosestGridPos(mouseWorldPos);

        if (currentGridMousePos != (new Vector2Int(-1, -1)))
        {
            if (Input.GetKeyDown(KeyCode.Mouse0) || Input.GetKeyDown(KeyCode.Mouse1))
            {
                undo_cell_values.Clear();
            }

            if (Input.GetKey(KeyCode.Mouse0) && currentGridMousePos != last_placed_position)
            {
                last_placed_position = currentGridMousePos;

                add_tile_to_undo_list(currentGridMousePos, Grid_script.getCellAtPoint(currentGridMousePos));

                Grid_script.placeTile(currentGridMousePos, Colors_images[current_color_index].color, type_cell_placing, current_color_index);

                if (using_random_colors)
                {
                    update_counter_colorIndex();
                }
            }
            if (Input.GetKey(KeyCode.Mouse1))
            {
                last_placed_position = new Vector2Int(-8, -8); //reset

                add_tile_to_undo_list(currentGridMousePos, Grid_script.getCellAtPoint(currentGridMousePos));

                Grid_script.deleteTile(currentGridMousePos);
            }
        }

        if(Input.mouseScrollDelta.y == 1)
        {
            current_type_placing++;
            if(current_type_placing > 2)
            {
                current_type_placing = 0;
            }
            apply_scrolling_types(current_type_placing);

        }
        else if(Input.mouseScrollDelta.y == -1)
        {
            if(current_type_placing > 0)
            {
                current_type_placing--;
            }
            else
            {
                current_type_placing = 2;
            }
            apply_scrolling_types(current_type_placing);
        }

        if (Input.GetKeyDown(KeyCode.KeypadEnter) && can_load)
        {
            can_load = false;

            //start loading cells
            StartCoroutine(CountingCoroutine());
        }

        if(Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Z))
        {
            Debug.Log("Undoing: " + undo_cell_values.Count);
            for (int i = 0; i < undo_cell_values.Count; i++)
            {
                //Debug.Log("saved_cell: " + undo_cell_values[i].cell.isActive);
                Grid_script.place_fullCell(undo_cell_values[i].yx_vals, undo_cell_values[i].cell);

            }
            undo_cell_values.Clear();
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            Grid_script.gridLength_SAVE += 1;
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            if (Grid_script.gridLength_SAVE > 1)
            {
                Grid_script.gridLength_SAVE -= 1;
            }
        }

    }

    private void update_counter_colorIndex()
    {
        counter_before_colorChange--;
        if(counter_before_colorChange <= 0)
        {
            counter_before_colorChange = counter_vals[Random.Range(0, counter_vals.Length)];

            Colors_images[current_color_index].rectTransform.sizeDelta = new Vector2(35, 35);

            int new_color_index = Random.Range(0, Constants.Max_colors_indexes);
            if(new_color_index == current_color_index) //whole thing to not get the same color twice
            {
                new_color_index++;
                if(new_color_index >= Constants.Max_colors_indexes)
                {
                    new_color_index = 0;
                }
            }
            current_color_index = new_color_index;

            Colors_images[current_color_index].rectTransform.sizeDelta = new Vector2(35, 50);
        }
        placing_color_int.text = counter_before_colorChange.ToString();
    }

    private void apply_scrolling_types(int new_type)
    {
        switch (new_type)
        {
            case 0:
                type_cell_placing = Cell.Cell_type.Ground;
                ui_sprite.sprite = type_sprite_list[0];
                break;
            case 1:
                type_cell_placing = Cell.Cell_type.collectable_coin;
                ui_sprite.sprite = type_sprite_list[1];
                break;
            case 2:
                type_cell_placing = Cell.Cell_type.collectable_big;
                ui_sprite.sprite = type_sprite_list[2];
                break;
        }


    }

    //loads cell async
    //but then puts them in the grid sync
    IEnumerator CountingCoroutine()
    {
        var task = Save_Script.LoadCells("long_test", "Bot", "Bot", 5);

        yield return new WaitUntil(() => task.IsCompleted);

        if (task.Exception != null)
        {
            Debug.LogError("Error in LoadCells: " + task.Exception);
            yield break;
        }

        Debug.Log("Loaded: " + Save_Script.grid_return.GetLength(0) + " | " + Save_Script.grid_return.GetLength(1));

        Grid_script.Fill_In_Loaded_grid(Save_Script.grid_return, 20);
        can_load = true;
        Debug.Log("LoadCells finished successfully!");
    }
}
