using UnityEngine;
using System.Collections;
using InControl;

public class Player : MonoBehaviour {

    public int playerNum;

    [Header("Status")]
    public float currentMoney;
    public int[] CompanyShares = new int[(int)CompanyName.size];
    public bool holdState = false;
    public float currDelayTime = 0f;
    public float currHoldTime = 0f;
    public CompanyName selectedCompany = CompanyName.none;

    [Header("Config")]
    public float delayActionTime = .5f; //time to hold the button before it starts auto buying/selling
    public float holdActionRate = 4f; //rate of auto buying/selling

    


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
        selectedCompany = CompanyName.none;



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

            //hold button down logic
            if(input.Action1 || input.Action2)
            {
                currDelayTime += Time.deltaTime;
            }

            if(currDelayTime >= delayActionTime)
            {
                currHoldTime += Time.deltaTime;
                if(currHoldTime >= 1f / holdActionRate)
                {
                    currHoldTime = 0f;
                    if(input.Action1)
                    {
                        BuyShares(selectedCompany);
                    }
                    if(input.Action2)
                    {
                        SellShares(selectedCompany);
                    }
                }
            }
        }

        if(currDelayTime != 0 && !(input.Action1 || input.Action2) || input.DPad.HasChanged)
        {
            currDelayTime = 0f;
            currHoldTime = 0f;
        }
    }


    void BuyShares(CompanyName company)
    {
        print("Bought " + company);
		CompanyShares [(int)company]++;
    }

    void SellShares(CompanyName company)
    {
        print("Sell " + company);
		CompanyShares [(int)company]--;
    }
}
