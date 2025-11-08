using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeteorSpawner : MonoBehaviour
{
    [SerializeField] private GameObject MeteorPrefab;

    [SerializeField] private Transform min_spawnPos;
    [SerializeField] private Transform max_spawnPos;

    [SerializeField] private float Min_timeBeforeMeteors_Spawn = 10;
    [SerializeField] private float Max_timeBeforeMeteors_Spawn = 15;

    [SerializeField] private float Min_timeBetween_meteors = 5;
    [SerializeField] private float Max_timeBetween_meteors = 15;

    [SerializeField] private float Min_timeBetween_meteors_METEORRAIN = 3;
    [SerializeField] private float Max_timeBetween_meteors_METEORRAIN = 7;

    private bool start_spawning = false;

    void Start()
    {
        StartCoroutine(wait_forSpawning_normal(Random.Range(Min_timeBeforeMeteors_Spawn, Max_timeBeforeMeteors_Spawn)));
    }

    IEnumerator wait_forSpawning_normal(float timeBeforeMeteor)
    {
        yield return new WaitForSeconds(timeBeforeMeteor);

        if(Random.Range(0,10) == 0) // 1 in 10 for meteor array?
        {

        }
        else
        {
            spawnOneMeteorRandom();
        }
        StartCoroutine(wait_forSpawning_normal(Random.Range(Min_timeBetween_meteors, Max_timeBetween_meteors)));
    }

    private void spawnOneMeteorRandom()
    {
        Instantiate(MeteorPrefab, new Vector2(Random.Range(min_spawnPos.position.x, max_spawnPos.position.x), min_spawnPos.position.y), Quaternion.identity, transform); //spawn as child of camera
    }

    private void spawnMeteorArray(int numbers)
    {
        float area = max_spawnPos.position.x - min_spawnPos.position.x;

        float step = area / (numbers - 1);

        Instantiate(MeteorPrefab, new Vector2(min_spawnPos.position.x, min_spawnPos.position.y), Quaternion.identity, transform); //spawn as child of camera

        for (int i = 1; i < (numbers); i++)
        {
            Instantiate(MeteorPrefab, new Vector2(min_spawnPos.position.x + (step * i), min_spawnPos.position.y), Quaternion.identity, transform); //spawn as child of camera
        }
    }

    // Update is called once per frame
    void Update()
    {
        //Debug
        if (Input.GetKeyDown(KeyCode.M))
        {
            spawnOneMeteorRandom();
        }
        if (Input.GetKeyDown(KeyCode.N))
        {
            spawnMeteorArray(10);
        }
    }
}
