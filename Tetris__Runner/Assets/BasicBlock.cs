using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicBlock : MonoBehaviour
{
    GameObject GO = null;

    //right now it is only for spawning special blocks
    //but maybe this should do more later?
    public void PlacedBlock(Vector2Int gridPos, int itsRotation, bool shoudSpawnSeparateObj, GameObject objToSpawn)
    {
        if (!shoudSpawnSeparateObj)
        {
            return;
        }

        GO = Instantiate(objToSpawn, new Vector3(gridPos.x, gridPos.y, 0) , Quaternion.identity);
        GO.GetComponent<Block_To_Inherit>().PlaceThisBlock(gridPos, itsRotation);
    }

    public void DestroyPlacedBlock()
    {
        if(GO != null)
        {
            GO.GetComponent<Block_To_Inherit>().DestroyThisBlock();
        }
    }
}
