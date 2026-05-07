using Common.Audio;
using Common.Flow;
using Common.Joystick;
using Common.Rendering;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static Common.Joystick.JoystickManager;

public class SokobanView : View
{
    // const
    private readonly static List<string> LoadingViewNames = new List<string>()
    {
        "Loading"
    };

    // private
    [Header("Camera")]
    [SerializeField] private string _mainCameraName = "MainCamera";

    [Header("Audio")]
    [SerializeField] private AudioHook _audioTriggerOnStart = null;

    private Camera _mainCamera = null;

    // properties
    protected Camera MainCamera
    {
        get
        {
            if (_mainCamera == null)
            {
                _mainCamera = CameraManager.Instance.GetCamera(_mainCameraName);
            }
            return _mainCamera;
        }
    }

    #region Unity Methods
    protected virtual void Awake()
	{
        // For debug purposes only.
        if (FlowManager.IsInstanceNull)
        {
            Application.targetFrameRate = 60;

            // Make sure managers are initialized.
            AudioManager.Instance.ResolveInstance();
            CameraManager.Instance.ResolveInstance();
            FlowManager.Instance.ResolveInstance();
            JoystickManager.Instance.ResolveInstance();

            SetupKeyboardInputs();
        }
    }

    protected virtual void Start()
    {
        if (_audioTriggerOnStart != null)
        {
            _audioTriggerOnStart.Play();
        }

        // Preload all loadings.
        for (int i = 0; i < LoadingViewNames.Count; ++i)
        {
            var LoadingViewName = LoadingViewNames[i];
            if (!FlowManager.Instance.IsLoadingViewAvailable(LoadingViewName))
            {
                FlowManager.Instance.PreloadLoadingView(LoadingViewName);
            }
        }
    }
    #endregion

    #region View Methods
    #endregion

    #region Protected Methods
    protected void ResetCamera()
    {
        // Reset the camera position.
        if (MainCamera != null)
        {
            MainCamera.transform.localPosition = new Vector3(0.0f, 0.0f, MainCamera.transform.localPosition.z);
        }
    }
    #endregion

    #region Private Methods
    private void SetupKeyboardInputs()
    {
        JoystickManager.Instance.SetKeyboard(new JoystickManager.KeyboardDefinition()
        {
            Buttons = new Dictionary<Button, IList<Key>>()
                {
                    {
                        Button.Xbox_A, new List<Key>()
                        {
                            Key.Space
                        }
                    },
                    {
                        Button.Xbox_LB, new List<Key>()
                        {
                            Key.Q, Key.DownArrow
                        }
                    },
                    {
                        Button.Xbox_RB, new List<Key>()
                        {
                            Key.E, Key.UpArrow
                        }
                    },
                    {
                        Button.Xbox_Left_Trigger, new List<Key>()
                        {
                            Key.Digit1
                        }
                    },
                    {
                        Button.Xbox_Right_Trigger, new List<Key>()
                        {
                            Key.Digit2
                        }
                    },
                    {
                        Button.Xbox_Y, new List<Key>()
                        {
                            Key.LeftCtrl, Key.RightCtrl
                        }
                    },
                    {
                        Button.Xbox_View, new List<Key>()
                        {
                            Key.R
                        }
                    },
                    {
                        Button.Xbox_Menu, new List<Key>()
                        {
                            Key.L
                        }
                    }
                },
            Joysticks = new Dictionary<JoystickManager.Joystick, IList<KeyboardJoystickDefinition>>()
                {
                    {
                        JoystickManager.Joystick.Left, new List<KeyboardJoystickDefinition>()
                        {
                            new KeyboardJoystickDefinition(Key.W, Key.A, Key.S, Key.D),
                            new KeyboardJoystickDefinition(Key.None, Key.LeftArrow, Key.None, Key.RightArrow)
                        }
                    },
                }
        });
    }
    #endregion
}
