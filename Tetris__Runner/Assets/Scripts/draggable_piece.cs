using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class draggable_piece : MonoBehaviour
{
    [SerializeField] protected GameObject sprite_ref;

    protected Draggable_instantiater CameraObj;
    protected GridEditor GridObj;


    //private Transform player_transform;

    [HideInInspector] public bool isBeingDragged = false;

    [HideInInspector] public int myDraggableIndex;

    [SerializeField] protected float draggable_escapeVelocityFAST = 20f;

    [Tooltip("the first one in the list needs to be the 'middle'")]
    public Vector2Int[] HighlightSpots;

    protected Rigidbody2D RB;

    protected Color Piece_color;

    protected virtual void Start()
    {
        RB = GetComponent<Rigidbody2D>();
        Piece_color = GridObj.GainRandomColor(); // get random color out of the ones chosen in grid-system
        spawn_sprites();
        //player_transform = GameObject.FindGameObjectWithTag("Player").transform;
    }
    protected void spawn_sprites()
    {
        for (int i = 0; i < HighlightSpots.Length; i++)
        {
            GameObject GO = Instantiate(sprite_ref, Vector3.zero, Quaternion.identity, transform);
            GO.transform.localPosition = (Vector2)HighlightSpots[i];
            GO.GetComponent<SpriteRenderer>().color = Piece_color;
            GO.GetComponent<SpriteRenderer>().sortingLayerName = "Floating";
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
    }

    [SerializeField] protected float toFarForPlace = 3f;
    public void EndDrag()
    {
        isBeingDragged = false;
        Time.timeScale = 1;

        if (Draggable_Speed > draggable_escapeVelocityFAST)
        {
            //add another speed that only counts if object is away from a placable spot

            //if we drag object to fast, remove it from list
            //Debug.Log("ESCAPE SPEED : " + Draggable_Speed);
            CameraObj.RemoveDraggable_atIndex(myDraggableIndex);
            gameObject.layer = 0;
            RB.bodyType = RigidbodyType2D.Dynamic;
            RB.velocity = new Vector2(Draggable_Velocity.x, Draggable_Velocity.y);
        }
        else
        {
            if (Vector2.Distance(LastTestedPlaceSpot, LastWorkingPlaceSpot) < toFarForPlace)
            {
                PlaceDraggable();
            }
        }

        CameraObj.ResetHighlighter_Positions();
    }

    protected Vector2Int LastWorkingPlaceSpot;
    protected Vector2Int LastTestedPlaceSpot;
    public void dragged_position(Vector2 newPosition)
    {
        transform.position = newPosition;
        Vector2Int newPos = Vector2Int.RoundToInt(newPosition);

        LastTestedPlaceSpot = newPos;

        for (int i = 0; i < HighlightSpots.Length; i++)
        {
            if (ValidSpaceToPlace(HighlightSpots[i] + newPos) == false) { return; }
        }

        LastWorkingPlaceSpot = newPos;

        for (int i = 0; i < HighlightSpots.Length; i++)
        {
            CameraObj.HighlightObjects[i].transform.position = (Vector2)(HighlightSpots[i] + newPos);
        }
    }
    
    protected virtual void PlaceDraggable()
    {
        CameraObj.RemoveDraggable_atIndex(myDraggableIndex);

        for (int i = 0; i < HighlightSpots.Length; i++)
        {
            GridObj.placeTile(LastWorkingPlaceSpot + HighlightSpots[i], Piece_color);
        }

        Destroy(this.gameObject);
        //instantiate place-effect or some thing
    }

    public virtual void touch_rotated()
    {
        LastWorkingPlaceSpot = new Vector2Int(-10, -10);

        for (int i = 0; i < HighlightSpots.Length; i++)
        {

            HighlightSpots[i] = new Vector2Int(HighlightSpots[i].y, -HighlightSpots[i].x);
            transform.GetChild(i).transform.localPosition = (Vector2)HighlightSpots[i];
        }
    }

    protected virtual bool ValidSpaceToPlace(Vector2Int pos)
    {
        return !(((Vector2)pos).y >= GridObj.gridHeight || GridObj.Cell_is_ground(pos));
    }

}
