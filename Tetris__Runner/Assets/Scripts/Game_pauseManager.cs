using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Game_pauseManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void reload_current_scene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); //reload current scene?
    }
}
