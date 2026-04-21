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

    [SerializeField] private GameObject cam;
    [SerializeField] private float shake = 0;
    [SerializeField] private float Maxshake = 0;
    [SerializeField] private float shakeAmount = 0.7f;
    [SerializeField] private float decreaseFactor = 1.0f;

    public void ShakeStart(float shakeing = 1, float shakePower = 0.7f)
    {
        shakeAmount = shakePower;
        shake = shakeing;
        Maxshake = shakeing;
    }

    void Update()
    {
        if (shake > 0)
        {
            cam.transform.localPosition = Random.insideUnitCircle * shakeAmount * (shake/Maxshake);
            shake -= Time.deltaTime * decreaseFactor;

        }
        else
        {
            cam.transform.localPosition = new Vector3(0, 0, 0);
            shake = 0.0f;
        }
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
