using UnityEngine;

[RequireComponent(typeof(BoatController))]
public class BoatAudio : MonoBehaviour
{
    [Header("Clip")]
    [SerializeField] private AudioClip waterSplashClip;

    [Header("Playback")]
    [Tooltip("Move speed (units/sec) at or above which splash audio plays.")]
    [SerializeField] private float velocityThreshold = 1.2f;
    [Tooltip("Stop when speed drops below threshold × this value (avoids rapid start/stop).")]
    [SerializeField, Range(0.5f, 1f)] private float stopHysteresis = 0.85f;
    [SerializeField, Range(0f, 1f)] private float volume = 0.65f;
    [SerializeField] private bool scaleVolumeWithSpeed = true;
    [SerializeField] private float maxSpeedForFullVolume = 4f;
    [Tooltip("Floor for scaled volume so splash is audible right above the threshold.")]
    [SerializeField, Range(0f, 1f)] private float minVolumeWhenPlaying = 0.4f;

    [Header("Source (2D)")]
    [SerializeField] private AudioSource splashAudioSource;

    private BoatController boat;

    private void Awake()
    {
        boat = GetComponent<BoatController>();
        EnsureAudioSource();
    }

    private void Update()
    {
        if (boat == null || splashAudioSource == null || waterSplashClip == null)
            return;

        if (!IsSailing())
        {
            StopSplash();
            return;
        }

        float speed = boat.CurrentMoveSpeed;
        float stopSpeed = velocityThreshold * stopHysteresis;
        bool shouldPlay = ShouldPlaySplash(speed);

        if (!splashAudioSource.isPlaying)
        {
            if (shouldPlay)
            {
                StartSplash(speed);
            }
        }
        else if (!shouldPlay || speed < stopSpeed)
        {
            StopSplash();
        }
        else if (scaleVolumeWithSpeed)
        {
            splashAudioSource.volume = GetVolumeForSpeed(speed);
        }
    }

    private bool ShouldPlaySplash(float speed)
    {
        return speed >= velocityThreshold && boat.HasMovementInput;
    }

    private bool IsSailing()
    {
        return GameController.Instance != null &&
               GameController.Instance.CurrentState == GameController.GameState.Sailing;
    }

    private void EnsureAudioSource()
    {
        if (splashAudioSource == null)
        {
            splashAudioSource = GetComponent<AudioSource>();
        }

        if (splashAudioSource == null)
        {
            splashAudioSource = gameObject.AddComponent<AudioSource>();
        }

        splashAudioSource.playOnAwake = false;
        splashAudioSource.loop = true;
        splashAudioSource.spatialBlend = 0f;
    }

    private void StartSplash(float speed)
    {
        splashAudioSource.clip = waterSplashClip;
        splashAudioSource.volume = GetVolumeForSpeed(speed);
        splashAudioSource.Play();
    }

    private void StopSplash()
    {
        if (splashAudioSource != null && splashAudioSource.isPlaying)
        {
            splashAudioSource.Stop();
        }
    }

    private float GetVolumeForSpeed(float speed)
    {
        if (!scaleVolumeWithSpeed || maxSpeedForFullVolume <= velocityThreshold)
        {
            return volume;
        }

        float t = Mathf.InverseLerp(velocityThreshold, maxSpeedForFullVolume, speed);
        float scaled = volume * Mathf.Clamp01(t);
        return Mathf.Max(scaled, minVolumeWhenPlaying * volume);
    }

    private void OnDisable()
    {
        StopSplash();
    }
}
