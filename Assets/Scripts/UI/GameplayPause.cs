using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class GameplayPause : MonoBehaviour
{
    [SerializeField] private TopDownCharacterControl playerController;

    private VisualElement pauseRoot;
    private Label volumeLabel;
    private InputAction exitAction;
    private bool isPaused;
    private bool enabledExitActionLocally;
    private int lastToggleFrame = -1;

    private void Awake()
    {
        UIDocument uiDocument = GetComponent<UIDocument>();
        if (uiDocument == null)
        {
            Debug.LogError("GameplayPause requires a UIDocument component.");
            return;
        }

        pauseRoot = uiDocument.rootVisualElement.Q<VisualElement>("PauseRoot");
        Button restartButton = uiDocument.rootVisualElement.Q<Button>("RestartButton");
        Button resumeButton = uiDocument.rootVisualElement.Q<Button>("ResumeButton");
        Button exitButton = uiDocument.rootVisualElement.Q<Button>("ExitButton");
        Slider volumeSlider = uiDocument.rootVisualElement.Q<Slider>("VolumeSlider");
        volumeLabel = uiDocument.rootVisualElement.Q<Label>("VolumeLabel");

        if (pauseRoot == null || restartButton == null || resumeButton == null || exitButton == null || volumeSlider == null || volumeLabel == null)
        {
            Debug.LogError("GameplayPause UI references are missing. Check GameplayPause.uxml names.");
            return;
        }

        restartButton.clicked += RestartScene;
        resumeButton.clicked += ResumeGame;
        exitButton.clicked += ExitGame;
        volumeSlider.RegisterValueChangedCallback(OnVolumeChanged);
        volumeSlider.value = AudioListener.volume;
        UpdateVolumeLabel(AudioListener.volume);

        exitAction = InputSystem.actions.FindAction("Exit");
        if (exitAction == null)
            Debug.LogWarning("Input action 'Exit' not found. Pause toggle will not work.");

        if (playerController == null)
            playerController = FindFirstObjectByType<TopDownCharacterControl>();
    }

    private void OnEnable()
    {
        if (exitAction != null)
        {
            if (!exitAction.enabled)
            {
                exitAction.Enable();
                enabledExitActionLocally = true;
            }
            exitAction.performed += OnExitPerformed;
        }
    }

    private void OnDisable()
    {
        if (exitAction != null)
        {
            exitAction.performed -= OnExitPerformed;
            if (enabledExitActionLocally && exitAction.enabled)
                exitAction.Disable();
        }
        enabledExitActionLocally = false;

        if (isPaused)
            Time.timeScale = 1f;

        if (playerController != null)
            playerController.UnblockInput();
    }

    private void Start()
    {
        SetPaused(false);
    }

    private void OnExitPerformed(InputAction.CallbackContext _)
    {
        TogglePause();
    }

    private void Update()
    {
        // Fallback so Escape still works even if the Exit action is not configured.
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
            TogglePause();
    }

    private void TogglePause()
    {
        if (lastToggleFrame == Time.frameCount)
            return;

        lastToggleFrame = Time.frameCount;
        SetPaused(!isPaused);
    }

    private void SetPaused(bool pause)
    {
        isPaused = pause;
        Time.timeScale = isPaused ? 0f : 1f;

        if (playerController != null)
        {
            if (isPaused)
                playerController.BlockInput();
            else
                playerController.UnblockInput();
        }

        if (pauseRoot == null)
            return;

        if (isPaused)
            pauseRoot.RemoveFromClassList("hidden");
        else
            pauseRoot.AddToClassList("hidden");
    }

    private void ResumeGame()
    {
        SetPaused(false);
    }

    private void RestartScene()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void ExitGame()
    {
        Time.timeScale = 1f;
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private void OnVolumeChanged(ChangeEvent<float> evt)
    {
        AudioListener.volume = evt.newValue;
        UpdateVolumeLabel(evt.newValue);
    }

    private void UpdateVolumeLabel(float volume)
    {
        int percentage = Mathf.RoundToInt(volume * 100f);
        volumeLabel.text = $"Volume: {percentage}%";
    }
}
