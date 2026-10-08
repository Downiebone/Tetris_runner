using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Placable
{
    public int item_id;
    public int spawnWeight;

    public Placable(int id_, int spawnWeight_)
    {
        item_id = id_;
        spawnWeight = spawnWeight_;
    }
}

public class Draggable_instantiater : MonoBehaviour
{
    [SerializeField] private GridEditor GridScript;

    [SerializeField] private Transform NegX;
    [SerializeField] private Transform PosX;
    [SerializeField] private Transform offside_spawnPos;

    public int NumberOfDraggables_N;

    [SerializeField]
    private List<GameObject> Draggables = new List<GameObject>();

    private draggable_piece currently_chosen_draggable = null;
    public draggable_piece Get_currently_highlighted_Draggable()
    {
        return currently_chosen_draggable;
    }

    private float X_PieceLenght; 

    [SerializeField] private float smoothSpeed = 0.125f;

 // ######  Blocks   --------------
    //0 L
    //1 Penis
    //2 
    //3 
    //4 Bomb
    //5    - Gold Bomb
    //6    - Ghost Block
    [SerializeField] private GameObject[] Spawnable_Draggable;

    private Placable[] weightTablePlaceables;
    int total_placable_weight = 0;

    //float timeBetweenSpawns = 1;
    //public float NormaltimeBetweenSpawns = 0.2f;
    //public float FASTtimeBetweenSpawns = 0.2f;
    //private float timer_ind = 255;

    [Space]
    [Header("PlayerPrefNames")]
    [SerializeField] private string Dynamite_Playerpref;
    [SerializeField] private string GoldBomb_Playerpref;
    [SerializeField] private string Jumppad_Playerpref;
    [SerializeField] private string ColorBomb_Playerpref;

    private void GeneratePlaceableTable()
    {
        weightTablePlaceables = new Placable[Spawnable_Draggable.Length];

        weightTablePlaceables[0] = new Placable(0, 20); // L
        weightTablePlaceables[1] = new Placable(1, 20); // Penis
        weightTablePlaceables[2] = new Placable(2, 20); //
        weightTablePlaceables[3] = new Placable(3, 20); // 
        weightTablePlaceables[4] = new Placable(4, 12); // bomb
        int goldWeight = 0;
        if(PlayerPrefs.GetInt(GoldBomb_Playerpref + "_Selected") == 1)
        {
            goldWeight = PlayerPrefs.GetInt(GoldBomb_Playerpref) * 3;
        }
        weightTablePlaceables[5] = new Placable(5, goldWeight); // gold bomb
        weightTablePlaceables[6] = new Placable(6, 0); // ghost block CURRENTLY NOT USED
        int jumppadWeight = 0;
        if (PlayerPrefs.GetInt(Jumppad_Playerpref + "_Selected") == 1)
        {
            jumppadWeight = PlayerPrefs.GetInt(Jumppad_Playerpref) * 3;
        }
        weightTablePlaceables[7] = new Placable(7, jumppadWeight); // jump pad
        int ColorBombWeight = 0;
        if (PlayerPrefs.GetInt(ColorBomb_Playerpref + "_Selected") == 1)
        {
            ColorBombWeight = PlayerPrefs.GetInt(ColorBomb_Playerpref) * 3;
        }
        weightTablePlaceables[8] = new Placable(8, ColorBombWeight); // Color bomb
        int DynamiteWeight = 0;
        if (PlayerPrefs.GetInt(Dynamite_Playerpref + "_Selected") == 1)
        {
            DynamiteWeight = PlayerPrefs.GetInt(Dynamite_Playerpref) * 3;
        }
        weightTablePlaceables[9] = new Placable(9, DynamiteWeight); // Dynamite

        total_placable_weight = 0;
        foreach (var item in weightTablePlaceables)
        {
            total_placable_weight += item.spawnWeight;
        }
    }

    //check if we have changed draggablelist (for example for some quest)
    public bool hasModifiedDraggableList = false;

    public void ModifyDraggableList(int[] DraggablesToInclude)
    {
        if(DraggablesToInclude.Length > Spawnable_Draggable.Length)
        {
            Debug.LogError("too many draggables to removes");
            throw new System.Exception();
        }

        hasModifiedDraggableList = true;

        GameObject[] TempDraggableList = new GameObject[DraggablesToInclude.Length];

        for (int i = 0; i < DraggablesToInclude.Length; i++)
        {
            if (DraggablesToInclude[i] >= Spawnable_Draggable.Length)
            {
                Debug.LogError("trying to remove draggable outside bounds");
                throw new System.Exception();
            }

            TempDraggableList[i] = Spawnable_Draggable[DraggablesToInclude[i]];
        }

        Spawnable_Draggable = TempDraggableList;
    }

    public GameObject[] HighlightObjects;

    [SerializeField] private GameObject blockadeThing;

    public void enable_top_blockade(bool shouldEnable)
    {
        if (shouldEnable)
        {
            blockadeThing.SetActive(true);
            HideDraggables();
        }
        else
        {
            blockadeThing.SetActive(false);
            ShowDraggables();
        }
    }
    public void HideDraggables()
    {
        for (int i = 0; i < Draggables.Count; i++)
        {
            Draggables[i].GetComponent<draggable_piece>().HideVisuals();
        }
    }
    public void ShowDraggables()
    {
        for (int i = 0; i < Draggables.Count; i++)
        {
            Draggables[i].GetComponent<draggable_piece>().ShowVisuals();
        }
    }

    public void set_currently_highlighted_draggable(draggable_piece draggable_piece_obj)
    {
        if (currently_chosen_draggable == draggable_piece_obj)
        {
            //nothing happens
            return;
        }

        for (int i = 0; i < Draggables.Count; i++)
        {
            if(Draggables[i] == draggable_piece_obj.gameObject)
            {

                if(currently_chosen_draggable != null)
                {
                    currently_chosen_draggable.Un_highlight();

                    draggable_piece_obj.Highlight();

                    currently_chosen_draggable = draggable_piece_obj;
                }
                else
                {
                    draggable_piece_obj.Highlight();

                    currently_chosen_draggable = draggable_piece_obj;
                }
            }
        }
    }

    void Start()
    {
        GeneratePlaceableTable();

        X_PieceLenght = (PosX.localPosition.x - NegX.localPosition.x) / (NumberOfDraggables_N - 1);

        spawn_all_draggables();
    }

    /// <summary>
    /// if we already have instantiated a draggable_piece of the type "index_", return true
    /// otherwise false
    /// </summary>
    private bool alreadyHasDraggableOfIndex(int index_)
    {
        for (int i = 0; i < Draggables.Count; i++)
        {
            if(Draggables[i].GetComponent<draggable_piece>().TypeOfDraggable_Index == index_)
            {
                return true;
            }
        }

        return false;
    }
    List<Placable> removedItemsFromLootTable = new List<Placable>();
    //make sure we dont get same item twice
    private void removeItemFromLootTable(int itemIndex, int itemOG_weight)
    {
        removedItemsFromLootTable.Add(new Placable(itemIndex, itemOG_weight));

        total_placable_weight -= weightTablePlaceables[itemIndex].spawnWeight;
        weightTablePlaceables[itemIndex].spawnWeight = 0;
    }
    //okej, item is gone we can add it back to loot table
    private void addItemBackToLootTable(int index_)
    {

        for (int i = 0; i < removedItemsFromLootTable.Count; i++)
        {
            //if we found the item_index of item we want to add back
            //then add back its spawnWeight, and adjust totalt weight
            if(removedItemsFromLootTable[i].item_id == index_)
            {
                weightTablePlaceables[index_].spawnWeight = removedItemsFromLootTable[i].spawnWeight;
                total_placable_weight += removedItemsFromLootTable[i].spawnWeight;
                removedItemsFromLootTable.RemoveAt(i);
                return;
            }
        }
    }

    private GameObject instantiate_draggable(Vector3 pos, Transform parent = null)
    {
        int randomWeight = Random.Range(1, total_placable_weight);

        for (int i = 0; i < weightTablePlaceables.Length; i++)
        {
            if (randomWeight <= weightTablePlaceables[i].spawnWeight)
            {
                removeItemFromLootTable(weightTablePlaceables[i].item_id, weightTablePlaceables[i].spawnWeight);

                GameObject dragGO = Instantiate(Spawnable_Draggable[weightTablePlaceables[i].item_id], pos, Quaternion.identity, parent);
                dragGO.GetComponent<draggable_piece>().setReferences(this, GridScript);
                dragGO.GetComponent<draggable_piece>().TypeOfDraggable_Index = weightTablePlaceables[i].item_id;
                return dragGO;
            }
            else randomWeight -= weightTablePlaceables[i].spawnWeight;
        }

        return null;
    }

    private void spawn_all_draggables()
    {
        for(int i = 0; i < NumberOfDraggables_N; i++)
        {
            GameObject GO = instantiate_draggable(offside_spawnPos.position, transform);
            GO.GetComponent<draggable_piece>().myDraggableIndex = Draggables.Count;
            Draggables.Add(GO);
        }
        highlight_first_draggable();
    }

    private void highlight_first_draggable()
    {
        currently_chosen_draggable = Draggables[0].GetComponent<draggable_piece>();
        currently_chosen_draggable.delayed_start_highlight = true;
    }

    private void spawn_draggable()
    {
        GameObject GO = instantiate_draggable(offside_spawnPos.position, transform);
        GO.GetComponent<draggable_piece>().myDraggableIndex = Draggables.Count;
        Draggables.Add(GO);
    }

    public void replace_draggable_with_other(draggable_piece new_draggable)
    {
        int draggable_index = 1;

        if (currently_chosen_draggable != null)
        {
            draggable_index = currently_chosen_draggable.myDraggableIndex;
        }

        new_draggable.myDraggableIndex = draggable_index;
        new_draggable.transform.parent = transform;

        currently_chosen_draggable = new_draggable;
        currently_chosen_draggable.Highlight();

        Destroy(Draggables[draggable_index].gameObject);

        Draggables[draggable_index] = new_draggable.gameObject;


    }

    void Update()
    {
        //if (Draggables.Count < NumberOfDraggables_N)
        //{
        //    if (timer_ind < timeBetweenSpawns)
        //    {
        //        timer_ind += Time.deltaTime;
        //    }
        //    else
        //    {
        //        GameObject GO = Instantiate(Spawnable_Draggable[Random.Range(0, Spawnable_Draggable.Length)], offside_spawnPos.position, Quaternion.identity, transform);
        //        GO.GetComponent<draggable_piece>().myDraggableIndex = Draggables.Count;
        //        GO.GetComponent<draggable_piece>().setReferences(this, GridScript);
        //        Draggables.Add(GO);
        //        timer_ind = 0;

        //        if (Draggables.Count == NumberOfDraggables_N)
        //        {
        //            timeBetweenSpawns = NormaltimeBetweenSpawns;
        //        }
        //    }

        //    if (Draggables.Count == 0)
        //    {
        //        timeBetweenSpawns = FASTtimeBetweenSpawns;
        //    }

        //}

        for (int i = 0; i < Draggables.Count; i++)
        {
            if (Draggables[i].GetComponent<draggable_piece>().isBeingDragged) { continue; } //dont move obj while moved by player

            Vector3 pos = new Vector3((NegX.localPosition.x + (i * X_PieceLenght)) + transform.position.x, NegX.position.y, 0);

            Vector3 smoothedPosition = Vector3.Lerp(Draggables[i].transform.position, pos, smoothSpeed * Time.deltaTime);
            Draggables[i].transform.position = smoothedPosition;
        }
    }

    public void RemoveDraggable_atIndex(int index)
    {
        //highlight next draggable
        if(index != NumberOfDraggables_N - 1)
        {
            currently_chosen_draggable = Draggables[index + 1].GetComponent<draggable_piece>();
        }
        else
        {
            currently_chosen_draggable = Draggables[0].GetComponent<draggable_piece>();
        }
        currently_chosen_draggable.Highlight();

        addItemBackToLootTable(Draggables[index].GetComponent<draggable_piece>().TypeOfDraggable_Index);

        Draggables.RemoveAt(index);

        for (int i = 0; i < Draggables.Count; i++)
        {
            Draggables[i].GetComponent<draggable_piece>().myDraggableIndex = i;
        }
        //fill in old spot
        spawn_draggable();
    }

    public void ResetHighlighter_Positions()
    {
        for (int i = 0; i < HighlightObjects.Length; i++)
        {
            HighlightObjects[i].transform.position = new Vector2(-1, -1);
        }
    }
}