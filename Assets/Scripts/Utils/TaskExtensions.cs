using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Utils
{
    public static class TaskExtensions
    {
        // const

        // public

        // protected

        // private

        // properties

        #region Unity Methods
        #endregion

        #region Public Methods
        public static async void WrapErrors(this Task task)
        {
            try
            {
                await task;
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
            }
        }
        #endregion

        #region Protected Methods
        #endregion

        #region Private Methods
        #endregion
    }
}
