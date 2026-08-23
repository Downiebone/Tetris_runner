using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LowgravityQuest : Quest
{
    //get a certain distance while low gravity

    public int WinDistance = 1000;

    public int Easy_WinDistance = 1000;
    public int Medium_WinDistance = 5000;
    public int Long_WinDistance = 10000;



    public override void ApplyWorldEffect()
    {
        //base.ApplyWorldEffect();

        //set low gravity
    }

    public override void CheckQuest()
    {
        //base.CheckQuest();

        //if distance is longer than win distance -- > win
        float playerX_pos = GameObject.FindGameObjectWithTag("Player").transform.position.x;

        if(playerX_pos > WinDistance)
        {
            CompleteQuest();
        }
    }

}
