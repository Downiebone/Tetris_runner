using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quest : ScriptableObject
{
    // this quest is not a passive
    public bool MustBeAccepted = false;

    public Sprite QuestSprite;

    public string QuestName;

    public string QuestDescription;

    
    //will probably allways need "MustBeAccepted" to be true, the passive ones should prob not affect the world
    public virtual void ApplyWorldEffect() { }

    //If the quest wants to save some sort of value or something, Called when you get the quest
    public virtual void JustGotNewQuest() { }

    //Should we just check the quest every so often? a second? at win? idk
    public virtual void CheckQuest() { }

    //If the quest wants to reset some sort of value or something, Called when you lose the quest
    public virtual void RemoveQuest() { }
}
