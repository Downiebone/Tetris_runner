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


    [Header("Selected PowerUp")]
    [SerializeField] private TMP_Text name_obj;
    [SerializeField] private TMP_Text description_obj;
    [SerializeField] private Image icon_sprit;
    [SerializeField] private GameObject Obj_holding_bars;

    [SerializeField] private power_up_inshop[] powers;

    void Start()
    {
        UpdateMoney_Text();
        select_last_selected_powerup();
    }

    private void select_last_selected_powerup()
    {
        powers[PlayerPrefs.GetInt("PowerUp_Selected")].transfer_my_selected(this);
    }
    public void UpdateMoney_Text()
    {
        money_texs.text = PlayerPrefs.GetInt("Money").ToString() + "x";
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
