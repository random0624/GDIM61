using UnityEngine;

[RequireComponent(typeof(SharkChaseController))]
public class SharkChaseAudio : MonoBehaviour
{
    [Header("Clip")]
    [SerializeField] private AudioClip jawsClip;

    [Header("Playback")]
    [SerializeField, Range(0f, 1f)] private float volume = 0.7f;

    [Header("Source (2D)")]
    [SerializeField] private AudioSource jawsAudioSource;

    private SharkChaseController shark;

    private void Awake()
    {
        shark = GetComponent<SharkChaseController>();
        EnsureAudioSource();
    }

    private void Update()
    {
        if (shark == null || jawsAudioSource == null || jawsClip == null)
            return;

        if (!IsSailing() || !shark.IsAggressiveState)
        {
            StopJaws();
            return;
        }

        if (!jawsAudioSource.isPlaying)
        {
            StartJaws();
        }
    }

    private bool IsSailing()
    {
        return GameController.Instance != null &&
               GameController.Instance.CurrentState == GameController.GameState.Sailing;
    }

    private void EnsureAudioSource()
    {
        if (jawsAudioSource == null)
        {
            jawsAudioSource = GetComponent<AudioSource>();
        }

        if (jawsAudioSource == null)
        {
            jawsAudioSource = gameObject.AddComponent<AudioSource>();
        }

        jawsAudioSource.playOnAwake = false;
        jawsAudioSource.loop = true;
        jawsAudioSource.spatialBlend = 0f;
    }

    private void StartJaws()
    {
        jawsAudioSource.clip = jawsClip;
        jawsAudioSource.volume = volume;
        jawsAudioSource.Play();
    }

    private void StopJaws()
    {
        if (jawsAudioSource != null && jawsAudioSource.isPlaying)
        {
            jawsAudioSource.Stop();
        }
    }

    private void OnDisable()
    {
        StopJaws();
    }
}
