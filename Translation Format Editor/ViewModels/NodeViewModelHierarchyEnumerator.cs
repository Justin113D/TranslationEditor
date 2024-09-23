using System.Collections;
using System.Collections.Generic;

namespace J113D.TranslationEditor.FormatApp.ViewModels
{
    internal class NodeViewModelHierarchyEnumerator : IEnumerable<NodeViewModel>, IEnumerator<NodeViewModel>
    {
        private readonly NodeViewModel[] _init;

        private readonly Stack<(IList<NodeViewModel> children, int index)> _treeStack;

        private IList<NodeViewModel>? _currentChildren;

        private int _currentIndex;

        public NodeViewModel Current => _currentChildren?[_currentIndex] ?? _init[0];

        object IEnumerator.Current => Current;

        public NodeViewModelHierarchyEnumerator(ParentNodeViewModel parent)
        {
            _init = [parent];
            _currentChildren = _init;
            _treeStack = new();
        }

        public void Dispose()
        {
            _currentChildren = null;
        }

        public bool MoveNext()
        {
            if(_currentChildren == null)
            {
                return false;
            }

            if(_currentChildren[_currentIndex] is ParentNodeViewModel parent && parent.ChildNodes?.Count > 0)
            {
                _treeStack.Push((_currentChildren, _currentIndex + 1));
                _currentChildren = parent.ChildNodes;
                _currentIndex = -1;
            }

            _currentIndex++;

            while(_currentIndex >= _currentChildren.Count)
            {
                if(_treeStack.Count == 0)
                {
                    return false;
                }

                (_currentChildren, _currentIndex) = _treeStack.Pop();
            }

            return true;
        }

        public void Reset()
        {
            _currentChildren = _init;
            _currentIndex = 0;
            _treeStack.Clear();
        }

        public IEnumerator<NodeViewModel> GetEnumerator()
        {
            return this;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this;
        }
    }
}
