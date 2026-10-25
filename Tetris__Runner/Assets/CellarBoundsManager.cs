using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CellarCubeLayer
{

    public Vector2 TopLeft;
    public Vector2 TopRight;
    [Space]
    public Vector2 BotLeft;
    public Vector2 BotRight;

    public bool PointInArea(Vector2 TestPoint)
    {
        if(TestPoint.x < TopRight.x && TestPoint.x > TopLeft.x)
        {
            if (TestPoint.y < TopRight.y && TestPoint.y > BotRight.y)
            {
                return true;
            }
        }

        return false;
    } 

}

public class CellarBoundsManager : MonoBehaviour
{
    public CellarCubeLayer[] CellAreaPlaceMent;

    public float GetDistanceToMoveDownGroundObj(Vector2 object_pos, CellarPlacableObj CellarOb)
    {
        float minDistance = 0;

        if (CellarOb.KindaObject == CellarPlacableObj.ObjectType.PlacableOnFloor)
        {
            RaycastHit2D hit1 = Physics2D.Raycast(object_pos + CellarOb.CellarObjCorners.BotLeft, Vector2.down);
            RaycastHit2D hit2 = Physics2D.Raycast(object_pos + CellarOb.CellarObjCorners.BotRight, Vector2.down);

            if (hit1 && hit2)
            {
                minDistance = hit1.distance < hit2.distance ? hit1.distance : hit2.distance;
            }
            else if (hit1)
            {
                minDistance = hit1.distance;
            }
            else if (hit2)
            {
                minDistance = hit2.distance;
            }
        }

        return minDistance;
    }

    public bool IsObjectInBounds(Vector2 object_pos, CellarPlacableObj CellarOb)
    {
        Vector2[] ObjectsCorners = CellarOb.CellarObjCorners.GetArrayOfPositions();

        for (int i = 0; i < CellAreaPlaceMent.Length; i++)
        {
            for (int x = 0; x < ObjectsCorners.Length; x++)
            {
                if(CellAreaPlaceMent[i].PointInArea(object_pos + ObjectsCorners[x]) == false)
                {
                    return false;
                } 
            }
        }

        return true;
    }
}
