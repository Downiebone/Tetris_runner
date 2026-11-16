using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[System.Serializable]
public class Zones
{
    public Game_ZoneManager.WorldZones ZoneActual;

    public ZoneSkins[] Skins;

}
[System.Serializable]
public class ZoneSkins
{
    public string SkinName;

    public GameObject startingSegment;

    public GameObject[] mySegments;

    public GameObject endingSegment;
}
public class Game_ZoneManager : MonoBehaviour
{
    public enum WorldZones
    {
        Facility = 0, //base zone. facility background, lasers could come from ceiling?
        Space = 1, //outside the base. Maybe the blocks should look different? Meteors could come regularly
    }

    private WorldZones CurrentZone = WorldZones.Facility;
    private int currentZoneSkinIndex = 0;

    [Header("References")]
    public Transform player;
    public Zones[] _Zones; //cointains tilemaps

    [Header("Settings")]
    public int segmentsOnScreen = 4;     // How many segments stay active at once
    public float spawn_offset = 25f;     // Distance in front of the player where it spawns segments
    public int startOffset = 20;

    private float spawnX = 0f;
    private List<GameObject> activeSegments = new List<GameObject>();

    void Start()
    {
        spawnX += startOffset;
        // Spawn initial segments so the screen starts full
        for (int i = 0; i < segmentsOnScreen; i++)
        {
            SpawnSegment();
        }
    }

    void Update()
    {
        // Check if the players "spawnoffset" is in front of the last spawn_segmentPos
        if (player.position.x + spawn_offset >= spawnX)
        {
            SpawnSegment();
            DeleteOldestSegment();
        }
    }

    //get the index of where in the array of zones that zone that we currently are in, are.
    private int getIndexOfCurrentZone()
    {
        for (int i = 0; i < _Zones.Length; i++)
        {
            if (_Zones[i].ZoneActual == CurrentZone)
            {
                return i;
            }
        }
        return 0;
    }
    int lastNumber = -1;
    int GetRandom(int min, int max)
    {
        int rand;
        do {
            rand = Random.Range(min, max);
        } while (rand == lastNumber);

        lastNumber = rand;
        return rand;
    }
    void SpawnSegment()
    {
        int zoneIndx = getIndexOfCurrentZone();
        int randomIndex = GetRandom(0, _Zones[zoneIndx].Skins[currentZoneSkinIndex].mySegments.Length);

        // Instantiate it at the current spawnX position
        Vector3 spawnPosition = new Vector3(spawnX, 1.5f, 0f);
        GameObject newSegment = Instantiate(_Zones[zoneIndx].Skins[currentZoneSkinIndex].mySegments[randomIndex], spawnPosition, Quaternion.identity, transform);
        Tilemap segmentTile;

        if (newSegment.GetComponent<Tilemap>() == null)
        {
            segmentTile = newSegment.transform.GetChild(0).GetComponent<Tilemap>();
        }
        else
        {
            segmentTile = newSegment.GetComponent<Tilemap>();
        }
        
        segmentTile.CompressBounds();

        activeSegments.Add(newSegment);
        //Debug.Log("new seg: " + segmentTile.cellBounds.xMax.ToString());
        spawnX += segmentTile.cellBounds.xMax; // Move the spawn point forward for the next piece
    }

    void DeleteOldestSegment()
    {
        // Remove the segment that is far behind the player
        Destroy(activeSegments[0]);
        activeSegments.RemoveAt(0);
    }
}
