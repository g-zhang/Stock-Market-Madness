using UnityEngine;
using System.Collections;
using InControl;

public class PlayerControls {
    public enum Profile { Layout0 = 0, Layout1, size }

    private int playerNum;
    private Profile curLayoutMode = Profile.Layout0;
    protected InputDevice inputDevice = null;


    public InputDevice Input
    {
        get
        {
            return inputDevice;
        }
    }

    InputDevice getInputDevice()
    {
        return (InputManager.Devices.Count > playerNum) ? InputManager.Devices[playerNum] : null;
    }

    // VJR(Virtual Joystick Region) Sample 
    // http://forum.unity3d.com/threads/vjr-virtual-joystick-region-sample.116076/
    public Vector2 GetRadius(Vector2 midPoint, Vector2 endPoint, float maxDistance)
    {
        Vector2 distance = endPoint;
        if (Vector2.Distance(midPoint, endPoint) > maxDistance)
        {
            distance = endPoint - midPoint; distance.Normalize();
            return (distance * maxDistance) + midPoint;
        }
        return distance;
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
                    return inputDevice.DPad.Down;

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
                    return inputDevice.DPad.Up;

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
                    return inputDevice.DPad.Down.WasPressed;

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
                    return inputDevice.DPad.Up.WasPressed;

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
                    return inputDevice.DPad.Left;

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
                    return inputDevice.DPad.Down;

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
                    return inputDevice.DPad.Right;

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


    public bool ButtonA
    {
        get
        {
            return inputDevice.Action1;
        }
    }

    public bool ButtonB
    {
        get
        {
            return inputDevice.Action2;
        }
    }

    public bool ButtonX
    {
        get
        {
            return inputDevice.Action3;
        }
    }

    public bool ButtonY
    {
        get
        {
            return inputDevice.Action4;
        }
    }
}
