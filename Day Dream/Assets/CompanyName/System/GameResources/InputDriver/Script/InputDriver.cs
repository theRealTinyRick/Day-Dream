using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Events;
using Rewired;

public class InputDriver : Singleton_MonoBehavior<InputDriver>
{
    //events
    public static JumpButtonEvent jumpButtonEvent = new JumpButtonEvent();
    public static LightAttackButtonEvent lightAttackButtonEvent = new LightAttackButtonEvent();
    public static EvadeButtonEvent evadeButtonEvent = new EvadeButtonEvent();

    //menu specific
    public static SelectButtonEvent selectButtonEvent = new SelectButtonEvent();
    public static BackButtonEvent backButtonEvent = new BackButtonEvent();

    // any key
    public static AnyButtonWasPressedEvent anyButtonWasPressedEvent = new AnyButtonWasPressedEvent();

    //vectors
    public static Vector3 LocomotionDirection = new Vector3();
    public static Vector3 LocomotionOrientationDirection = new Vector3();
    public static Vector3 RightInputDirection = new Vector3();

    //rewired reciever
    private Player input;
    public Player Input 
    {
        get
        {
            if(input == null)
            {
                input = ReInput.players.GetPlayer(0);
            }
            return input;
        }
    }

	private void Update () 
	{
        PlayerInputListener();
        MenuInputListener();
        AnyButtonPressed();
    }

    private void PlayerInputListener() 
    {
        //receive locomotion input
        float _locomotionX = Input.GetAxis(InputDataBase.LeftStickHorizontal);
        float _locomotionY = Input.GetAxis(InputDataBase.LeftStickVertical);

        // if no input could not be found check for input from the keyboard
        if(_locomotionX == 0 && _locomotionY == 0)
        {
            _locomotionX = UnityEngine.Input.GetAxis(InputDataBase.Horizontal);
            _locomotionY= UnityEngine.Input.GetAxis(InputDataBase.Vertical);
        }

        //assign the value above
        LocomotionDirection.x = _locomotionX;
        LocomotionDirection.z = _locomotionY;

        float _rightInputDirectionX = Input.GetAxis(InputDataBase.RightStickHorizontal);
        float _rightInputDirectionY = Input.GetAxis(InputDataBase.RightStickVertical);

        if(_rightInputDirectionX == 0 && _rightInputDirectionY == 0)
        {
            _rightInputDirectionX = UnityEngine.Input.GetAxis(InputDataBase.MouseX);
            _rightInputDirectionY = UnityEngine.Input.GetAxis(InputDataBase.MouseY);
        }

        RightInputDirection.x = _rightInputDirectionX;
        RightInputDirection.y = _rightInputDirectionY;

        //jump button from game pad then keyboard
        if(Input.GetButtonDown(InputDataBase.AButton) || UnityEngine.Input.GetKeyDown(KeyCode.Space))
        {
            if(jumpButtonEvent != null)
            {
                jumpButtonEvent.Invoke();
            }
        }

        if(Input.GetButtonDown(InputDataBase.XButton) || UnityEngine.Input.GetMouseButtonDown(0))
        {
            if(lightAttackButtonEvent != null)
            {
                lightAttackButtonEvent.Invoke();
            }
        }

        if(Input.GetButtonDown(InputDataBase.BButton) || UnityEngine.Input.GetKeyDown(KeyCode.LeftShift))
        {
            if(evadeButtonEvent != null)
            {
                evadeButtonEvent.Invoke();
            }
        }

        //Held input ->
    }

    private void MenuInputListener()
    {
        // Back
        if(Input.GetButtonDown(InputDataBase.BButton) || UnityEngine.Input.GetKeyDown(KeyCode.Escape))
        {
            if(backButtonEvent != null)
            {
                backButtonEvent.Invoke();
            }
        }

        // Select
        if(Input.GetButtonDown(InputDataBase.AButton) || UnityEngine.Input.GetKeyDown(KeyCode.Return))
        {
            if(selectButtonEvent != null)
            {
                selectButtonEvent.Invoke();
            }
        }
    }

    private void AnyButtonPressed()
    {
        if(Input.GetAnyButtonDown() || UnityEngine.Input.anyKeyDown)
        {
            if(anyButtonWasPressedEvent != null)
            {
                anyButtonWasPressedEvent.Invoke();
            }
        }
    }
}

[System.Serializable]
public class InputEventHandler
{
  
}

[System.Serializable]
public class InputDataBase
{
	public const string LeftStickHorizontal = "LeftStickHorizontal";
    public const string LeftStickVertical = "LeftStickVertical"; 
    
    public const string RightStickHorizontal = "RightStickHorizontal";
    public const string RightStickVertical = "RightStickVertical";

    public const string AButton = "AButton";
    public const string BButton = "BButton";
    public const string YButton = "YButton";
    public const string XButton = "XButton";

    public const string RightBumper = "RightBumper";
    public const string RightTrigger = "RightTrigger";

    public const string LeftBumper = "LeftBumper";
    public const string LeftTrigger = "LeftTrigger";
    
    // Keyboard inputs
    public const string Horizontal = "Horizontal";
    public const string Vertical = "Vertical";

    public const string MouseX = "Mouse X";
    public const string MouseY = "Mouse Y";
}