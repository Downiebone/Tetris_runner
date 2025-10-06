using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Power_up_goldenRing : Power_up_obj
{

    [SerializeField] private GameObject Golden_Ring_Prefab_draggable;
    public override void Activate(Vector2 power_up_btn_pos)
    {
        //give player a new draggable
        GameObject GO = Instantiate(Golden_Ring_Prefab_draggable, power_up_btn_pos, Quaternion.identity);

        Draggable_instantiater di = Camera.main.GetComponent<Draggable_instantiater>();
        GridEditor ge = GameObject.FindGameObjectWithTag("Grid").GetComponent<GridEditor>();
        draggable_piece dp = GO.GetComponent<draggable_piece>();

        dp.setReferences(di, ge);

        di.replace_draggable_with_other(dp);

    }
}