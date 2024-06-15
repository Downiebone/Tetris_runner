using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class draggable_piece : MonoBehaviour
{
    private GameObject CameraObj;

    private Transform player_transform;

    [HideInInspector] public bool isBeingDragged = false;

    [HideInInspector] public int myDraggableIndex;

    [SerializeField] private float draggable_escapeVelocityFAST = 0.1f;

    private Rigidbody2D RB;
    void Start()
    {
        RB = GetComponent<Rigidbody2D>();
        player_transform = GameObject.FindGameObjectWithTag("Player").transform;
    }
    public void setCameraObj(GameObject CameraOb)
    {
        CameraObj = CameraOb;
    }

    private  float speed_change_threshhold = 0.001F;
    private Vector3 _position;
    private float _speed;
    private Vector3 _cameraPos;

    private float Draggable_Speed;
    private Vector3 Draggable_Velocity;

    public float slowDownTime = 0.5f;
    void FixedUpdate()
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
            Camera.main.GetComponent<Draggable_instantiater>().RemoveDraggable_atIndex(myDraggableIndex);
            gameObject.layer = 0;
            RB.bodyType = RigidbodyType2D.Dynamic;
            RB.velocity = new Vector2(Draggable_Velocity.x, Draggable_Velocity.y);
        }
    }
    public void dragged_position(Vector2 newPosition)
    {
        transform.position = newPosition;
    }

    private void PlaceDraggable()
    {

    }

    public virtual void touch_rotated()
    {
        transform.Rotate(new Vector3(0, 0, 90));
    }

}
