using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

namespace AIEnemy
{
	[TaskCategory("HushAIEnemy")]
	public class IsSoundHeard : Conditional
	{
		public AudioSource AudioSource;
		
		public SharedVector2 TargetPosition;
	
		[Range(0f, 1f)]
		public float HearingThreshold = 0.3f;
	
		public bool ApplyListenerVolume = true;

		[BehaviorDesigner.Runtime.Tasks.Tooltip("Log computed perceived sound volume of the ai agent")]
		public bool DebugPerceivedSoundVolume = false;

		public override TaskStatus OnUpdate()
		{
			if (AudioSource == null)
				return TaskStatus.Failure;

			if (!AudioSource.isPlaying || AudioSource.mute)
				return TaskStatus.Failure;

			float baseLevel = Mathf.Clamp01(AudioSource.volume);
			float attenuation = GetDistanceAttenuation(AudioSource, transform.position, AudioSource.transform.position);

			if (ApplyListenerVolume && AudioListener.volume > 0f)
				baseLevel *= Mathf.Clamp01(AudioListener.volume);

			float perceived = baseLevel * attenuation;
			if (DebugPerceivedSoundVolume)
			{
				Debug.Log($"Enemy perceived: {perceived}");
			}
			if (perceived >= HearingThreshold)
			{
				var p = AudioSource.transform.position;
				TargetPosition.Value = new Vector2(p.x, p.y);
				return TaskStatus.Success;
			}

			return TaskStatus.Failure;
		}

		// simulates hearing by distance even for spatialBlend 0 (2D)
		private float GetDistanceAttenuation(AudioSource source, Vector3 listenerWorld, Vector3 emitterWorld)
		{
			float distance = Vector2.Distance(
				new Vector2(listenerWorld.x, listenerWorld.y),
				new Vector2(emitterWorld.x, emitterWorld.y));

			if (distance <= source.minDistance)
				return 1f;
			if (distance >= source.maxDistance)
				return 0f;

			float span = source.maxDistance - source.minDistance;
			if (span <= Mathf.Epsilon)
				return distance <= source.minDistance ? 1f : 0f;

			switch (source.rolloffMode)
			{
				case AudioRolloffMode.Linear:
					return Mathf.Clamp01(1f - (distance - source.minDistance) / span);
				case AudioRolloffMode.Logarithmic:
					return Mathf.Clamp01(source.minDistance / distance);
				default:
					return Mathf.Clamp01(1f - (distance - source.minDistance) / span);
			}
		}
	}
}
