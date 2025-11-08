using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeteorWarning : MonoBehaviour
{
    public Vector2Int[] ExplosionSpots;

    [SerializeField] private AudioClip ljud;

    private bool beginMeteor = false;

    [SerializeField] private Transform meteor;

    [SerializeField] private GameObject graphixObj;

    [SerializeField] private float flashTime = 1.0f;

    [SerializeField] private float extraTime_beforeSpawn = 1.0f;

    [Space]

    [SerializeField] private float downSpeedMin = 3.0f;
    [SerializeField] private float downSpeedMax = 3.0f;

    private GridEditor grid;

    Vector2 flightvector;

    void Start()
    {
        grid = GameObject.FindGameObjectWithTag("Grid").GetComponent<GridEditor>();

        StartCoroutine(flash());

        flightvector = new Vector2(3.5f, Random.Range(-downSpeedMin, -downSpeedMax));
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (!beginMeteor) { return; }

        meteor.Translate(flightvector * Time.deltaTime);

        Vector2Int currSpot = Vector2Int.CeilToInt((Vector2)meteor.position);

        Cell onCell = grid.getCellAtPoint(currSpot);
        if(onCell.isActive == true && onCell.type == Cell.Cell_type.Ground)
        {
            //play some explosionSound
            MusicManager.Instance.play_soundeffect(ljud);

            for (int i = 0; i < ExplosionSpots.Length; i++)
            {
                grid.bombTile(currSpot + ExplosionSpots[i]);
            }

            Destroy(this.gameObject);
        }
        
    }

    IEnumerator flash()
    {
        //play some warning sound

        int manyFlashes = Mathf.RoundToInt(flashTime * 10);
        if(manyFlashes%2 != 0)//make it even
        {
            manyFlashes++;
        }

        for (int i = 0; i < manyFlashes; i++) //even flashes should make it on?
        {
            yield return new WaitForSeconds(0.1f);
            graphixObj.SetActive(!graphixObj.activeSelf);
        }

        yield return new WaitForSeconds(extraTime_beforeSpawn);

        graphixObj.SetActive(false);

        beginMeteor = true;

        transform.SetParent(null); //remove from camera
        transform.position = new Vector3(transform.position.x, transform.position.y, 0);
    }
}
