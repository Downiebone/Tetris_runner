using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cellarUIPlacableOjb : MonoBehaviour
{
    // Start is called before the first frame update

    public CellarPlacableObj myCellarObj;
    public ItemPlacerManager itemManagerRef;

    public void SetupObject(CellarPlacableObj ob, ItemPlacerManager itemref)
    {
        myCellarObj = ob;
        itemManagerRef = itemref;
    }

    public void ClickMe()
    {
        itemManagerRef.startPreivewItem(myCellarObj);
    }
}
