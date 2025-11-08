using UnityEngine;

[System.Serializable]
public struct MusicTrack
{
    public string trackName;
    public AudioClip clip;
}

public class MusicLibrary : MonoBehaviour
{
    public MusicTrack[] tracks;

    private int current_track_index = 0;

    MusicTrack current_track;

    private void Awake()
    {
        current_track = new MusicTrack();
        current_track.clip = null;

        current_track_index = Random.Range(0, tracks.Length);
    }

    public AudioClip GetNextSong()
    {

        current_track = tracks[current_track_index];

        AudioClip return_clip = tracks[current_track_index].clip;
        current_track_index++;

        if(current_track_index >= tracks.Length)
        {
            current_track_index = 0;
        }

        return return_clip;

    }

    public AudioClip GetClipFromName(string trackName)
    {
        foreach (var track in tracks)
        {
            if (track.trackName == trackName)
            {
                return track.clip;
            }
        }
        return null;
    }
}