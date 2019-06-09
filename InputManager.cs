using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Buttons
{
    Right,
    Left,
    Up,
    Down,
    Fire,
    Dash_Mouse,
    Dash_Controller,
    Next_Weapon,
    Last_Weapon,
    cAimVerticalUp,
    cAimVerticalDown,
    cAimHorizontalUp,
    cAimHorizontalDown,
    Blank,
    WeaponWheel
}

public enum Condition
{
    GreaterThan,
    LessThan
}

[System.Serializable]
public class InputAxisState
{
    public string axisName;
    public float offValue;
    public Buttons button;
    public Condition condition;

    public float value
    {
        get
        {
            var val = Input.GetAxis(axisName);
            return val;
        }
    }

    public bool key
    {
        get
        {
            var val = Input.GetAxis(axisName);
            switch (condition)
            {
                case Condition.GreaterThan:
                    return val > offValue;
                case Condition.LessThan:
                    return val < offValue;
            }
            return false;
        }
    }

    public float holdTime;
}

public class InputManager : MonoBehaviour
{

    public InputAxisState[] inputs;
    public InputState inputState;

    public static InputManager instance = null;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }
    // Update is called once per frame
    void Update()
    {
        if (!JP_GameManager.instance.inMenu)
        {
            if (inputState == null)
            {
                inputState = FindObjectOfType<InputState>();
            }

            foreach (var input in inputs)
            {
                inputState.SetButtonValue(input.button, input.key);
                inputState.SetAxisValue(input.button, input.value);
            } 
        }
    }
}
