using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorBomb_draggable : draggable_piece
{
    //protected bool rotated = false;
    //protected Vector2Int[] HighlightSpots_Original;
    //public Vector2Int[] HighlightSpots_RotateVariant;

    [SerializeField] private SpriteRenderer highlight_renderer;

    protected override void Start()
    {
        RB = GetComponent<Rigidbody2D>();

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
        //Cant rotate Color Bomb
        return;
    }

    protected override void PlaceDraggable()
    {
        Transform player_pos = GameObject.FindGameObjectWithTag("Player").transform;

        CameraObj.RemoveDraggable_atIndex(myDraggableIndex);

        Vector2Int tryPos = LastWorkingPlaceSpot + HighlightSpots[0];

        Cell cellAtPlacedSpot = GridObj.getCellAtPoint(tryPos);

        if (!cellAtPlacedSpot.isActive) //explode early at only 1 spot, if you bomb the air
        {
            GridObj.bombTilePlayer(LastWorkingPlaceSpot + HighlightSpots[0], false); //the false means it will spawn spiral animation
            MusicManager.Instance.play_soundeffect(place_sound, use_pitch_varying);

            Destroy(this.gameObject);
            return;
        }

        Cell.Cell_type spotCelltype = cellAtPlacedSpot.type;

        List<Vector2Int> foundpositions = new List<Vector2Int>();
        foundpositions.Add(tryPos);

        Queue<Vector2Int> checkingPosition = new Queue<Vector2Int>();
        checkingPosition.Enqueue(tryPos);

        int safeGuard = 0;
        //Get all blocks that connect to og block of same color
        while (checkingPosition.Count > 0) // check all positions around, add active ones of same type and color to list. Only check color if they are blocks
        {
            safeGuard++;
            if(safeGuard > 64)
            {
                Debug.LogError("Break: " + checkingPosition.Count.ToString());
                break;
            }

            Vector2Int CurrentsPosition = checkingPosition.Dequeue();

            Cell CheckingCell = GridObj.getCellAtPoint(CurrentsPosition);
            Cell CompareToCell = new Cell();
            Vector2Int LookAtSpot = new Vector2Int();

            LookAtSpot = CurrentsPosition + new Vector2Int(0, 1);
            if (!foundpositions.Contains(LookAtSpot))
            {
                CompareToCell = GridObj.getCellAtPoint(LookAtSpot);
                if (CompareToCell.isActive)
                {
                    if (GridObj.AreCellsOfSameColorQualia(CheckingCell, CompareToCell))
                    {
                        checkingPosition.Enqueue(LookAtSpot);
                        foundpositions.Add(LookAtSpot);
                    }
                }
            }

            LookAtSpot = CurrentsPosition + new Vector2Int(1, 0);
            if (!foundpositions.Contains(LookAtSpot))
            {
                CompareToCell = GridObj.getCellAtPoint(LookAtSpot);
                if (CompareToCell.isActive)
                {
                    if (GridObj.AreCellsOfSameColorQualia(CheckingCell, CompareToCell))
                    {
                        checkingPosition.Enqueue(LookAtSpot);
                        foundpositions.Add(LookAtSpot);
                    }
                }
            }
            LookAtSpot = CurrentsPosition + new Vector2Int(0, -1);
            if (!foundpositions.Contains(LookAtSpot))
            {
                CompareToCell = GridObj.getCellAtPoint(LookAtSpot);
                if (CompareToCell.isActive)
                {
                    if (GridObj.AreCellsOfSameColorQualia(CheckingCell, CompareToCell))
                    {
                        checkingPosition.Enqueue(LookAtSpot);
                        foundpositions.Add(LookAtSpot);
                    }
                }
            }
            LookAtSpot = CurrentsPosition + new Vector2Int(-1, 0);
            if (!foundpositions.Contains(LookAtSpot))
            {
                CompareToCell = GridObj.getCellAtPoint(LookAtSpot);
                if (CompareToCell.isActive)
                {
                    if (GridObj.AreCellsOfSameColorQualia(CheckingCell, CompareToCell))
                    {
                        checkingPosition.Enqueue(LookAtSpot);
                        foundpositions.Add(LookAtSpot);
                    }
                }
            }
        }





        for (int i = 0; i < foundpositions.Count; i++)
        {
            GridObj.bombTilePlayer(foundpositions[i], false); //the false means it will spawn spiral animation
        }

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
