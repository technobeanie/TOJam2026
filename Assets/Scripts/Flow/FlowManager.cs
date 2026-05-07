using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Common.Flow
{
    public class FlowManager : Singleton<FlowManager>
    {
        // class
        private class SceneView
        {
            public View View = null;
            public Scene Scene;
        }

		// const
		private const float VIEW_DEPTH_OFFSET = 100.0f;

		// private
		private int _busyCount = 0;
        private List<SceneView> _openedViews = new List<SceneView>();
		private Dictionary<string, SceneView> _loadingViews = new Dictionary<string, SceneView>();
		private List<string> _openingViews = new List<string>();
		private List<string> _prepareClosingViews = new List<string>();

		private SceneView _currentView = null;
        private List<GameObject> _currentRootGameObjects = new List<GameObject>();

		// properties

		public bool IsBusy
        {
            get { return _busyCount > 0; }
        }

        #region Unity Methods
        private void Awake()
        {
            DontDestroyOnLoad(gameObject);

            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.sceneUnloaded += OnSceneUnloaded;
        }

        private void Start()
        {
			// Setup the current already loaded view.
			var parameters = new Dictionary<string, object>();

			OnViewLoaded(_currentView, parameters, true);
			StartCoroutine(OpenViewAsync(_currentView, parameters));
		}

        private void OnDestroy()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            SceneManager.sceneUnloaded -= OnSceneUnloaded;
        }
        #endregion

        #region Public Methods
        public void OpenView(string viewName, Dictionary<string, object> parameters = null, bool popup = false, string loadingViewName = "")
        {
			StartCoroutine(HandleLoadNewViewAsync(viewName, parameters, popup, loadingViewName));
		}

		public void CloseView(string viewName, Dictionary<string, object> parameters = null)
		{
			StartCoroutine(HandleCloseViewAsync(viewName, parameters));
		}

		public void PreloadLoadingView(string loadingViewName)
        {
			StartCoroutine(HandleLoadNewLoadingViewAsync(loadingViewName));
		}

		public void RemoveLoadingView(string loadingViewName)
		{
			StartCoroutine(HandleRemoveLoadingViewAsync(loadingViewName));
		}

		public bool IsLoadingViewAvailable(string loadingViewName)
        {
			return _loadingViews.ContainsKey(loadingViewName);
        }
		#endregion

		#region Private Methods
		private IEnumerator HandleLoadNewLoadingViewAsync(string loadingViewName)
		{
			while (IsBusy)
			{
				yield return null;
			}

			++_busyCount;

			yield return LoadLoadingView(loadingViewName);

			--_busyCount;
		}

		private IEnumerator LoadLoadingView(string loadingViewName)
        {
			var parameters = new Dictionary<string, object>();

			yield return LoadView(loadingViewName);
			OnViewLoaded(_currentView, parameters, false);

			_loadingViews.Add(loadingViewName, _currentView);
			_openedViews.Remove(_currentView);
		}

		private IEnumerator HandleRemoveLoadingViewAsync(string loadingViewName)
		{
			while (IsBusy)
			{
				yield return null;
			}

			++_busyCount;

			if (_loadingViews.ContainsKey(loadingViewName))
			{
				yield return SceneManager.UnloadSceneAsync(_loadingViews[loadingViewName].Scene);
			}

			--_busyCount;
		}

		private IEnumerator HandleLoadNewViewAsync(string viewName, Dictionary<string, object> parameters, bool popup, string loadingViewName = "")
		{
			while (IsBusy)
			{
				yield return null;
			}

			++_busyCount;

			SceneView loadingView = null;
			if (!string.IsNullOrEmpty(loadingViewName))
            {
				if (_loadingViews.ContainsKey(loadingViewName))
                {
					loadingView = _loadingViews[loadingViewName];
				}
				else
                {
					yield return LoadLoadingView(loadingViewName);
                }
            }

			if (loadingView != null)
			{
				// Put back the loading view at the top.
				_openedViews.Add(loadingView);

				// Prepare the closing (before the loading screen).
				yield return PrepareCloseViewsAsync();

				// Display loading first.
				OnViewLoaded(loadingView, parameters, false);
				yield return OpenViewAsync(loadingView, parameters);

				// Close others first.
				yield return CloseOtherViewsAsync(null, parameters, popup);

				// Then, open.
				yield return LoadView(viewName);
				OnViewLoaded(_currentView, parameters, true);

				// Hide loading.
				yield return PrepareCloseViewAsync(loadingView);
				yield return CloseViewAsync(loadingView, parameters, false);
			}
			else
			{
				// Prepare the closing (before the loading screen).
				yield return PrepareCloseViewsAsync();

				// Open first.
				yield return LoadView(viewName);
				OnViewLoaded(_currentView, parameters, false);

				// Then, close others.
				yield return CloseOtherViewsAsync(_currentView, parameters, popup);
			}

			// The new view is now ready to open!
			yield return OpenViewAsync(_currentView, parameters);

			--_busyCount;
		}

		private IEnumerator HandleCloseViewAsync(string viewName, Dictionary<string, object> parameters)
		{
			while (IsBusy)
			{
				yield return null;
			}

			++_busyCount;

			// Find out if the one we are closing is the top view.
			var isTopView = _openedViews[_openedViews.Count - 1].Scene.name == viewName;

			// Close the selected view. 
			yield return CloseViewAsync(viewName, parameters);

			// If we closed the top view, we need to return to the view under it.
			if (isTopView)
			{
				var newTopView = FindTopView();
				if (newTopView != null)
				{
					_currentView = newTopView;

					SceneManager.SetActiveScene(_currentView.Scene);
					_currentView.View.ReturnView(parameters);
				}
			}

			--_busyCount;
		}

		private SceneView FindTopView()
        {
			for (int i = _openedViews.Count - 1; i >= 0; --i)
            {
				var sceneView = _openedViews[i];
				if (!_loadingViews.ContainsKey(sceneView.Scene.name))
                {
					return sceneView;
				}
            }

			return null;
        }

		private IEnumerator LoadView(string viewName)
        {
			_currentView = null;
			yield return SceneManager.LoadSceneAsync(viewName, LoadSceneMode.Additive);

			// This is to wait for the currentView to be properly set (a quick delay might be necessary).
			while (_currentView == null)
			{
				yield return null;
			}
		}

		private void OnViewLoaded(SceneView sceneView, Dictionary<string, object> parameters, bool setVisible)
		{
			if (setVisible)
			{
				sceneView?.View?.gameObject.SetActive(true);
			}

			sceneView?.View?.LoadView(parameters, sceneView?.Scene.name);
		}

		private IEnumerator PrepareCloseViewsAsync()
		{
			for (int i = _openedViews.Count - 1; i >= 0; --i)
			{
				var openedView = _openedViews[i];
				if (!_loadingViews.ContainsKey(openedView.Scene.name))
				{
					yield return PrepareCloseViewAsync(openedView);
				}
			}
		}

		private IEnumerator PrepareCloseViewAsync(SceneView sceneView)
		{
			// Inform the view that it's preparing to be closed.
			_prepareClosingViews.Add(sceneView.Scene.name);
			sceneView?.View?.PrepareCloseView( OnViewPrepareClosed);

			while (_prepareClosingViews.Contains(sceneView.Scene.name))
			{
				yield return null;
			}
		}

		private void OnViewPrepareClosed(string viewName)
		{
			_prepareClosingViews.Remove(viewName);
		}

		private IEnumerator CloseOtherViewsAsync(SceneView sceneView, Dictionary<string, object> parameters, bool popup)
        {
			for (int i = _openedViews.Count - 1; i >= 0; --i)
			{
				var openedView = _openedViews[i];
				if ((sceneView == null || openedView.Scene != sceneView.Scene) && !_loadingViews.ContainsKey(openedView.Scene.name))
				{
					if (!popup)
					{
						yield return CloseViewAsync(openedView, parameters, true);
					}
					else
                    {
						openedView?.View?.HideView();
					}
				}
			}
		}

		private IEnumerator CloseViewAsync(string viewName, Dictionary<string, object> parameters)
		{
			for (int i = _openedViews.Count - 1; i >= 0; --i)
			{
				SceneView sceneView = _openedViews[i];
				if (sceneView.Scene.name == viewName)
				{
					yield return CloseViewAsync(sceneView, parameters, true);
					break;
				}
			}
		}

		private IEnumerator OpenViewAsync(SceneView sceneView, Dictionary<string, object> parameters)
        {
			// TODO: Set the Z depth position.
			sceneView.View.transform.position = new Vector3(sceneView.View.transform.position.x, sceneView.View.transform.position.y, _openedViews.IndexOf(sceneView) * -VIEW_DEPTH_OFFSET);

			SceneManager.SetActiveScene(sceneView.Scene);

			// Inform the view that it is being opened.
			_openingViews.Add(sceneView?.Scene.name);
			sceneView?.View?.gameObject.SetActive(true);
			sceneView?.View?.OpenView(OnViewOpened);

			while (_openingViews.Contains(sceneView?.Scene.name))
			{
				yield return null;
			}
		}

		private void OnViewOpened(string viewName)
		{
			_openingViews.Remove(viewName);
		}

		private IEnumerator CloseViewAsync(SceneView sceneView, Dictionary<string, object> parameters, bool unload)
		{
			// Inform the view that it's being closed.
			sceneView?.View?.CloseView(parameters);
			sceneView?.View?.gameObject.SetActive(false);

			if (unload)
			{
				yield return SceneManager.UnloadSceneAsync(sceneView.Scene);
			}
			else
            {
				_openedViews.Remove(sceneView);
			}
		}
		#endregion

		#region Callbacks
		private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            View view = null;

            scene.GetRootGameObjects(_currentRootGameObjects);
            for (int i = 0; i < _currentRootGameObjects.Count; ++i)
            {
                var viewTemp = _currentRootGameObjects[i].GetComponent<View>();
                if (viewTemp != null)
                {
                    view = viewTemp;

					// Hide it for now.
					view.gameObject.SetActive(false);

					break;
                }
            }

            if (view == null)
            {
                Debug.LogError($"View not found in {scene.name}");
            }

            _currentView = new SceneView()
            {
                Scene = scene,
                View = view
            };

            _openedViews.Add(_currentView);
        }

        private void OnSceneUnloaded(Scene scene)
        {
            for (int i = _openedViews.Count - 1; i >= 0 ; --i)
			{
				if (_openedViews[i].Scene == scene)
				{
					_openedViews.RemoveAt(i);
					break;
				}
			}

			if (_loadingViews.ContainsKey(scene.name))
            {
				_loadingViews.Remove(scene.name);
			}
        }
        #endregion
    }
}
