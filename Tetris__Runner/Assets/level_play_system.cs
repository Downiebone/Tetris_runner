using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class level_play_system : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private Pause_Manager pause_manage;
    [SerializeField] private Player_Script player_scrip;

    private int money_from_level = 0;

    [SerializeField] private move_to_position[] coins_for_animation;
    private int current_coint_ind = 0;
    private int max_coins = 0;

    [SerializeField] private TMP_Text money_text;
    [SerializeField] private TMP_Text end_of_level_money_text;

    [SerializeField] private GameObject end_of_level_screen;

    [SerializeField] private TMP_Text revive_text;
    private int Number_Of_Revives = 1;
    public bool has_revives()
    {
        return Number_Of_Revives > 0;
    }
    private void update_revive_text()
    {
        revive_text.text = Number_Of_Revives.ToString() + "x";
    }
    public void Remove_Revive()
    {
        Number_Of_Revives--;
        update_revive_text();
    }
    public void Add_Revive()
    {
        Number_Of_Revives++;
        update_revive_text();
    }
    public void player_died_collectionFunc()
    {
        pause_manage.Pause_Game_without_menu(); // we want to pause the game, but not show the pause-screen

        end_of_level_money_text.text = money_from_level.ToString() + "x";

        end_of_level_screen.SetActive(true);
    }

    public void Revive_Player_After_AD()
    {
        pause_manage.Pause_Game_without_menu(); // unpause game

        end_of_level_screen.SetActive(false);

        player_scrip.Revived();
    }

    public void save_moneys()
    {
        PlayerPrefs.SetInt("Money", PlayerPrefs.GetInt("Money") + money_from_level); // add money to playerpref
    }

    [SerializeField] private PowerUp_manager power_up_manager;

    public void Add_money()
    {
        money_from_level++;
        update_money_text();

        power_up_manager.add_coin_forPowerUp(1);
    }
    private void update_money_text()
    {
        //shake the counter a little :)
        money_text.text = money_from_level.ToString() + "x";
    }

    void Start()
    {
        max_coins = coins_for_animation.Length;

        update_money_text();
        update_revive_text();
    }

    public void spawn_coin(Vector2 start_pos, Vector2 end_pos)
    {
        Add_money();
        coins_for_animation[current_coint_ind].dotween(start_pos, end_pos, 0.5f);
        current_coint_ind++;
        if(current_coint_ind > max_coins)
        {
            current_coint_ind = 0;
        }
    }

    public void spawn_coin_referance(Vector2 start_pos, Transform end_pos)
    {
        Add_money();
        coins_for_animation[current_coint_ind].dotween_referance(start_pos, end_pos, 0.25f);
        current_coint_ind++;
        if (current_coint_ind >= max_coins)
        {
            current_coint_ind = 0;
        }
    }
}
