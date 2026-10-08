using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Script : MonoBehaviour
{
    [SerializeField] private Draggable_instantiater drag_inst;
    [SerializeField] private Following_player cameraFollow_script;

    [SerializeField] private GridEditor GridScript;

    [SerializeField] private level_play_system play_system;

    [SerializeField] private expanding_bomb expand_bomb;

    public GameObject playerDeath;

    private Cell Cell_body_current;

    private bool Cell_floor_exist;

    private bool Cell_ceiling_exist;

    [SerializeField] private Rigidbody2D RB;
    //[SerializeField] private SpriteRenderer sprit;
    [SerializeField] private Animator playerAnimator;

    [Header("Player Variables")]


    // The initial speed of the player
    public float initialMultiplier = 1.0f;
    // The maximum speed the player will reach
    public float maxMultiplier = 2.0f;
    // The time duration over which the speed increases (175 seconds based on Subway Surfers mechanics)
    public float durationToMaxSpeed = 175f;

    private float speedIncreaseRate;

    [SerializeField] private float player_multiplier = 1;

    // The playerSpeed value used by your player movement script
    [SerializeField] private float playerSpeed = 3.5f;

    [SerializeField] private float acceleration_time = 3;

    //private float bomb_timer = 0;

    private float player_up_speed = 0;

    private bool was_upping = false;

    //[SerializeField] private float blast_speed_multiplier = 1;

    public float gravity_modifier = 1;

    private float acceleration;

    private float counter = 0f;

    private bool im_dead = false;

    Vector2Int player_lastpos;
    bool skipping_steps = false;

    private bool HaveCoinMagnet = false;

    [SerializeField] AudioClip SplatSound;
    [SerializeField] AudioClip JumpSound;


    // ############################################### --- Power Ups --- #################################################

    public enum transformation_type
    {
        Normal = 0,
        Dashing = 1,
        Bird = 2
    }

    public enum KindsOFDeath
    {
        RunIntoWall = 0,
        RunIntoCorner = 1,
        Laser = 2,
        Spike = 3,
        RunIntoWallAir = 4,
    }

    public transformation_type current_form = transformation_type.Normal;

    private float time_in_transformation_counter = 99;
    private float time_before_transformation_ends = 0;

    private float dashing_speed = 1;
    [SerializeField] private float bird_jump_height;

    public void Dash_Power(float dash_timer, float dash_speed)
    {
        current_form = transformation_type.Dashing;
        dashing_speed = dash_speed;

        //visual change
        //sprit.color = Color.green;
        //end

        time_before_transformation_ends = dash_timer;
        time_in_transformation_counter = 0;

        //just in case
        was_upping = false;

        gravity_modifier = 0;
        RB.velocity = new Vector2(0, 0);
        RB.gravityScale = gravity_modifier * player_multiplier;

        //just in case
        player_up_speed = 0;
    }

    public void Bird_Power(float bird_timer)
    {
        drag_inst.enable_top_blockade(true);

        current_form = transformation_type.Bird;

        //short invincibility
        invincible_Counter = 1;

        //visual change
        //sprit.color = Color.yellow;
        //end

        time_before_transformation_ends = bird_timer;
        time_in_transformation_counter = 0;
    }

    bool Have_BombJump = false;

    public void Bomb_player(float bomb_force)
    {
        if (!Have_BombJump) { return; } //cant jump if you dont have item

        if (current_form != transformation_type.Normal) { return; } //cant bomb in form

        playerAnimator.SetTrigger("JumpSilly");

        //Debug.Log("BOMBED!!!!!!!!!");

        //bomb_timer = 0.5f;
        //blast_speed_multiplier = 0.5f;
        //counter = acceleration_time/2;
        was_upping = true;

        gravity_modifier = 0;
        RB.velocity = new Vector2(0, 0);
        RB.gravityScale = gravity_modifier * player_multiplier;

        player_up_speed = bomb_force;
    }

    public void forcedJump(float jumpForce)
    {
        was_upping = true;

        playerAnimator.SetTrigger("JumpSilly");

        MusicManager.Instance.play_soundeffect(JumpSound);

        gravity_modifier = 0;
        RB.velocity = new Vector2(0, 0);
        RB.gravityScale = gravity_modifier * player_multiplier;

        player_up_speed = jumpForce;
    }

    public void PlaceBlockOnPlayer()
    {
        transform.position = new Vector3(transform.position.x, player_lastpos.y + 1, 0); // jump up one block

        RB.gravityScale = 0;
        RB.velocity = new Vector2(0, 0);
    }

    float invincible_Counter = 0;

    private void DIE(KindsOFDeath DeathType)
    {
        if (im_dead) { return; }
        if(current_form == transformation_type.Dashing || invincible_Counter > 0) { return; } //cant die while dashing or invincible

        //if(current_form == transformation_type.Bird) //end bird
        //{
        //    current_form = transformation_type.Normal;
        //    sprit.color = Color.blue;
        //    drag_inst.enable_top_blockade(false);

        //    cameraFollow_script.ShakeStart(1, 0.5f);

        //    Instantiate(playerDeath, transform.position, Quaternion.identity);
        //    expand_bomb.expanding_explosion(20, Vector2Int.RoundToInt((Vector2)transform.position)); //explode a little

        //    return; //nothing more, we only lose bird when dying as bird
        //}

        if (play_system.has_revives()) //we can revive
        {
            //maybe some screen shake? large explosion
            play_system.Remove_Revive();

            cameraFollow_script.ShakeStart(0.8f, 0.5f);

            Instantiate(playerDeath, transform.position, Quaternion.identity);
            expand_bomb.expanding_explosion(25, Vector2Int.RoundToInt((Vector2)transform.position));
        }
        else //we are cooked
        {
            cameraFollow_script.ShakeStart(0.5f, 0.5f);

            im_dead = true;
            RB.gravityScale = 0;
            RB.velocity = new Vector2(0, 0);

            switch (DeathType)
            {
                case KindsOFDeath.RunIntoWall:
                    playerAnimator.SetTrigger("DeathSplatNormal");
                    StartCoroutine(DeathCoroutine());
                    SplatFix();
                    break;
                case KindsOFDeath.RunIntoCorner:
                    playerAnimator.SetTrigger("DeathSplatCorner");
                    StartCoroutine(DeathCoroutine());
                    SplatFix();
                    break;
                case KindsOFDeath.Laser:
                    playerAnimator.SetTrigger("DeathMario");
                    StartCoroutine(DeathCoroutine());
                    break;
                case KindsOFDeath.Spike:
                    playerAnimator.SetTrigger("DeathMario");
                    StartCoroutine(DeathCoroutine());
                    break;
                case KindsOFDeath.RunIntoWallAir:
                    playerAnimator.SetTrigger("DeathSplatNormal");
                    StartCoroutine(DeathCoroutine(true));
                    SplatFix();
                    break;
            }
        }
    }
    private void SplatFix()
    {
        transform.position = new Vector2(Mathf.RoundToInt(transform.position.x - 1), transform.position.y);
        MusicManager.Instance.play_soundeffect(SplatSound);
    }

    private IEnumerator DeathCoroutine(bool ContinueAir = false)
    {
        yield return new WaitForSeconds(1.5f);

        if (ContinueAir)
        {
            playerAnimator.SetTrigger("ContinueAir");
        }
        else
        {
            playerAnimator.SetTrigger("Continue");
        }

        yield return new WaitForSeconds(1f);

        play_system.player_died_collectionFunc();
    }

    public void Revived() //add big fancy explosion?
    {
        im_dead = false;

        playerAnimator.SetTrigger("Run");

        Instantiate(playerDeath, transform.position, Quaternion.identity);
        expand_bomb.expanding_explosion(25, Vector2Int.RoundToInt((Vector2)transform.position));
    }

    private void Start()
    {
        player_multiplier = initialMultiplier;

        // Calculate how much speed to add per second to reach the max speed
        float speedDifference = maxMultiplier - initialMultiplier;
        speedIncreaseRate = speedDifference / durationToMaxSpeed;

        if(PlayerPrefs.GetInt("BombJump") == 2) //1 = item owned, 2 = item selected now
        {
            Have_BombJump = true;
        }
        if (PlayerPrefs.GetInt("CoinMagnet") == 2) //1 = item owned, 2 = item selected
        {
            HaveCoinMagnet = true;
        }
        
    }

    public void checkBodyPosForSpecials(Vector2Int BodyCellPos)
    {
        if (GridScript.Cell_is_active_type(BodyCellPos, Cell.Cell_type.Jumppad))
        {
            forcedJump(17);
        }
        if (GridScript.Cell_is_dangerous(BodyCellPos)) //we die if we are occupying same block as spike
        {
            DIE(KindsOFDeath.Spike);
        }
    }

//    private void PerformBirdJump()
//    {
//#if UNITY_EDITOR
//        if (Input.GetKeyDown(KeyCode.Mouse0))
//        {
//            gravity_modifier = 0;
//            RB.velocity = new Vector2(0, 0);
//            RB.gravityScale = gravity_modifier * player_multiplier;

//            player_up_speed = bird_jump_height;

//            was_upping = true;

//        }
//#else
//        if (Input.touchCount > 0) {
//            Touch touch_0 = Input.GetTouch(0);

//            if (touch_0.phase == TouchPhase.Began)
//            {
//                gravity_modifier = 0;
//                RB.velocity = new Vector2(0, 0);
//                RB.gravityScale = gravity_modifier * player_multiplier;

//                player_up_speed = bird_jump_height;

//                was_upping = true;
//            }
//        }
//#endif
//    }

    private void Update()
    {
        if(Pause_Manager.GAME_IS_PAUSED == true || im_dead) {
            return; 
        }

        if(invincible_Counter > 0)
        {
            invincible_Counter -= Time.deltaTime;
        }

        // Increase speed only if it hasn't reached the maximum yet
        if (player_multiplier < maxMultiplier)
        {
            // Add the speed increase for the time passed since the last frame
            player_multiplier += speedIncreaseRate * Time.deltaTime;

            // Clamp the speed to ensure it doesn't exceed the maximum value
            player_multiplier = Mathf.Clamp(player_multiplier, initialMultiplier, maxMultiplier);
        }

        //set player animation speed. So it matches with game speed
        playerAnimator.SetFloat("RunningMultiplier", player_multiplier * acceleration);

        transform.Translate(Vector2.right * playerSpeed * player_multiplier * acceleration * Time.deltaTime); //movement always happens

        if (current_form != transformation_type.Normal)
        {

            if (time_in_transformation_counter < time_before_transformation_ends)
            {
                time_in_transformation_counter += Time.deltaTime;
            }
            else //end transformation?
            {
                invincible_Counter = 1; //short invinc-timer

                if(current_form == transformation_type.Bird)
                {
                    //was bird?
                    drag_inst.enable_top_blockade(false);
                }

                current_form = transformation_type.Normal;
                dashing_speed = 1;

                gravity_modifier = 1;

                //explode upon ending transformation
                Instantiate(playerDeath, transform.position, Quaternion.identity);
                expand_bomb.expanding_explosion(20, Vector2Int.RoundToInt((Vector2)transform.position));

                //make sure we look normal
                //sprit.color = Color.blue;
            }

            //return; // dont do other stuff while transformed
        }

        if (counter < acceleration_time)
        {
            counter += Time.deltaTime;

            acceleration = Mathf.Lerp(0, 1, counter / acceleration_time);
        }

        if(player_up_speed > 0)
        {
            player_up_speed -= Time.deltaTime * 9.82f * 3;
        }else if (gravity_modifier != 1 && was_upping == true)
        {
            //start falling

            was_upping = false;

            gravity_modifier = 1;
            RB.velocity = new Vector2(0, 0);
            RB.gravityScale = gravity_modifier * player_multiplier;
            //blast_speed_multiplier = 1;
        }
    }

    Vector2Int[] checkCoin_positions =
    {
        new Vector2Int(0,0),
        new Vector2Int(0,1),
        new Vector2Int(1,1),
        new Vector2Int(1,0),
    };
    Vector2Int[] checkMagnetCoin_positions =
    {
        new Vector2Int(0,-1),
        new Vector2Int(1,2),
        new Vector2Int(2,0),
        new Vector2Int(2,1),
        new Vector2Int(0,2),
        new Vector2Int(1,-1),
        new Vector2Int(2,-1),
        new Vector2Int(0,-2),
        new Vector2Int(1,-2),
        //new Vector2Int(0, 3),
        //new Vector2Int(1, 3),
        new Vector2Int(2, 2),
        //new Vector2Int(3, 1),
        //new Vector2Int(3, 0),
    };

    private void check_misc_connection(Vector2Int body_pos)
    {
        checkBodyPosForSpecials(body_pos);

        if (HaveCoinMagnet)
        {
            for (int i = 0; i < checkMagnetCoin_positions.Length; i++)
            {
                if (GridScript.Cell_is_active_type(body_pos + checkMagnetCoin_positions[i], Cell.Cell_type.collectable_coin))
                {
                    GridScript.deleteTile_exeptFloor(body_pos + checkMagnetCoin_positions[i]);
                    play_system.spawn_coin_referance(body_pos + checkMagnetCoin_positions[i], transform);
                }
            }
        }
        else
        {
            for (int i = 0; i < checkCoin_positions.Length; i++)
            {
                if (GridScript.Cell_is_active_type(body_pos + checkCoin_positions[i], Cell.Cell_type.collectable_coin))
                {
                    GridScript.deleteTile(body_pos + checkCoin_positions[i]);
                    play_system.spawn_coin_referance(body_pos + checkCoin_positions[i], transform);
                }
            }
        }
        
    }

    private void FixedUpdate()
    {
        //if(RB.gravityScale == 0) { return; }

        playerAnimator.SetFloat("Downvelocity", RB.velocity.y);
    }

    private void LateUpdate()
    {
        if(Pause_Manager.GAME_IS_PAUSED == true || im_dead) {
            RB.gravityScale = 0;
            RB.velocity = new Vector2(0, 0); //temp fix please
            return;
        }

        if (player_up_speed > 0)
        {
            if (/*player_up_speed > 1 && */((Cell_ceiling_exist && (transform.position.y % 1 <= 0.5f || transform.position.y % 1 >= 0.95f)) || Mathf.RoundToInt(transform.position.y) >= GridScript.gridHeight - 1)) // bumb head on ceiling while rising or top of level
            {
                transform.position = new Vector2(transform.position.x, Mathf.RoundToInt(transform.position.y));
                player_up_speed = 0;
            }
            else
            {
                transform.Translate(Vector2.up * player_up_speed * Time.deltaTime);
            }
        }

        Vector2Int Player_2Int_pos = new Vector2Int(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y));

        if(Player_2Int_pos == player_lastpos)
        {
            skipping_steps = true;
        }
        else
        {
            skipping_steps = false;
            player_lastpos = Player_2Int_pos;
        }

        Vector2Int Floor_pos = Player_2Int_pos + new Vector2Int(0, -1);

        

        

        if(skipping_steps == false)
        {

            GridScript.player_x_val_set(Player_2Int_pos.x);

            Vector2Int Ceiling_pos = Player_2Int_pos + new Vector2Int(0, 1);

            check_misc_connection(Player_2Int_pos);

            Cell_body_current = GridScript.getCellAtPoint(Player_2Int_pos);

            Cell_ceiling_exist = GridScript.Cell_is_walkable(Ceiling_pos);

            if (Cell_floor_exist) //remove perhaps
            {
                if (GridScript.Cell_is_walkable(Cell_body_current) && !Cell_ceiling_exist)
                {
                    if (GridScript.Cell_is_walkable(Ceiling_pos + new Vector2Int(-1, 0))) //this is when you die in a corner
                    {
                        DIE(KindsOFDeath.RunIntoCorner);
                    }
                    else //if (current_form != transformation_type.Dashing) // dashing cant move up
                    {
                        if(transform.position.y + 1 >= GridScript.gridHeight - 0.4f) //stop player from going to top of screen
                        {
                            DIE(KindsOFDeath.RunIntoWall);
                        }
                        else
                        {
                            Vector2 new_pos = new Vector2(transform.position.x, Mathf.RoundToInt(transform.position.y + 1));
                            Floor_pos = new Vector2Int((int)new_pos.x, (int)new_pos.y - 1);


                            transform.position = new_pos;
                            RB.velocity = new Vector2(0, 0);
                        }

                    }
                }
                else if (GridScript.Cell_is_walkable(Cell_body_current))
                {
                    DIE(KindsOFDeath.RunIntoWall);
                }
            }
            else
            {
                if (GridScript.Cell_is_walkable(Cell_body_current) && !Cell_ceiling_exist)
                {
                    if (GridScript.Cell_is_walkable(Ceiling_pos + new Vector2Int(-1, 0))) //this is when you die in a corner
                    {
                        DIE(KindsOFDeath.RunIntoWallAir);
                    }
                    else
                    {
                        // is at very bottom of a block
                        if (transform.position.y % 1 > 0.5f && transform.position.y % 1 < 0.8f)
                        {
                            DIE(KindsOFDeath.RunIntoWallAir);
                        }
                        else
                        {
                            if (transform.position.y + 1 >= GridScript.gridHeight - 0.4f) //stop player from going to top of screen
                            {
                                DIE(KindsOFDeath.RunIntoWallAir);
                            }
                            else
                            {
                                Vector2 new_pos = new Vector2(transform.position.x, Mathf.RoundToInt(transform.position.y + 1));
                                Floor_pos = new Vector2Int((int)new_pos.x, (int)new_pos.y - 1);


                                transform.position = new_pos;
                                RB.velocity = new Vector2(0, 0);
                            }
                        }
                    }
                }
                else if (GridScript.Cell_is_walkable(Cell_body_current))
                {
                    DIE(KindsOFDeath.RunIntoWallAir);
                }
            }
        }
        
        Cell_floor_exist = GridScript.Cell_is_walkable(Floor_pos);

        if (!Cell_floor_exist || (transform.position.y > Floor_pos.y + 1.05f))
        {
            if(player_up_speed > 0) { return; } //dont fall when rising

            if (im_dead || current_form == transformation_type.Dashing) { return; } //dead or dashing are not affected by gravity

            RB.gravityScale = gravity_modifier * player_multiplier;
        }
        else if(Cell_floor_exist) //land on ground ?
        {
            if(RB.gravityScale != 0)
            {
                transform.position = new Vector2(transform.position.x, Mathf.RoundToInt(transform.position.y));
            }
            RB.gravityScale = 0;
            RB.velocity = new Vector2(0,0);
        }
        
    }


}
