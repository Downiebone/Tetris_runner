using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class prieview : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private SpriteRenderer[] rend;

    public void set_Color_of_rends(Color col)
    {
        for(int i = 0; i < rend.Length; i++)
        {
            rend[i].color = col;
        }
    }

    private void Awake()
    {
        disable_rends();
    }

    public void disable_rends()
    {
        for (int i = 0; i < rend.Length; i++)
        {
            rend[i].enabled = false;
        }
    }

    public void enable_rends()
    {
        for (int i = 0; i < rend.Length; i++)
        {
            rend[i].enabled = true;
        }
    }
}
