using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Script : MonoBehaviour
{

    [SerializeField] private GridEditor GridScript;
    public GameObject playerDeath;

    private Cell Cell_body_current;

    private bool Cell_floor_exist;

    private bool Cell_ceiling_exist;

    [SerializeField] private Rigidbody2D RB;

    [Header("Player Variables")]

    [SerializeField] private float playerSpeed = 10;

    [SerializeField] private float acceleration_time = 1;

    public float gravity_modifier = 1;

    private float acceleration;

    private float counter = 0f;
    private void Update()
    {
        if (counter < acceleration_time)
        {
            counter += Time.deltaTime;

            acceleration = Mathf.Lerp(0, 1, counter / acceleration_time);
        }

        transform.Translate(Vector2.right * playerSpeed * acceleration * Time.deltaTime);
    }

    Vector2Int player_lastpos;
    bool skipping_steps = false;
    private void LateUpdate()
    {
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
            Vector2Int Ceiling_pos = Player_2Int_pos + new Vector2Int(0, 1);

            Cell_body_current = GridScript.Cell_on_position(Player_2Int_pos);

            Cell_ceiling_exist = GridScript.Cell_is_ground(Ceiling_pos);

            if (Cell_floor_exist)
            {
                if (Cell_body_current.isActive && !Cell_ceiling_exist && Cell_body_current.type == Cell.Cell_type.Ground)
                {
                    if (GridScript.Cell_is_ground(Ceiling_pos + new Vector2Int(-1, 0)))
                    {
                        Instantiate(playerDeath, transform.position, Quaternion.identity);
                    }
                    else
                    {
                        //MOVING UP ONE PIECE
                        transform.position = new Vector2(transform.position.x, transform.position.y + 1);
                    }
                }
                else if (Cell_body_current.isActive && Cell_body_current.type == Cell.Cell_type.Ground)
                {
                    Instantiate(playerDeath, transform.position, Quaternion.identity);
                }
            }
            else
            {
                if (Cell_body_current.isActive && Cell_body_current.type == Cell.Cell_type.Ground)
                {
                    Instantiate(playerDeath, transform.position, Quaternion.identity);
                }
            }
        }
        
        Cell_floor_exist = GridScript.Cell_is_ground(Floor_pos);

        if (!Cell_floor_exist || (transform.position.y > Floor_pos.y + 1))
        {
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
