using UnityEngine;

public class BGMManager : MonoBehaviour
{
    public static BGMManager Instance;

    public AudioSource audioSource;
    public AudioClip bgm1;
    public AudioClip bgm2;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
    }

    public void PlayBGM1()
    {
        audioSource.clip = bgm1;
        audioSource.Play();
    }

    public void PlayBGM2()
    {
        audioSource.clip = bgm2;
        audioSource.Play();
    }

    // Optional: Fancy crossfade
    public void FadeToBGM2(float fadeTime = 1f)
    {
        StartCoroutine(FadeMusic(bgm2, fadeTime));
    }

    private System.Collections.IEnumerator FadeMusic(AudioClip newClip, float duration)
    {
        float startVolume = audioSource.volume;

        // Fade out
        while (audioSource.volume > 0)
        {
            audioSource.volume -= startVolume * Time.deltaTime / duration;
            yield return null;
        }

        audioSource.clip = newClip;
        audioSource.Play();

        // Fade in
        while (audioSource.volume < startVolume)
        {
            audioSource.volume += startVolume * Time.deltaTime / duration;
            yield return null;
        }
    }
}
