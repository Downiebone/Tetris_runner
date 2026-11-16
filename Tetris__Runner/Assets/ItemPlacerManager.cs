using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ItemPlacerManager : MonoBehaviour
{
    public CellarBoundsManager[] boundsManager;
    [SerializeField] private CellarAlienBehavior cellAlien;

    public CellarPlacableObj[] PlacableObj;
    private List<CellarPlacableObj> ItemsPlacedInWorld = new List<CellarPlacableObj>();

    public GameObject UI_PreviewButtons;
    public GameObject UI_PlacableCellarItem;
    public GameObject UI_MenuStart;
    public Transform UI_CellarItemsHolder;
    public GameObject UI_cellarItem_Prefab;
    public CanvasGroup PreviewCanvasGroup;
    public GameObject PreviewButton;
    public GameObject VisualsForAltering_previewBoxSize;
    [Tooltip("8, 16, 32, 64")] public float[] GreenBoxProportions;

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
    private bool Placable_FaceDirection_trueIsRight = true;
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

    public GameObject GemSparkle_medium;
    public GameObject GemSparkle_bigger;
    public GameObject GemSparkle_huge;

    //should return true if the cellar is nice enough
    //nice enough should maybe mean a light source and somewhere it can sit
    public bool isCellarLivable()
    {
        bool HaslightSource = false;
        bool hasSittable = false;
        for (int i = 0; i < ItemsPlacedInWorld.Count; i++)
        {
            if(HaslightSource == false && ItemsPlacedInWorld[i].Objects_properties == CellarPlacableObj.ObjectProperties.LightSource)
            {
                HaslightSource = true;
                continue;
            }
            if (hasSittable == false && ItemsPlacedInWorld[i].Objects_properties == CellarPlacableObj.ObjectProperties.Sittable)
            {
                hasSittable = true;
            }
            if(HaslightSource && hasSittable)
            {
                break;
            }
        }
        return (HaslightSource && hasSittable) == true;
    }

    void Start()
    {
        DiamondAmmount = PlayerPrefs.GetInt("Diamonds");
        updateDiamondTextUI();

        main_cam = Camera.main;

        fillCellarShopItems();

        fillBoughtItems_FromPlayerprefs();

        //make sure it starts after placer
        cellAlien.DelayedStart();
    }

    public float GetRandom_X_pos_In_CurrentBounds(Vector2 alienPos)
    {
        return boundsManager[0].GetRandom_X_value_inCurrentRoom(alienPos);
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
                ItemsPlacedInWorld.Add(PlacableObj[i]);
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

            if (boundsManager[0].IsObjectInBounds(worldPosition, currentlyPreviewedCellarObj))
            {

                HighlightObjectSpriterend_Box.color = Color.green;
                CanPlacePreviewItem = true;

                if (currentlyPreviewedCellarObj.KindaObject == CellarPlacableObj.ObjectType.PlacableOnFloor)
                {
                    float moveDownDistance = boundsManager[0].GetDistanceToMoveDownGroundObj(worldPosition, currentlyPreviewedCellarObj);
                    MovingToPosition = new Vector2(worldPosition.x, worldPosition.y - moveDownDistance);
                    //HighlightPortion_visuals.transform.position = new Vector2(worldPosition.x, worldPosition.y - moveDownDistance);
                    //HighlightPortion_visuals.transform.position = Vector3.Lerp(HighlightPortion_visuals.transform.position, new Vector2(worldPosition.x, worldPosition.y - moveDownDistance), smoothSpeed * Time.deltaTime);
                }
                if (currentlyPreviewedCellarObj.KindaObject == CellarPlacableObj.ObjectType.PlacableOnCeiling)
                {
                    float moveUpDistance = boundsManager[0].GetDistanceToMoveUpCeilingObj(worldPosition, currentlyPreviewedCellarObj);
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
            //boundsManager[0].IsObjectInBounds(worldPosition, currentlyPreviewedCellarObj);
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
        SpawnSparkeParticles(currentlyPreviewedCellarObj.CellarPrize, HighlightPortion_visuals.transform.position);

        string playerPrefName = currentlyPreviewedCellarObj.getPlayerPrefName();

        int lastAmmountBought = PlayerPrefs.GetInt(playerPrefName); //last ammount bought
        int newAmmount = lastAmmountBought + 1;

        PlayerPrefs.SetInt(playerPrefName, newAmmount); //add one

        PlayerPrefs.SetFloat(playerPrefName + "_x" + "_" + newAmmount, HighlightPortion_visuals.transform.position.x);
        PlayerPrefs.SetFloat(playerPrefName + "_y" + "_" + newAmmount, HighlightPortion_visuals.transform.position.y);

        //spawn actual object
        GameObject GO = Instantiate(currentlyPreviewedCellarObj.PrefabToUse, HighlightPortion_visuals.transform.position, Quaternion.identity);
        ItemsPlacedInWorld.Add(currentlyPreviewedCellarObj); // add to list that knows of all spawned items

        if(currentlyPreviewedCellarObj.Objects_properties == CellarPlacableObj.ObjectProperties.Sittable)
        {
            GO.GetComponent<CellarSitObj>().FacingRight = Placable_FaceDirection_trueIsRight;
        }

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

        //tell alien we placed a block
        cellAlien.PlacedItemCall();
    }

    private void SpawnSparkeParticles(int ObjectPrize, Vector3 aposition)
    {
        if(ObjectPrize >= 100)
        {
            Instantiate(GemSparkle_huge, aposition, Quaternion.identity);
        }
        else if(ObjectPrize >= 20)
        {
            Instantiate(GemSparkle_bigger, aposition, Quaternion.identity);
        }
        else
        {
            Instantiate(GemSparkle_medium, aposition, Quaternion.identity);
        }
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

    //when you first click an item in the shop
    public void startPreivewItem(CellarPlacableObj itemToPreview)
    {
        HighlightObjectSpriterend.sprite = itemToPreview.SpriteAndVariance[0]; //change to accomidate varriance later

        currentlyPreviewedCellarObj = itemToPreview;

        PreviewingItem = true;

        CanPlacePreviewItem = false;
        HighlightObjectSpriterend_Box.color = Color.red;

        setGreenPreviewBoxSize(itemToPreview.Objects_size);

        HighlightPortion.transform.position = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, 0);

        //#####################################   Preview UI  ###################################################################

        PreviewPriceText.text = itemToPreview.CellarPrize.ToString() + "<sprite=6>";
        PreviewPriceImage.sprite = itemToPreview.SpriteAndVariance[0]; //change to accomidate varriance later

        UI_PreviewButtons.SetActive(true);
        UI_PlacableCellarItem.SetActive(false);
    }

    private void setGreenPreviewBoxSize(CellarPlacableObj.ObjectSize newSize)
    {
        float newSizeToSet = 1.1f;

        switch (newSize)
        {
            case CellarPlacableObj.ObjectSize.is_8_8:
                newSizeToSet = GreenBoxProportions[0];
                break;
            case CellarPlacableObj.ObjectSize.is_16_16:
                newSizeToSet = GreenBoxProportions[1];
                break;
            case CellarPlacableObj.ObjectSize.is_32_32:
                newSizeToSet = GreenBoxProportions[2];
                break;
            case CellarPlacableObj.ObjectSize.is_64_64:
                newSizeToSet = GreenBoxProportions[3];
                break;

        }

        VisualsForAltering_previewBoxSize.transform.localScale = new Vector3(newSizeToSet, newSizeToSet, 1);
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

    [SerializeField] private Animator UI_Fade_Animator;

    IEnumerator waitforMenuLoad(string sceneName)
    {
        UI_Fade_Animator.SetTrigger("Fade");

        yield return new WaitForSeconds(1.3f);

        SceneManager.LoadScene(sceneName);
    }

    public void loadMenu()
    {
        StartCoroutine(waitforMenuLoad("Menu"));
    }
}
