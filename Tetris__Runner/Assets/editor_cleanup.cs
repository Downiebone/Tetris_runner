//#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public static class PlayModeWatcher
{
    static PlayModeWatcher()
    {
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
    }

    private static void OnPlayModeStateChanged(PlayModeStateChange state)
    {
        switch (state)
        {
            case PlayModeStateChange.EnteredPlayMode:
                Debug.Log("Entered Play Mode");
                break;

            case PlayModeStateChange.ExitingPlayMode:
                Debug.Log("Exiting Play Mode — unload resources here");
                // For example:
                Resources.UnloadUnusedAssets();
                System.GC.Collect();
                break;
        }
    }
}

//#endif