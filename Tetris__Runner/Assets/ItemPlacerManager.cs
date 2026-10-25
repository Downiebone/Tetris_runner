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

    public float smoothSpeed = 5;
    public GameObject HighlightPortion;
    public GameObject HighlightPortion_visuals;
    public SpriteRenderer HighlightObjectSpriterend;
    public SpriteRenderer HighlightObjectSpriterend_Box;

    public TMP_Text diamonds_Text;

    Camera main_cam;
    private bool PreviewingItem = false;
    private bool CanPlacePreviewItem = false;
    private Vector2 MovingToPosition;
    private CellarPlacableObj currentlyPreviewedCellarObj;

    [Header("PreviewMenu")]

    public TMP_Text PreviewPriceText;
    public Image PreviewPriceImage;

    void Start()
    {
        diamonds_Text.text = "x " + PlayerPrefs.GetInt("Diamonds");

        main_cam = Camera.main;
    }

    void LateUpdate()
    {
        if(PreviewingItem == false) { return; }

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
                Debug.Log("object in bounds!!!");

                HighlightObjectSpriterend_Box.color = Color.green;
                CanPlacePreviewItem = true;

                if (currentlyPreviewedCellarObj.KindaObject == CellarPlacableObj.ObjectType.PlacableOnFloor)
                {
                    float moveDownDistance = boundsManager.GetDistanceToMoveDownGroundObj(worldPosition, currentlyPreviewedCellarObj);
                    MovingToPosition = new Vector2(worldPosition.x, worldPosition.y - moveDownDistance);
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

    public void BuyPreviewItem()
    {
        //if you cant afford, dont do anything

        PreviewingItem = false;
        CanPlacePreviewItem = false;

        HighlightPortion.transform.position = new Vector2(-25, -25); //hide obj
        HighlightPortion_visuals.transform.localPosition = Vector2.zero;

        UI_PreviewButtons.SetActive(false);
        UI_PlacableCellarItem.SetActive(true);
    }

    public void CancelPreview()
    {
        CanPlacePreviewItem = false;
        PreviewingItem = false;

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
