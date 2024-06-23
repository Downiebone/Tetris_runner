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
#if true

    Vector2 screenPosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
    Vector2 worldPosition = Camera.main.ScreenToWorldPoint(screenPosition);

    Collider2D touched_drag_Collider = Physics2D.OverlapPoint(worldPosition, drag_layers);

    if (Input.GetKeyDown(KeyCode.Mouse0))
    {
        Debug.Log("Start drag system");

        if (touched_drag_Collider != null)
        {
            currMovingObject = true;
            cur_dragObject = touched_drag_Collider.gameObject.GetComponent<draggable_piece>();
            cur_dragObject.BeginDrag();
        }
    }
    if(cur_dragObject == null) { return; }
    if (Input.GetKey(KeyCode.Mouse0))
    {
        if (currMovingObject)
        {
            cur_dragObject.dragged_position(worldPosition);
        }
    }
    if (Input.GetKeyUp(KeyCode.Mouse0))
    {
        if (currMovingObject)
        {
            currMovingObject = false;
            cur_dragObject.EndDrag();
            cur_dragObject = null;
        }
    }

    //touch 2 stuff

    if (Input.GetKeyDown(KeyCode.Mouse1))
    {
        cur_dragObject.touch_rotated();
    }

#else
    if (Input.touchCount <= 0) { return; }

    Touch touch_0 = Input.GetTouch(0);

    Touch_0_pos = main_cam.ScreenToWorldPoint(touch_0.position);

    if (touch_0.phase == TouchPhase.Began)
    {
        Collider2D touched_drag_Collider = Physics2D.OverlapPoint(Touch_0_pos, drag_layers);

        if (touched_drag_Collider != null)
        {
            currMovingObject = true;
            cur_dragObject = touched_drag_Collider.gameObject.GetComponent<draggable_piece>();
            cur_dragObject.BeginDrag();
        }
    }
    if(cur_dragObject == null) { return; }
    if (touch_0.phase == TouchPhase.Moved || touch_0.phase == TouchPhase.Stationary)
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
            cur_dragObject.EndDrag();
            cur_dragObject = null;
        }
    }

    //touch 2 stuff
    if (cur_dragObject == null) { return; } //can only rotate if we are holding object

    Touch touch_1 = Input.GetTouch(1);

    if (touch_1.phase == TouchPhase.Began)
    {
        cur_dragObject.touch_rotated();
    }
#endif
    }
}
