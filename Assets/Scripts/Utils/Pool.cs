using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pool<T> where T : MonoBehaviour
{
    // const

    // public

    // protected

    // private
    private List<T> _availables = new List<T>();
    private List<T> _inUse = new List<T>();

    private T _prefab = null;
    private Transform _parent = null;

    // properties
    public IReadOnlyList<T> InUse
    {
        get { return _inUse.AsReadOnly(); }
    }

    #region Unity Methods
    public Pool(T prefab, Transform parent)
    {
        _prefab = prefab;
        _parent = parent;
    }
    #endregion

    #region Public Methods
    public void Prewarm(int prewarmAmount = 0)
    {
        if (_availables.Count < prewarmAmount)
        {
            CreateInstance(prewarmAmount - _availables.Count);
        }
    }

    public T Pop()
    {
        T instance = null;
        if (_availables.Count == 0)
        {
            CreateInstance();
        }

        if (_availables.Count > 0)
        {
            instance = _availables[0];
            instance.gameObject.SetActive(true);

            _availables.RemoveAt(0);
            _inUse.Add(instance);
        }

        return instance;
    }

    public void Push(T instance)
    {
        if (instance != null && _inUse.Contains(instance))
        {
            _inUse.Remove(instance);

            SetupInstance(instance);
        }
    }

    public void Push(List<T> instances)
    {
        for (int i = 0; i < instances.Count; ++i)
        {
            Push(instances[i]);
        }
    }

    public void PushBackAll()
    {
        for (int i = _inUse.Count - 1; i >= 0; --i)
        {
            var instance = _inUse[i];

            _inUse.Remove(instance);

            SetupInstance(instance);
        }
    }

    public void AddInstances(T[] instances)
    {
        for (int i = 0; i < instances.Length; ++i)
        {
            var instance = instances[i];

            SetupInstance(instance);
        }
    }
    #endregion

    #region Protected Methods
    #endregion

    #region Private Methods
    private void CreateInstance(int amount = 1)
    {
        if (_prefab != null)
        {
            for (int i = 0; i < amount; ++i)
            {
                var instance = GameObject.Instantiate(_prefab, _parent, false);

                SetupInstance(instance);
            }
        }
    }

    private void SetupInstance(T instance)
    {
        instance.transform.SetParent(_parent, false);
        instance.gameObject.SetActive(false);
        _availables.Add(instance);
    }
    #endregion
}
