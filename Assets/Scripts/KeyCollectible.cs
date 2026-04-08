using UnityEngine;

public class KeyCollectible : MonoBehaviour
{
    [Tooltip("One-shot sound when a key is picked up.")]
    [SerializeField] private AudioClip pickupSfx;

    [SerializeField] private float pickupSfxVolume = 1f;

    private bool _collected;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_collected)
            return;

        var player = other.GetComponent<TopDownCharacterControl>()
                     ?? other.GetComponentInParent<TopDownCharacterControl>();
        if (player == null)
            return;

        _collected = true;

        if (KeyManager.Instance != null)
            KeyManager.Instance.CollectKey();

        if (pickupSfx != null)
            AudioSource.PlayClipAtPoint(pickupSfx, transform.position, pickupSfxVolume);

        Destroy(gameObject);
    }
}
