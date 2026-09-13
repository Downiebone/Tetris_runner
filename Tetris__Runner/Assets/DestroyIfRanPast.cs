using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyIfRanPast : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField] private float DistanceToCountAsOffScreen = 5;

    private Transform playerTrans;

    void Start()
    {
        playerTrans = GameObject.FindGameObjectWithTag("Player").transform;
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        if(transform.position.x < playerTrans.position.x - DistanceToCountAsOffScreen)
        {
            Destroy(this.gameObject);
        }
    }
}
