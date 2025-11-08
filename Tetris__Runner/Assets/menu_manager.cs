using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class menu_manager : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject Menu_part;
    public GameObject Settings_part;

    void Awake()
    {
        fix_playerprefs();
    }
   
    private void fix_playerprefs()
    {
        
        if (PlayerPrefs.GetInt("PowerUp_Golden") == 0) //first time playing game?
        {
            PlayerPrefs.SetInt("PowerUp_Golden", 1);

            //original offset?
            PlayerPrefs.SetFloat("X_off", -0.5f);
            PlayerPrefs.SetFloat("Y_off", 0);

            PlayerPrefs.SetFloat("Sound_Effects", 1);
            PlayerPrefs.SetFloat("Sound_Music", 1);
        }
    }

    public void add_money_debug()
    {
        PlayerPrefs.SetInt("Money", PlayerPrefs.GetInt("Money") + 1000); // add money debug
    }

    public void reset_playerprefs_Debug()
    {
        PlayerPrefs.SetInt("PowerUp_Golden", 1);
        PlayerPrefs.SetInt("PowerUp_Dash", 0);
        PlayerPrefs.SetInt("PowerUp_Bird", 0);

        PlayerPrefs.SetInt("PowerUp_Selected", 0);

        PlayerPrefs.SetInt("BombJump", 0);
        PlayerPrefs.SetInt("BombCoin", 0);

        PlayerPrefs.SetInt("Item_Selected_1", 0);
        PlayerPrefs.SetInt("Item_Selected_2", 0);

        PlayerPrefs.SetInt("Money", 0);

        PlayerPrefs.SetFloat("X_off", -0.5f);
        PlayerPrefs.SetFloat("Y_off", 0);

        PlayerPrefs.SetFloat("Sound_Effects", 1);
        PlayerPrefs.SetFloat("Sound_Music", 1);
    }

    public void Start_Game()
    {
        SceneManager.LoadScene("GameScene"); //reload current scene?
    }

    public void Load_shop_scren()
    {
        SceneManager.LoadScene("Shop"); //load shop
    }

    public void Settings_Button()
    {
        Settings_part.SetActive(true);
        Menu_part.SetActive(false);
    }

    public void Exit_settings_button()
    {
        Menu_part.SetActive(true);
        Settings_part.SetActive(false);
    }
}
