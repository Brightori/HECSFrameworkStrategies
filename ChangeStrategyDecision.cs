using Commands;
using HECSFramework.Core;

namespace Strategies
{
    internal class ChangeStrategyDecision : InterDecision
    {
        public override string TitleOfNode { get; } = "Change Strategy";
        [UnityEngine.SerializeField] public Strategy Strategy;

        protected override void Run(IEntity entity)
        {
            entity.Command(new ChangeStrategyCommand { Strategy = this.Strategy });
            Next.Execute(entity);
        }
    }
}