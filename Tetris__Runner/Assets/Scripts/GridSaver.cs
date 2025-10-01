using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using SerializableTypes;
using System;
using TMPro;
using System.Threading.Tasks;

public class GridSaver : MonoBehaviour
{
    [SerializeField] private GridEditor GridObj;

    private int CurrentSaveHeight;
    private int CurrentSaveWidth;

    private int difficulity = 1;

    public string save_level_name;

    private string from_str = "Bot";
    private string to_str = "Bot";

    public void on_inputField_changed_test(TMP_InputField val)
    {
        Debug.Log("value_changed: " + val.text);
        save_level_name = val.text;
    }

    public void From_Changed_value(TMP_Dropdown change)
    {
        from_str = change.value == 2 ? "Top" : change.value == 1 ? "Mid" : "Bot";
        Debug.Log("from_str: " + from_str);
    }
    public void To_Changed_value(TMP_Dropdown change)
    {
        to_str = change.value == 2 ? "Top" : change.value == 1 ? "Mid" : "Bot";
        Debug.Log("To_str: " + to_str);
    }
    public void DifficulityChanged(TMP_Dropdown change)
    {
        Debug.Log("New Value : " + (change.value + 1));
        difficulity = (change.value + 1);
    }

    public void Setup_AreaSize(GridEditor GridOb, int height, int width)
    {
        GridObj = GridOb;
        CurrentSaveHeight = height;
        CurrentSaveWidth = width;
    }

    public void public_saveFunc()
    {
        SaveCells(save_level_name);
    }

    private byte[] Serializer(object _object)
    {
        byte[] bytes;
        using (var _MemoryStream = new MemoryStream())
        {
            IFormatter _BinaryFormatter = new BinaryFormatter();
            _BinaryFormatter.Serialize(_MemoryStream, _object);
            bytes = _MemoryStream.ToArray();
        }
        return bytes;
    }

    private T Deserializer<T>(byte[] _byteArray)
    {
        T ReturnValue;
        using (var _MemoryStream = new MemoryStream(_byteArray))
        {
            IFormatter _BinaryFormatter = new BinaryFormatter();
            ReturnValue = (T)_BinaryFormatter.Deserialize(_MemoryStream);
        }
        return ReturnValue;
    }

    public void SaveCells(string fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName))
            return;


        string path = "Assets/Resources/Levels/" + (from_str + "_" + to_str) + "/" + difficulity.ToString() + "/" +  fileName;

        Debug.Log("SAVING TO " + path);

        if (!File.Exists(path + "/chunks.bytes"))
        {
            Directory.CreateDirectory(path);

        }
        else
        {
            Debug.LogError("FILE ALREADY EXISTS, OVERRIDING OLD SAVE");
        }
        CurrentSaveHeight = GridObj.gridHeight_SAVE;
        CurrentSaveWidth = GridObj.gridLength_SAVE;

        List<SVector2Int> savable_positionList = new List<SVector2Int>();
        List<Cell> savable_cellsList = new List<Cell>();

        for (int h = 0; h < CurrentSaveHeight; h++)
        {
            for (int w = 0; w < CurrentSaveWidth; w++)
            {
                Cell CurrCell = GridObj.Grid[h, w];

                CurrCell.convertToSavableColor();

                savable_positionList.Add(new SVector2Int(w, h));
                savable_cellsList.Add(GridObj.Grid[h, w]);
            }
        }

        byte[] vectorByte = Serializer(savable_positionList);
        Debug.Log("saving vectorsString: " + vectorByte.Length + " " + savable_positionList.Count);
        byte[] cellByte = Serializer(savable_cellsList);
        Debug.Log("saving chunksString: " + cellByte.Length + " " + savable_cellsList.Count);

        //byte[] difficulityByte = Serializer(difficulity);
        //byte[] rarityByte = Serializer(rarity);

        string dataStr = 
              CurrentSaveHeight.ToString() + Environment.NewLine
            + CurrentSaveWidth.ToString() + Environment.NewLine
            + difficulity.ToString() + Environment.NewLine;




        File.WriteAllBytes(path + "/vectors.bytes", vectorByte);
        File.WriteAllBytes(path + "/cells.bytes", cellByte);
        File.WriteAllText(path + "/dataStr.txt", dataStr);


#if UNITY_EDITOR
        UnityEditor.AssetDatabase.Refresh();
#endif

    }

    //public void Load_Grid_size(string fileName, string from_load, string to_load, int difficulity_load, ref int height, ref int width)
    //{
    //    if (string.IsNullOrWhiteSpace(fileName))
    //        return;


    //    string path = "Levels/" + (from_load + "_" + to_load) + "/" + difficulity_load.ToString() + "/" + fileName;

    //    if (!File.Exists(path + "/cells.bytes"))
    //    {
    //        Debug.LogError("FILE DOEST NOT EXIST, CANT LOAD");
    //        return;
    //    }


    //}

    public Cell[,] grid_return;

    [Tooltip("returns width of gird-part")]
    public async Task LoadCells(string fileName, string from_load, string to_load, int difficulity_load)
    {
        if (string.IsNullOrWhiteSpace(fileName))
            return;


        string path = "Assets/Resources/Levels/" + (from_load + "_" + to_load) + "/" + difficulity_load.ToString() + "/" + fileName;
#if UNITY_EDITOR || true

#else //TODO: MAKE IT LOAD FROM RESOURCES!! maybe: https://answers.unity.com/questions/8187/how-can-i-read-binary-files-from-resources.html

#endif

        Debug.Log("Loading from: " + path);

        if (!File.Exists(path + "/cells.bytes"))
        {
            Debug.LogError("FILE DOEST NOT EXIST, CANT LOAD");
            return;
        }

        string data_str = await LoadTextAsync(path + "/dataStr.txt");
        byte[] vectorString = await LoadBytesAsync(path + "/vectors.bytes");
        byte[] cellString = await LoadBytesAsync(path + "/cells.bytes");

        List<SVector2Int> vectorList = Deserializer<List<SVector2Int>>(vectorString);
        List<Cell> savable_cellsList = Deserializer<List<Cell>>(cellString);

        string[] data_str_parts = data_str.Split(Environment.NewLine, StringSplitOptions.None);
        int heights = int.Parse(data_str_parts[0]);
        int widths = int.Parse(data_str_parts[1]);

        Debug.Log("Loading stats");
        Debug.Log("h: " + heights + " | w: " + widths);
        Debug.Log("vectors: " + vectorList.Count);
        Debug.Log("cells: " + savable_cellsList.Count);

        Cell[,] grid = new Cell[heights, widths];

        for(int i = 0; i < vectorList.Count; i++)
        {
            grid[vectorList[i].y, vectorList[i].x] = savable_cellsList[i];
        }

        grid_return = grid;
    }

    //private void Start()
    //{
    //    test_read_directory("Bot", "Mid", 1);
    //}

    //public void test_read_directory(string from_load, string to_load, int difficulity_load)
    //{
    //    string path = "Assets/Resources/Levels/" + (from_load + "_" + to_load) + "/" + difficulity_load.ToString();

    //    string[] fileInfo = Directory.GetDirectories(path);

    //    for (int i = 0; i < fileInfo.Length; i++)
    //    {

    //        fileInfo[i] = fileInfo[i].Replace("\\", "/");

    //        Debug.Log("fil: " + fileInfo[i]);
    //    }
    //}

    public async Task LoadCells_at_random(string from_load, string to_load, int difficulity_load)
    {

        string path = "";

        while (true)
        {

            string originPath = "Assets/Resources/Levels/" + (from_load + "_" + to_load) + "/" + difficulity_load.ToString();

#if UNITY_EDITOR || true

#else //TODO: MAKE IT LOAD FROM RESOURCES!! maybe: https://answers.unity.com/questions/8187/how-can-i-read-binary-files-from-resources.html

            //stuff from dreamers chaos:
            if (!inEditorScene)
            {
                string path = "MarchingCubes/" + fileName;
                LoadedvectorTextAsset = (Resources.Load(path + "/vectors") as TextAsset).bytes;
                LoadedchunkTextAsset = (Resources.Load(path + "/chunks") as TextAsset).bytes;
                LoadedcolorTextAsset = (Resources.Load(path + "/colors") as TextAsset).bytes;
            }
            await Task.Run(() =>
            {
                string path = "MarchingCubes/" + fileName;
                loadResourcesFileGAME(path);
            });

#endif
            string[] fileInfo = Directory.GetDirectories(originPath);

            if(fileInfo.Length != 0)
            {
                for (int i = 0; i < fileInfo.Length; i++)
                {

                    fileInfo[i] = fileInfo[i].Replace("\\", "/");

                    //Debug.Log("fil: " + fileInfo[i]);
                }


                path = fileInfo[UnityEngine.Random.Range(0, fileInfo.Length)];

                break;
            }
            else
            {
                //cycle through difficulities until we find one that has levels
                difficulity_load++;
                if(difficulity_load > 5)
                {
                    difficulity_load = 1;
                }
            }

        }


        Debug.Log("Loading from: " + path);

        if (!File.Exists(path + "/cells.bytes"))
        {
            Debug.LogError("FILE DOEST NOT EXIST, CANT LOAD: " + path);
            return;
        }

        string data_str = await LoadTextAsync(path + "/dataStr.txt");
        byte[] vectorString = await LoadBytesAsync(path + "/vectors.bytes");
        byte[] cellString = await LoadBytesAsync(path + "/cells.bytes");

        List<SVector2Int> vectorList = Deserializer<List<SVector2Int>>(vectorString);
        List<Cell> savable_cellsList = Deserializer<List<Cell>>(cellString);

        string[] data_str_parts = data_str.Split(Environment.NewLine, StringSplitOptions.None);
        int heights = int.Parse(data_str_parts[0]);
        int widths = int.Parse(data_str_parts[1]);

        Debug.Log("Loading stats");
        Debug.Log("h: " + heights + " | w: " + widths);
        Debug.Log("vectors: " + vectorList.Count);
        Debug.Log("cells: " + savable_cellsList.Count);

        Cell[,] grid = new Cell[heights, widths];

        for (int i = 0; i < vectorList.Count; i++)
        {
            grid[vectorList[i].y, vectorList[i].x] = savable_cellsList[i];
        }

        grid_return = grid;
    }

    public async Task<byte[]> LoadBytesAsync(string filePath)
    {
        // File.ReadAllBytesAsync is already async, no need for Task.Run
        return await File.ReadAllBytesAsync(filePath);
    }

    public async Task<string> LoadTextAsync(string filePath)
    {
        // File.ReadAllBytesAsync is already async, no need for Task.Run
        return await File.ReadAllTextAsync(filePath);
    }
}
