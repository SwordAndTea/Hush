using UnityEngine;

namespace AIEnemy.Spider
{
    public class SpiderWeb : MonoBehaviour
    {
        [SerializeField] LayerMask nonTraversableLayers = 0;

        Vector2 _direction;
        float _speed;
        float _maxDistance;
        float _traveledDistance;
        float _fixedZ;

        public void Launch(Vector2 worldStart, Vector2 worldTarget, float speed, float maxDistance, float fixedZ)
        {
            Vector2 delta = worldTarget - worldStart;
            _direction = delta.sqrMagnitude > 1e-5f ? delta.normalized : Vector2.right;
            _speed = Mathf.Max(0f, speed);
            _maxDistance = Mathf.Max(0f, maxDistance);
            _traveledDistance = 0f;
            _fixedZ = fixedZ;
            transform.position = new Vector3(worldStart.x, worldStart.y, _fixedZ);
        }

        private void Update()
        {
            float step = _speed * Time.deltaTime;
            if (step <= 0f)
                return;

            Vector3 current = transform.position;
            Vector2 planar = new Vector2(current.x, current.y) + _direction * step;
            transform.position = new Vector3(planar.x, planar.y, _fixedZ);
            _traveledDistance += step;

            if (_traveledDistance >= _maxDistance)
                Destroy(gameObject);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other == null || other.isTrigger)
                return;

            if (((1 << other.gameObject.layer) & nonTraversableLayers.value) == 0)
                return;

            var t = other.transform;
            if (t == transform || t.IsChildOf(transform))
                return;

            Destroy(gameObject);
        }
    }
}
