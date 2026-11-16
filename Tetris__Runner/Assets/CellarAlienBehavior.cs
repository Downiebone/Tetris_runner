using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utility
{
    //showcase:
    //this.Invoke(MyFunctionNoParams, 1f);
    //this.Invoke(() => MyFunctionParams(0, false), 1f);

    public static void Invoke(this MonoBehaviour mb, Action f, float delay)
    {
        mb.StartCoroutine(InvokeRoutine(f, delay));
    }

    private static IEnumerator InvokeRoutine(System.Action f, float delay)
    {
        yield return new WaitForSeconds(delay);
        f();
    }
}

public class CellarAlienBehavior : MonoBehaviour
{
    [Header("Walking")]
    //maybe remove +
    [SerializeField] private LayerMask WallLayer;
    [SerializeField] private float toCloseToWallDistance = 1;
    //end remove -
    [SerializeField] private float walkingSpeed = 2;
    [Tooltip("TimeToReachMaxSpeed")][SerializeField] private float acceleration_time = 0.5f;
    private float accelerationCounter;

    [Space]

    [SerializeField] private float AimForTimeToStand = 20;
    [SerializeField] private float AimForTimeToStand_hysteries = 5;
    [SerializeField] private float AimForTimeToSit = 15;
    [SerializeField] private float AimForTimeToSit_hysteries = 5;

    //if alien has stood for too long, it should start walking more. Looking for somewhere to sit
    private bool DesperateForAPlaceToSit = false;

    //these times should kinda even out each other
    [SerializeField] private float timeStandingUp = 0;
    [SerializeField] private float timeSittingDown = 0;

    [SerializeField] private float timerto_ForStanding = 0;
    [SerializeField] private float timerto_ForSitting = 0;

    [Space]
    [SerializeField] private Vector2 RAND_TimeToStandStillBeforeSit = new Vector2(0.3f, 0.8f);
    [SerializeField] private Vector2 RAND_TimeToFindNewWalkToPos = new Vector2(1f, 8f);

    private enum AlienState
    {
        idle = 1,
        crying = 2,
        walking = 3,
        sittning = 4,
        betweenStates = 5,
    }

    private enum AlienEmotions
    {
        Happy = 1,
        Sad = 2,
        Heart = 3,
        Angry = 4,
    }

    [SerializeField] private float currentWalkingToDestination;
    private bool walkingDestination_is_right = true;

    private bool facingRight = true;
    private float lastTimeSinceTurn = 0; //dont turn several times in a row
    private int facingDirection_SpeedMultiplier = 1; // 1 or -1 for right or left
    private float acceleration = 0;
    private AlienState CurrentState = AlienState.idle;

    [Space]
    [Header("Referances")]
    [SerializeField] private Animator alienAnim;
    [SerializeField] private Transform alienVisualTransform;
    [SerializeField] private SpriteRenderer alienRend;
    [SerializeField] ItemPlacerManager PlaceManager;
    public void DelayedStart()
    {
        testIfRoomIsLivable();
    }

    private void DoEmotion(AlienEmotions todo_emotion)
    {
        //terraria "speach bubble"
    }

    //if room isnt livable, alien cries
    private void testIfRoomIsLivable()
    {
        if (!PlaceManager.isCellarLivable()) //room isnt livable
        {
            alienAnim.SetTrigger("Cry");
            CurrentState = AlienState.crying;
        }
        else
        {
            alienAnim.SetBool("Stand", true);
            CurrentState = AlienState.idle;

            //First walk command
            this.Invoke(StartWalkingToPos, UnityEngine.Random.Range(RAND_TimeToFindNewWalkToPos.x, RAND_TimeToFindNewWalkToPos.y));
        }
    }

    //called everytime we place an item, might be used to for alien to express emotion
    public void PlacedItemCall()
    {
        if(CurrentState == AlienState.crying) //if alien is sad, test if place is nice now
        {
            testIfRoomIsLivable();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (CurrentState == AlienState.sittning)
        {
            timeSittingDown += Time.deltaTime;
            if (timeStandingUp > 0)
            {
                timeStandingUp -= Time.deltaTime * 2;
            }

            if (timeSittingDown > timerto_ForSitting) //sat for so long we wanted
            {
                timerto_ForStanding = AimForTimeToStand + UnityEngine.Random.Range(-AimForTimeToStand_hysteries, AimForTimeToStand_hysteries);
                goFromSittingToStanding(); //stop sitting
            }
        }
        else if (CurrentState == AlienState.idle || CurrentState == AlienState.walking)
        {
            timeStandingUp += Time.deltaTime;
            if (timeSittingDown > 0)
            {
                timeSittingDown -= Time.deltaTime;
            }
            if (timeStandingUp > timerto_ForStanding) //stood for so long we wanted
            {
                DesperateForAPlaceToSit = true;
            }
        }
        if (accelerationCounter < acceleration_time)
        {
            accelerationCounter += Time.deltaTime;

            acceleration = Mathf.Lerp(0, 1, accelerationCounter / acceleration_time);
        }

        if (CurrentState == AlienState.walking)
        {
            walking();

            if(walkingDestination_is_right && (transform.position.x >= currentWalkingToDestination)) //reached destination to right
            {
                stopWalkingMoving();
                this.Invoke(StartWalkingToPos, UnityEngine.Random.Range(RAND_TimeToFindNewWalkToPos.x, RAND_TimeToFindNewWalkToPos.y));
            }
            else if (!walkingDestination_is_right && (transform.position.x <= currentWalkingToDestination)) //reached destination to right
            {
                stopWalkingMoving();
                this.Invoke(StartWalkingToPos, UnityEngine.Random.Range(RAND_TimeToFindNewWalkToPos.x, RAND_TimeToFindNewWalkToPos.y));
            }
        }
    }

    void stopWalkingMoving()
    {
        alienAnim.SetBool("Walking", false);
        CurrentState = AlienState.idle;
    }

    void goFromSittingToStanding()
    {
        alienVisualTransform.localPosition = new Vector3(0,0,0); //reset visuals position
        CurrentState = AlienState.idle;
        alienAnim.SetTrigger("StandUp");

        //wait a bit, then find somewhere to go. /2 first time because it should start walking faster when first standing
        this.Invoke(StartWalkingToPos, UnityEngine.Random.Range(RAND_TimeToFindNewWalkToPos.x, RAND_TimeToFindNewWalkToPos.y / 2));
    }

    void StartWalkingToPos()
    {
        accelerationCounter = 0; //make character accelerate
        CurrentState = AlienState.walking;
        alienAnim.SetBool("Walking", true);

        float newX_pos = PlaceManager.GetRandom_X_pos_In_CurrentBounds(transform.position);
        if(Vector2.Distance(transform.position, new Vector2(newX_pos, transform.position.y)) < 0.5f) //if too close to player, do it again for test
        {
            newX_pos = PlaceManager.GetRandom_X_pos_In_CurrentBounds(transform.position);
        }

        Debug.Log("newpos: " + newX_pos.ToString());

        currentWalkingToDestination = newX_pos;
        //if destination x i less than current x, then destination is to the left and wise versa
        walkingDestination_is_right = newX_pos < transform.position.x ? false : true;

        if(walkingDestination_is_right != facingRight) //if we are looking in direction opposite to walking direction, turn.
        {
            turnDirection();
        }

    }

    void sit_or_lay_Atposition(int sit_1Chair_2Couch_3Bed, Vector2 pos)
    {
        CurrentState = AlienState.sittning;

        switch (sit_1Chair_2Couch_3Bed)
        {
            case 1:
                alienAnim.SetTrigger("SitChair");
                break;
            case 2:
                alienAnim.SetTrigger("SitCouch");
                break;
            case 3:
                alienAnim.SetTrigger("LayDown"); //temp in animator atm
                break;
        }

        alienVisualTransform.position = pos;
    }

    private void turnDirection()
    {
        alienAnim.SetTrigger("Flip");
        facingRight = !facingRight;
        facingDirection_SpeedMultiplier = facingDirection_SpeedMultiplier * -1;
        this.Invoke(flipSprite, 0.5f);
    }
    void flipSprite()
    {
        alienRend.flipX = !alienRend.flipX;
    }
    private void turnDirection_withoutAnimation()
    {
        facingRight = !facingRight;
        facingDirection_SpeedMultiplier = facingDirection_SpeedMultiplier * -1;
        alienRend.flipX = !alienRend.flipX;
    }

    void walking()
    {
        //animator walking bool should already be true

        transform.Translate(Vector2.right * walkingSpeed * acceleration * facingDirection_SpeedMultiplier * Time.deltaTime); //movement

        //TurnIfToCloseToWall();
    }
    private void TurnIfToCloseToWall()
    {
        RaycastHit2D hit2 = Physics2D.Raycast(transform.position, Vector2.right, 20, WallLayer);
        RaycastHit2D hit1 = Physics2D.Raycast(transform.position, Vector2.left, 20, WallLayer);
        if (hit1.distance < toCloseToWallDistance) //to close to wall on left
        {
            this.Invoke(turnDirection, UnityEngine.Random.Range(0, 0.5f)); //turn around in 0 to 0.5 seconds
        }
        if (hit2.distance < toCloseToWallDistance) //to close to wall on right
        {
            this.Invoke(turnDirection, UnityEngine.Random.Range(0, 0.5f)); //turn around in 0 to 0.5 seconds
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("ENTER TRIGGER ALIEN");

        CellarSitObj sitObj;
        if(collision.gameObject.GetComponent<CellarSitObj>() != null)
        {
            sitObj = collision.gameObject.GetComponent<CellarSitObj>();
        }
        else
        {
            return;
        }


        if(wantToSit() == true)
        {
            alienAnim.SetBool("Walking", false); //stops walking animation
            CurrentState = AlienState.betweenStates;

            timerto_ForSitting = AimForTimeToSit + UnityEngine.Random.Range(-AimForTimeToSit_hysteries, AimForTimeToSit_hysteries) + timeStandingUp/2; //compensate some(/2) of the time we stood up

            if (facingRight != sitObj.FacingRight) //turn to match placed object direction
                turnDirection_withoutAnimation();


            if (sitObj.hasLayPosition)
            {
                if(UnityEngine.Random.value > 0.5f)
                {
                    Vector2 sitPos = (Vector2)sitObj.gameObject.transform.position + sitObj.Layposition;
                    this.Invoke(() => sit_or_lay_Atposition(3, sitPos), 1f);
                }
                else
                {
                    this.Invoke(() => sit_or_lay_Atposition(
                        sitObj.trueChair_falseCouch == true ? 1 : 2, 
                        getClosestPositionFromSitObj(sitObj)), 
                        UnityEngine.Random.Range(RAND_TimeToStandStillBeforeSit.x, RAND_TimeToStandStillBeforeSit.y));
                }
            }
            else
            {
                this.Invoke(() => sit_or_lay_Atposition(sitObj.trueChair_falseCouch == true ? 1 : 2, 
                    getClosestPositionFromSitObj(sitObj)), 
                    UnityEngine.Random.Range(RAND_TimeToStandStillBeforeSit.x, RAND_TimeToStandStillBeforeSit.y));
            }
        }
    }
    private Vector2 getClosestPositionFromSitObj(CellarSitObj sitObj)
    {
        float shortestDist = 64;
        int retInd = 0;

        for (int i = 0; i < sitObj.SitPositions.Length; i++)
        {
            float objDist = Vector2.Distance((Vector2)sitObj.gameObject.transform.position + sitObj.SitPositions[i], transform.position);
            if (objDist < shortestDist)
            {
                shortestDist = objDist;
                retInd = i;
            }
        }
        return (Vector2)sitObj.gameObject.transform.position + sitObj.SitPositions[retInd];
    }
    
    //check if player wants to sit when reaching something sittable
    private bool wantToSit()
    {
        if(DesperateForAPlaceToSit) //if player stood for too long
        {
            DesperateForAPlaceToSit = false;
            return true;
        }
        if(timeStandingUp > timeSittingDown) //we've stood longer than we sat, so maybe add a small chance to sit?
        {
            if(timeSittingDown == 0)
            {
                if(UnityEngine.Random.value > 0.5f) //50/50
                {
                    return true;
                }
            }
            else if(timeStandingUp > (AimForTimeToStand/2.0f))
            {
                if (UnityEngine.Random.value > 0.8f) //20% chance
                {
                    return true;
                }
            }
        }

        return false;
    }
}
