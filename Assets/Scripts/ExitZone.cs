using UnityEngine;

public class ExitZone : MonoBehaviour
{
    [Tooltip("Reference to the KeyHUD to show popup messages.")]
    [SerializeField] private KeyHUD keyHUD;

    private bool _triggered;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_triggered)
            return;

        var player = other.GetComponent<TopDownCharacterControl>()
                     ?? other.GetComponentInParent<TopDownCharacterControl>();
        if (player == null)
            return;

        if (KeyManager.Instance != null && KeyManager.Instance.HasAllKeys())
        {
            _triggered = true;
            player.TriggerWinEvent();
        }
        else
        {
            int collected = KeyManager.Instance != null ? KeyManager.Instance.KeysCollected : 0;
            int required = KeyManager.Instance != null ? KeyManager.Instance.KeysRequired : 3;

            if (keyHUD != null)
                keyHUD.ShowMessage($"You need all {required} keys to escape ({collected}/{required} found)", 3f);
        }
    }
}
