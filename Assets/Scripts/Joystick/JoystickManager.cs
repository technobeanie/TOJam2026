using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

namespace Common.Joystick
{
    public class JoystickManager : Singleton<JoystickManager>
    {
        // const
        public enum Button
        {
            None = 0,
            Xbox_A,
            Xbox_B,
            Xbox_X,
            Xbox_Y,
            Xbox_LB,
            Xbox_RB,
            Xbox_Left_Trigger,
            Xbox_Right_Trigger,
            Xbox_Left_Stick,
            Xbox_Right_Stick,
            Xbox_Dpad_Up,
            Xbox_Dpad_Left,
            Xbox_Dpad_Down,
            Xbox_Dpad_Right,
            Xbox_View,
            Xbox_Menu,
            Xbox_Home,
            Switch_Capture,
        }

        public enum Joystick
        {
            Left,
            Right,
            Dpad,
        }

        public enum Trigger
        {
            Left,
            Right
        }

        // struct
        public struct KeyboardDefinition
        {
            public IDictionary<Button, IList<Key>> Buttons;
            public IDictionary<Joystick, IList<KeyboardJoystickDefinition>> Joysticks;

            public KeyboardDefinition(IDictionary<Button, IList<Key>> buttons, IDictionary<Joystick, IList<KeyboardJoystickDefinition>> joysticks)
            {
                Buttons = buttons;
                Joysticks = joysticks;
            }
        }

        public struct KeyboardJoystickDefinition
        {
            public Key Up;
            public Key Left;
            public Key Down;
            public Key Right;

            public KeyboardJoystickDefinition(Key keyUp, Key keyleft, Key keyDown, Key keyRight)
            {
                Up = keyUp;
                Left = keyleft;
                Down = keyDown;
                Right = keyRight;
            }
        }

        // public

        // protected

        // private
        private KeyboardDefinition _keyboardDefinition = default;

        // properties

        #region Unity Methods
        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }
        #endregion

        #region Public Methods
        public void SetKeyboard(KeyboardDefinition keyboardDefinition)
        {
            _keyboardDefinition = keyboardDefinition;
        }

        public Vector2 GetAnyMovement(Joystick joystick)
        {
            var movement = Vector2.zero;

            movement += GetKeyboardMovement(joystick, Keyboard.current);
            movement += GetGamepadMovement(joystick, Gamepad.current);

            return movement;
        }

        public Vector2 GetMovement(Joystick joystick, InputDevice inputDevice)
        {
            if (inputDevice != null)
            {
                if (inputDevice is Keyboard keyboard)
                {
                    return GetKeyboardMovement(joystick, keyboard);
                }
                else if (inputDevice is Gamepad gamepad)
                {
                    return GetGamepadMovement(joystick, gamepad);
                }
            }

            return Vector2.zero;
        }

        public bool IsButtonDown(Button button)
        {
            if (!GetKeyboardButtonDown(button, Keyboard.current))
            {
                return GetGamepadButtonDown(button, Gamepad.current);
            }

            return false;
        }

        public bool IsButtonDown(Button button, InputDevice inputDevice)
        {
            if (inputDevice != null)
            {
                if (inputDevice is Keyboard keyboard)
                {
                    return GetKeyboardButtonDown(button, keyboard);
                }
                else if (inputDevice is Gamepad gamepad)
                {
                    return GetGamepadButtonDown(button, gamepad);
                }
            }

            return false;
        }

        public bool IsButtonDownThisFrame(Button button)
        {
            if (!GetKeyboardButtonDownThisFrame(button, Keyboard.current))
            {
                return GetGamepadButtonDownThisFrame(button, Gamepad.current);
            }

            return false;
        }

        public bool IsButtonDownThisFrame(Button button, InputDevice inputDevice)
        {
            if (inputDevice != null)
            {
                if (inputDevice is Keyboard keyboard)
                {
                    return GetKeyboardButtonDownThisFrame(button, keyboard);
                }
                else if (inputDevice is Gamepad gamepad)
                {
                    return GetGamepadButtonDownThisFrame(button, gamepad);
                }
            }

            return false;
        }

        public float IsTriggerDown(Trigger trigger)
        {
            var triggerValue = 0.0f;

            triggerValue = GetKeyboardTrigger(trigger, Keyboard.current);
            if (triggerValue <= 0.0f)
            {
                triggerValue = GetGamepadTrigger(trigger, Gamepad.current);
            }

            return triggerValue;
        }

        public float IsTriggerDown(Trigger trigger, InputDevice inputDevice)
        {
            if (inputDevice != null)
            {
                if (inputDevice is Keyboard keyboard)
                {
                    return GetKeyboardTrigger(trigger, keyboard);
                }
                else if (inputDevice is Gamepad gamepad)
                {
                    return GetGamepadTrigger(trigger, gamepad);
                }
            }

            return 0.0f;
        }
        #endregion

        #region Protected Methods
        #endregion

        #region Private Methods
        private Vector2 GetKeyboardMovement(Joystick joystick, Keyboard keyboard)
        {
            if (keyboard == null)
            {
                return Vector2.zero;
            }

            Vector2 movement = Vector2.zero;

            if (_keyboardDefinition.Joysticks != null && _keyboardDefinition.Joysticks.ContainsKey(joystick))
            {
                IList<KeyboardJoystickDefinition> keyboardJoystickDefinitions = _keyboardDefinition.Joysticks[joystick];
                for (int i = 0; i < keyboardJoystickDefinitions.Count; ++i)
                {
                    if (keyboardJoystickDefinitions[i].Left != Key.None && keyboard[keyboardJoystickDefinitions[i].Left].isPressed)
                    {
                        movement.x += -1.0f;
                    }
                    if (keyboardJoystickDefinitions[i].Right != Key.None && keyboard[keyboardJoystickDefinitions[i].Right].isPressed)
                    {
                        movement.x += 1.0f;
                    }
                    if (keyboardJoystickDefinitions[i].Up != Key.None && keyboard[keyboardJoystickDefinitions[i].Up].isPressed)
                    {
                        movement.y += 1.0f;
                    }
                    if (keyboardJoystickDefinitions[i].Down != Key.None && keyboard[keyboardJoystickDefinitions[i].Down].isPressed)
                    {
                        movement.y += -1.0f;
                    }
                }
            }

            return movement.normalized;
        }

        private Vector2 GetGamepadMovement(Joystick joystick, Gamepad gamepad)
        {
            if (gamepad == null)
            {
                return Vector2.zero;
            }

            switch (joystick)
            {
                case Joystick.Left:
                    return gamepad.leftStick.ReadValue();
                case Joystick.Right:
                    return gamepad.rightStick.ReadValue();
                case Joystick.Dpad:
                    return gamepad.dpad.ReadValue();
                default:
                    return Vector2.zero;
            }
        }

        private bool GetKeyboardButtonDown(Button button, Keyboard keyboard)
        {
            var keys = GetKeyboardButtonControl(button, keyboard);
            if (keys != null)
            {
                for (int i = 0; i < keys.Count; ++i)
                {
                    if (keyboard[keys[i]].isPressed)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private bool GetKeyboardButtonDownThisFrame(Button button, Keyboard keyboard)
        {
            var keys = GetKeyboardButtonControl(button, keyboard);
            if (keys != null)
            {
                for (int i = 0; i < keys.Count; ++i)
                {
                    if (keyboard[keys[i]].wasPressedThisFrame)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private IList<Key> GetKeyboardButtonControl(Button button, Keyboard keyboard)
        {
            if (keyboard == null)
            {
                return null;
            }

            if (_keyboardDefinition.Buttons != null && _keyboardDefinition.Buttons.ContainsKey(button))
            {
                return _keyboardDefinition.Buttons[button];
            }

            return null;
        }

        private bool GetGamepadButtonDown(Button button, Gamepad gamepad)
        {
            var buttonControl = GetGamepadButtonControl(button, gamepad);
            return buttonControl != null && buttonControl.isPressed;
        }

        private bool GetGamepadButtonDownThisFrame(Button button, Gamepad gamepad)
        {
            var buttonControl = GetGamepadButtonControl(button, gamepad);
            return buttonControl != null && buttonControl.wasPressedThisFrame;
        }

        private ButtonControl GetGamepadButtonControl(Button button, Gamepad gamepad)
        {
            if (gamepad == null)
            {
                return null;
            }

            switch (button)
            {
                case Button.Xbox_A:
                    return gamepad.aButton;
                case Button.Xbox_B:
                    return gamepad.bButton;
                case Button.Xbox_X:
                    return gamepad.xButton;
                case Button.Xbox_Y:
                    return gamepad.yButton;
                case Button.Xbox_LB:
                    return gamepad.leftShoulder;
                case Button.Xbox_RB:
                    return gamepad.rightShoulder;
                case Button.Xbox_Left_Trigger:
                    return gamepad.leftTrigger;
                case Button.Xbox_Right_Trigger:
                    return gamepad.rightTrigger;
                case Button.Xbox_Left_Stick:
                    return gamepad.leftStickButton;
                case Button.Xbox_Right_Stick:
                    return gamepad.rightStickButton;
                case Button.Xbox_Dpad_Up:
                    return gamepad.dpad.up;
                case Button.Xbox_Dpad_Left:
                    return gamepad.dpad.left;
                case Button.Xbox_Dpad_Down:
                    return gamepad.dpad.down;
                case Button.Xbox_Dpad_Right:
                    return gamepad.dpad.right;
                case Button.Xbox_View:
                    return gamepad.selectButton;
                case Button.Xbox_Menu:
                    return gamepad.startButton;
                case Button.Xbox_Home:
                    if (gamepad is UnityEngine.InputSystem.DualShock.DualShockGamepad psGamepad)
                    {
                        return psGamepad.touchpadButton;
                    }
                    else if (gamepad is UnityEngine.InputSystem.Switch.SwitchProControllerHID switchGamepad1)
                    {
                        return switchGamepad1.homeButton;
                    }
                    return null;
                case Button.Switch_Capture:
                    if (gamepad is UnityEngine.InputSystem.Switch.SwitchProControllerHID switchGamepad2)
                    {
                        return switchGamepad2.captureButton;
                    }
                    return null;
                default:
                    return null;
            }
        }

        private float GetKeyboardTrigger(Trigger trigger, Keyboard keyboard)
        {
            if (keyboard == null)
            {
                return 0.0f;
            }

            switch (trigger)
            {
                case Trigger.Left:
                    return GetKeyboardButtonDown(Button.Xbox_Left_Trigger, keyboard) ? 1.0f : 0.0f;
                case Trigger.Right:
                    return GetKeyboardButtonDown(Button.Xbox_Right_Trigger, keyboard) ? 1.0f : 0.0f;
                default:
                    return 0.0f;
            }
        }

        private float GetGamepadTrigger(Trigger trigger, Gamepad gamepad)
        {
            if (gamepad == null)
            {
                return 0.0f;
            }

            switch (trigger)
            {
                case Trigger.Left:
                    return gamepad.leftTrigger.ReadValue();
                case Trigger.Right:
                    return gamepad.rightTrigger.ReadValue();
                default:
                    return 0.0f;
            }
        }
        #endregion
    }
}