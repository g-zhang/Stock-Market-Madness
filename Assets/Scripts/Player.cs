using UnityEngine;
using System.Collections;
using InControl;

public enum CompanyName { A, B, C, size, none }

public class Player : MonoBehaviour {

    public int playerNum;
    private PlayerControls controls;
    private Trader playerData;
    private GamePhases prevPhase;

    [Header("Status")]
    public float currDelayTime = 0f;
    public float currHoldTime = 0f;
    public CompanyName selectedCompany = CompanyName.none;
    public CompanyDecisions selectedDecision = CompanyDecisions.none;
    private CompanyName prevCompany = CompanyName.none;

    [Header("Config")]
    public PlayerControls.Profile controlLayout = PlayerControls.Profile.Layout0;
    public float delayActionTime = .5f; //time to hold the button before it starts auto buying/selling
    public float holdActionRate = 4f; //rate of auto buying/selling
    int sharesPerAction = 1000; //number of shares bought per button press

    //Properties
    public float currentMoney
    {
        get
        {
            if(playerData != null)
            {
                return playerData.money;
            } else
            {
                return 0f;
            }
        }
    }

    public int getPlayerStockData(CompanyName name)
    {
        if(name < CompanyName.size)
        {
            return playerData.shares[Model.Instance.stocks[(int)name]];
        } else
        {
            return 0;
        }
    }


	// Use this for initialization
	void Start () {
        controls = new PlayerControls(playerNum);
        controls.profile = controlLayout;
        prevPhase = Model.Instance.gamePhase;

        //get player data on the market
        //print(playerNum);
        //print(Model.Instance.traders.Count);
        playerData = Model.Instance.traders[playerNum];
        if(playerData == null)
        {
            print("ERROR Trader object doesn't exist!");
        }
	}
	
	// Update is called once per frame
	void Update () {
        controls.profile = controlLayout;
        if (controls.Update())
        {
            ControlsUpdate();
        }

        if(prevPhase != Model.Instance.gamePhase)
        {
            ResetBusinessDecision();
            prevPhase = Model.Instance.gamePhase;
        }
    }

    void ControlsUpdate()
    {
        if (controls.MenuButton)
        {
            //rotate between the layout profiles
            controlLayout = (PlayerControls.Profile)((int)(controlLayout + 1) % (int)PlayerControls.Profile.size);
        }

        prevCompany = selectedCompany;
        selectedCompany = CompanyName.none;

        switch (Model.Instance.gamePhase)
        {
            case GamePhases.Market:
                RealTimePhaseControls();
                break;

            case GamePhases.Business:
                BusinessDecisionPhaseControls();
                break;

            default:
                return;
        }
        
        
    }

    void ResetBusinessDecision()
    {
        selectedDecision = CompanyDecisions.none;
    }

    void BusinessDecisionPhaseControls()
    {
        if(controls.ButtonX)
        {
            selectedDecision = CompanyDecisions.X;
        }
        if (controls.ButtonY)
        {
            selectedDecision = CompanyDecisions.Y;
        }
        if (controls.ButtonA)
        {
            selectedDecision = CompanyDecisions.A;
        }
        if (controls.ButtonB)
        {
            selectedDecision = CompanyDecisions.B;
        }
    }

    void RealTimePhaseControls()
    {
        if (controls.Selection1)
        {
            selectedCompany = CompanyName.A;
        }
        if (controls.Selection2)
        {
            selectedCompany = CompanyName.B;
        }
        if (controls.Selection3)
        {
            selectedCompany = CompanyName.C;
        }

        if (selectedCompany != CompanyName.none)
        {
            if (controls.BuyPressed)
            {
                BuyShares(selectedCompany);
            }
            if (controls.SellPressed)
            {
                SellShares(selectedCompany);
            }

            //hold button down logic
            if (controls.Buy || controls.Sell)
            {
                currDelayTime += Time.deltaTime;
            }

            if (currDelayTime >= delayActionTime)
            {
                currHoldTime += Time.deltaTime;
                if (currHoldTime >= 1f / holdActionRate)
                {
                    currHoldTime = 0f;
                    if (controls.Buy)
                    {
                        BuyShares(selectedCompany);
                    }
                    if (controls.Sell)
                    {
                        SellShares(selectedCompany);
                    }
                }
            }
        }

        if (currDelayTime != 0 && !(controls.Buy || controls.Sell) ||
                           prevCompany != selectedCompany)
        {
            currDelayTime = 0f;
            currHoldTime = 0f;
        }
    }

    void BuyShares(CompanyName company)
    {
        Model.Instance.Buy(playerData, Model.Instance.stocks[(int)company], sharesPerAction);
    }

    void SellShares(CompanyName company)
    {
        Model.Instance.Sell(playerData, Model.Instance.stocks[(int)company], sharesPerAction);
    }
}
