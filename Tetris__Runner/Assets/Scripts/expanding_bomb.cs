using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class expanding_bomb : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField] private int max_explosions = 3;
    private int current_explosion_radius = 0;

    [SerializeField] private float time_between_explosions = 0.25f;
    private float counter = 10;

    private GridEditor grid_script;

    private Vector2Int my_explosion_pos;

    private bool started = false;

    [Space]
    [Tooltip("starts explosion on load")]
    [SerializeField] private bool start_on_load = false;
    [SerializeField] private bool destroy_on_use = false;

    void Start()
    {
        grid_script = GameObject.FindGameObjectWithTag("Grid").GetComponent<GridEditor>();

        if (start_on_load)
        {
            expanding_explosion(max_explosions, Vector2Int.RoundToInt((Vector2)transform.position));
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(started == false) { return; }

        if(counter < time_between_explosions)
        {
            counter += Time.deltaTime;
        }
        else
        {
            //Debug.Log("curr: " + current_explosion_radius);

            if (current_explosion_radius > max_explosions) {
                if(destroy_on_use)
                {
                    Destroy(this.gameObject);
                }
                started = false;
            } //no more explosions

            

            if (current_explosion_radius == 0){ //initial blast
                grid_script.deleteTile(my_explosion_pos);
            }
            else
            {
                for(int i = -current_explosion_radius; i <= current_explosion_radius; i++)
                {
                    for (int j = -current_explosion_radius; j <= current_explosion_radius; j++)
                    {
                        if(Mathf.Abs(i) + Mathf.Abs(j) == current_explosion_radius) //star shape
                        {
                            grid_script.deleteTile_exeptFloor(my_explosion_pos + new Vector2Int(i, j));
                            //grid_script.bombTile(my_explosion_pos + new Vector2Int(i, j));
                        }
                    }
                }
            }
            counter = 0;
            current_explosion_radius++;
        }
    }

    public void expanding_explosion(int new_max_explosions, Vector2Int pos)
    {
        started = true;

        current_explosion_radius = 0;
        max_explosions = new_max_explosions;
        my_explosion_pos = pos;
        counter = 0;
    }
}
