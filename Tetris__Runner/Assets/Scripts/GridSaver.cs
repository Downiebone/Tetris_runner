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

[System.Serializable]
public class LevelManifest
{
    public LevelEntry[] Levels;
}

[System.Serializable]
public class LevelEntry
{
    public string name;
    public SubfolderEntry[] subfolders;
}

[System.Serializable]
public class SubfolderEntry
{
    public string id;
    public string[] files;
}

public class GridSaver : MonoBehaviour
{

    //THE MANIFEST
    LevelManifest manifest;


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


        addFilesToManifest(from_str, to_str, difficulity, fileName);

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

        //Debug.Log("Loading stats");
        //Debug.Log("h: " + heights + " | w: " + widths);
        //Debug.Log("vectors: " + vectorList.Count);
        //Debug.Log("cells: " + savable_cellsList.Count);

        Cell[,] grid = new Cell[heights, widths];

        for (int i = 0; i < vectorList.Count; i++)
        {
            grid[vectorList[i].y, vectorList[i].x] = savable_cellsList[i];
        }

        grid_return = grid;
    }

    private void Start()
    {
        TextAsset textAsset = Resources.Load<TextAsset>("manifest");
        manifest = JsonUtility.FromJson<LevelManifest>(textAsset.text);

        //string[] levels = GetFiles("Bot", "Mid", 1);

        //for (int i = 0; i < levels.Length; i++)
        //{
        //    Debug.Log(levels[i]);
        //}

    }

    private string[] GetFiles(string from_str, string to_str, int difficulity)
    {
        if (manifest == null)
        {
            Debug.LogError("Manifest not loaded! Call LoadManifest() first.");
            return new string[0];
        }

        string levelName = (from_str + "_" + to_str);
        string subfolderId = difficulity.ToString();

        LevelEntry level = Array.Find(manifest.Levels, l => l.name == levelName);
        if (level == null)
        {
            Debug.LogWarning("Level not found: " + levelName);
            return new string[0];
        }

        SubfolderEntry sub = Array.Find(level.subfolders, s => s.id == subfolderId);
        if (sub == null)
        {
            Debug.LogWarning($"Subfolder {subfolderId} not found in level {levelName}");
            return new string[0];
        }

        return sub.files;
    }

    private bool addFilesToManifest(string from_str, string to_str, int difficulity, string newFile)
    {
        if (manifest == null)
        {
            Debug.LogError("Manifest not loaded!");
            return false;
        }

        string levelName = (from_str + "_" + to_str);
        string subfolderId = difficulity.ToString();

        LevelEntry level = Array.Find(manifest.Levels, l => l.name == levelName);
        if (level == null)
        {
            Debug.LogWarning("Level not found: " + levelName);
            return false;
        }

        SubfolderEntry sub = Array.Find(level.subfolders, s => s.id == subfolderId);
        if (sub == null)
        {
            Debug.LogWarning($"Subfolder {subfolderId} not found in level {levelName}");
            return false;
        }

        // Expand array to add new file
        int oldLength = sub.files.Length;
        Array.Resize(ref sub.files, oldLength + 1);
        sub.files[oldLength] = newFile;

        // Serialize back to JSON with pretty print
        string json = JsonUtility.ToJson(manifest, true);

        // Save to disk
        string manifestPath = "Assets/Resources/manifest.json";
        File.WriteAllText(manifestPath, json);

        // Refresh AssetDatabase so Unity sees the updated file
#if UNITY_EDITOR
        UnityEditor.AssetDatabase.Refresh();
#endif

        Debug.Log($"Added '{newFile}' to {levelName} subfolder {subfolderId}");

        return true;
    }

    //private void test_manifest()
    //{
    //    // Find Bot_Mid
    //    LevelEntry botMid = System.Array.Find(manifest.Levels, l => l.name == "Bot_Mid");

    //    if (botMid != null)
    //    {
    //        // Find subfolder with id "3"
    //        SubfolderEntry sub3 = System.Array.Find(botMid.subfolders, s => s.id == "3");
    //        if (sub3 != null)
    //        {
    //            Debug.Log("Files under Bot_Mid 3:");
    //            foreach (var file in sub3.files)
    //                Debug.Log(file);
    //        }
    //    }
    //}


    /// <summary>
    /// Loads a level from resources at random (using from, to, and difficulity ofcourse).
    /// </summary>
    /// <param name="from_load"> 0-2 in order: "Bot" "Mid" "Top".</param>
    /// <param name="to_load"> 0-2 in order: "Bot" "Mid" "Top".</param>
    /// <param name="difficulity_load">1-5 in rising difficulity (1 = easiest).</param>
    /// <returns></returns>
    public IEnumerator LoadCells_at_random_fromResources(string from_load, string to_load, int difficulity_load)
    {

        string path = "";

        while (true)
        {

            //string originPath = "Assets/Resources/Levels/" + (from_load + "_" + to_load) + "/" + difficulity_load.ToString();


            string[] fileInfo = GetFiles(from_load, to_load, difficulity_load);

            if (fileInfo.Length != 0)
            {
                path = "Levels/" + (from_load + "_" + to_load) + "/" + difficulity_load.ToString() + "/" + fileInfo[UnityEngine.Random.Range(0, fileInfo.Length)];

                break;
            }
            else
            {
                //Debug.Log("could not: " + from_load + "_" + to_load + "/" + difficulity_load.ToString());
                //cycle through difficulities until we find one that has levels
                difficulity_load++;
                if (difficulity_load > 5)
                {
                    difficulity_load = 1;
                }
            }

        }


        //Debug.Log("Loading from: " + path);


        //stuff from dreamers chaos:

        //await Task.Run(() =>
        //{
        //    string path = "MarchingCubes/" + fileName;
        //    loadResourcesFileGAME(path);
        //});

        

        //string data_str = await LoadTextAsync(path + "/dataStr.txt");
        //byte[] vectorString = await LoadBytesAsync(path + "/vectors.bytes");
        //byte[] cellString = await LoadBytesAsync(path + "/cells.bytes");

        //string resources_path = "Levels/" + (from_load + "_" + to_load) + "/" + difficulity_load.ToString();

        ResourceRequest resourceRequest_dataStr = Resources.LoadAsync<TextAsset>(path + "/dataStr");
        yield return resourceRequest_dataStr;
        ResourceRequest resourceRequest_vectors = Resources.LoadAsync<TextAsset>(path + "/vectors");
        yield return resourceRequest_vectors;
        ResourceRequest resourceRequest_cells = Resources.LoadAsync<TextAsset>(path + "/cells");
        yield return resourceRequest_cells;

        string data_str = (resourceRequest_dataStr.asset as TextAsset).text;
        byte[] vectorString = (resourceRequest_vectors.asset as TextAsset).bytes;
        byte[] cellString = (resourceRequest_cells.asset as TextAsset).bytes;

        List<SVector2Int> vectorList = Deserializer<List<SVector2Int>>(vectorString);
        List<Cell> savable_cellsList = Deserializer<List<Cell>>(cellString);

        string[] data_str_parts = data_str.Split(Environment.NewLine, StringSplitOptions.None);
        int heights = int.Parse(data_str_parts[0]);
        int widths = int.Parse(data_str_parts[1]);

        //Debug.Log("Loading stats");
        //Debug.Log("h: " + heights + " | w: " + widths);
        //Debug.Log("vectors: " + vectorList.Count);
        //Debug.Log("cells: " + savable_cellsList.Count);

        Cell[,] grid = new Cell[heights, widths];

        for (int i = 0; i < vectorList.Count; i++)
        {
            grid[vectorList[i].y, vectorList[i].x] = savable_cellsList[i];
        }

        grid_return = grid;

        yield break;
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
