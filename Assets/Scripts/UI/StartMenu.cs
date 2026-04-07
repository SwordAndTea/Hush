using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class StartMenu : MonoBehaviour
{
    [SerializeField] private string gameplaySceneName = "TestLevel";

    private VisualElement mainPanel;
    private VisualElement settingsPanel;
    private Label volumeValueLabel;

    private void Start()
    {
        UIDocument uiDocument = GetComponent<UIDocument>();
        if (uiDocument == null)
        {
            Debug.LogError("StartMenu requires a UIDocument component.");
            return;
        }

        VisualElement root = uiDocument.rootVisualElement;
        mainPanel = root.Q<VisualElement>("MainPanel");
        settingsPanel = root.Q<VisualElement>("SettingsPanel");
        Button startButton = root.Q<Button>("StartButton");
        Button settingsButton = root.Q<Button>("SettingsButton");
        Button backButton = root.Q<Button>("BackButton");
        Slider volumeSlider = root.Q<Slider>("VolumeSlider");
        volumeValueLabel = root.Q<Label>("VolumeValueLabel");

        if (mainPanel == null || settingsPanel == null || startButton == null ||
            settingsButton == null || backButton == null || volumeSlider == null || volumeValueLabel == null)
        {
            Debug.LogError("StartMenu UI references are missing. Check names in UXML.");
            return;
        }

        startButton.clicked += OnStartClicked;
        settingsButton.clicked += ShowSettingsPanel;
        backButton.clicked += ShowMainPanel;

        volumeSlider.value = AudioListener.volume;
        volumeSlider.RegisterValueChangedCallback(OnVolumeChanged);
        UpdateVolumeLabel(AudioListener.volume);
        ShowMainPanel();
    }

    private void OnStartClicked()
    {
        if (!string.IsNullOrWhiteSpace(gameplaySceneName))
        {
            SceneManager.LoadScene(gameplaySceneName);
        }
        else
        {
            Debug.LogWarning("Gameplay scene name is empty on StartMenu.");
        }
    }

    private void ShowMainPanel()
    {
        mainPanel.RemoveFromClassList("hidden");
        settingsPanel.AddToClassList("hidden");
    }

    private void ShowSettingsPanel()
    {
        settingsPanel.RemoveFromClassList("hidden");
        mainPanel.AddToClassList("hidden");
    }

    private void OnVolumeChanged(ChangeEvent<float> evt)
    {
        AudioListener.volume = evt.newValue;
        UpdateVolumeLabel(evt.newValue);
    }

    private void UpdateVolumeLabel(float volumeValue)
    {
        int percentage = Mathf.RoundToInt(volumeValue * 100f);
        volumeValueLabel.text = $"Volume: {percentage}%";
    }
}
