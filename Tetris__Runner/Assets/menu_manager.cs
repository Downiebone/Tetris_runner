using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class menu_manager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        fix_playerprefs();
    }
   
    private void fix_playerprefs()
    {
        
        if (PlayerPrefs.GetInt("PowerUp_Golden") == 0)
        {
            PlayerPrefs.SetInt("PowerUp_Golden", 1);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
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

        PlayerPrefs.SetInt("Money", 0);
    }

    public void Start_Game()
    {
        SceneManager.LoadScene("GameScene"); //reload current scene?
    }

    public void Load_shop_scren()
    {
        SceneManager.LoadScene("Shop"); //load shop
    }
}
