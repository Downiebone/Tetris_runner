using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class death : MonoBehaviour
{
    [SerializeField] private float deathTimer = 1;
    void Start()
    {
        Destroy(this.gameObject, deathTimer);
    }
}
