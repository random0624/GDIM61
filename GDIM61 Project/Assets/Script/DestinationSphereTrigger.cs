using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class DestinationSphereTrigger : MonoBehaviour
{
    [Header("Prompt UI")]
    [SerializeField] private GameObject promptTextObject;
    [SerializeField] private bool hideOnStart = true;
    [SerializeField] private float totalDisplayDuration = 5f;
    [SerializeField] private float fadeInDuration = 0.8f;
    [SerializeField] private float fadeOutDuration = 0.8f;
    [SerializeField] private bool triggerOnlyOnce = true;

    [Header("Island Discovery Audio")]
    [SerializeField] private AudioClip islandDiscoveryCue;
    [SerializeField, Range(0f, 1f)] private float islandDiscoveryVolume = 1f;
    [SerializeField] private AudioSource overrideAudioSource;

    private CanvasGroup promptCanvasGroup;
    private bool hasTriggered;
    private Coroutine fadeRoutine;

    void Reset()
    {
        GetComponent<Collider>().isTrigger = true;
    }

    void Awake()
    {
        GetComponent<Collider>().isTrigger = true;

        if (promptTextObject != null)
        {
            promptCanvasGroup = promptTextObject.GetComponent<CanvasGroup>();
            if (promptCanvasGroup == null)
            {
                promptCanvasGroup = promptTextObject.AddComponent<CanvasGroup>();
            }

            promptCanvasGroup.alpha = 0f;
        }

        if (hideOnStart && promptTextObject != null)
            promptTextObject.SetActive(false);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.GetComponentInParent<BoatController>() == null && other.GetComponent<BoatController>() == null)
            return;

        if (triggerOnlyOnce && hasTriggered)
            return;

        if (promptTextObject == null)
            return;

        hasTriggered = true;
        PlayIslandDiscoveryAudio();

        if (fadeRoutine != null)
        {
            StopCoroutine(fadeRoutine);
        }

        fadeRoutine = StartCoroutine(FadeSequence());
    }

    void OnTriggerExit(Collider other)
    {
        // Intentionally left blank:
        // once triggered, the message completes its timed fade sequence.
    }

    private IEnumerator FadeSequence()
    {
        promptTextObject.SetActive(true);

        float fadeIn = Mathf.Max(0.01f, fadeInDuration);
        float fadeOut = Mathf.Max(0.01f, fadeOutDuration);
        float total = Mathf.Max(0.05f, totalDisplayDuration);
        float hold = Mathf.Max(0f, total - fadeIn - fadeOut);

        yield return Fade(0f, 1f, fadeIn);

        if (hold > 0f)
        {
            yield return new WaitForSeconds(hold);
        }

        yield return Fade(1f, 0f, fadeOut);
        promptTextObject.SetActive(false);
        fadeRoutine = null;
    }

    private IEnumerator Fade(float from, float to, float duration)
    {
        float elapsed = 0f;
        promptCanvasGroup.alpha = from;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            promptCanvasGroup.alpha = Mathf.Lerp(from, to, t);
            yield return null;
        }

        promptCanvasGroup.alpha = to;
    }

    private void PlayIslandDiscoveryAudio()
    {
        if (islandDiscoveryCue == null)
        {
            return;
        }

        if (overrideAudioSource != null)
        {
            overrideAudioSource.PlayOneShot(islandDiscoveryCue, islandDiscoveryVolume);
            return;
        }

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySound(islandDiscoveryCue, islandDiscoveryVolume);
        }
    }
}
