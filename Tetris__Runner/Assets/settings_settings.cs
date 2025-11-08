using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class settings_settings : MonoBehaviour
{

    public TMP_Text x_text;
    public TMP_Text y_text;

    public Slider slider_effect;
    public Slider slider_music;

    private float x_val = 0;
    private float y_val = 0;

    private float music_vol = 1;
    private float effect_vol = 1;

        

    private void OnEnable()
    {
        x_val = PlayerPrefs.GetFloat("X_off");
        y_val = PlayerPrefs.GetFloat("Y_off");

        slider_effect.value = PlayerPrefs.GetFloat("Sound_Effects");
        slider_music.value = PlayerPrefs.GetFloat("Sound_Music");

        x_text.text = "X: " + x_val.ToString("#0.00");
        y_text.text = "Y: " + y_val.ToString("#0.00");
    }

    public void set_sound_effect(float change)
    {
        effect_vol = change;
        PlayerPrefs.SetFloat("Sound_Effects", change);
    }

    public void set_sound_music(float change)
    {
        music_vol = change;
        PlayerPrefs.SetFloat("Sound_Music", change);

        if(MusicManager.Instance != null)
        {
            MusicManager.Instance.update_max_volume(music_vol);
        }
    }

    public void Increase_x()
    {
        x_val += 0.25f;
        PlayerPrefs.SetFloat("X_off", x_val);

        x_text.text = "X: " + x_val.ToString("#0.00");
    }
    public void Decrease_x()
    {
        x_val -= 0.25f;
        PlayerPrefs.SetFloat("X_off", x_val);

        x_text.text = "X: " + x_val.ToString("#0.00");
    }
    public void Increase_y()
    {
        y_val += 0.25f;
        PlayerPrefs.SetFloat("Y_off", y_val);

        y_text.text = "Y: " + y_val.ToString("#0.00");
    }
    public void Decrease_y()
    {
        y_val -= 0.25f;
        PlayerPrefs.SetFloat("Y_off", y_val);

        y_text.text = "Y: " + y_val.ToString("#0.00");
    }
}
