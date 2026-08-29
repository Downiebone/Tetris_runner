using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class power_up_inshop : MonoBehaviour
{
    [SerializeField] private SHOP_MANAGER shop_Manage;

    [SerializeField] private TMP_Text name_obj;
    [SerializeField] private TMP_Text description_obj;
    [SerializeField] private Image icon_sprit;
    [SerializeField] private TMP_Text Prize_Text;

    [Space]
    [SerializeField] private string player_pref_powerup;
    [SerializeField] private int player_pref_ordering;
    [Space]

    [SerializeField] private GameObject buy_screen;
    [SerializeField] private CanvasGroup canva_group;

    [SerializeField] private string myName;
    [SerializeField] private string myDescript;
    [SerializeField] private Sprite mySprit;

    [SerializeField] private int Prize = 50;
    [SerializeField] private float Prize_Multiplier_on_bought = 2;
    [SerializeField] private int upgrade_parts = 4;
    private int current_bought_level = 0;

    [SerializeField] private GameObject Obj_holding_bars;

    void Start()
    {
        current_bought_level = PlayerPrefs.GetInt(player_pref_powerup);

        shop_Manage = GameObject.FindGameObjectWithTag("Shop").GetComponent<SHOP_MANAGER>();

        Prize = Mathf.RoundToInt(Mathf.Pow(Prize_Multiplier_on_bought, current_bought_level) * Prize);

        

        updateVisuals();
    }

    private void updateVisuals()
    {
        name_obj.text = myName;
        description_obj.text = myDescript;
        icon_sprit.sprite = mySprit;
        Prize_Text.text = Prize.ToString() + "$";

        for (int i = 0; i < Obj_holding_bars.transform.childCount; i++) // make only @upgrade_parts children visable
        {
            if(i < upgrade_parts)
            {
                Obj_holding_bars.transform.GetChild(i).gameObject.SetActive(true);
            }
            else
            {
                Obj_holding_bars.transform.GetChild(i).gameObject.SetActive(false);
            }

            if(i < current_bought_level)
            {
                Obj_holding_bars.transform.GetChild(i).gameObject.GetComponent<Image>().color = Color.green; // bought level
            }
            else
            {
                Obj_holding_bars.transform.GetChild(i).gameObject.GetComponent<Image>().color = Color.white;
            }
        }
    }

    public void leave_enter_state()
    {
        buy_screen.SetActive(false);
        canva_group.alpha = 1f;
    }
    public void enter_selected_state()
    {
        if (buy_screen.activeSelf == true)//if already entered -> leave
        {
            buy_screen.SetActive(false);
            canva_group.alpha = 1f;
        }
        else
        {
            buy_screen.SetActive(true);
            canva_group.alpha = 0.3f;
        }
    }
    public void upgrade_me()
    {
        if(PlayerPrefs.GetInt("Money") > Prize && PlayerPrefs.GetInt(player_pref_powerup) < upgrade_parts)
        {
            PlayerPrefs.SetInt("Money", PlayerPrefs.GetInt("Money") - Prize); // remove money
            shop_Manage.UpdateMoney_Text();

            current_bought_level++;
            PlayerPrefs.SetInt(player_pref_powerup, PlayerPrefs.GetInt(player_pref_powerup) + 1);

            

            Prize = Mathf.RoundToInt(Prize_Multiplier_on_bought * Prize);

            updateVisuals();

            leave_enter_state();
        }
    }
    public void select_me()
    {
        if (current_bought_level != 0)
        {
            shop_Manage.Set_selected_PowerUp(myName, myDescript, mySprit, upgrade_parts, current_bought_level);

            PlayerPrefs.SetInt("PowerUp_Selected", player_pref_ordering);
        }

        leave_enter_state();
    }

    public void transfer_my_selected(SHOP_MANAGER managered)
    {
        current_bought_level = PlayerPrefs.GetInt(player_pref_powerup);

        managered.Set_selected_PowerUp(myName, myDescript, mySprit, upgrade_parts, current_bought_level);
    }
}
