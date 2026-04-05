using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace AIEnemy
{
	[TaskCategory("HushAIEnemy")]
	public class IsWithinLightRange : Conditional
	{
		public Light2D Light;
		public SharedVector2 TargetPosition;
		public bool RequireLineOfSight;

		public override TaskStatus OnUpdate()
		{
			if (Light == null || !Light.enabled || Light.lightType != Light2D.LightType.Point)
				return TaskStatus.Failure;

			if (Light.intensity <= 0f || Light.color.a <= 0f)
				return TaskStatus.Failure;

			var agentPosition = new Vector2(transform.position.x, transform.position.y);
			var lightPosition = new Vector2(Light.transform.position.x, Light.transform.position.y);

			float dist = Vector2.Distance(agentPosition, lightPosition);
			float outer = Mathf.Max(0f, Light.pointLightOuterRadius);
			float inner = Mathf.Max(0f, Light.pointLightInnerRadius);

			if (dist > outer || dist < inner)
				return TaskStatus.Failure;

			float outerAngle = Light.pointLightOuterAngle;
			if (outerAngle < 360f - 0.01f && dist > 0.0001f)
			{
				Vector2 toAgent = (agentPosition - lightPosition) / dist;
				Vector2 forward = new Vector2(Light.transform.up.x, Light.transform.up.y).normalized;
				if (Vector2.Angle(forward, toAgent) > outerAngle * 0.5f)
					return TaskStatus.Failure;
			}

			if (RequireLineOfSight && !HasClearLineOfSight2D(agentPosition, lightPosition, dist))
				return TaskStatus.Failure;

			var p = Light.transform.position;
			TargetPosition.Value = new Vector2(p.x, p.y);
			return TaskStatus.Success;
		}

		private bool HasClearLineOfSight2D(Vector2 from, Vector2 to, float distance)
		{
			if (distance < 0.0001f)
				return true;

			Vector2 dir = (to - from) / distance;
			const float endSlack = 0.02f;

			var hits = Physics2D.RaycastAll(from, dir, distance, Physics2D.DefaultRaycastLayers);
			if (hits.Length == 0)
				return true;

			System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

			foreach (var hit in hits)
			{
				if (!hit.collider || hit.collider.isTrigger)
					continue;
				if (hit.distance >= distance - endSlack)
					break;

				var t = hit.collider.transform;
				if (t == transform || t.IsChildOf(transform))
					continue;
				if (t == Light.transform || t.IsChildOf(Light.transform))
					continue;

				return false;
			}

			return true;
		}
	}
}
