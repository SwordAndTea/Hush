using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

namespace AIEnemy.Spider
{
	[TaskCategory("HushAIEnemy")]
	public class SpiderMovement : Action
	{
		public SharedVector2 TargetPosition;

		private SpiderEnemy _boundSpiderEnemy;

		public override void OnAwake()
		{
			_boundSpiderEnemy = gameObject.GetComponent<SpiderEnemy>();
		}

		public override void OnStart()
		{
			_boundSpiderEnemy.SetPathfindingTarget(TargetPosition.Value);
			_boundSpiderEnemy.StartPathfindToTarget();
		}

		public override TaskStatus OnUpdate()
		{

			if (_boundSpiderEnemy.ReachedDestination)
			{
				_boundSpiderEnemy.StopPathfinding();
				return TaskStatus.Success;
			}
		
			_boundSpiderEnemy.SetPathfindingTarget(TargetPosition.Value);
			return TaskStatus.Running;
		}
	}
}