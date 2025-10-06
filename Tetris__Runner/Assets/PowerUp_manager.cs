using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp_manager : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField] private SpriteRenderer PowerUp_spot_sprite;
    [SerializeField] private SpriteRenderer PowerUp_holder_cooldown;

    [SerializeField] private Sprite[] Holder_sprites;

    [SerializeField] private int max_coins_for_PowerUp = 20;
    [SerializeField] private int current_coins_for_PowerUp = 0;

    [SerializeField] private Power_up_obj Selected_power_up;

    public void Use_PowerUp()
    {
        if(current_coins_for_PowerUp < max_coins_for_PowerUp) { return; } //cant activate yet

        Selected_power_up.Activate(new Vector2(1, 1));
        current_coins_for_PowerUp = 0;

        determine_current_holder_percent();
    }

    void Start()
    {
        determine_current_holder_percent();
    }

    public void add_coin_forPowerUp(int coins)
    {
        if(current_coins_for_PowerUp >= max_coins_for_PowerUp) { return; } //full already

        current_coins_for_PowerUp += coins;

        determine_current_holder_percent();
    }

    private void determine_current_holder_percent()
    {
        int percent = Mathf.FloorToInt(((float)current_coins_for_PowerUp / (float)max_coins_for_PowerUp) * 10); //0-1 * 10 -> 0-10

        if(percent == 10) // max
        {
            PowerUp_holder_cooldown.sprite = null;

            Color tmp = PowerUp_spot_sprite.color;
            tmp.a = 1f;
            PowerUp_spot_sprite.color = tmp;
            
        }
        else
        {
            PowerUp_holder_cooldown.sprite = Holder_sprites[percent];

            Color tmp = PowerUp_spot_sprite.color;
            tmp.a = 0.7f;
            PowerUp_spot_sprite.color = tmp;
        }
    }
}
