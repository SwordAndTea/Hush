using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class GameOver : MonoBehaviour
{
    [Tooltip("Pixel font for the GameOver text.")]
    [SerializeField] private Font pixelFont;

    private VisualElement gameOverRoot;
    private VisualElement failImage;
    private VisualElement winImage;
    private Label statusLabel;

    private void Awake()
    {
        UIDocument uiDocument = GetComponent<UIDocument>();
        if (uiDocument == null)
        {
            Debug.LogError("GameOver requires a UIDocument component.");
            return;
        }

        gameOverRoot = uiDocument.rootVisualElement.Q<VisualElement>("GameOverRoot");
        failImage = uiDocument.rootVisualElement.Q<VisualElement>("FailImage");
        winImage = uiDocument.rootVisualElement.Q<VisualElement>("WinImage");
        statusLabel = uiDocument.rootVisualElement.Q<Label>("StatusLabel");
        Button replayButton = uiDocument.rootVisualElement.Q<Button>("ReplayButton");
        Button exitButton = uiDocument.rootVisualElement.Q<Button>("ExitButton");

        if (gameOverRoot == null || failImage == null || winImage == null || statusLabel == null || replayButton == null || exitButton == null)
        {
            Debug.LogError("GameOver UI references are missing. Check GameOver.uxml names.");
            return;
        }

        if (pixelFont != null)
        {
            ApplyPixelFont(replayButton);
            ApplyPixelFont(exitButton);
        }

        replayButton.clicked += Replay;
        exitButton.clicked += ExitGame;
    }

    private void ApplyPixelFont(VisualElement element)
    {
        element.style.unityFontDefinition = new StyleFontDefinition(StyleKeyword.None);
        element.style.unityFont = new StyleFont(pixelFont);
    }

    private void Start()
    {
        Hide();
    }

    public void ShowFail()
    {
        ShowStatus("You Fail", true);
    }

    public void ShowWin()
    {
        ShowStatus("You Win", false);
    }

    public void Hide()
    {
        if (gameOverRoot != null)
            gameOverRoot.AddToClassList("hidden");
    }

    private void ShowStatus(string text, bool isFail)
    {
        if (gameOverRoot == null || statusLabel == null)
            return;

        statusLabel.text = text;
        if (isFail)
        {
            failImage.RemoveFromClassList("hidden");
            winImage.AddToClassList("hidden");
            statusLabel.RemoveFromClassList("win");
            statusLabel.AddToClassList("fail");
            statusLabel.text = string.Empty;
        }
        else
        {
            failImage.AddToClassList("hidden");
            winImage.RemoveFromClassList("hidden");
            statusLabel.RemoveFromClassList("fail");
            statusLabel.AddToClassList("win");
            statusLabel.text = string.Empty;
        }

        gameOverRoot.RemoveFromClassList("hidden");
        Time.timeScale = 0f;
    }

    private void Replay()
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
}
