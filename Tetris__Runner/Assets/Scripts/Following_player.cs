using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Following_player : MonoBehaviour
{
    public Transform player;

    public float x_offset;

    // Update is called once per frame
    void LateUpdate()
    {
        transform.position = new Vector3(player.transform.position.x + x_offset, transform.position.y, transform.position.z);
    }
}
