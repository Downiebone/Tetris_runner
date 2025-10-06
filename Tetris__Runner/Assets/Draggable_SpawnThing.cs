using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Draggable_SpawnThing : draggable_piece
{
    [SerializeField] private GameObject prefab_to_spawn;

    [SerializeField] private SpriteRenderer highlight_renderer;


    [Header("Player launch")]
    [SerializeField] private float player_launch_rate = 3;

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
        return;
    }

    protected override void PlaceDraggable()
    {
        CameraObj.RemoveDraggable_atIndex(myDraggableIndex);

        Instantiate(prefab_to_spawn, (Vector2)LastWorkingPlaceSpot, Quaternion.identity);

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
