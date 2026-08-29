using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quest_UI_Handler : MonoBehaviour
{

    public QuestHolder Quest1Holder;
    public QuestHolder Quest2Holder;
    public QuestHolder Quest3Holder;

    private void OnEnable()
    {
        Quest1Holder.InitQuest(QuestManager.Instance.GetQuest(1));
        Quest1Holder.InitQuest(QuestManager.Instance.GetQuest(2));
        Quest1Holder.InitQuest(QuestManager.Instance.GetQuest(3));
    }


    public void ExitQuestMenu()
    {
        this.gameObject.SetActive(false);
    }
}
