using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformationManager : MonoBehaviour
{
    public bool player_Is_Transformed = false;

    [Space]

    [SerializeField] private float DistanceToCountAsTouching_TransObj = 1;
    [SerializeField] private float X_DistanceSufficientlyBehindPlayer = 5;

    private bool TransformationSpawn_IsSpawned = false;
    //there will always only be one, so it can just be in the world. Inactive
    [SerializeField] private GameObject SpawnTransformationObject;

    float timeUntilNext_TransformationSpawn = 0;
    [SerializeField] private int minTimeFor_TransSpawn = 0;
    [SerializeField] private int maxTimeFor_TransSpawn = 0;
    [SerializeField] private GridEditor gridScript;

    void Start()
    {
        setTimeTillNext_Transformation();
    }

    public void Spawn_TransformationObject(Vector3 spawnPos)
    {
        Debug.Log("SPAWNED TRANS");

        SpawnTransformationObject.SetActive(true);

        SpawnTransformationObject.transform.position = spawnPos;

        TransformationSpawn_IsSpawned = true;
    }

    private void Collect_TransformationObject()
    {
        Debug.Log("COLLECTED TRANS");

        SpawnTransformationObject.SetActive(false);
        //Probably do some kind of effect here. A pickup effect
        //Also do some sound effect

        TransformationSpawn_IsSpawned = false;

        //DO THE ACTUAL TRANSFORMATION HERE
        //Some UI Stuff
        //Freeze the game while the transformations "constructs"
        //Then slowly unfreeze the game

        //DEBUG ONLY. LATER IT SHOULD ONLY START SPAWNING AGAIN AFTER WE "DIE" WITH THE TRANSFORMATION
        setTimeTillNext_Transformation();
    }
    private void MissToCollect_TransformationObject()
    {
        Debug.Log("MISSED TRANS");

        SpawnTransformationObject.SetActive(false);

        TransformationSpawn_IsSpawned = false;

        setTimeTillNext_Transformation();
    }

    bool countingDown = false;
    void setTimeTillNext_Transformation()
    {
        timeUntilNext_TransformationSpawn = Random.Range(minTimeFor_TransSpawn, maxTimeFor_TransSpawn);
        countingDown = true;
    }

    void Update()
    {

        if(countingDown == true)
        {
            timeUntilNext_TransformationSpawn -= Time.deltaTime;
            if(timeUntilNext_TransformationSpawn <= 0)
            {
                countingDown = false;
                gridScript.TimeToSpawn_TransObj();
            }
        }
    }

    private void FixedUpdate()
    {
        //dont check for collision with trans_obj if it doesnt exist right now
        if(!TransformationSpawn_IsSpawned) { return; }

        if(Vector2.Distance(transform.position, SpawnTransformationObject.transform.position) <= DistanceToCountAsTouching_TransObj)
        {
            Collect_TransformationObject();
        }
        if(SpawnTransformationObject.transform.position.x <= (transform.position.x - X_DistanceSufficientlyBehindPlayer))
        {
            MissToCollect_TransformationObject();
        }

    }
}
