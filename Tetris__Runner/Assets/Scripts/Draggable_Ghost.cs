using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Draggable_Ghost : draggable_piece
{
    public Vector2Int[] HighlightSpot_L = new Vector2Int[]
    {
        new Vector2Int(0,0),
        new Vector2Int(0,1),
        new Vector2Int(-1,0)
    };
    public Vector2Int[] HighlightSpot_Penis = new Vector2Int[]
    {
        new Vector2Int(0,0),
        new Vector2Int(1,0),
        new Vector2Int(-1,0),
        new Vector2Int(0,-1),
    };
    public Vector2Int[] HighlightSpot_Squigly = new Vector2Int[]
    {
        new Vector2Int(0,0),
        new Vector2Int(1,0),
        new Vector2Int(0,-1),
        new Vector2Int(-1,-1),
    };
    public Vector2Int[] HighlightSpot_Line = new Vector2Int[]
    {
        new Vector2Int(0,0),
        new Vector2Int(0,1),
        new Vector2Int(0,-1)
    };

    protected override void Start()
    {
        int randPiece = Random.Range(0, 4);
        switch (randPiece)
        {
            case 0:
                HighlightSpots = HighlightSpot_L;
                break;
            case 1:
                HighlightSpots = HighlightSpot_Penis;
                break;
            case 2:
                HighlightSpots = HighlightSpot_Squigly;
                break;
            case 3:
                HighlightSpots = HighlightSpot_Line;
                break;

        }

        base.Start();

    }

    protected override void spawn_sprites()
    {
        renderers = new SpriteRenderer[HighlightSpots.Length];
        Highlight_renderers = new SpriteRenderer[HighlightSpots.Length];

        for (int i = 0; i < HighlightSpots.Length; i++)
        {
            GameObject GO = Instantiate(sprite_ref, Vector3.zero, Quaternion.identity, transform);
            renderers[i] = GO.GetComponent<SpriteRenderer>();
            Highlight_renderers[i] = GO.transform.GetChild(0).GetComponent<SpriteRenderer>();
            GO.transform.localPosition = (Vector2)HighlightSpots[i];
            Piece_color.a = 0.5f; //------------------------------------------------------------------ ONLY DIFFERENCE FROM PARENT CLASS
            renderers[i].color = Piece_color;
            renderers[i].sortingLayerName = "Floating";
        }
    }

    protected override bool ValidSpaceToPlace(Vector2Int pos)
    {
        return (pos.y < GridObj.gridHeight && pos.y >= 0);
    }
}
