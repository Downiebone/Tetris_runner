using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Pause_Manager : MonoBehaviour
{

    public static bool GAME_IS_PAUSED = false;

    [SerializeField] private GameObject UI_pause_menu;
    [SerializeField] private level_play_system play_System;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Pause_Game()
    {
        GAME_IS_PAUSED = !GAME_IS_PAUSED;

        if (GAME_IS_PAUSED) { //just paused game
            MusicManager.Instance.dampen_music(0.3f);
        }
        else //just unpaused
        {
            MusicManager.Instance.unDamp_music();
        }

        UI_pause_menu.SetActive(GAME_IS_PAUSED);
    }
    public void Pause_Game_without_menu()
    {
        GAME_IS_PAUSED = !GAME_IS_PAUSED;

        if (GAME_IS_PAUSED)
        { //just paused game
            MusicManager.Instance.dampen_music(0.3f);
        }
        else //just unpaused
        {
            MusicManager.Instance.unDamp_music();
        }
    }

    public void Load_Menu()
    {
        play_System.save_moneys();

        GAME_IS_PAUSED = false;
        Resources.UnloadUnusedAssets();
        System.GC.Collect();

        SceneManager.LoadScene("Menu");
    }
}
