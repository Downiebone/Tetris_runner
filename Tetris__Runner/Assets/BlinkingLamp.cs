using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class BlinkingLamp : MonoBehaviour
{
    [SerializeField] private float blinkOfTime = 0.1f;
    [SerializeField] private int NumOfBlinks = 4;

    [SerializeField] private float TimeBetweenBlinks = 5;
    [SerializeField] private float RandomTimeDefine = 1;

    [SerializeField] private float TimeOf = 0;

    [Space]

    [SerializeField] private Light2D[] LightsToTurnOf;
    [SerializeField] private SpriteRenderer MySprit;

    private bool CurrentlyOn = true;


    void Start()
    {
        StartCoroutine(TimeBetweenBlinksCO(Random.Range(TimeBetweenBlinks / 2, TimeBetweenBlinks * 2)));
    }

    private void Blink()
    {
        if (CurrentlyOn) //blink off
        {
            for (int i = 0; i < LightsToTurnOf.Length; i++)
            {
                LightsToTurnOf[i].enabled = false;
            }
            if (MySprit != null)
            {
                MySprit.color = Color.black;
            }
            CurrentlyOn = false;
        }
        else //blink on
        {
            for (int i = 0; i < LightsToTurnOf.Length; i++)
            {
                LightsToTurnOf[i].enabled = true;
            }
            if(MySprit != null)
            {
                MySprit.color = Color.white;
            }
            CurrentlyOn = true;
        }
    }

    IEnumerator TimeBetweenBlinksCO(float waitTime)
    {
        yield return new WaitForSeconds(Random.Range(waitTime - RandomTimeDefine, waitTime + RandomTimeDefine));

        if(TimeOf != 0)
        {
            StartCoroutine(BlinkToTurnOfCo());
        }
        else
        {
            StartCoroutine(BlinkCo());
        }
    }

    IEnumerator BlinkToTurnOfCo()
    {
        for (int i = 0; i < NumOfBlinks; i++)
        {
            Blink();
            yield return new WaitForSeconds(blinkOfTime);
        }

        if(NumOfBlinks % 2 == 0)
        {
            Blink();
            //blink an extra time if even so that it changes state
        }

        StartCoroutine(TimeBetweenBlinksCO(TimeOf));
    }

    IEnumerator BlinkCo()
    {
        for (int i = 0; i < NumOfBlinks; i++)
        {
            Blink();
            yield return new WaitForSeconds(blinkOfTime);
        }

        StartCoroutine(TimeBetweenBlinksCO(TimeBetweenBlinks));
    }
}
