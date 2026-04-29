using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour
{
    // ����ʵ��
    public static AudioManager Instance { get; private set; }

    [Header("Button Sounds")]
    // private AudioSource audioSource;
    public AudioClip clickSound;
    public AudioClip hoverSound;

    public AudioSource sourceSFX;
    public AudioSource sourceBGM;

    [Header("BGM")]
    public AudioClip mainMenuBGM;
    public AudioClip sailingBGM;
    public AudioSource sourceMainMenu;
    public AudioSource sourceSailing;

    private void Awake()
    {
        // ȷ��������ֻ��һ�� AudioManager
        if (Instance == null)
        {
            Instance = this;

            transform.SetParent(null);
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        sourceSFX.playOnAwake = false;
        sourceBGM.playOnAwake = false;

        // Load saved audio settings when switch scenes
        // LoadAudioSettings();
    }

    // �ṩһ��ͨ�õĲ��Žӿ�
    public void PlaySound(AudioClip clip, float volume = 1f)
    {
        if (clip != null)
        {
            sourceSFX.PlayOneShot(clip, volume);
        }
    }

    public void PlayClick()
    {
        if (clickSound != null)
            sourceSFX.PlayOneShot(clickSound);
    }

    public void PlayHover()
    {
        if (hoverSound != null)
            sourceSFX.PlayOneShot(hoverSound);
    }


}
