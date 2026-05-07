using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PoolGameObject<T> : MonoBehaviour where T : MonoBehaviour
{
    // const

    // public

    // protected

    // private
    [SerializeField] private T _prefab = null;
    [SerializeField] private int _prewarmAmount = 0;

    public Pool<T> Pool
    {
        get; private set;
    }

    // properties

    #region Unity Methods
    private void Awake()
    {
        Pool = new Pool<T>(_prefab, transform);
        Pool.AddInstances(GetComponentsInChildren<T>(true));
        Pool.Prewarm(_prewarmAmount);
    }
    #endregion

    #region Public Methods
    #endregion

    #region Protected Methods
    #endregion

    #region Private Methods
    #endregion
}
