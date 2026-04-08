using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;

public class StoryAndTutorial : MonoBehaviour
{
    private static bool s_hasShownIntroThisSession;

    private UIDocument _uiDocument;
    [SerializeField] private TopDownCharacterControl playerController;

    private VisualElement introRoot;
    private Button continueButton;
    private bool isPaused;

    private void Awake()
    {
        _uiDocument = GetComponent<UIDocument>();

        if (playerController == null)
        {
            playerController = FindFirstObjectByType<TopDownCharacterControl>();
        }
    }

    private void OnEnable()
    {
        SetupIntroUI();
    }

    private IEnumerator Start()
    {
        // Re-apply after other Start methods (e.g. GameplayPause) run.
        yield return null;
        SetupIntroUI();
    }

    private void OnDisable()
    {
        if (continueButton != null)
        {
            continueButton.clicked -= OnContinueClicked;
        }

        SetPaused(false);
    }

    private void SetupIntroUI()
    {
        if (_uiDocument == null)
        {
            return;
        }

        VisualElement root = _uiDocument.rootVisualElement;
        introRoot = root.Q<VisualElement>("story-tutorial-root");
        continueButton = root.Q<Button>("story-tutorial-continue");

        if (introRoot == null || continueButton == null)
        {
            return;
        }

        continueButton.clicked -= OnContinueClicked;
        continueButton.clicked += OnContinueClicked;

        if (s_hasShownIntroThisSession)
        {
            SetIntroVisible(false);
            return;
        }

        SetIntroVisible(true);
    }

    private void OnContinueClicked()
    {
        s_hasShownIntroThisSession = true;

        if (introRoot != null)
        {
            SetIntroVisible(false);
        }
    }

    private void SetIntroVisible(bool visible)
    {
        introRoot.style.display = visible ? DisplayStyle.Flex : DisplayStyle.None;
        SetPaused(visible);
    }

    private void SetPaused(bool pause)
    {
        isPaused = pause;
        Time.timeScale = isPaused ? 0f : 1f;

        if (playerController == null)
        {
            return;
        }

        if (isPaused)
        {
            playerController.BlockInput();
        }
        else
        {
            playerController.UnblockInput();
        }
    }
}
