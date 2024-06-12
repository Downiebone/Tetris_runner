using UnityEngine;

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
    public SpriteRenderer sprite_rend;
    public Color cellColor;
}