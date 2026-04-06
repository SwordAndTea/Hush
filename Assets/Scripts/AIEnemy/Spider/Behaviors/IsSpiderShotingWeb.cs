using BehaviorDesigner.Runtime.Tasks;

namespace AIEnemy.Spider.Behaviors
{
    [TaskCategory("HushAIEnemy")]
    public class IsSpiderShotingWeb : Conditional
    {
        SpiderEnemy _spider;

        public override void OnAwake()
        {
            _spider = gameObject.GetComponent<SpiderEnemy>();
        }

        public override TaskStatus OnUpdate()
        {
            if (_spider == null)
                return TaskStatus.Failure;

            return _spider.IsShotingWeb ? TaskStatus.Success : TaskStatus.Failure;
        }
    }
}
