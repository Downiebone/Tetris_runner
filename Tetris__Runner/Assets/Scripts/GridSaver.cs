using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using SerializableTypes;

public class GridSaver : MonoBehaviour
{

    private GridEditor GridObj;

    private int CurrentSaveHeight;
    private int CurrentSaveWidth;

    public void Setup_AreaSize(GridEditor GridOb, int height, int width)
    {
        GridObj = GridOb;
        CurrentSaveHeight = height;
        CurrentSaveWidth = width;
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

    public void SaveCellsToJson(string fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName))
            return;


        string path = "Assets/Resources/Levels/" + fileName;

        Debug.Log("SAVING TO " + path);

        if (!File.Exists(path + "/chunks.bytes"))
        {
            Directory.CreateDirectory(path);

        }
        else
        {
            Debug.LogError("FILE ALREADY EXISTS, OVERRIDING OLD SAVE");
        }

        List<SVector2Int> savable_positionList = new List<SVector2Int>();
        List<Cell> savable_cellsList = new List<Cell>();

        for (int h = 0; h < CurrentSaveHeight; h++)
        {
            for (int w = 0; w < CurrentSaveWidth; w++)
            {
                Cell CurrCell = GridObj.Grid[h, w];

                savable_positionList.Add(new SVector2Int(w, h));
                savable_cellsList.Add(GridObj.Grid[h, w]);
            }
        }

        byte[] vectorByte = Serializer(savable_positionList);
        Debug.Log("saving vectorsString: " + vectorByte.Length + " " + savable_positionList.Count);
        byte[] cellByte = Serializer(savable_cellsList);
        Debug.Log("saving chunksString: " + cellByte.Length + " " + savable_cellsList.Count);

        int difficulity = 0;
        byte[] difficulityByte = Serializer(difficulity);




        File.WriteAllBytes(path + "/vectors.bytes", vectorByte);
        File.WriteAllBytes(path + "/cells.bytes", cellByte);
        File.WriteAllBytes(path + "/difficulity.bytes", difficulityByte);


#if UNITY_EDITOR
        UnityEditor.AssetDatabase.Refresh();
#endif

    }
}
