using UnityEngine;
using System.Collections;
using InControl;

public class PlayerControls {
    public enum Profile { Layout0 = 0, Layout1, size }

    private int playerNum;
    private Profile curLayoutMode = Profile.Layout0;
    protected InputDevice inputDevice = null;

    InputDevice getInputDevice()
    {
        return (InputManager.Devices.Count > playerNum) ? InputManager.Devices[playerNum] : null;
    }

    // Use this for initialization
    public PlayerControls(int pnum)
    {
        playerNum = pnum;
        inputDevice = getInputDevice();
    }

    //returns true when the controler is active, false otherwise, also calls update
    //should should called once per frame in Player
    public bool Update()
    {
        inputDevice = getInputDevice();
        return isActive;
    }

    //Properties
    public bool isActive
    {
        get { return inputDevice != null; }
    }

    public Profile profile
    {
        get { return curLayoutMode; }
        set { curLayoutMode = value; }
    }

    public bool Buy
    {
        get
        {
            switch(curLayoutMode)
            {
                case Profile.Layout0:
                    return inputDevice.Action1;

                case Profile.Layout1:
                    return inputDevice.DPad.Down || inputDevice.LeftStick.Down;

                default:
                    return false;
            }
        }
    }

    public bool Sell
    {
        get
        {
            switch (curLayoutMode)
            {
                case Profile.Layout0:
                    return inputDevice.Action2;

                case Profile.Layout1:
                    return inputDevice.DPad.Up || inputDevice.LeftStick.Up;

                default:
                    return false;
            }
        }
    }

    public bool BuyPressed
    {
        get
        {
            switch (curLayoutMode)
            {
                case Profile.Layout0:
                    return inputDevice.Action1.WasPressed;

                case Profile.Layout1:
                    return inputDevice.DPad.Down.WasPressed || inputDevice.LeftStick.Down.WasPressed;

                default:
                    return false;
            }
        }
    }

    public bool SellPressed
    {
        get
        {
            switch (curLayoutMode)
            {
                case Profile.Layout0:
                    return inputDevice.Action2.WasPressed;

                case Profile.Layout1:
                    return inputDevice.DPad.Up.WasPressed || inputDevice.LeftStick.Up.WasPressed;

                default:
                    return false;
            }
        }
    }

    public bool Selection1
    {
        get
        {
            switch (curLayoutMode)
            {
                case Profile.Layout0:
                    return inputDevice.DPad.Left || inputDevice.LeftStick.Left;

                case Profile.Layout1:
                    return inputDevice.Action3;

                default:
                    return false;
            }
        }
    }

    public bool Selection2
    {
        get
        {
            switch (curLayoutMode)
            {
                case Profile.Layout0:
                    return inputDevice.DPad.Down || inputDevice.LeftStick.Down;

                case Profile.Layout1:
                    return inputDevice.Action1;

                default:
                    return false;
            }
        }
    }

    public bool Selection3
    {
        get
        {
            switch (curLayoutMode)
            {
                case Profile.Layout0:
                    return inputDevice.DPad.Right || inputDevice.LeftStick.Right;

                case Profile.Layout1:
                    return inputDevice.Action2;

                default:
                    return false;
            }
        }
    }

    public bool MenuButton
    {
        get
        {
            switch (curLayoutMode)
            {
                case Profile.Layout0:
                    return inputDevice.MenuWasPressed;

                case Profile.Layout1:
                    return inputDevice.MenuWasPressed;

                default:
                    return false;
            }
        }
    }
}
