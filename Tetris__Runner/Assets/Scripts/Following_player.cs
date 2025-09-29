using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Following_player : MonoBehaviour
{
    public Transform player;

    public float x_offset;

    public float smoothSpeed = 5;

    private float y_pos;
    private float z_pos;
    private void Start()
    {
        y_pos = transform.position.y;
        z_pos = transform.position.z;
    }
    void LateUpdate()
    {
        //Vector3 smoothedPosition = Vector3.Lerp(transform.position, new Vector3(player.transform.position.x + x_offset, 5, -10), smoothSpeed * Time.deltaTime);
        //smoothedPosition.y = y_pos;
        //smoothedPosition.z = z_pos;
        //transform.position = smoothedPosition;


        transform.position = new Vector3(player.transform.position.x + x_offset, y_pos, z_pos);
    }
}
