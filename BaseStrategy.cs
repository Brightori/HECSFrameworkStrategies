using HECSFramework.Core;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Strategies
{
    public abstract class BaseStrategy : ScriptableObject, IInitable
    {
        private int indexCash = -1;
        public List<BaseDecisionNode> nodes = new List<BaseDecisionNode>(16);
        public List<NodeToMetaNode> Metanodes = new List<NodeToMetaNode>(16);
        private BaseDecisionNode start;

        [NonSerialized]
        protected bool isInited;

        private OnForceStopNode onForceStopNode;

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
        public static event Action<BaseStrategy, string> GetWindow;
        public static event Action<BaseStrategy, string, Vector2> GetWindowAndFocus;
#endif

#if UNITY_EDITOR
        [Button(ButtonSizes.Large)]
        public void OpenStrategy()
        {
            OnOpenActions();
            var path = AssetDatabase.GetAssetPath(this);
            GetWindow.Invoke(this, path);
        }

        public void OpenStrategy(Vector2 vector2)
        {
            OnOpenActions();
            var path = AssetDatabase.GetAssetPath(this);
            GetWindowAndFocus.Invoke(this, path, vector2);
        }

        protected virtual void OnOpenActions() { }

#endif
        #endregion

        public virtual void Execute(Entity entity)
        {
            start.Execute(entity);
        }

        public virtual void Init()
        {
            if (isInited) return;

            isInited = true;

            foreach (var node in nodes)
            {
                if (node is IInitable initable)
                {
                    initable.Init();
                }
            }

            onForceStopNode = nodes.FirstOrDefault(x => x is OnForceStopNode) as OnForceStopNode;
            start = nodes.FirstOrDefault(x => x is StartDecision);
        }

        public virtual void ForceInit(bool forceInit = false)
        {
            isInited = false;
            Init();
        }

        public void ForceStop(Entity entity)
        {
            onForceStopNode?.Execute(entity);
        }
    }
}