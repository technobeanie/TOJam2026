using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

// https://docs.microsoft.com/en-us/dotnet/desktop/wpf/advanced/weak-event-patterns?view=netframeworkdesktop-4.8
public class EventManager<T> where T : System.Enum
{
		
	private static EventManager<T> _Instance;
	private Dictionary<T, Action<object, object>> _Callbacks = new Dictionary<T, Action<object, object>>();

	public static EventManager<T> Instance
	{
		get
		{
			if (_Instance == null)
			{
				_Instance = new EventManager<T>();
			}

			return _Instance;
		}
	}

	public static bool IsNotNull
	{
		get
		{
			return _Instance != null;
		}
	}

	private EventManager()
	{

	}

	~EventManager()
	{

	}

	public void Register(T e, Action<object, object> callback)
	{
		if (!_Callbacks.ContainsKey(e))
		{
			_Callbacks.Add(e, callback);
		}
		else
		{
			_Callbacks[e] += callback;
		}
	}

	public void Unregister(T e, Action<object, object> callback)
	{
		if (_Callbacks.ContainsKey(e))
		{
			_Callbacks[e] -= callback;
		}
		else
		{
			Debug.LogError($"Can unregister to the event {e.ToString()}");
		}
	}

	public void Trigger(T e, object sender, object param)
	{
		if (_Callbacks.ContainsKey(e) && _Callbacks[e] != null)
		{
			_Callbacks[e](sender, param);
		}
	}
}