using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb_draggable : draggable_piece
{
    protected bool rotated = false;
    protected Vector2Int[] HighlightSpots_Original;
    public Vector2Int[] HighlightSpots_RotateVariant;

    [SerializeField] private SpriteRenderer highlight_renderer;


    [Header("Player launch")]
    [SerializeField] private float player_launch_rate = 3;

    protected override void Start()
    {
        RB = GetComponent<Rigidbody2D>();

        HighlightSpots_Original = HighlightSpots;

        transform.localScale = new Vector3(floating_scale, floating_scale, 1);

        renderers = new SpriteRenderer[1];
        renderers[0] = GetComponent<SpriteRenderer>();

        if (delayed_start_highlight)
        {
            Highlight();
        }
    }

    public override void touch_rotated()
    {
        if (!rotated)
        {
            HighlightSpots = HighlightSpots_RotateVariant;
        }
        else
        {
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

            if((LastWorkingPlaceSpot + HighlightSpots[i]) == Vector2Int.RoundToInt((Vector2)player_pos.position)) //if we bomb the player
            {
                player_pos.gameObject.GetComponent<Player_Script>().Bomb_player(player_launch_rate);
            }

            GridObj.bombTile(LastWorkingPlaceSpot + HighlightSpots[i]);
        }

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

    protected override void enable_highlight()
    {
        highlight_renderer.enabled = true;
    }
    protected override void disable_highlight()
    {
        highlight_renderer.enabled = false;
    }
}
