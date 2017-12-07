using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Settings : MonoBehaviour {

    private TouchScreenKeyboard keyboard;

    // Player Speed Mult
    public static float PlayerSpeedMult = 4f;
    private bool PlayerSpeedMultMenuOpen = false;

    [SerializeField]
    private Text playerSpeedMultButton;

    private void Start()
    {
        playerSpeedMultButton.text = PlayerSpeedMult.ToString();
    }

    public void GetFloatFromPlayer(string text)
    {
        PlayerSpeedMultMenuOpen = true;
        keyboard = TouchScreenKeyboard.Open(text, TouchScreenKeyboardType.NumberPad);
    }

    private void Update()
    {
        if (keyboard != null && (keyboard.done))
        {
            Debug.Log("Android Debug: Keyboard Input Completed");
            PlayerSpeedMult = float.Parse(keyboard.text);
            playerSpeedMultButton.text = keyboard.text;
            ResetKeyboard(ref PlayerSpeedMultMenuOpen);
        }
    }

    private void ResetKeyboard(ref bool b)
    {
        Debug.Log("Android Debug: Keyboard Reset");
        b = false;
        keyboard = null;
    }
}
