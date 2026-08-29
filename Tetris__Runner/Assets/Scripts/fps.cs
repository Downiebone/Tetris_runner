using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class fps : MonoBehaviour
{
    // Start is called before the first frame update
    public TMP_Text tmpText;
    public float updateHZ = 1;
    private float updateTimer = 0;

    private void Start()
    {
        Application.targetFrameRate = 60;
    }
    // Update is called once per frame
    void Update()
    {
        updateTimer += Time.unscaledDeltaTime;
        if(updateTimer >= updateHZ)
        {
            tmpText.text = "FPS: " + (1f / Time.unscaledDeltaTime);

            updateTimer = 0;
        }
    }
}
