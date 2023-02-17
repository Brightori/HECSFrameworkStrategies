using HECSFramework.Core;
using Sirenix.Serialization;
using Strategies;
using System;
using System.Collections.Generic;

namespace Components
{
    [Serializable, Documentation(Doc.AI, Doc.Strategy, Doc.State, Doc.Debug, "Компонент который вешается на ентити которая участвует в данном стейте, нужен для дебага")]
    public partial class StateInfoComponent : BaseComponent
    {
        public bool NeedInfo = false;
        public bool NeedClean = false;

        public int MaxCycles = 3;
        public int CurrentCycle = 0;

        [OdinSerialize]
        public List<LogNode> StateStack = new List<LogNode>();
    }
}