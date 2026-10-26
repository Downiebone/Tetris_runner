using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class cellarUIPlacableOjb : MonoBehaviour
{
    // Start is called before the first frame update

    public CellarPlacableObj myCellarObj;
    public ItemPlacerManager itemManagerRef;

    public Image uiImage;
    public TMP_Text uiPrice;

    public void SetupObject(CellarPlacableObj ob, ItemPlacerManager itemref)
    {
        myCellarObj = ob;
        itemManagerRef = itemref;

        uiPrice.text = ob.CellarPrize.ToString() + "<sprite=6>";
        uiImage.sprite = ob.SpriteAndVariance[0]; // need to accomidate variance later
    }

    public void ClickMe()
    {
        itemManagerRef.startPreivewItem(myCellarObj);
    }
}
