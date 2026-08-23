using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

public class QuestManager : MonoBehaviour
{
    //debug
    public TMP_Text last_time;
    public TMP_Text now_time;





    public static QuestManager Instance;

    private void Awake() //make object exist between scenes
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public int Quest1_ID = 0;
    public int Quest2_ID = 0;
    public int Quest3_ID = 0;

    public Quest[] AllExistingQuests;
    //Maybe split the up into teirs of differing rareties? Todo

    void Start()
    {
        InitQuests();

    }

    private void timeDebuging()
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
        //maybe when doing the quests we save the current date/time plus X hours
        //then save these seperatly for each quest slot (prob 3)
        //then we check to see if current time surpasses any of them at start
    }

    public void InitQuests()
    {
        Quest1_ID = PlayerPrefs.GetInt("Quest1ID");
        Quest2_ID = PlayerPrefs.GetInt("Quest2ID");
        Quest3_ID = PlayerPrefs.GetInt("Quest3ID");
        if (Quest1_ID == 0)
        {
            Quest1_ID = GenerateNewQuest();
        }
        if (Quest2_ID == 0)
        {
            Quest2_ID = GenerateNewQuest();
        }
        if (Quest3_ID == 0)
        {
            Quest3_ID = GenerateNewQuest();
        }

    }

    //gets a quest id that isnt on any of the other quests
    private int GenerateNewQuest()
    {
        //Check list of old quests

        //->

        // Get a new quest id from among all possible quests
        // But dont get a quest from among the last X Cleared/Skipped
        // Nor get a Quest from the currently active ones

        int getQuest = UnityEngine.Random.Range(0, AllExistingQuests.Length);

        while(AllExistingQuests[getQuest].QuestID == Quest1_ID ||
            AllExistingQuests[getQuest].QuestID == Quest2_ID ||
            AllExistingQuests[getQuest].QuestID == Quest3_ID)
        {
            getQuest++;
            if(getQuest >= AllExistingQuests.Length)
            {
                getQuest = 0;
            }
        }

        return AllExistingQuests[getQuest].QuestID;
    }

    private Quest FindQuestFromID(int id)
    {
        for (int i = 0; i < AllExistingQuests.Length; i++)
        {
            if(AllExistingQuests[i].QuestID == id)
            {
                return AllExistingQuests[i];
            }
        }

        return null;
    }

    public string getCurrentTime()
    {


        return "";
    }

    

    public Quest GetQuest(int one_to_three)
    {
        if(one_to_three == 1)
        {
            return FindQuestFromID(Quest1_ID);
        }
        if (one_to_three == 2)
        {
            return FindQuestFromID(Quest2_ID);
        }
        if (one_to_three == 3)
        {
            return FindQuestFromID(Quest3_ID);
        }

        return null;
    }
}
