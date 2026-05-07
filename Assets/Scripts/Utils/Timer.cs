using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utils
{
    public class Timer
    {
        // const

        // public

        // protected

        // private
        private float _duration = 0.0f;
        private bool _loop = false;
        private Action<Timer> _onCompleted = null;
        private Action<Timer, float> _onUpdated = null;

        // properties
        public bool IsActive
        {
            get; private set;
        }

        public bool IsPaused
        {
            get; private set;
        }

        public float CurrentTime
        {
            get; private set;
        }

        #region Unity Methods
        public Timer(float duration, Action<Timer> onCompleted = null, bool loop = false, Action<Timer, float> onUpdated = null)
        {
            _duration = duration;
            _loop = loop;
            _onCompleted = onCompleted;
            _onUpdated = onUpdated;
        }
        #endregion

        #region Public Methods
        public void Begin()
        {
            IsActive = true;
            IsPaused = false;

            CurrentTime = _duration;
            if (CurrentTime <= 0.0f)
            {
                OnCompleted();
            }
        }

        public void Begin(float newDuration)
        {
            _duration = newDuration;

            Begin();
        }

        public void Step(float stepTime)
        {
            if (!IsPaused && IsActive && CurrentTime > 0.0f)
            {
                CurrentTime -= stepTime;

                OnUpdated();

                while (IsActive && CurrentTime <= 0.0f)
                {
                    OnCompleted();
                }
            }
        }

        public void Pause()
        {
            IsPaused = true;
        }

        public void Resume()
        {
            IsPaused = false;
        }

        public void Stop()
        {
            IsActive = false;
            CurrentTime = 0.0f;
        }
        #endregion

        #region Protected Methods
        #endregion

        #region Private Methods
        private void OnCompleted()
        {
            _onCompleted?.Invoke(this);

            if (_loop)
            {
                CurrentTime += _duration;
            }
            else
            {
                IsActive = false;
            }
        }

        private void OnUpdated()
        {
            _onUpdated?.Invoke(this, Mathf.Clamp01(1.0f - (CurrentTime / _duration)));
        }
        #endregion
    }
}
