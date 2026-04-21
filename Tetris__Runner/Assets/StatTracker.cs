using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatTracker : MonoBehaviour
{
    public int Blocks_Travaled = 0;
    public int Blocks_Placed = 0;
    public int Bombs_Placed = 0;
    public int Ults_Used = 0;
    public int Coins_Collected = 0;

    [SerializeField] private Transform playerTrans;

    public void SaveStats()
    {
        if(Blocks_Travaled > PlayerPrefs.GetInt("Record"))
        {
            //set our record?
            PlayerPrefs.SetInt("Record", Blocks_Travaled);
        }

        //Global stats? Might be fun to save. For the player to look at later
        PlayerPrefs.SetInt("Traveled", PlayerPrefs.GetInt("Traveled") + Blocks_Travaled);
        PlayerPrefs.SetInt("Blocks_Placed", PlayerPrefs.GetInt("Blocks_Placed") + Blocks_Placed);
        PlayerPrefs.SetInt("Bombs_Placed", PlayerPrefs.GetInt("Bombs_Placed") + Bombs_Placed);
        PlayerPrefs.SetInt("Ults_Used", PlayerPrefs.GetInt("Ults_Used") + Ults_Used);
        PlayerPrefs.SetInt("Coins_Collected", PlayerPrefs.GetInt("Coins_Collected") + Coins_Collected);
    }

    void FixedUpdate()
    {
        Blocks_Travaled = (int)playerTrans.position.x;
    }
}
