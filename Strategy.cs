using HECSFramework.Core;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using HECSFramework.Documentation;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Strategies
{
    [CreateAssetMenu]
    [Documentation(Doc.GameLogic, "Это корневой объект для визуального редактора Decision Tree")]
    public class Strategy : ScriptableObject, IInitable
    {
        private int indexCash = -1;
        public Strategy GetCopy => Instantiate(this);
        public List<BaseDecisionNode> nodes = new List<BaseDecisionNode>(16);
        private BaseDecisionNode start;

        public int StrategyIndex
        {
            get
            {
                if (indexCash == -1)
                    indexCash = IndexGenerator.GenerateIndex(name);

                return indexCash;
            }
        }

        #region Editor
#if UNITY_EDITOR //это для проброса в эдитор
        public static event Action<Strategy, string> GetWindow;
#endif

#if UNITY_EDITOR
        [Button(ButtonSizes.Large)]
        private void OpenStrategy()
        {
            var path = AssetDatabase.GetAssetPath(this);
            GetWindow.Invoke(this, path);
        }
#endif
        #endregion

        public virtual void Execute(IEntity entity)
        {
            if (start == null)
                start = nodes.FirstOrDefault(x => x is StartDecision);

            if (start == null)
            {
                Debug.LogAssertion("нет стартовой ноды у " + this.name);
                return;
            }

            start.Execute(entity);
        }

        public virtual void Init()
        {
            foreach (var node in nodes)
            {
                if (node is IInitable initable)
                {
                    initable.Init();
                }
            }

            start = nodes.FirstOrDefault(x => x is StartDecision);
        }
    }
}