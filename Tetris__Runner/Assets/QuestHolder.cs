using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class QuestHolder : MonoBehaviour
{
    //the held quest by this holder
    public Quest MyQuest;


    public Image QuestSprite;
    public TMP_Text QuestName;
    public TMP_Text QuestDescription;
    public GameObject TogglePart;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void InitQuest(Quest questToBecome)
    {
        MyQuest = questToBecome;

        QuestSprite.sprite = questToBecome.QuestSprite;
        QuestName.text = questToBecome.name;
        QuestDescription.text = questToBecome.QuestDescription;

        //If quest must not be accepted, this disables the toggle button
        TogglePart.SetActive(questToBecome.MustBeAccepted);
    }

    public void ToggleQuest(bool ToggleON)
    {

    }
}
