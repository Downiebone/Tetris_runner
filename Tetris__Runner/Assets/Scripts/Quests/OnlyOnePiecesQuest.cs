using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class OnlyOnePiecesQuest : Quest
{
    [Tooltip("0: L, 1: Penis, 2: Squigly, 3: Line, 4: Bomb")] public int[] PiecesToHave;

    public bool Randomize_Piece = false;
    public override void ApplyWorldEffect()
    {
        base.ApplyWorldEffect();

        if(Randomize_Piece == true)
        {
            PiecesToHave = new int[] { Random.Range(0, 5) }; // 0 - 4 (5 is excluded for integer randomness)
        }

        GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Draggable_instantiater>().ModifyDraggableList(PiecesToHave);
    }


}
