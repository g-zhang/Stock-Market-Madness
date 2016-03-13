using UnityEngine;
using System.Collections;
using InControl;

public class Player : MonoBehaviour {

    public int playerNum;

    [Header("Status")]
    public float currentMoney;
    public int[] CompanyShares = new int[(int)CompanyName.size];

    [Header("Config")]
    public float delayActionTime = .5f;
    public float holdActionRate = 4f;
    float currDelayTime = 0f;


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        var inputDevice = (InputManager.Devices.Count > playerNum) ? InputManager.Devices[playerNum] : null;
        if (inputDevice != null)
        {
            ControlsUpdate(inputDevice);
        } else
        {
            return;
        }
    }

    void ControlsUpdate(InputDevice input)
    {
        CompanyName selectedCompany = CompanyName.none;
        if(input.DPadLeft)
        {
            selectedCompany = CompanyName.A;
        }
        if(input.DPadDown)
        {
            selectedCompany = CompanyName.B;
        }
        if(input.DPadRight)
        {
            selectedCompany = CompanyName.C;
        }

        if(selectedCompany != CompanyName.none)
        {
            if(input.Action1.WasPressed)
            {
                BuyShares(selectedCompany);
            }
            if(input.Action2.WasPressed)
            {
                SellShares(selectedCompany);
            }
        }
    }


    void BuyShares(CompanyName company)
    {
        print("Bought " + company);
    }

    void SellShares(CompanyName company)
    {
        print("Sell " + company);
    }
}
