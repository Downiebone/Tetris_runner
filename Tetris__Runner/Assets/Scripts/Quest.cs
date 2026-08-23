using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Playerprefs to save about Quests:
//************************************

// The ID's of active Quests

// The ID's of past X Quests (Completed and? Skipped)

// The Difficulity of each quest
//      - to change values like 'how far the player must play to complete the quest'
//      - More difficulity = More Reward

// If you need to wait for a quest, Like what the date atleast needs to be in order to get a quest in this slot
//      - When we complete a quest we save the date on that slot + X hours.
//          (Can watch ads to get quests, or buy unlimited quests)




public class Quest : ScriptableObject
{
    // this quest is not a passive
    public bool MustBeAccepted = false;

    // is this quest affected by another value that should correspond to the quests difficulity
    public bool AffectedByDifficulity = true;

    public Sprite QuestSprite;

    public string QuestName;

    //Each quest should have an ID, and maybe save inte some sort of queue thing
    //maybe save the last several quests you did, so you dont have to do similar ones
    public int QuestID;

    //what you need to do to complete the quest
    public string QuestDescription;

    //prob like one to three. Like how rare a quest is to appear
    //lower number = more common, 1 = most common
    public int QuestRarity = 1;

    //Gain reward based on quest difficulity
    //also one to three
    public int QuestDifficulity = 1;

    public int Quest_Easy_Difficulity_reward = 100;
    public int Quest_Medium_Difficulity_reward = 200;
    public int Quest_Hard_Difficulity_reward = 300;


    //will probably allways need "MustBeAccepted" to be true, the passive ones should prob not affect the world
    //but should still be called for each quest when game begins. -> the quests themselfes can decide if they dont do anything
    public virtual void ApplyWorldEffect() { }

    //If the quest wants to save some sort of value or something, Called when you get the quest
    public virtual void JustGotNewQuest() { }

    //Should we just check the quest every so often? a second? at win? idk
    public virtual void CheckQuest() { }

    // Completed the quest
    public virtual void CompleteQuest() { 
        //check IDs of all current quests
        
        //Find this quest --> remove it
        //give player the reward

        //tell quest manager to make new quest?

    }

    //If the quest wants to reset some sort of value or something, Called when you lose the quest
    public virtual void ResetQuest() { 
        
    }
}
