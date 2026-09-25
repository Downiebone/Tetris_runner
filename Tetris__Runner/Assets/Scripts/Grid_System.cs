using UnityEngine;
using System;
using SerializableTypes;

public static class Constants
{
    public const int Max_colors_indexes = 5; //0,1,2,3,4
}

[Serializable]
public class Cell
{
    public enum Cell_type
    {
        Ground = 0,
        collectable_coin = 1,
        collectable_big = 2,
        Jumppad = 3,
        Spike = 4,
        InvisibleDanger = 5, //for like laser maybe?
        LaserBlock = 6
    }

    /// <summary>
    /// How sprite should be oriented.
    /// 0 = Normal (Up)
    /// 1 = Right,
    /// 2 = Down,
    /// 3 = Left
    /// </summary>
    public int Rotation = 0;

    public bool isActive = false;
    public Cell_type type = Cell_type.Ground;
    //public int colorIndex = 0;
    [NonSerialized] public SpriteRenderer sprite_rend;
    [NonSerialized] public BasicBlock BasicBlockScript;
    [NonSerialized] public Color cellColor;
    public SColor saved_cellColor;
    //5 colors??
    public int color_index = 0; //color 0,1,2,3,4 --> to be adapted during gameplay

    public int Width;
    public int Height;

    public void convertToSavableColor()
    {
        saved_cellColor = cellColor;
    }

    public void copyCell(Cell old_cell)
    {

        isActive = old_cell.isActive;

        type = old_cell.type;

        cellColor = old_cell.cellColor;
        color_index = old_cell.color_index;

    }
}