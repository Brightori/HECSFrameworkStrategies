using HECSFramework.Core;
using HECSFramework.Documentation;
using Strategies;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Strategies
{
    [Serializable]
    [Documentation(Doc.Strategy, Doc.AI, "Это подвид стратегии для ")]
    public class State : Strategy, IState, IInitable
    {
        private List<INeedGlobalStart> startNodes = new List<INeedGlobalStart>();
        private List<IUpdatable> updatables = new List<IUpdatable>();
        private List<ILateUpdatable> lateUpdatables = new List<ILateUpdatable>();

        public void Pause()
        {
            throw new NotImplementedException();
        }

        public void Stop()
        {
            throw new NotImplementedException();
        }

        public void UnPause()
        {
            throw new NotImplementedException();
        }

        public void UpdateLocal()
        {
            throw new NotImplementedException();
        }

        public override void Execute(IEntity entity)
        {

        }

        public void Init()
        {
            throw new NotImplementedException();
        }
    }

    public interface IState : IUpdatable, IHavePause 
    {
        void Execute(IEntity entity);
        void Stop();
    }

    public enum StrategyState { Start, Run, Pause, Stop }
}