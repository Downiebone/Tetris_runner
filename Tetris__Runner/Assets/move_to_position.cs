using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class move_to_position : MonoBehaviour
{
    // Start is called before the first frame update
    float counter = 10;
    float time_to_movethere = 0.5f;

    private bool moving_with_referance = false;

    private Vector2 start_pos;
    private Vector2 end_pos;

    private Transform end_pos_referance;

    private SpriteRenderer sprit;

    void Start()
    {
        sprit = GetComponent<SpriteRenderer>();
    }

    public void dotween(Vector2 start__pos, Vector2 end__pos, float time_)
    {
        transform.position = start__pos;
        sprit.enabled = true;
        moving_with_referance = false;

        start_pos = (start__pos - end__pos) + start__pos;
        end_pos = end__pos;

        counter = 0;
        time_to_movethere = time_;
    }

    public void dotween_referance(Vector2 start__pos, Transform end__pos, float time_)
    {
        transform.position = start__pos;
        sprit.enabled = true;
        moving_with_referance = true;

        start_pos = (start__pos - (Vector2)end__pos.transform.position) + start__pos;
        end_pos_referance = end__pos;

        counter = 0;
        time_to_movethere = time_;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (counter < time_to_movethere)
        {
            counter += Time.deltaTime;

            if (moving_with_referance)
            {
                transform.position = Vector2.Lerp(start_pos, (Vector2)end_pos_referance.transform.position, EaseInBack(0.5f, 1, counter / time_to_movethere));
            }
            else
            {
                transform.position = Vector2.Lerp(start_pos, end_pos, EaseInBack(0.5f, 1, counter / time_to_movethere));
            }
        }
        else if(sprit.enabled)
        {
            sprit.enabled = false;
        }
    }

    public static float EaseInOutBack(float start, float end, float value)
    {
        float s = 1.70158f;
        end -= start;
        value /= .5f;
        if ((value) < 1)
        {
            s *= (1.525f);
            return end * 0.5f * (value * value * (((s) + 1) * value - s)) + start;
        }
        value -= 2;
        s *= (1.525f);
        return end * 0.5f * ((value) * value * (((s) + 1) * value + s) + 2) + start;
    }
    public static float EaseInBack(float start, float end, float value)
    {
        end -= start;
        value /= 1;
        float s = 1.70158f;
        return end * (value) * value * ((s + 1) * value - s) + start;
    }

}
