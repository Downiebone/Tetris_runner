using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//maybe rename to draggable_spawn_gold_ring specifically
public class Draggable_SpawnThing : draggable_piece
{
    [SerializeField] private GameObject prefab_to_spawn;

    [SerializeField] private SpriteRenderer highlight_renderer;


    [Header("Player launch")]
    [SerializeField] private float player_launch_rate = 3;

    [SerializeField] private int spawn_golden_area = 1;

    protected override void Start()
    {

        RB = GetComponent<Rigidbody2D>();

        spawn_golden_area = PlayerPrefs.GetInt("PowerUp_Golden") + 1;

        int parts_of_highlight_spots = 0;

        //Debug.Log("INTING POWERUP: area:  " + spawn_area.ToString());

        if (spawn_golden_area == 0)
        { //initial blast
            parts_of_highlight_spots++;
        }
        else
        {
            for (int i = -spawn_golden_area; i <= spawn_golden_area; i++)
            {
                for (int j = -spawn_golden_area; j <= spawn_golden_area; j++)
                {
                    if (Mathf.Abs(i) + Mathf.Abs(j) <= spawn_golden_area) //star shape
                    {
                        //new Vector2Int(i, j)
                        parts_of_highlight_spots++;
                    }
                }
            }
        }

        HighlightSpots = new Vector2Int[parts_of_highlight_spots];

        Debug.Log("high_spots.length: " + HighlightSpots.Length.ToString());
        int curr_index = 0;

        for (int i = -spawn_golden_area; i <= spawn_golden_area; i++)
        {
            for (int j = -spawn_golden_area; j <= spawn_golden_area; j++)
            {
                if (Mathf.Abs(i) + Mathf.Abs(j) <= spawn_golden_area) //star shape
                {
                    //new Vector2Int(i, j)
                    HighlightSpots[curr_index] = new Vector2Int(i, j);
                    curr_index++;


                }
            }
        }

        transform.localScale = new Vector3(floating_scale, floating_scale, 1);

        renderers = new SpriteRenderer[1];
        renderers[0] = GetComponent<SpriteRenderer>();

        Debug.Log("renderer: " + renderers[0].gameObject.name);

        if (delayed_start_highlight)
        {
            Highlight();
        }
    }

    public override void touch_rotated()
    {
        return;
    }

    public override void dragged_position(Vector2 newPosition)
    {
        if (Vector2.Distance(LastTestedPlaceSpot, LastWorkingPlaceSpot) > toFarForPlace)
        {
            if (to_far_to_place == false)
            {
                for (int i = 0; i < HighlightSpots.Length; i++)
                {
                    CameraObj.HighlightObjects[i].GetComponent<SpriteRenderer>().color = Color.red;
                }
            }
            to_far_to_place = true;
        }
        else
        {
            if (to_far_to_place == true)
            {
                for (int i = 0; i < HighlightSpots.Length; i++)
                {
                    CameraObj.HighlightObjects[i].GetComponent<SpriteRenderer>().color = Color.white;
                }
            }
            to_far_to_place = false;
        }

        transform.position = newPosition;
        Vector2Int newPos = Vector2Int.RoundToInt(newPosition);

        //testing this now
        LastTestedPlaceSpot = newPos;

        bool failed_pos = false;


        if (ValidSpaceToPlace((new Vector2Int(0, 0)) + newPos) == false) { failed_pos = true; }
        if (ValidSpaceToPlace((new Vector2Int(0, 1)) + newPos) == false) { failed_pos = true; }
        if (ValidSpaceToPlace((new Vector2Int(0, -1)) + newPos) == false) { failed_pos = true; }

        //check closest spot aswell (cyotetime :)
        if (failed_pos)
        {
            float number = 0.5f;
            float closest_numb = (newPosition.x % 1);
            bool is_x_val = true;
            float difference = Mathf.Abs(number - closest_numb);

            float currentDifference = Mathf.Abs(number - (newPosition.y % 1));
            if (currentDifference < difference)
            {
                closest_numb = (newPosition.y % 1);
                is_x_val = false;
            }
            if (is_x_val)
            {
                if (closest_numb > 0.5f)
                {
                    newPos = Vector2Int.RoundToInt(newPosition + new Vector2(-1, 0));
                }
                else
                {
                    newPos = Vector2Int.RoundToInt(newPosition + new Vector2(1, 0));
                }
            }
            else
            {
                if (closest_numb > 0.5f)
                {
                    newPos = Vector2Int.RoundToInt(newPosition + new Vector2(0, -1));
                }
                else
                {
                    newPos = Vector2Int.RoundToInt(newPosition + new Vector2(0, 1));
                }
            }

            for (int i = 0; i < HighlightSpots.Length; i++)
            {
                //--------------------------------------------------------------------------------------final fail of placing blocks-(where it returns if all fails)------------------------------------------------
                if (ValidSpaceToPlace(HighlightSpots[i] + newPos) == false) { return; }
            }
        }

        LastWorkingPlaceSpot = newPos;

        for (int i = 0; i < HighlightSpots.Length; i++)
        {
            CameraObj.HighlightObjects[i].transform.position = (Vector2)(HighlightSpots[i] + newPos);
        }
    }

    protected override void PlaceDraggable()
    {
        CameraObj.RemoveDraggable_atIndex(myDraggableIndex);

        GameObject GO = Instantiate(prefab_to_spawn, (Vector2)LastWorkingPlaceSpot, Quaternion.identity);

        GO.GetComponent<Golden_ring>().expanding_explosion(spawn_golden_area - 1, LastWorkingPlaceSpot);

        MusicManager.Instance.play_soundeffect(place_sound, use_pitch_varying);

        Destroy(this.gameObject);
        //instantiate bomb-effect or some thing
    }

    protected override bool ValidSpaceToPlace(Vector2Int pos)
    {
        return (pos.y < GridObj.gridHeight && pos.y >= 0);
    }
    protected override void enable_highlight()
    {
        highlight_renderer.enabled = true;
    }
    protected override void disable_highlight()
    {
        highlight_renderer.enabled = false;
    }
}
