using Common.Audio;
using Common.Joystick;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using Utils;

public class PlayerPackController : PlayerController
{
    // const

    // public

    // protected

    // private

    // properties
    public int MaxToOpen
    {
        get; set;
    }

    public int CurrentOpen
    {
        get; private set;
    }

    #region Unity Methods
    protected override void FixedUpdate()
    {
        base.FixedUpdate();
       
        if (IsDone)
        {
            return;
        }
        
        if (JoystickManager.Instance.IsButtonDown(JoystickManager.Button.Xbox_A, InputDevice, KeyboardPlayerId))
        {
            OpenPack();
        }
    }
    #endregion

    #region Public Methods
    #endregion

    #region Protected Methods
    #endregion

    #region Private Methods
    private void OpenPack()
    {
        if (CurrentOpen >= MaxToOpen)
        {
            return;
        }

        // Raycast and find nearest sticker.
        var hits = Physics2D.RaycastAll(_player.position, Vector3.back);
        if (hits != null && hits.Length > 0)
        {
            foreach (var hit in hits)
            {
                var packController = hit.transform.GetComponentInParent<PackController>();
                if (packController != null)
                {
                    if (!packController.IsOpened)
                    {
                        ++CurrentOpen;
                        packController.Open();

                        break;
                    }
                }
            }
        }

        if (CurrentOpen >= MaxToOpen)
        {
            GameDone();
        }
    }
    #endregion
}
