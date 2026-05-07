using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Common
{
    public class MultiSequencer
    {
        // const

        // public

        // protected

        // private
        private Dictionary<string, Sequencer> _sequencers = new Dictionary<string, Sequencer>();
        private bool _isActive = false;

        private bool _loop = false;
        private Action _onQueueCompleted = null;
        private Action<Command> _onCommandCompleted = null;

        // properties
        public bool AreSequencersActive
        {
            get
            {
                foreach (var sequencer in _sequencers.Values)
                {
                    if (sequencer.IsActive)
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        public bool HasCommands
        {
            get
            {
                foreach (var sequencer in _sequencers.Values)
                {
                    if (sequencer.HasCommands)
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        #region Class Methods
        public MultiSequencer(bool loop, Action onQueueCompleted = null, Action<Command> onCommandCompleted = null)
        {
            _loop = loop;
            _onQueueCompleted = onQueueCompleted;
            _onCommandCompleted = onCommandCompleted;
        }
        #endregion

        #region Public Methods
        public void Step(float deltaTime)
        {
            // Run the sequencer.
            foreach (var sequencer in _sequencers.Values)
            {
                sequencer.Step(deltaTime);
            }
        }

        public void AddCommand(string sequencerId, Command command, bool autoComplete = false, bool skipHistory = false)
        {
            if (!_sequencers.ContainsKey(sequencerId))
            {
                _sequencers.Add(sequencerId, new Sequencer(_loop, OnSequencerQueueCompleted, OnCommandCompleted));
                if (_isActive)
                {
                    _sequencers[sequencerId].Begin();
                }
            }

            _sequencers[sequencerId].AddCommand(command, autoComplete, skipHistory);
        }

        public void Begin()
        {
            if (_isActive)
            {
                return;
            }

            _isActive = true;

            foreach (var sequencer in _sequencers.Values)
            {
                sequencer.Begin();
            }
        }

        public void Stop()
        {
            if (!_isActive)
            {
                return;
            }

            foreach (var sequencer in _sequencers.Values)
            {
                sequencer.Stop();
            }
        }
        #endregion

        #region Protected Methods
        #endregion

        #region Private Methods
        private void OnCommandCompleted(Command command)
        {
            _onCommandCompleted?.Invoke(command);
        }

        private void OnSequencerQueueCompleted()
        {
            if (!HasCommands)
            {
                OnQueueCompleted();
            }
        }

        private void OnQueueCompleted()
        {
            if (!_loop)
            {
                _isActive = false;
            }

            _onQueueCompleted?.Invoke();
        }
        #endregion
    }
}
