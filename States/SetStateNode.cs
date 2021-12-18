using Components;
using HECSFramework.Core;
using System;

namespace Strategies
{
    public class SetStateNode : BaseDecisionNode, IInitable
    {
        [Connection(ConnectionPointType.In, "Input")]
        public BaseDecisionNode Input;

        [Connection(ConnectionPointType.Out, "Exit")]
        public BaseDecisionNode Exit;

        [UnityEngine.SerializeField]
        public State State;

        //тут мы выставляем стейт если запускаем этот стейт из стейта, чтобы мы могли вернуться в текущий стейт при выходе из того стейта что запустим
        [NonSerialized] public (bool weHaveFromState, State FromState) ExternalState; 

        public override string TitleOfNode { get; } = "Set State";

        public override void Execute(IEntity entity)
        {
            if (entity.TryGetHecsComponent(HMasks.StateContextComponent, out StateContextComponent stateContextComponent))
            {
                stateContextComponent.StateHolder.RemoveFromState(entity);
                stateContextComponent.ExitFromState();
            }

            State.Execute(entity, this);
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
    }
}
