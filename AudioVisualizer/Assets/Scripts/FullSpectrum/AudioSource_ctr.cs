using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AudioSource_ctr : MonoBehaviour
{
    public Object[] clips;
    public AudioSource _audioSource;
    public TMP_Dropdown songList;


    // Start is called before the first frame update
    void Start()
    {
        clips = Resources.LoadAll("Songs", typeof(AudioClip));

        if (clips.Length > 0)
        {
            _audioSource.clip = (AudioClip)clips[0];
            _audioSource.Play();
            songList.ClearOptions();
            List<string> options = new List<string>();

            for (int i = 0; i < clips.Length; i++)
            {
                options.Add(clips[i].name);
            }
            songList.AddOptions(options);
        }
    }

    public void ChangeTrack()
    {
        _audioSource.Stop();
        _audioSource.clip = (AudioClip)clips[songList.value];
        _audioSource.Play();
    }
}
