using UnityEngine;

namespace AIEnemy.Spider
{
    /// <summary>
    /// Top-down 2D arc: travels along a parabola in the XY plane (no physics gravity).
    /// Offset peaks at the midpoint using 4·h·t·(1−t).
    /// </summary>
    public class SpiderWeb : MonoBehaviour
    {
        Vector2 _start;
        Vector2 _end;
        float _arcHeight;
        float _duration;
        float _elapsed;
        float _fixedZ;
        bool _finished;

        public void Launch(Vector2 worldStart, Vector2 worldEnd, float arcHeight, float duration, float fixedZ)
        {
            _start = worldStart;
            _end = worldEnd;
            _arcHeight = arcHeight;
            _duration = Mathf.Max(0.01f, duration);
            _fixedZ = fixedZ;
            _elapsed = 0f;
            _finished = false;
        }

        private void Update()
        {
            if (_finished)
                return;

            _elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(_elapsed / _duration);

            Vector2 chord = _end - _start;
            float len = chord.magnitude;
            Vector2 u = len > 1e-5f ? chord / len : Vector2.right;
            Vector2 perp = new Vector2(-u.y, u.x);
            float bend = 4f * _arcHeight * t * (1f - t);
            Vector2 planar = Vector2.Lerp(_start, _end, t) + perp * bend;

            transform.position = new Vector3(planar.x, planar.y, _fixedZ);

            if (t >= 1f)
            {
                _finished = true;
                Destroy(gameObject);
            }
        }
    }
}
