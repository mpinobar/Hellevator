using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputChecker : MonoBehaviour
{
    [SerializeField] GameObject keyboardText;
    [SerializeField] GameObject gamepadText;

    private void CheckDevice(InputDevice device, InputDeviceChange change)
    {


        switch (change)
        {
            case InputDeviceChange.Added:
                {
                    Debug.LogError("Devide added" + device);
                    UIController.Instance.ShowPauseMenu();
                    Cursor.visible = true;
                    print(Gamepad.current);
                    print(Keyboard.current);
                    if (Gamepad.current != null)
                    {
                        SetGamepadText();
                    }
                    else
                        SetKeyboardText();

                }
                break;
            case InputDeviceChange.Disconnected:
                {
                    UIController.Instance.ShowPauseMenu();
                    Cursor.visible = true;
                    if (Gamepad.current != null)
                    {
                        SetGamepadText();
                    }
                    else
                        SetKeyboardText();
                }
                break;
            case InputDeviceChange.Reconnected:
                {

                    UIController.Instance.ShowPauseMenu();
                    Cursor.visible = true;
                    if (Gamepad.current != null)
                    {
                        SetGamepadText();
                    }
                    else
                        SetKeyboardText();
                }
                break;
            case InputDeviceChange.Removed:
                // Remove from Input System entirely; by default, Devices stay in the system once discovered.
                break;
            default:
                // See InputDeviceChange reference for other event types.
                break;
        }

    }


    public void SetGamepadText()
    {
        keyboardText.SetActive(false);
        gamepadText.SetActive(true);
    }

    public void SetKeyboardText()
    {
        keyboardText.SetActive(true);
        gamepadText.SetActive(false);
    }

    private void OnEnable()
    {
        InputSystem.onDeviceChange += CheckDevice;
        if (Gamepad.current != null)
        {
            SetGamepadText();
        }
        else
            SetKeyboardText();
    }
    private void OnDisable()
    {
        InputSystem.onDeviceChange -= CheckDevice;
    }

}
