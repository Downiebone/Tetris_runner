using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class level_play_system : MonoBehaviour
{
    // Start is called before the first frame update

    private int money_from_level = 0;

    [SerializeField] private move_to_position[] coins_for_animation;
    private int current_coint_ind = 0;
    private int max_coins = 0;

    [SerializeField] private TMP_Text money_text;

    public void Add_money()
    {
        money_from_level++;
        update_money_text();
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
