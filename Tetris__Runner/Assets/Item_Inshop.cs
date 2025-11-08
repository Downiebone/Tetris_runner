using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Item_Inshop : MonoBehaviour
{
    [SerializeField] private SHOP_MANAGER shop_Manage;

    [SerializeField] private TMP_Text name_obj;
    [SerializeField] private TMP_Text description_obj;
    [SerializeField] private Image icon_sprit;
    [SerializeField] private TMP_Text Prize_Text;

    [Space]
    [SerializeField] private string player_pref_Item;
    [SerializeField] private int player_pref_ordering;
    [Space]

    [SerializeField] private GameObject buy_screen;
    [SerializeField] private CanvasGroup canva_group;

    [SerializeField] private string myName;
    [SerializeField] private string myDescript;
    [SerializeField] private Sprite mySprit;

    [SerializeField] private int Prize = 50;

    private bool bought = false;


    void Start()
    {
        shop_Manage = GameObject.FindGameObjectWithTag("Shop").GetComponent<SHOP_MANAGER>();

        if(PlayerPrefs.GetInt(player_pref_Item) == 1) //have own item
        {
            bought = true;
        }

        updateVisuals();
    }

    private void updateVisuals()
    {
        name_obj.text = myName;
        description_obj.text = myDescript;
        icon_sprit.sprite = mySprit;
        Prize_Text.text = Prize.ToString() + "$";
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
    public void Buy_Me()
    {
        if (PlayerPrefs.GetInt("Money") > Prize && PlayerPrefs.GetInt(player_pref_Item) == 0)
        {
            bought = true;

            PlayerPrefs.SetInt("Money", PlayerPrefs.GetInt("Money") - Prize); // remove money
            shop_Manage.UpdateMoney_Text();

            PlayerPrefs.SetInt(player_pref_Item, 1);

            updateVisuals();

            select_me();
        }
    }
    public void select_me()
    {
        if (bought == true)
        {
            shop_Manage.Set_selected_Item(myName, mySprit, player_pref_ordering);
        }

        leave_enter_state();
    }

    public void transfer_my_selected(SHOP_MANAGER managered, int itemToSelect) //select item 1 or 2
    {

        managered.Set_selected_Item(myName, mySprit, player_pref_ordering);
    }
}
