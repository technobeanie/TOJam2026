using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Common
{
    public class Sequencer
    {
        // const

        // public

        // protected

        // private
        private List<Command> _commands = new List<Command>();
        private Command _currentCommand = null;

        private bool _loop = false;
        private Action _onQueueCompleted = null;
        private Action<Command> _onCommandCompleted = null;

        // properties
        public bool IsActive
        {
            get; private set;
        }

        public bool HasCommands
        {
            get { return _commands.Count > 0; }
        }

        #region Class Methods
        public Sequencer(bool loop, Action onQueueCompleted = null, Action<Command> onCommandCompleted = null)
        {
            _loop = loop;
            _onQueueCompleted = onQueueCompleted;
            _onCommandCompleted = onCommandCompleted;
        }
        #endregion

        #region Public Methods
        public void Step(float deltaTime)
        {
            if (_currentCommand != null)
            {
                _currentCommand.Step(deltaTime);
            }
            else if (IsActive && HasCommands)
            {
                BeginNextCommand();
            }
        }

        public void AddCommand(Command command, bool autoComplete = false, bool skipHistory = false)
        {
            if (autoComplete)
            {
                command.AutoComplete();
                if (!skipHistory)
                {
                    _onCommandCompleted?.Invoke(command);
                }
            }
            else
            {
                _commands.Add(command);
            }
        }

        public void Begin()
        {
            if (IsActive)
            {
                return;
            }

            IsActive = true;

            BeginNextCommand();
        }

        public void Stop()
        {
            if (!IsActive)
            {
                return;
            }

            _commands.Clear();
            _currentCommand = null;

            OnQueueCompleted();
        }
        #endregion

        #region Protected Methods
        #endregion

        #region Private Methods
        private void BeginNextCommand()
        {
            if (!HasCommands)
            {
                OnQueueCompleted();

                return;
            }

            _currentCommand = _commands[0];
            _currentCommand.Begin(OnCommandCompleted);
        }

        private void OnCommandCompleted(Command command)
        {
            _commands.Remove(command);
            _currentCommand = null;

            _onCommandCompleted?.Invoke(command);

            BeginNextCommand();
        }

        private void OnQueueCompleted()
        {
            if (!_loop)
            {
                IsActive = false;
            }

            _onQueueCompleted?.Invoke();
        }
        #endregion
    }
}
