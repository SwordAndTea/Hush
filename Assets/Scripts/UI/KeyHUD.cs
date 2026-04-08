using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class KeyHUD : MonoBehaviour
{
    [Tooltip("USS stylesheet to apply.")]
    [SerializeField] private StyleSheet styleSheet;

    [Tooltip("Pixel font.")]
    [SerializeField] private Font pixelFont;

    private Label _keyLabel;
    private VisualElement _messageContainer;
    private Label _messageLabel;
    private Coroutine _hideCoroutine;

    private void Awake()
    {
        var uiDoc = GetComponent<UIDocument>();
        if (uiDoc == null)
        {
            Debug.LogError("KeyHUD requires a UIDocument component.");
            return;
        }

        var root = uiDoc.rootVisualElement;

        if (styleSheet != null)
            root.styleSheets.Add(styleSheet);

        _keyLabel = root.Q<Label>("KeyLabel");
        _messageContainer = root.Q<VisualElement>("MessageContainer");
        _messageLabel = root.Q<Label>("MessageLabel");

        if (_keyLabel == null || _messageContainer == null || _messageLabel == null)
        {
            Debug.LogError("KeyHUD UI references are missing. Check KeyHUD.uxml element names.");
            return;
        }

        if (pixelFont != null)
        {
            _keyLabel.style.unityFontDefinition = new StyleFontDefinition(StyleKeyword.None);
            _keyLabel.style.unityFont = new StyleFont(pixelFont);
            _messageLabel.style.unityFontDefinition = new StyleFontDefinition(StyleKeyword.None);
            _messageLabel.style.unityFont = new StyleFont(pixelFont);
        }

        _messageContainer.AddToClassList("hidden");
    }

    private void Start()
    {
        UpdateLabel(0, KeyManager.Instance != null ? KeyManager.Instance.KeysRequired : 3);

        if (KeyManager.Instance != null)
            KeyManager.Instance.onKeyCountChanged.AddListener(UpdateLabel);
    }

    private void OnDestroy()
    {
        if (KeyManager.Instance != null)
            KeyManager.Instance.onKeyCountChanged.RemoveListener(UpdateLabel);
    }

    private void UpdateLabel(int collected, int required)
    {
        if (_keyLabel != null)
            _keyLabel.text = $"Keys: {collected} / {required}";
    }

    public void ShowMessage(string text, float duration)
    {
        if (_messageContainer == null || _messageLabel == null)
            return;

        _messageLabel.text = text;
        _messageContainer.RemoveFromClassList("hidden");

        if (_hideCoroutine != null)
            StopCoroutine(_hideCoroutine);

        _hideCoroutine = StartCoroutine(HideAfter(duration));
    }

    private IEnumerator HideAfter(float seconds)
    {
        yield return new WaitForSecondsRealtime(seconds);
        if (_messageContainer != null)
            _messageContainer.AddToClassList("hidden");
        _hideCoroutine = null;
    }
}
