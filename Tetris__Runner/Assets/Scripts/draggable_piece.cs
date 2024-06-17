using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class draggable_piece : MonoBehaviour
{
    protected Draggable_instantiater CameraObj;
    protected GridEditor GridObj;

    //private Transform player_transform;

    [HideInInspector] public bool isBeingDragged = false;

    [HideInInspector] public int myDraggableIndex;

    [SerializeField] protected float draggable_escapeVelocityFAST = 0.1f;

    [Tooltip("the first one in the list needs to be the 'middle'")]
    public Vector2Int[] HighlightSpots;

    private Rigidbody2D RB;
    void Start()
    {
        RB = GetComponent<Rigidbody2D>();
        //player_transform = GameObject.FindGameObjectWithTag("Player").transform;
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

        CameraObj.ResetHighlighter_Positions();
    }

    protected Vector2Int LastWorkingPlaceSpot;
    public void dragged_position(Vector2 newPosition)
    {
        transform.position = newPosition;
        Vector2Int newPos = Vector2Int.RoundToInt(newPosition);

        for (int i = 0; i < HighlightSpots.Length; i++)
        {
            if (ValidSpaceToPlace(HighlightSpots[i] + newPos) == false) { return; }
        }

        LastWorkingPlaceSpot = HighlightSpots[0] + newPos;

        for (int i = 0; i < HighlightSpots.Length; i++)
        {
            CameraObj.HighlightObjects[i].transform.position = (Vector2)(HighlightSpots[i] + newPos);
        }
    }

    protected virtual void PlaceDraggable()
    {

    }

    public virtual void touch_rotated()
    {
        transform.Rotate(new Vector3(0, 0, 90));
    }

    protected virtual bool ValidSpaceToPlace(Vector2Int pos)
    {
        return !(((Vector2)pos).y >= GridObj.gridHeight || GridObj.Cell_is_ground(pos));
    }

}
