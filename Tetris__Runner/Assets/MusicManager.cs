using System.Collections;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;

    [SerializeField]
    private MusicLibrary musicLibrary;
    [SerializeField]
    private AudioSource musicSource;

    [SerializeField]
    private AudioSource effectsSource;

    [SerializeField]
    private AudioSource coinSource;
    [SerializeField]
    private AudioClip coinSound;

    private float damping_multiplier = 1.0f;

    private void Awake()
    {
        if (Instance != null)
        {
            MusicManager.Instance.unDamp_music();

            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    private bool mid_transition = false;
    private float max_vol_toReach = 1;

    private void Start()
    {
        start_music_loop(8);

        max_vol_toReach = PlayerPrefs.GetFloat("Sound_Music");
    }

    public void dampen_music(float music_percen)
    {
        damping_multiplier = music_percen;

        if (!mid_transition)
        {
            musicSource.volume = max_vol_toReach * damping_multiplier;
        }
    }
    public void unDamp_music()
    {
        damping_multiplier = 1; //max?

        if (!mid_transition)
        {
            musicSource.volume = max_vol_toReach * damping_multiplier;
        }
    }

    public void play_soundeffect(AudioClip sund, bool pitchVarying = false)
    {
        if (pitchVarying)
        {
            effectsSource.pitch = Random.Range(0.8f, 1.2f);
        }
        else
        {
            effectsSource.pitch = 1;
        }

        effectsSource.PlayOneShot(sund);
    }

    float coinTimer = 0;
    int coinsCollected_withinTime = 0;

    public void play_coinEffect()
    {
        bool dontPlaySoundAgain = false; // dont play coin sound to close to eachother

        if(coinTimer > 0) //collected another coin within time_frame
        {
            if (coinTimer > 0.9f) {
                dontPlaySoundAgain = true;
            }
            coinsCollected_withinTime++;

        }
        else
        {
            coinsCollected_withinTime = 0;
        }
        coinTimer = 1;

        

        if (!dontPlaySoundAgain)
        {
            coinSource.pitch = Mathf.Min(0.7f + (0.05f * coinsCollected_withinTime), 2);

            coinSource.PlayOneShot(coinSound);
        }
    }

    private void Update()
    {
        if(coinTimer > 0)
        {
            coinTimer -= Time.deltaTime;
        }
    }


    public void PlayMusic(string trackName, float fadeDuration = 0.5f)
    {
        StartCoroutine(AnimateMusicCrossfade(musicLibrary.GetClipFromName(trackName), fadeDuration));
    }

    float my_loop_fadeduration = 0;

    bool first_time_around = true;

    public void start_music_loop(float fadeDuration = 2)
    {
        my_loop_fadeduration = fadeDuration;

        AudioClip next_clip = musicLibrary.GetNextSong();

        StartCoroutine(AnimateMusicCrossfade(next_clip, my_loop_fadeduration));

        StartCoroutine(waitforsec_forNextSong(Random.Range(next_clip.length/2.0f, next_clip.length - (my_loop_fadeduration + 1)))); //song will play between 50% and 100% before switch to next
    }

    

    public void update_max_volume(float max_vol)
    {
        if (!mid_transition)
        {
            musicSource.volume = max_vol * damping_multiplier;
        }

        max_vol_toReach = max_vol;
    }

    IEnumerator AnimateMusicCrossfade(AudioClip nextTrack, float fadeDuration = 0.5f)
    {
        mid_transition = true;

        float percent = 0;

        if (!first_time_around) //dont fade out first time
        {
            while (percent < 1)
            {
                percent += Time.deltaTime * 1 / fadeDuration;
                musicSource.volume = Mathf.Lerp(max_vol_toReach * damping_multiplier, 0, percent);
                yield return null;
            }
        }
        else
        {
            first_time_around = false;
        }
        

        musicSource.clip = nextTrack;
        musicSource.Play();

        percent = 0;
        while (percent < 1)
        {
            percent += Time.deltaTime * 1 / fadeDuration;
            musicSource.volume = Mathf.Lerp(0, max_vol_toReach * damping_multiplier, percent);
            yield return null;
        }
        mid_transition = false;
    }

    IEnumerator waitforsec_forNextSong(float sec)
    {
        yield return new WaitForSeconds(sec);

        AudioClip next_clip = musicLibrary.GetNextSong();

        StartCoroutine(AnimateMusicCrossfade(next_clip, my_loop_fadeduration));

        StartCoroutine(waitforsec_forNextSong(Random.Range(next_clip.length / 2.0f, next_clip.length - (my_loop_fadeduration + 1))));
    }
}