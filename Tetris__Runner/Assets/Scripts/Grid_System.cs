using UnityEngine;
using System;
using SerializableTypes;

[Serializable]
public class Cell
{
    public enum Cell_type
    {
        Ground = 0,
        special_1 = 1,
        special_2 = 2,
        special_3 = 3
    }

    public bool isActive = false;
    public Cell_type type = Cell_type.Ground;
    //public int colorIndex = 0;
    [NonSerialized] public SpriteRenderer sprite_rend;
    [NonSerialized] public Color cellColor;
    public SColor saved_cellColor;

    public int Width;
    public int Height;

    public void convertToSavableColor()
    {
        saved_cellColor = cellColor;
    }
}