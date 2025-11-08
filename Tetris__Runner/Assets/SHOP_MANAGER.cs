using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
public class SHOP_MANAGER : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private TMP_Text money_texs;

    public GameObject powerup_side;
    public GameObject item_side;

    public void EnterPowerupSide()
    {
        powerup_side.SetActive(true);
        item_side.SetActive(false);
    }
    public void EnterItemSide()
    {
        item_side.SetActive(true);
        powerup_side.SetActive(false);
    }


    [Header("Selected PowerUp")]
    [SerializeField] private TMP_Text name_obj;
    [SerializeField] private TMP_Text description_obj;
    [SerializeField] private Image icon_sprit;
    [SerializeField] private GameObject Obj_holding_bars;

    [Header("Selected Item")]
    private int itemToSelect = 1;

    [SerializeField] private TMP_Text item_name_1;
    [SerializeField] private Image item_icon_1;
    [Space]
    [SerializeField] private TMP_Text item_name_2;
    [SerializeField] private Image item_icon_2;

    [SerializeField] private power_up_inshop[] powers;
    [SerializeField] private Item_Inshop[] items;

    void Start()
    {
        UpdateMoney_Text();
        select_last_selected_powerup();
    }

    private void select_last_selected_powerup()
    {
        powers[PlayerPrefs.GetInt("PowerUp_Selected")].transfer_my_selected(this);

        if(PlayerPrefs.GetInt("Item_Selected_1") != 0)
        {
            itemToSelect = 1;
            items[PlayerPrefs.GetInt("Item_Selected_1") - 1].transfer_my_selected(this, 1);
        }
        if (PlayerPrefs.GetInt("Item_Selected_2") != 0)
        {
            itemToSelect = 2;
            items[PlayerPrefs.GetInt("Item_Selected_2") - 1].transfer_my_selected(this, 2);
        }
    }
    public void UpdateMoney_Text()
    {
        money_texs.text = PlayerPrefs.GetInt("Money").ToString() + "x";
    }

    public void setSelectedItemThing(int itemSelect)
    {
        itemToSelect = itemSelect;
        //highlight
    }

    public void Set_selected_Item(string myName, Sprite mySprit, int playerPrefOrder)
    {
        if(itemToSelect == 1)
        {
            item_name_1.text = myName;
            item_icon_1.sprite = mySprit;

            if(item_name_2.text == myName) //item is already on other slot
            {
                item_name_2.text = "empty";
                item_icon_2.sprite = null;
                PlayerPrefs.SetInt("Item_Selected_2", 0);
            }

            PlayerPrefs.SetInt("Item_Selected_1", playerPrefOrder);

            itemToSelect = 2;
            //highlight other
        }
        else if(itemToSelect == 2) 
        {
            item_name_2.text = myName;
            item_icon_2.sprite = mySprit;

            if (item_name_1.text == myName) //item is already on other slot
            {
                item_name_1.text = "empty";
                item_icon_1.sprite = null;
                PlayerPrefs.SetInt("Item_Selected_1", 0);
            }

            PlayerPrefs.SetInt("Item_Selected_2", playerPrefOrder);

            itemToSelect = 1;
            //highlight other
        }


    }

    public void Set_selected_PowerUp(string myName, string myDescript, Sprite mySprit, int upgrade_parts, int current_bought_level)
    {
        name_obj.text = myName;
        description_obj.text = myDescript;
        icon_sprit.sprite = mySprit;

        for (int i = 0; i < Obj_holding_bars.transform.childCount; i++) // make only @upgrade_parts children visable
        {
            if (i < upgrade_parts)
            {
                Obj_holding_bars.transform.GetChild(i).gameObject.SetActive(true);
            }
            else
            {
                Obj_holding_bars.transform.GetChild(i).gameObject.SetActive(false);
            }

            if (i < current_bought_level)
            {
                Obj_holding_bars.transform.GetChild(i).gameObject.GetComponent<Image>().color = Color.green; // bought level
            }
            else
            {
                Obj_holding_bars.transform.GetChild(i).gameObject.GetComponent<Image>().color = Color.white;
            }
        }
    }

    public void Load_Main_Menu()
    {
        SceneManager.LoadScene("Menu");
    }
}
