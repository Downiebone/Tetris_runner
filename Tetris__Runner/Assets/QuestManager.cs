using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

public class QuestManager : MonoBehaviour
{
    // Start is called before the first frame update

    public TMP_Text last_time;
    public TMP_Text now_time;
    void Start()
    {
        DateTime dtNow = DateTime.Now;

        Debug.Log("n: " + dtNow.ToString("HH:mm:ss dd MMMM, yyyy"));
        now_time.text = "Now: " + dtNow.ToString("HH:mm:ss dd MMMM, yyyy");

        if (!string.IsNullOrEmpty(PlayerPrefs.GetString("Time")))
        {
            DateTime lastTime = DateTime.FromBinary(Convert.ToInt64(PlayerPrefs.GetString("Time")));

            Debug.Log("t: " + lastTime.ToString("HH:mm:ss dd MMMM, yyyy"));
            last_time.text = "Then: " + lastTime.ToString("HH:mm:ss dd MMMM, yyyy");


            Debug.Log(dtNow.Subtract(lastTime).ToString(@"dd\:hh\:mm\:ss"));
            Debug.Log("hours gone: " + dtNow.Subtract(lastTime).TotalHours.ToString());
        }


        PlayerPrefs.SetString("Time", dtNow.ToBinary().ToString()); //must save after
        //maybe when doing the quests we save the current date/time plus lets say 4 hours
        //then save these seperatly for each quest slot (prob 3)
        //then we check to see if current time surpasses any of them at start

    }

    public string getCurrentTime()
    {


        return "";
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
