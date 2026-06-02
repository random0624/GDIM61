using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class DeathScreenUI : MonoBehaviour
{
    [SerializeField] private GameObject deathPanel;
    [SerializeField] private TMP_Text deathLineText;

    [Header("Random Death Lines")]
    [SerializeField] private string[] deathLines =
    {
        "Lost Beneath the Waves",
        "Your hull couldn't take the sea.",
        "You sank... but not for long.",
        "The waves were merciless this time.",
        "A hard lesson from the ocean."
    };

    private BoatIntegrity subscribedIntegrity;
    private bool isDead;
    private Coroutine subscribeRoutine;

    private void Awake()
    {
        if (deathPanel != null)
        {
            deathPanel.SetActive(false);
        }
    }

    private void OnEnable()
    {
        subscribeRoutine = StartCoroutine(WaitAndSubscribe());
    }

    private void Start()
    {
        if (subscribeRoutine == null)
        {
            subscribeRoutine = StartCoroutine(WaitAndSubscribe());
        }
    }

    private void OnDisable()
    {
        if (subscribeRoutine != null)
        {
            StopCoroutine(subscribeRoutine);
            subscribeRoutine = null;
        }

        if (subscribedIntegrity != null)
        {
            subscribedIntegrity.OnIntegrityEmpty -= ShowDeathScreen;
            subscribedIntegrity = null;
        }
    }

    private System.Collections.IEnumerator WaitAndSubscribe()
    {
        while (BoatIntegrity.Instance == null)
        {
            yield return null;
        }

        if (subscribedIntegrity != null)
        {
            subscribedIntegrity.OnIntegrityEmpty -= ShowDeathScreen;
        }

        subscribedIntegrity = BoatIntegrity.Instance;
        subscribedIntegrity.OnIntegrityEmpty -= ShowDeathScreen;
        subscribedIntegrity.OnIntegrityEmpty += ShowDeathScreen;
        subscribeRoutine = null;
    }

    private void ShowDeathScreen()
    {
        if (isDead)
        {
            return;
        }

        isDead = true;
        SetRandomDeathLine();

        if (deathPanel != null)
        {
            deathPanel.SetActive(true);
        }

        Time.timeScale = 0f;
    }

    private void Update()
    {
        if (!isDead)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            RestartLevel();
        }
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f;
        Scene activeScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene("IntroDialogue");
    }

    private void SetRandomDeathLine()
    {
        if (deathLineText == null || deathLines == null || deathLines.Length == 0)
        {
            return;
        }

        int randomIndex = Random.Range(0, deathLines.Length);
        deathLineText.text = deathLines[randomIndex];
    }

    public void TriggerDeath()
    {
        ShowDeathScreen();
    }
}
