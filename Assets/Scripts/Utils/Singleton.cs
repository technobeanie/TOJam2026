using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T : Component
{
	private const string PrefabPath = "Managers/{0}";

	private static T _instance = null;
	public static T Instance
	{
		get
		{
			// Find first.
			if (_instance == null)
			{
				_instance = FindAnyObjectByType<T>();
			}

			// Load it.
			if (_instance == null)
			{
				var instancePrefab = Resources.Load<T>(string.Format(PrefabPath, typeof(T).Name));
				if (instancePrefab != null)
				{
					_instance = Instantiate(instancePrefab);
				}
			}

			// Create then.
			if (_instance == null)
			{
				var flowManagerObj = new GameObject(typeof(T).Name);
				_instance = flowManagerObj.AddComponent<T>();
			}

			return _instance;
		}
	}

	public static bool IsInstanceNull
	{
		get { return _instance == null; }
	}

	public void ResolveInstance() { }
}
