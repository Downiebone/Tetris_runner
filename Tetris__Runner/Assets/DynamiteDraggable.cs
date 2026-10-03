using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamiteDraggable : draggable_piece
{
    protected bool rotated = false;
    protected Vector2Int[] HighlightSpots_Original;
    public Vector2Int[] HighlightSpots_RotateVariant;

    private int HighlightsLenght_Oversize = 0;

    [SerializeField] private SpriteRenderer highlight_renderer;


    [Header("Player launch")]
    [SerializeField] private float player_launch_rate = 3;

    protected override void Start()
    {
        RB = GetComponent<Rigidbody2D>();

        HighlightSpots_Original = HighlightSpots;

        //HighlightsLenght_Oversize = Mathf.Abs(HighlightSpots_Original.Length - HighlightSpots_RotateVariant.Length);

        transform.localScale = new Vector3(floating_scale, floating_scale, 1);

        renderers = new SpriteRenderer[1];
        renderers[0] = GetComponent<SpriteRenderer>();

        if (delayed_start_highlight)
        {
            Highlight();
        }
    }

    private int degrees = 0;

    public override void touch_rotated()
    {
        degrees -= 45;
        transform.rotation = Quaternion.Euler(Vector3.forward * degrees);


        if (!rotated)
        {
            for (int i = 0; i < HighlightSpots_RotateVariant.Length; i++)
            {
                HighlightSpots_RotateVariant[i] = new Vector2Int(HighlightSpots_RotateVariant[i].y, -HighlightSpots_RotateVariant[i].x);
            }

            HighlightSpots = HighlightSpots_RotateVariant;

        }
        else
        {
            for (int i = 0; i < HighlightSpots_Original.Length; i++)
            {
                HighlightSpots_Original[i] = new Vector2Int(HighlightSpots_Original[i].y, -HighlightSpots_Original[i].x);
            }
            HighlightSpots = HighlightSpots_Original;
        }
        rotated = !rotated;
    }

    protected override void PlaceDraggable()
    {
        Transform player_pos = GameObject.FindGameObjectWithTag("Player").transform;

        CameraObj.RemoveDraggable_atIndex(myDraggableIndex);

        for (int i = 0; i < HighlightSpots.Length; i++)
        {


            //Debug.Log("player_pos: " + Vector2Int.RoundToInt((Vector2)player_pos.position) + " | high_spot: " + HighlightSpots[i]);

            if ((LastWorkingPlaceSpot + HighlightSpots[i]) == Vector2Int.RoundToInt((Vector2)player_pos.position)) //if we bomb the player
            {
                player_pos.gameObject.GetComponent<Player_Script>().Bomb_player(player_launch_rate);
            }

            GridObj.bombTilePlayer(LastWorkingPlaceSpot + HighlightSpots[i]);
        }

        //tell stuff like lasers to update
        GridObj.GridBlocksUpdate();

        MusicManager.Instance.play_soundeffect(place_sound, use_pitch_varying);

        Destroy(this.gameObject);
        //instantiate bomb-effect or some thing
    }

    protected override bool ValidSpaceToPlace(Vector2Int pos)
    {
        return (pos.y < GridObj.gridHeight && pos.y >= 0);
    }

    //need override because bomb_draggable does not use highlight_spots when selected
    //protected override void spawn_sprites()
    //{
    //    renderers = new SpriteRenderer[HighlightSpots.Length];

    //    for (int i = 0; i < HighlightSpots.Length; i++)
    //    {
    //        GameObject GO = Instantiate(sprite_ref, Vector3.zero, Quaternion.identity, transform);
    //        renderers[i] = GO.GetComponent<SpriteRenderer>();
    //        GO.transform.localPosition = (Vector2)HighlightSpots[i];
    //        renderers[i].color = Piece_color;
    //        renderers[i].sortingLayerName = "Floating";
    //    }
    //}

    protected override void hide_visuals_internal()
    {
        GetComponent<SpriteRenderer>().enabled = false;
        highlight_renderer.enabled = false;
    }
    protected override void show_visuals_internal()
    {
        GetComponent<SpriteRenderer>().enabled = true;

        if (is_chosen)
        {
            highlight_renderer.enabled = true;
        }
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
