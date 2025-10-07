using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Script : MonoBehaviour
{

    [SerializeField] private GridEditor GridScript;

    [SerializeField] private level_play_system play_system;

    [SerializeField] private expanding_bomb expand_bomb;

    public GameObject playerDeath;

    private Cell Cell_body_current;

    private bool Cell_floor_exist;

    private bool Cell_ceiling_exist;

    [SerializeField] private Rigidbody2D RB;
    [SerializeField] private SpriteRenderer sprit;

    [Header("Player Variables")]

    [SerializeField] private float playerSpeed = 10;

    [SerializeField] private float acceleration_time = 1;

    //private float bomb_timer = 0;

    private float player_up_speed = 0;

    private bool was_upping = false;

    //[SerializeField] private float blast_speed_multiplier = 1;

    public float gravity_modifier = 1;

    private float acceleration;

    private float counter = 0f;

    private bool im_dead = false;


    // ############################################### --- Power Ups --- #################################################

    public enum transformation_type
    {
        Normal = 0,
        Dashing = 1,
        Bird = 2
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
        sprit.color = Color.green;
        //end

        time_before_transformation_ends = dash_timer;
        time_in_transformation_counter = 0;

        //just in case
        was_upping = false;

        gravity_modifier = 0;
        RB.velocity = new Vector2(0, 0);
        RB.gravityScale = gravity_modifier;

        //just in case
        player_up_speed = 0;
    }

    public void Bird_Power(float bird_timer)
    {
        current_form = transformation_type.Bird;

        //visual change
        sprit.color = Color.yellow;
        //end

        time_before_transformation_ends = bird_timer;
        time_in_transformation_counter = 0;

        
    }



    public void Bomb_player(float bomb_force)
    {
        if (current_form != transformation_type.Normal) { return; } //cant bomb in form

        //Debug.Log("BOMBED!!!!!!!!!");

        //bomb_timer = 0.5f;
        //blast_speed_multiplier = 0.5f;
        //counter = acceleration_time/2;
        was_upping = true;

        gravity_modifier = 0;
        RB.velocity = new Vector2(0, 0);
        RB.gravityScale = gravity_modifier;

        player_up_speed = bomb_force;
    }

    private void DIE()
    {
        if(current_form == transformation_type.Dashing) { return; } //cant die while dashing

        if(current_form == transformation_type.Bird) //end bird
        {
            current_form = transformation_type.Normal;
            sprit.color = Color.blue;

            Instantiate(playerDeath, transform.position, Quaternion.identity);
            expand_bomb.expanding_explosion(20, Vector2Int.RoundToInt((Vector2)transform.position)); //explode a little

            return; //nothing more, we only lose bird when dying as bird
        }

        if (play_system.has_revives()) //we can revive
        {
            //maybe some screen shake? large explosion
            play_system.Remove_Revive();

            Instantiate(playerDeath, transform.position, Quaternion.identity);
            expand_bomb.expanding_explosion(25, Vector2Int.RoundToInt((Vector2)transform.position));
        }
        else //we are cooked
        {
            im_dead = true;
            play_system.player_died_collectionFunc();
            RB.gravityScale = 0;
            RB.velocity = new Vector2(0, 0);
        }
       
        
    }

    public void Revived() //add big fancy explosion?
    {
        im_dead = false;

        Instantiate(playerDeath, transform.position, Quaternion.identity);
        expand_bomb.expanding_explosion(25, Vector2Int.RoundToInt((Vector2)transform.position));
    }

    private void Update()
    {
        if(Pause_Manager.GAME_IS_PAUSED == true) {
            return; 
        }

        transform.Translate(Vector2.right * playerSpeed * dashing_speed * acceleration * Time.deltaTime); //movement always happens

        if (current_form != transformation_type.Normal)
        {
            if (current_form == transformation_type.Bird)
            {

#if UNITY_EDITOR
                if (Input.GetKeyDown(KeyCode.Mouse0))
                {
                    gravity_modifier = 0;
                    RB.velocity = new Vector2(0, 0);
                    RB.gravityScale = gravity_modifier;

                    player_up_speed = bird_jump_height;

                    was_upping = true;

                }
#else
                if (Input.touchCount > 0) {
                    Touch touch_0 = Input.GetTouch(0);

                    if (touch_0.phase == TouchPhase.Began)
                    {
                        gravity_modifier = 0;
                        RB.velocity = new Vector2(0, 0);
                        RB.gravityScale = gravity_modifier;

                        player_up_speed = bird_jump_height;

                        was_upping = true;
                    }
                }
#endif

            }

            if (time_in_transformation_counter < time_before_transformation_ends)
            {
                time_in_transformation_counter += Time.deltaTime;
            }
            else
            {
                current_form = transformation_type.Normal;
                dashing_speed = 1;

                gravity_modifier = 1;

                //explode upon ending transformation
                Instantiate(playerDeath, transform.position, Quaternion.identity);
                expand_bomb.expanding_explosion(20, Vector2Int.RoundToInt((Vector2)transform.position));

                //make sure we look normal
                sprit.color = Color.blue;
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
            was_upping = false;

            gravity_modifier = 1;
            //RB.velocity = new Vector2(0, 0);
            RB.gravityScale = gravity_modifier;
            //blast_speed_multiplier = 1;
        }
    }

    private void check_misc_connection(Vector2Int body_pos)
    {
        Vector2Int top_pos = body_pos + new Vector2Int(0, 1);
        Vector2Int top_right_pos = body_pos + new Vector2Int(1, 1);
        Vector2Int right_pos = body_pos + new Vector2Int(1, 0);

        if(GridScript.Cell_is_active_type(body_pos, Cell.Cell_type.collectable_coin))
        {
            GridScript.deleteTile(body_pos);
            play_system.spawn_coin_referance(top_right_pos, transform);
        }
        if (GridScript.Cell_is_active_type(top_pos, Cell.Cell_type.collectable_coin))
        {
            GridScript.deleteTile(top_pos);
            play_system.spawn_coin_referance(top_pos, transform.gameObject.transform);
        }
        if (GridScript.Cell_is_active_type(top_right_pos, Cell.Cell_type.collectable_coin))
        {
            GridScript.deleteTile(top_right_pos);
            play_system.spawn_coin_referance(top_right_pos, transform.gameObject.transform);
        }
        if (GridScript.Cell_is_active_type(right_pos, Cell.Cell_type.collectable_coin))
        {
            GridScript.deleteTile(right_pos);
            play_system.spawn_coin_referance(right_pos, transform.gameObject.transform);
        }
    }

    Vector2Int player_lastpos;
    bool skipping_steps = false;
    private void LateUpdate()
    {
        if(Pause_Manager.GAME_IS_PAUSED == true) {
            RB.gravityScale = 0;
            RB.velocity = new Vector2(0, 0); //temp fix please
            return;
        }

        if (player_up_speed > 0)
        {
            if ((Cell_ceiling_exist && transform.position.y % 1 > 0.5f) || Mathf.RoundToInt(transform.position.y) > 10) // bumb head on ceiling while rising or top of level
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

            Cell_ceiling_exist = GridScript.Cell_is_active_type(Ceiling_pos, Cell.Cell_type.Ground);

            if (Cell_floor_exist || true) //remove perhaps
            {
                if (Cell_body_current.isActive && !Cell_ceiling_exist && Cell_body_current.type == Cell.Cell_type.Ground)
                {
                    if (GridScript.Cell_is_active_type(Ceiling_pos + new Vector2Int(-1, 0), Cell.Cell_type.Ground))
                    {
                        Instantiate(playerDeath, transform.position, Quaternion.identity);
                    }
                    else if (current_form != transformation_type.Dashing) // dashing cant move up
                    {
                        //MOVING UP ONE PIECE

                        Vector2 new_pos = new Vector2(transform.position.x, Mathf.RoundToInt(transform.position.y + 1));

                        transform.position = new_pos;
                        RB.velocity = new Vector2(0, 0);
                    }
                }
                else if (Cell_body_current.isActive && Cell_body_current.type == Cell.Cell_type.Ground)
                {
                    DIE();
                }
            }
            else
            {
                if (Cell_body_current.isActive && Cell_body_current.type == Cell.Cell_type.Ground)
                {
                    DIE();
                }
            }
        }
        
        Cell_floor_exist = GridScript.Cell_is_active_type(Floor_pos, Cell.Cell_type.Ground);

        if (!Cell_floor_exist || (transform.position.y > Floor_pos.y + 1))
        {
            if (im_dead || current_form == transformation_type.Dashing) { return; } //dead or dashing are not affected by gravity
            RB.gravityScale = gravity_modifier;
        }
        else
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
