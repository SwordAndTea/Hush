using UnityEngine;
using UnityEngine.Events;

public class KeyManager : MonoBehaviour
{
    public static KeyManager Instance { get; private set; }

    [SerializeField] private int keysRequired = 3;

    [Tooltip("Fired when a key is collected. Args: (keysCollected, keysRequired).")]
    public UnityEvent<int, int> onKeyCountChanged = new UnityEvent<int, int>();

    public int KeysCollected { get; private set; }
    public int KeysRequired => keysRequired;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void CollectKey()
    {
        KeysCollected++;
        onKeyCountChanged?.Invoke(KeysCollected, keysRequired);
    }

    public bool HasAllKeys() => KeysCollected >= keysRequired;
}
