using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISerializable
{
    // const

    // public

    // protected

    // private

    // properties

    #region Unity Methods
    #endregion

    #region Public Methods
    public abstract Dictionary<string, object> Serialize();
    public abstract void Deserialize(Dictionary<string, object> serialization);
    #endregion

    #region Protected Methods
    #endregion

    #region Private Methods
    #endregion
}
