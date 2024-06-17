using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Draggable_instantiater : MonoBehaviour
{
    [SerializeField] private GridEditor GridScript;

    [SerializeField] private Transform NegX;
    [SerializeField] private Transform PosX;
    [SerializeField] private Transform offside_spawnPos;

    public int NumberOfDraggables_N;

    private List<GameObject> Draggables = new List<GameObject>();
    private float X_PieceLenght;

    [SerializeField] private float smoothSpeed = 0.125f;

    [SerializeField] private GameObject Spawnable_Draggable;

    float timeBetweenSpawns = 1;
    public float NormaltimeBetweenSpawns = 0.2f;
    public float FASTtimeBetweenSpawns = 0.2f;
    private float timer_ind = 255;

    public GameObject[] HighlightObjects;

    void Start()
    {
        X_PieceLenght = (PosX.localPosition.x - NegX.localPosition.x) / (NumberOfDraggables_N - 1);
    }

    void Update()
    {
        if (Draggables.Count < NumberOfDraggables_N)
        {
            if (timer_ind < timeBetweenSpawns)
            {
                timer_ind += Time.deltaTime;
            }
            else
            {
                GameObject GO = Instantiate(Spawnable_Draggable, offside_spawnPos.position, Quaternion.identity, transform);
                GO.GetComponent<draggable_piece>().myDraggableIndex = Draggables.Count;
                GO.GetComponent<draggable_piece>().setReferences(this, GridScript);
                Draggables.Add(GO);
                timer_ind = 0;

                if (Draggables.Count == NumberOfDraggables_N)
                {
                    timeBetweenSpawns = NormaltimeBetweenSpawns;
                }
            }

            if (Draggables.Count == 0)
            {
                timeBetweenSpawns = FASTtimeBetweenSpawns;
            }

        }

        for (int i = 0; i < Draggables.Count; i++)
        {
            if (Draggables[i].GetComponent<draggable_piece>().isBeingDragged) { continue; } //dont move obj while moved by player

            Vector3 pos = new Vector3((NegX.localPosition.x + (i * X_PieceLenght)) + transform.position.x, NegX.position.y, 0);

            Vector3 smoothedPosition = Vector3.Lerp(Draggables[i].transform.position, pos, smoothSpeed);
            Draggables[i].transform.position = smoothedPosition;
        }
    }

    public void RemoveDraggable_atIndex(int index)
    {
        Draggables.RemoveAt(index);

        for (int i = 0; i < Draggables.Count; i++)
        {
            Draggables[i].GetComponent<draggable_piece>().myDraggableIndex = i;
        }
    }

    public void ResetHighlighter_Positions()
    {
        for (int i = 0; i < HighlightObjects.Length; i++)
        {
            HighlightObjects[i].transform.position = new Vector2(-1, -1);
        }
    }
}