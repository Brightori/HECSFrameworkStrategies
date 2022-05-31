using Components;
using HECSFramework.Core;
using System;

namespace Strategies
{
    public class SetStateNode : LogNode, IInitable
    {
        [Connection(ConnectionPointType.In, "Input")]
        public BaseDecisionNode Input;

        [Connection(ConnectionPointType.In, "Execute on Start")]
        public BaseDecisionNode OnStartStateNodes;

        [Connection(ConnectionPointType.Out, "Exit")]
        public BaseDecisionNode Exit;

        [UnityEngine.SerializeField]
        public State State;

        //тут мы выставляем стейт если запускаем этот стейт из стейта, чтобы мы могли вернуться в текущий стейт при выходе из того стейта что запустим
        [NonSerialized] public (bool weHaveFromState, State FromState) ExternalState; 

        public override string TitleOfNode { get; } = "Set State";

        public override void Execute(IEntity entity)
        {
           Run(entity);
        }

        public void OnExit(IEntity entity)
        {
            if (ExternalState.weHaveFromState)
                ExternalState.FromState.SetupState(entity);

            Exit.Execute(entity);
        }

        public void Init()
        {
            State.Init();
        }

        protected override void Run(IEntity entity)
        {
            OnStartStateNodes?.Execute(entity);
            State.Execute(entity, this);
        }
    }
}
