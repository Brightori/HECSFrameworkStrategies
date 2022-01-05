using HECSFramework.Core;
using Sirenix.Serialization;
using Strategies;
using System;
using System.Collections.Generic;

namespace Components
{
    [Serializable, Documentation(Doc.AI, Doc.Strategy, Doc.State, "Компонент который вешается на ентити которая участвует в данном стейте")]
    public class StateInfoComponent : BaseComponent
    {
        public bool NeedInfo = false;

        [OdinSerialize]
        public List<LogNode> PreviousFrame = new List<LogNode>(16);

        //если у нас на ентити будет несколько стейтов, то скорее всего здесь будет дикшенарь стейтов
        public Stack<LogNode> StateStack = new Stack<LogNode>();
    }
}