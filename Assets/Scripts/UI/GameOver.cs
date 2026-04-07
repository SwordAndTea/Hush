using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class GameOver : MonoBehaviour
{
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

        if (gameOverRoot == null || failImage == null || winImage == null || statusLabel == null || replayButton == null)
        {
            Debug.LogError("GameOver UI references are missing. Check GameOver.uxml names.");
            return;
        }

        replayButton.clicked += Replay;
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
}
