using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Common
{
    public abstract class Command
    {
        // const

        // public

        // protected

        // private
        private Action<Command> _onCompleted = null;

        // properties
        public bool IsCompleted
        {
            get; private set;
        }

        #region Class Methods
        #endregion

        #region Public Methods
        public abstract void Step(float deltaTime);

        public void Undo()
        {
            if (IsCompleted)
            {
                IsCompleted = false;
                OnUndo();
            }
        }

        public void Redo()
        {
            if (!IsCompleted)
            {
                End();
            }
        }

        public void Begin(Action<Command> onCompleted)
        {
            _onCompleted = onCompleted;

            OnPrepareBegin();
            OnBegin();
        }

        public void AutoComplete()
        {
            OnPrepareBegin();
            End();
        }
        #endregion

        #region Protected Methods
        protected abstract void OnPrepareBegin();
        protected abstract void OnBegin();
        protected abstract void OnUndo();

        protected virtual void End()
        {
            OnEnd();
        }
        #endregion

        #region Private Methods
        private void OnEnd()
        {
            IsCompleted = true;

            var temp = _onCompleted;
            _onCompleted = null;
            temp?.Invoke(this);
        }
        #endregion
    }
}
