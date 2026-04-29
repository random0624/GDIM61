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

    public AudioClip sailBGM;
    private void Awake()
    {
        // ȷ��������ֻ��һ�� AudioManager
       /* if (Instance == null)
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
        // LoadAudioSettings();*/
    }

    private void OnEnable()
    {
        if (GameController.Instance != null)
        {
            GameController.Instance.OnStateChanged += HandleGameStateChanged;
            HandleGameStateChanged(GameController.Instance.CurrentState);
        }
    }

    private void OnDisable()
    {
        if (GameController.Instance != null)
        {
            GameController.Instance.OnStateChanged -= HandleGameStateChanged;
        }
    }

    private void HandleGameStateChanged(GameController.GameState state)
    {
        if (state == GameController.GameState.MainMenu)
        {
            PlaySailBGM();
        }
        else
        {
            StopBGM();
        }
    }

    private void PlaySailBGM()
    {
        if (sourceBGM == null || sailBGM == null)
            return;

        if (sourceBGM.clip != sailBGM)
        {
            sourceBGM.clip = sailBGM;
        }

        if (!sourceBGM.isPlaying)
        {
            sourceBGM.loop = true;
            sourceBGM.Play();
        }
    }

    private void StopBGM()
    {
        if (sourceBGM != null && sourceBGM.isPlaying)
        {
            sourceBGM.Stop();
        }
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
