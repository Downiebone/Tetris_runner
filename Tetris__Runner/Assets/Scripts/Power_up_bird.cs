using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Power_up_bird : Power_up_obj
{
    [SerializeField] private float bird_timer_length = 5;
    [SerializeField] private float added_time_per_upgrade = 2;
    public override void Activate(Vector2 power_up_btn_pos)
    {
        //give player a new draggable
        GameObject GO = Instantiate(press_button_effect, power_up_btn_pos, Quaternion.identity);

        GameObject.FindGameObjectWithTag("Player").GetComponent<Player_Script>().Bird_Power(bird_timer_length + added_time_per_upgrade * PlayerPrefs.GetInt("PowerUp_Bird"));

    }
}