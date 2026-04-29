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

    //public AudioSource sourceSFX;
    //public AudioSource sourceBGM;

    [Header("BGM")]
    public AudioClip mainMenuBGM;
    public AudioClip sailingBGM;
    public AudioSource sourceMainMenu;
    public AudioSource sourceSailing;

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
        }
    }

    private void Start()
    {
        if (GameController.Instance != null)
        {
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

    // �ṩһ��ͨ�õĲ��Žӿ�
    /*public void PlaySound(AudioClip clip, float volume = 1f)
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
    }*/

    public void PlayMainMenuBGM()
    {
        if (sourceMainMenu == null || mainMenuBGM == null)
            return;

        if (sourceMainMenu.clip != mainMenuBGM)
        {
            sourceMainMenu.clip = mainMenuBGM;
        }

        sourceMainMenu.loop = true;
        if (!sourceMainMenu.isPlaying)
        {
            sourceMainMenu.Play();
        }
    }

    public void PlaySailingBGM()
    {
        if (sailingBGM != null)
        sourceSailing.PlayOneShot(sailingBGM);
    }

    private void HandleGameStateChanged(GameController.GameState state)
    {
        if (state == GameController.GameState.MainMenu)
        {
            PlayMainMenuBGM();
            return;
        }

        if (sourceMainMenu != null && sourceMainMenu.isPlaying)
        {
            sourceMainMenu.Stop();
        }
    }
}
