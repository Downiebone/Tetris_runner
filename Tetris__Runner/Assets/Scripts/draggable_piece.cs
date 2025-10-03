using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class draggable_piece : MonoBehaviour
{

    [SerializeField] protected GameObject sprite_ref;

    protected Draggable_instantiater CameraObj;
    protected GridEditor GridObj;

    protected bool is_chosen = false;

    protected bool to_far_to_place = true;

    [SerializeField] protected float floating_scale = 0.75f;
    [SerializeField] protected float selected_scale = 0.75f;

    protected float current_scale = 1f;

    [SerializeField] protected float hovering_Visibility = 0.5f;

    protected SpriteRenderer[] renderers;

    private SpriteRenderer[] Highlight_renderers;


    //private Transform player_transform;

    [HideInInspector] public bool isBeingDragged = false;

    [HideInInspector] public int myDraggableIndex;

    [SerializeField] protected float draggable_escapeVelocityFAST = 12f;

    [Tooltip("the first one in the list needs to be the 'middle'")]
    public Vector2Int[] HighlightSpots;

    protected Rigidbody2D RB;

    protected Color Piece_color;

    public bool delayed_start_highlight = false;

    protected virtual void Start()
    {
        transform.localScale = new Vector3(floating_scale, floating_scale, 1);
        current_scale = floating_scale;

        RB = GetComponent<Rigidbody2D>();
        Piece_color = GridObj.GainRandomColor(); // get random color out of the ones chosen in grid-system
        spawn_sprites();
        //player_transform = GameObject.FindGameObjectWithTag("Player").transform;
        if(delayed_start_highlight)
        {
            Highlight();
        }
    }

    protected virtual void spawn_sprites()
    {
        renderers = new SpriteRenderer[HighlightSpots.Length];
        Highlight_renderers = new SpriteRenderer[HighlightSpots.Length];

        for (int i = 0; i < HighlightSpots.Length; i++)
        {
            GameObject GO = Instantiate(sprite_ref, Vector3.zero, Quaternion.identity, transform);
            renderers[i] = GO.GetComponent<SpriteRenderer>();
            Highlight_renderers[i] = GO.transform.GetChild(0).GetComponent<SpriteRenderer>();
            GO.transform.localPosition = (Vector2)HighlightSpots[i];
            renderers[i].color = Piece_color;
            renderers[i].sortingLayerName = "Floating";
        }
    }



    public void setReferences(Draggable_instantiater CameraOb, GridEditor GridOb)
    {
        CameraObj = CameraOb;
        GridObj = GridOb;
    }

    //private  float speed_change_threshhold = 0.001F;
    protected Vector3 _position;
    protected float _speed;
    protected Vector3 _cameraPos;

    protected float Draggable_Speed;
    protected Vector3 Draggable_Velocity;

    public float slowDownTime = 0.5f;

    protected Vector2Int LastWorkingPlaceSpot;
    protected Vector2Int LastTestedPlaceSpot;
    protected void FixedUpdate()
    {
        Vector3 newPos = transform.position;
        Vector3 newCameraPos = CameraObj.transform.position;

        Draggable_Velocity = (newPos - (_position + (newCameraPos - _cameraPos))) / Time.fixedUnscaledDeltaTime;

        Draggable_Speed = Draggable_Velocity.magnitude;
        //Draggable_Speed = Vector3.Distance(newPos, _position + (newCameraPos - _cameraPos)) / Time.fixedDeltaTime;

        if ((Draggable_Speed == 0 && _speed != 0) || Mathf.Abs(Draggable_Speed - _speed) > 0.001f)
        {
            _speed = Draggable_Speed;
            //newSpeed is its speed
        }
        _position = newPos;
        _cameraPos = CameraObj.transform.position;

    }
    public void BeginDrag()
    {
        isBeingDragged = true;
        Time.timeScale = slowDownTime;

        foreach (SpriteRenderer rend in renderers)
        {
            Color tmp = rend.color;
            tmp.a = hovering_Visibility;
            rend.color = tmp;
        }

        transform.localScale = new Vector3(floating_scale, floating_scale, 1);
        if (is_chosen)
        {
            disable_highlight();
        }

        LastTestedPlaceSpot = Vector2Int.RoundToInt(transform.position);
        LastWorkingPlaceSpot = new Vector2Int(-42, -42); // at a spot so that it should always be to far away
    } 

    [SerializeField] protected float toFarForPlace = 3f;
    public void EndDrag()
    {
        isBeingDragged = false;
        Time.timeScale = 1;

        //transform.localScale = new Vector3(current_scale, current_scale, 1);
        if (is_chosen)
        {
            enable_highlight();
        }

        foreach (SpriteRenderer rend in renderers)
        {
            Color tmp = rend.color;
            tmp.a = 1f;
            rend.color = tmp;
        }

        if (to_far_to_place == false) //meaning it is close enough
        {
            PlaceDraggable();
        }
        //if (Draggable_Speed > draggable_escapeVelocityFAST)
        //{
        //    //add another speed that only counts if object is away from a placable spot

        //    //if we drag object to fast, remove it from list
        //    //Debug.Log("ESCAPE SPEED : " + Draggable_Speed);
        //    CameraObj.RemoveDraggable_atIndex(myDraggableIndex);
        //    gameObject.layer = 0;
        //    RB.bodyType = RigidbodyType2D.Dynamic;
        //    RB.velocity = new Vector2(Draggable_Velocity.x, Draggable_Velocity.y);
        //}
        //else
        //{
        //    if (Vector2.Distance(LastTestedPlaceSpot, LastWorkingPlaceSpot) < toFarForPlace)
        //    {
        //        PlaceDraggable();
        //    }
        //}

        CameraObj.ResetHighlighter_Positions();
    }

    
    public void dragged_position(Vector2 newPosition)
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

        for (int i = 0; i < HighlightSpots.Length; i++)
        {
            if (ValidSpaceToPlace(HighlightSpots[i] + newPos) == false) { failed_pos = true; }
        }
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
            if(is_x_val)
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
    
    protected virtual void PlaceDraggable()
    {
        

        for (int i = 0; i < HighlightSpots.Length; i++)
        {
            GridObj.placeTile(new Vector2Int((int)CameraObj.HighlightObjects[i].transform.position.x, (int)CameraObj.HighlightObjects[i].transform.position.y), Piece_color);

            //change to ground layer (this should not matter as they are being destroyed this frame??)
            renderers[i].sortingLayerName = "Ground";
        }

        CameraObj.RemoveDraggable_atIndex(myDraggableIndex);

        Destroy(this.gameObject);
        //instantiate place-effect or some thing
    }

    public virtual void touch_rotated()
    {
        for (int i = 0; i < HighlightSpots.Length; i++)
        {
            HighlightSpots[i] = new Vector2Int(HighlightSpots[i].y, -HighlightSpots[i].x);
            renderers[i].transform.localPosition = (Vector2)HighlightSpots[i];
        }
    }

    protected virtual bool ValidSpaceToPlace(Vector2Int pos)
    {

        return (pos.y < GridObj.gridHeight && pos.y >= 0) && !GridObj.Cell_is_active_type(pos, Cell.Cell_type.Ground);



        //return false;

        //return !(((Vector2)pos).y >= GridObj.gridHeight || GridObj.Cell_is_ground(pos));
    }

    public void Un_highlight()
    {
        disable_highlight();
        is_chosen = false;
        //transform.localScale = new Vector3(floating_scale, floating_scale, 1);
        //current_scale = floating_scale;
    }

    public void Highlight()
    {
        enable_highlight();
        is_chosen = true;
        //transform.localScale = new Vector3(selected_scale, selected_scale, 1);
        //current_scale = selected_scale;
    }

    protected virtual void enable_highlight()
    {
        foreach (SpriteRenderer rend in Highlight_renderers)
        {
            rend.enabled = true;
        }
    }
    protected virtual void disable_highlight()
    {
        foreach (SpriteRenderer rend in Highlight_renderers)
        {
            rend.enabled = false;
        }
    }

    public void destroy_piece()
    {
        Time.timeScale = 1;

        for (int i = 0; i < HighlightSpots.Length; i++)
        {
            CameraObj.HighlightObjects[i].GetComponent<SpriteRenderer>().color = Color.white;
        }

        CameraObj.RemoveDraggable_atIndex(myDraggableIndex);

        CameraObj.ResetHighlighter_Positions();

        Destroy(this.gameObject);
    }

}
