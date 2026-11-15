using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    public AudioClip backgroundMusic;

    void Start()
    {
        AudioManager.Instance.PlayMusic(backgroundMusic);
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created

}
