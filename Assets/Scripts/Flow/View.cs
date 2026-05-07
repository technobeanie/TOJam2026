using System;
using System.Collections.Generic;
using UnityEngine;

namespace Common.Flow
{
    public class View : MonoBehaviour
    {
        public enum ViewStateIds
        {
            Loading,
            Loaded,
            Opening,
            Opened,
            Closing,
            Closed,
            Hidden
        }

        // protected
        protected Dictionary<string, object> _parameters = new Dictionary<string, object>();

        // private
        private Action<string> _onDone = null;

        // properties
        public string ViewName
        {
            get; private set;
        }

        public ViewStateIds ViewState
        {
            get; private set;
        }

        #region View Methods
        public void LoadView(Dictionary<string, object> parameters, string viewName)
        {
            ViewState = ViewStateIds.Loading;

            _parameters = parameters;
            ViewName = viewName;

            OnViewLoaded();
        }

        public void OpenView(Action<string> onOpened)
        {
            ViewState = ViewStateIds.Opening;

            _onDone = onOpened;

            if (_onDone == null)
            {
                Debug.LogError("There should always be a callback for when the loading is completed");
            }

            OnViewOpening();
        }

        public void PrepareCloseView(Action<string> onClosed)
        {
            ViewState = ViewStateIds.Closing;

            _onDone = onClosed;

            if (_onDone == null)
            {
                Debug.LogError("There should always be a callback for when the prepare closing is completed");
            }

            OnViewPrepareClosing();
        }

        public void CloseView(Dictionary<string, object> parameters)
        {
            ViewState = ViewStateIds.Closing;

            OnViewClosed(parameters);
        }

        public void HideView()
        {
            ViewState = ViewStateIds.Hidden;

            OnViewHidden();
        }

        public void ReturnView(Dictionary<string, object> parameters)
        {
            ViewState = ViewStateIds.Opened;

            OnViewReturned(parameters);
        }
        #endregion

        #region Protected Methods
        protected virtual void OnViewLoaded()
        {
            ViewState = ViewStateIds.Loaded;
        }

        protected virtual void OnViewOpening()
        {
            ViewState = ViewStateIds.Opening;

            OnViewOpened();
        }

        protected virtual void OnViewOpened()
        {
            ViewState = ViewStateIds.Opened;

            var onDone = _onDone;
            _onDone = null;
            onDone?.Invoke(ViewName);
        }

        protected virtual void OnViewPrepareClosing()
        {
            ViewState = ViewStateIds.Closing;

            OnViewPrepareClosed();
        }

        protected virtual void OnViewPrepareClosed()
        {
            ViewState = ViewStateIds.Closing;

            var onDone = _onDone;
            _onDone = null;
            onDone?.Invoke(ViewName);
        }

        protected virtual void OnViewClosed(Dictionary<string, object> parameters)
        {
            ViewState = ViewStateIds.Closed;
        }

        protected virtual void OnViewHidden()
        {
            ViewState = ViewStateIds.Hidden;
        }

        protected virtual void OnViewReturned(Dictionary<string, object> parameters)
        {
            ViewState = ViewStateIds.Opened;
        }
        #endregion

        #region UI Methods
        public void UI_OpenView(string viewName)
        {
            FlowManager.Instance.OpenView(viewName);
        }

		public void UI_OpenPopup(string viewName)
		{
			FlowManager.Instance.OpenView(viewName, popup: true);
        }

        public void UI_CloseSelf()
        {
            FlowManager.Instance.CloseView(ViewName);
        }
        #endregion
    }
}
