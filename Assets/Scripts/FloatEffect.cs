using UnityEngine;

public class FloatEffect : MonoBehaviour
{
    [Header("Float Settings")]
    [SerializeField] private float amplitude = 0.15f;
    [SerializeField] private float frequency = 1.5f;
    [SerializeField] private bool randomizeOffset = true;

    private Vector3 _startPos;
    private float _timeOffset;

    private void Start()
    {
        _startPos = transform.localPosition;
        _timeOffset = randomizeOffset ? Random.Range(0f, Mathf.PI * 2f) : 0f;
    }

    private void Update()
    {
        var yOffset = Mathf.Sin((Time.time + _timeOffset) * frequency) * amplitude;
        transform.localPosition = _startPos + new Vector3(0f, yOffset, 0f);
    }
}
