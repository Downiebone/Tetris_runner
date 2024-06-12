using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Touch_System : MonoBehaviour
{
    private bool currMovingObject = false;
    private Vector2 Touch_0_pos;
    private Vector2 Touch_1_pos;

    [Header("Touch Script")]

    [SerializeField] private LayerMask drag_layers;

    Camera main_cam;

    draggable_piece cur_dragObject;

    private void Start()
    {
        main_cam = Camera.main;
    }

    void Update()
    {
        if(Input.touchCount <= 0) { return; }

        Touch touch_0 = Input.GetTouch(0);

        Touch_0_pos = main_cam.ScreenToWorldPoint(touch_0.position);

        if(touch_0.phase == TouchPhase.Began)
        {
            Collider2D touched_drag_Collider = Physics2D.OverlapPoint(Touch_0_pos, drag_layers);

            if(touched_drag_Collider != null)
            {
                currMovingObject = true;
                cur_dragObject = touched_drag_Collider.gameObject.GetComponent<draggable_piece>();
                cur_dragObject.dragged_toggle(true);
            }
        }
        else if(touch_0.phase == TouchPhase.Moved)
        {
            if (currMovingObject)
            {
                cur_dragObject.dragged_position(Touch_0_pos);
            }
        }
        else if (touch_0.phase == TouchPhase.Ended)
        {
            if (currMovingObject)
            {
                currMovingObject = false;
                cur_dragObject.dragged_toggle(false);
                cur_dragObject = null;
            }
        }
    }
}
