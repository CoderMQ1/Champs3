using System.Collections.Generic;
using QFramework;
using UnityEngine;

namespace SquareHero.Hotfix
{
    public class Loading
    {
        private float _relProgress;
        private float _virtualProgress;
        private List<ILoadingNode> _nodes;
        private int _index = -1;
        private ILoadingNode _currentNode;
        private long _id;
        
        public delegate void Handler();

        public Handler OnCompleted;
        public Loading()
        {
        }

        public Loading(long id)
        {
            _id = id;
        }

        public void Start()
        {
            if (MoveNext())
            {
                ActionKit.OnUpdate.Register(Update);
            }
        }

        public void AddNode<T>(T node) where T : ILoadingNode
        {
            if (_nodes == null)
            {
                _nodes = new List<ILoadingNode>();
            }

            node.Initialize();
            _nodes.Add(node);
        }
        
        private bool MoveNext()
        {
            _index ++;
            bool hasNext = _index < _nodes.Count;
            
            if (hasNext)
            {
                _currentNode = _nodes[_index];
                _currentNode.Enter();
                _currentNode.Start();
            }
            else
            {
                _currentNode = null;
            }
            return hasNext;
        }

        private void Update()
        {
            if (_currentNode != null)
            {
                if (!_currentNode.IsDone())
                {
                    _currentNode.Update();
                }
                else
                {
                   _currentNode.Finish();
                   _currentNode.Exit();
                   _relProgress = (_index + 1f) / _nodes.Count;
                   MoveNext();
                }
            }
            else
            {
                if (_relProgress - _virtualProgress <= 0.02f)
                {
                    OnCompleted?.Invoke();
                    LogKit.I($"finish loading {_id}");
                    Release();
                }
            }
            _virtualProgress = Mathf.Lerp(_virtualProgress, _relProgress, Time.deltaTime * 8f);
        }

        public float GetRelProgress()
        {
            return _relProgress;
        }

        public float GetVirtualProgress()
        {
            return _virtualProgress;
        }

        public long GetID()
        {
            return _id;
        }

        public void Release()
        {
            ActionKit.OnUpdate.UnRegister(Update);
            _nodes.Clear();
            _nodes = null;
            OnCompleted = null;
        }
    }
}