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

    public GameObject draggable_side;
    public GameObject item_side;
    public GameObject skins_side;

    public void EnterPowerupSide()
    {
        draggable_side.SetActive(true);
        item_side.SetActive(false);
        skins_side.SetActive(false);
    }
    public void EnterItemSide()
    {
        item_side.SetActive(true);
        draggable_side.SetActive(false);
        skins_side.SetActive(false);
    }
    public void EnterSkinsSide()
    {
        skins_side.SetActive(true);
        item_side.SetActive(false);
        draggable_side.SetActive(false);
    }

    [Space]
    [Header("Selected Item")]
    [SerializeField] private TMP_Text item_name_1;
    [SerializeField] private Image item_icon_1;
    [SerializeField] private Image Item_1_Holder;
    private string item_playerPref_1;
    [Space]
    [SerializeField] private TMP_Text item_name_2;
    [SerializeField] private Image item_icon_2;
    [SerializeField] private Image Item_2_Holder;
    private string item_playerPref_2;
    private int itemToSelect = 1;


    [Header("Draggables")]

    [SerializeField] private Transform DraggablesSelected_Holder;

    [SerializeField] private power_up_inshop[] powers;
    [SerializeField] private GameObject Draggable_Selected_Prefab;
    [SerializeField] private Item_Inshop[] items;

    void Start()
    {
        UpdateMoney_Text();
        select_last_selected_powerup();
    }

    private void select_last_selected_powerup()
    {
        for (int i = 0; i < powers.Length; i++)
        {
            if(PlayerPrefs.GetInt(powers[i].player_pref_draggable + "_Selected") == 1)
            {
                GameObject GO = Instantiate(Draggable_Selected_Prefab, Vector3.one, Quaternion.identity, DraggablesSelected_Holder);
                GO.GetComponent<SelectedDraggable>().SetupSelectedDraggable(powers[i].player_pref_draggable, powers[i].mySprit);
            }
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
        HighlightItemHolder(itemSelect);
    }

    private void HighlightItemHolder(int which)
    {
        if(which == 1)
        {
            Item_1_Holder.color = Color.green;
            Item_2_Holder.color = Color.white;
        }
        else
        {
            Item_2_Holder.color = Color.green;
            Item_1_Holder.color = Color.white;
        }
    }

    public void Set_selected_Item(string myName, Sprite mySprit, int playerPrefOrder, string player_pref_Item)
    {
        if(itemToSelect == 1)
        {
            PlayerPrefs.SetInt(item_playerPref_1, 1); // set old item in this slot to just bought

            item_playerPref_1 = player_pref_Item;
            item_name_1.text = myName;
            item_icon_1.sprite = mySprit;

            if(item_name_2.text == myName) //item is already on other slot
            {
                item_playerPref_2 = "";
                item_name_2.text = "empty";
                item_icon_2.sprite = null;
                PlayerPrefs.SetInt("Item_Selected_2", 0);
            }

            PlayerPrefs.SetInt("Item_Selected_1", playerPrefOrder);
            PlayerPrefs.SetInt(player_pref_Item, 2); // set new item in this slot to selected

            itemToSelect = 2;
            HighlightItemHolder(2);
            //highlight other
        }
        else if(itemToSelect == 2) 
        {
            PlayerPrefs.SetInt(item_playerPref_2, 1); // set old item in this slot to just bought

            item_playerPref_2 = player_pref_Item;
            item_name_2.text = myName;
            item_icon_2.sprite = mySprit;

            if (item_name_1.text == myName) //item is already on other slot
            {
                item_playerPref_1 = "";
                item_name_1.text = "empty";
                item_icon_1.sprite = null;
                PlayerPrefs.SetInt("Item_Selected_1", 0);
            }

            PlayerPrefs.SetInt("Item_Selected_2", playerPrefOrder);
            PlayerPrefs.SetInt(player_pref_Item, 2); // set new item in this slot to selected

            itemToSelect = 1;
            HighlightItemHolder(1);
            //highlight other
        }


    }

    public void Set_selected_PowerUp(Sprite mySprit, string PlayerPrefName)
    {
        if (PlayerPrefs.GetInt(PlayerPrefName + "_Selected") == 1)
        {
            //if object already Selected, dont do anything
            return;
        }

        PlayerPrefs.SetInt(PlayerPrefName + "_Selected", 1);

        GameObject GO = Instantiate(Draggable_Selected_Prefab, Vector3.one, Quaternion.identity, DraggablesSelected_Holder);
        GO.GetComponent<SelectedDraggable>().SetupSelectedDraggable(PlayerPrefName, mySprit);
    }

    //public void RemovePowerUp(string PlayerPrefName)
    //{
    //    PlayerPrefs.SetInt(PlayerPrefName + "_Selected", 0);
    //}

    public void Load_Main_Menu()
    {
        SceneManager.LoadScene("Menu");
    }
}
