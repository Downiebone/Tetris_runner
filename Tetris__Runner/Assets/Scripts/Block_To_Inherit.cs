using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block_To_Inherit : MonoBehaviour
{
    public virtual void PlaceThisBlock(Vector2Int gridPos, int itsRotation)
    {

    }
    public virtual void DestroyThisBlock()
    {
        Destroy(this.gameObject);
    }
}