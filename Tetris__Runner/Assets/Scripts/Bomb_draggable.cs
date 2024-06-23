using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb_draggable : draggable_piece
{
    protected bool rotated = false;
    protected Vector2Int[] HighlightSpots_Original;
    public Vector2Int[] HighlightSpots_RotateVariant;

    protected override void Start()
    {
        RB = GetComponent<Rigidbody2D>();

        HighlightSpots_Original = HighlightSpots;
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
        CameraObj.RemoveDraggable_atIndex(myDraggableIndex);

        for (int i = 0; i < HighlightSpots.Length; i++)
        {
            GridObj.deleteTile(LastWorkingPlaceSpot + HighlightSpots[i]);
        }

        Destroy(this.gameObject);
        //instantiate bomb-effect or some thing
    }

    protected override bool ValidSpaceToPlace(Vector2Int pos)
    {
        return !(((Vector2)pos).y >= GridObj.gridHeight);
    }
}
