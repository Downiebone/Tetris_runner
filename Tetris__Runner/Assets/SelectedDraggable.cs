using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectedDraggable : MonoBehaviour
{
    public string Draggable_PlayerprefName;
    [SerializeField] private UnityEngine.UI.Image sprit;

    //private SHOP_MANAGER shop_Manage;

    //private void Start()
    //{
    //    shop_Manage = GameObject.FindGameObjectWithTag("Shop").GetComponent<SHOP_MANAGER>();
    //}

    public void SetupSelectedDraggable(string prefName, Sprite ItemSprite)
    {
        Draggable_PlayerprefName = prefName;
        sprit.sprite = ItemSprite;
    }

    public void RemoveThisItem()
    {
        PlayerPrefs.SetInt(Draggable_PlayerprefName + "_Selected", 0);

        Destroy(this.gameObject);
    }
}
