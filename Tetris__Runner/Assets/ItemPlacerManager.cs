using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ItemPlacerManager : MonoBehaviour
{
    public CellarBoundsManager boundsManager;

    public CellarPlacableObj[] PlacableObj;

    public GameObject UI_PreviewButtons;
    public GameObject UI_PlacableCellarItem;
    public GameObject UI_MenuStart;
    public Transform UI_CellarItemsHolder;
    public GameObject UI_cellarItem_Prefab;
    public CanvasGroup PreviewCanvasGroup;
    public GameObject PreviewButton;

    [Space]

    public float smoothSpeed = 5;
    public GameObject HighlightPortion;
    public GameObject HighlightPortion_visuals;
    public GameObject HighlightPortion_visuals_ForDisabling;
    public SpriteRenderer HighlightObjectSpriterend;
    public SpriteRenderer HighlightObjectSpriterend_Box;

    private int DiamondAmmount = 0;
    public TMP_Text diamonds_Text;

    Camera main_cam;
    private bool PreviewingItem = false;
    private bool CanPlacePreviewItem = false;
    private Vector2 MovingToPosition;
    private CellarPlacableObj currentlyPreviewedCellarObj;
    //actual bool to control when we turn off ui for previewing
    private bool PrieviewingItemUI = false;


    [Space]
    [Header("PreviewMenu")]

    public TMP_Text PreviewPriceText;
    public Image PreviewPriceImage;

    private GameObject Preview_PlacedObj;

    void Start()
    {
        DiamondAmmount = PlayerPrefs.GetInt("Diamonds");
        updateDiamondTextUI();

        main_cam = Camera.main;

        fillCellarShopItems();

        fillBoughtItems_FromPlayerprefs();
    }

    private void updateDiamondTextUI()
    {
        diamonds_Text.text = "x " + DiamondAmmount.ToString();
    }

    private void fillBoughtItems_FromPlayerprefs()
    {
        for (int i = 0; i < PlacableObj.Length; i++)
        {
            string I_playerprefNem = PlacableObj[i].getPlayerPrefName();
            int ammountOfThisItem = PlayerPrefs.GetInt(I_playerprefNem);

            for (int x = 0; x < ammountOfThisItem; x++)
            {
                float itemX = PlayerPrefs.GetFloat(I_playerprefNem + "_x" + "_" + (x + 1).ToString());
                float itemY = PlayerPrefs.GetFloat(I_playerprefNem + "_y" + "_" + (x + 1).ToString());

                Instantiate(PlacableObj[i].PrefabToUse, new Vector3(itemX, itemY, 0), Quaternion.identity);
            }
        }
    }

    private void fillCellarShopItems()
    {
        for (int i = 0; i < PlacableObj.Length; i++)
        {
            GameObject GO = Instantiate(UI_cellarItem_Prefab, Vector3.zero, Quaternion.identity, UI_CellarItemsHolder);
            GO.GetComponent<cellarUIPlacableOjb>().SetupObject(PlacableObj[i], this);
        }
    }

    void LateUpdate()
    {
        if(PreviewingItem == false) { return; }
        if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject()) return; //return if we click on UI
        if(PrieviewingItemUI == true) { return; } //cant move around item while previewing it

#if UNITY_EDITOR


        Vector2 screenPosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        Vector2 worldPosition = main_cam.ScreenToWorldPoint(screenPosition);

        if (CanPlacePreviewItem)
        {
            HighlightPortion_visuals.transform.position = Vector3.Lerp(HighlightPortion_visuals.transform.position, MovingToPosition, smoothSpeed * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.Mouse0))
        {
            HighlightPortion.transform.position = worldPosition;

            if (boundsManager.IsObjectInBounds(worldPosition, currentlyPreviewedCellarObj))
            {

                HighlightObjectSpriterend_Box.color = Color.green;
                CanPlacePreviewItem = true;

                if (currentlyPreviewedCellarObj.KindaObject == CellarPlacableObj.ObjectType.PlacableOnFloor)
                {
                    float moveDownDistance = boundsManager.GetDistanceToMoveDownGroundObj(worldPosition, currentlyPreviewedCellarObj);
                    MovingToPosition = new Vector2(worldPosition.x, worldPosition.y - moveDownDistance);
                    //HighlightPortion_visuals.transform.position = new Vector2(worldPosition.x, worldPosition.y - moveDownDistance);
                    //HighlightPortion_visuals.transform.position = Vector3.Lerp(HighlightPortion_visuals.transform.position, new Vector2(worldPosition.x, worldPosition.y - moveDownDistance), smoothSpeed * Time.deltaTime);
                }
                if (currentlyPreviewedCellarObj.KindaObject == CellarPlacableObj.ObjectType.PlacableOnCeiling)
                {
                    float moveUpDistance = boundsManager.GetDistanceToMoveUpCeilingObj(worldPosition, currentlyPreviewedCellarObj);
                    MovingToPosition = new Vector2(worldPosition.x, worldPosition.y + moveUpDistance);
                    //HighlightPortion_visuals.transform.position = new Vector2(worldPosition.x, worldPosition.y - moveDownDistance);
                    //HighlightPortion_visuals.transform.position = Vector3.Lerp(HighlightPortion_visuals.transform.position, new Vector2(worldPosition.x, worldPosition.y - moveDownDistance), smoothSpeed * Time.deltaTime);
                }

            }
            else
            {
                HighlightObjectSpriterend_Box.color = Color.red;
                CanPlacePreviewItem = false;

                //HighlightPortion_visuals.transform.position = worldPosition;
                HighlightPortion_visuals.transform.position = Vector3.Lerp(HighlightPortion_visuals.transform.position, worldPosition, smoothSpeed * Time.deltaTime);
            }
        }
        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            //boundsManager.IsObjectInBounds(worldPosition, currentlyPreviewedCellarObj);
        }
#endif
    }

    public void testPreviewItem_Toggle()
    {
        PrieviewingItemUI = !PrieviewingItemUI;

        if (PrieviewingItemUI) //enter preview
        {
            startpreview();
        }
        else // end preview
        {
            endPreview();
        }
        
    }
    private void startpreview()
    {

        PreviewCanvasGroup.alpha = 0;
        PreviewCanvasGroup.interactable = false;

        PreviewButton.transform.Rotate(Vector3.forward, (float)180);

        Preview_PlacedObj = Instantiate(currentlyPreviewedCellarObj.PrefabToUse, HighlightPortion_visuals.transform.position, Quaternion.identity);

        HighlightPortion_visuals_ForDisabling.SetActive(false);
    }
    private void endPreview()
    {

        PreviewCanvasGroup.alpha = 1;
        PreviewCanvasGroup.interactable = true;

        PreviewButton.transform.Rotate(Vector3.forward, (float)180);

        if (Preview_PlacedObj != null)
        {
            Destroy(Preview_PlacedObj);
            Preview_PlacedObj = null;
        }
        HighlightPortion_visuals_ForDisabling.SetActive(true);
    }
    public void BuyPreviewItem()
    {
        //if you cant afford, dont do anything
        if(DiamondAmmount < currentlyPreviewedCellarObj.CellarPrize) { return; }
        //cant buy item if you cant place it
        if(CanPlacePreviewItem == false) { return; }

        DiamondAmmount = DiamondAmmount - currentlyPreviewedCellarObj.CellarPrize;
        PlayerPrefs.SetInt("Diamonds", DiamondAmmount);
        updateDiamondTextUI();

        string playerPrefName = currentlyPreviewedCellarObj.getPlayerPrefName();

        int lastAmmountBought = PlayerPrefs.GetInt(playerPrefName); //last ammount bought
        int newAmmount = lastAmmountBought + 1;

        PlayerPrefs.SetInt(playerPrefName, newAmmount); //add one

        PlayerPrefs.SetFloat(playerPrefName + "_x" + "_" + newAmmount, HighlightPortion_visuals.transform.position.x);
        PlayerPrefs.SetFloat(playerPrefName + "_y" + "_" + newAmmount, HighlightPortion_visuals.transform.position.y);

        //spawn actual object
        Instantiate(currentlyPreviewedCellarObj.PrefabToUse, HighlightPortion_visuals.transform.position, Quaternion.identity);

        PreviewingItem = false;
        CanPlacePreviewItem = false;

        //stop preview visuals
        if(PrieviewingItemUI != false)
        {
            PrieviewingItemUI = false;
            endPreview();
        }
        

        HighlightPortion.transform.position = new Vector2(-25, -25); //hide obj
        HighlightPortion_visuals.transform.localPosition = Vector2.zero;

        UI_PreviewButtons.SetActive(false);
        UI_PlacableCellarItem.SetActive(true);
    }

    public void CancelPreview()
    {
        CanPlacePreviewItem = false;
        PreviewingItem = false;

        //stop preview visuals
        if (PrieviewingItemUI != false)
        {
            PrieviewingItemUI = false;
            endPreview();
        }

        HighlightPortion.transform.position = new Vector2(-25, -25);
        HighlightPortion_visuals.transform.localPosition = Vector2.zero;

        UI_PreviewButtons.SetActive(false);
        UI_PlacableCellarItem.SetActive(true);
    }

    public void startPreivewItem(CellarPlacableObj itemToPreview)
    {
        HighlightObjectSpriterend.sprite = itemToPreview.SpriteAndVariance[0]; //change to accomidate varriance later

        currentlyPreviewedCellarObj = itemToPreview;

        PreviewingItem = true;

        CanPlacePreviewItem = false;
        HighlightObjectSpriterend_Box.color = Color.red;

        HighlightPortion.transform.position = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, 0);

        //#####################################   Preview UI  ###################################################################

        PreviewPriceText.text = itemToPreview.CellarPrize.ToString() + "<sprite=6>";
        PreviewPriceImage.sprite = itemToPreview.SpriteAndVariance[0]; //change to accomidate varriance later

        UI_PreviewButtons.SetActive(true);
        UI_PlacableCellarItem.SetActive(false);
    }

    public void OpenPurchaseItems()
    {
        UI_MenuStart.SetActive(false);
        UI_PlacableCellarItem.SetActive(true);
    }
    public void backToStartMenu()
    {
        UI_MenuStart.SetActive(true);
        UI_PlacableCellarItem.SetActive(false);
    }

    public void loadMenu()
    {
        SceneManager.LoadScene("Menu");
    }
}
