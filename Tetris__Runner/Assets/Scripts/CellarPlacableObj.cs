using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CellarObjectsCorners
{
    public Vector2 TopLeft;
    public Vector2 TopRight;
    [Space]
    public Vector2 BotLeft;
    public Vector2 BotRight;

    public Vector2[] GetArrayOfPositions()
    {
        Vector2[] retArr = new Vector2[4];
        retArr[0] = TopLeft;
        retArr[1] = TopRight;
        retArr[2] = BotLeft;
        retArr[3] = BotRight;

        return retArr;
    }
}

[CreateAssetMenu]
public class CellarPlacableObj : ScriptableObject
{
    public enum ObjectType
    {
        PlacableOnFloor = 0,
        PlaceableOnWall = 1,
        Wallpaper = 2
    }


    [SerializeField] protected string playerPrefName;
    public string getPlayerPrefName() { return playerPrefName; }

    public Sprite[] SpriteAndVariance;

    public GameObject PrefabToUse;

    public ObjectType KindaObject;

    public int CellarPrize;

    public CellarObjectsCorners CellarObjCorners;

    //public virtual void Activate(Vector2 power_up_btn_pos) { }
}