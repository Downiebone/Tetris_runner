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
    private Vector2 origin_drag_pos;
    private bool reached_far_enough_distance = false;
    [SerializeField] private float far_enough_distance = 1;

    [SerializeField] private Draggable_instantiater drag_inster_SYSTEM;

    [SerializeField] private Transform UI_rotate_trash_btn;
    [SerializeField] private SpriteRenderer UI_rend;
    [SerializeField] private float distance_to_become_trashcan = 3;
    [SerializeField] private float distance_to_be_put_in_trash = 1;
    [SerializeField] private Sprite rotate_sprite;
    [SerializeField] private Sprite trash_sprite;

    

    private void Start()
    {
        main_cam = Camera.main;
    }

    void Update()
    {

#if UNITY_EDITOR

        Vector2 screenPosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        Vector2 worldPosition = main_cam.ScreenToWorldPoint(screenPosition);

        Collider2D touched_drag_Collider = Physics2D.OverlapPoint(worldPosition, drag_layers);

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            //Debug.Log("Start drag system");

            if (touched_drag_Collider != null)
            {
                

                currMovingObject = true;
                cur_dragObject = touched_drag_Collider.gameObject.GetComponent<draggable_piece>();
                drag_inster_SYSTEM.set_currently_highlighted_draggable(cur_dragObject);
                cur_dragObject.BeginDrag();
                origin_drag_pos = new Vector2(cur_dragObject.transform.position.x, cur_dragObject.transform.position.y);
                reached_far_enough_distance = false;
            }
            else
            {
                if(drag_inster_SYSTEM.Get_currently_highlighted_Draggable() != null)
                {
                    currMovingObject = true;
                    cur_dragObject = drag_inster_SYSTEM.Get_currently_highlighted_Draggable();
                    cur_dragObject.BeginDrag();
                    origin_drag_pos = new Vector2(cur_dragObject.transform.position.x, cur_dragObject.transform.position.y);
                    reached_far_enough_distance = false;
                }
            }
        }
        if (cur_dragObject == null) { return; }
        if (Input.GetKey(KeyCode.Mouse0))
        {
            if (currMovingObject)
            {
                if (reached_far_enough_distance)
                {
                    if(Vector2.Distance(worldPosition, UI_rotate_trash_btn.position) < distance_to_become_trashcan)
                    {
                        UI_rend.sprite = trash_sprite;

                        if (Vector2.Distance(worldPosition, UI_rotate_trash_btn.position) < distance_to_be_put_in_trash)
                        {
                            //make block red or something???
                            //or enlarge the trashcan
                        }
                    }
                    else
                    {
                        UI_rend.sprite = rotate_sprite;
                    }

                    cur_dragObject.dragged_position(worldPosition);
                }
                else
                {
                    if(Vector2.Distance(worldPosition, origin_drag_pos) > far_enough_distance)
                    {
                        reached_far_enough_distance = true;
                    }
                }
                
            }
        }
        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            if (currMovingObject)
            {
                if (Vector2.Distance(worldPosition, UI_rotate_trash_btn.position) < distance_to_be_put_in_trash) //close enough to trash can
                {
                    cur_dragObject.destroy_piece();
                }
                else
                {
                    cur_dragObject.EndDrag();
                }

                currMovingObject = false;
                cur_dragObject = null;

                UI_rend.sprite = rotate_sprite;

                
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
