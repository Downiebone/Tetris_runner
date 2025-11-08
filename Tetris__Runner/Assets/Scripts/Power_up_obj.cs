using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Power_up_obj : ScriptableObject
{
    // Start is called before the first frame update
    public int coins_for_using = 20;

    public Sprite power_up_sprite;

    public AudioClip ActivateSound;

    [SerializeField] protected GameObject press_button_effect;
    public virtual void Activate(Vector2 power_up_btn_pos) {  }
}
