using UnityEngine;
using System.Collections;
using InControl;

public class Player : MonoBehaviour {

    public enum Phase { BuySell = 0, Business, size };

    public int playerNum;
    private PlayerControls controls;

    [Header("Status")]
    public Phase currentGamePhase = Phase.BuySell;
    private Phase prevPhase;
    public float currentMoney;
    public int[] CompanyShares = new int[(int)CompanyName.size];
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


	// Use this for initialization
	void Start () {
        controls = new PlayerControls(playerNum);
        controls.profile = controlLayout;
        prevPhase = currentGamePhase;
	}
	
	// Update is called once per frame
	void Update () {
        controls.profile = controlLayout;
        if (controls.Update())
        {
            ControlsUpdate();
        }

        if(prevPhase != currentGamePhase)
        {
            ResetBusinessDecision();
            prevPhase = currentGamePhase;
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

        switch(currentGamePhase)
        {
            case Phase.BuySell:
                RealTimePhaseControls();
                break;

            case Phase.Business:
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
        StockManager stocks = CompanyManager.S.GetStocks(company);
        float moneyRequired = stocks.Price * sharesPerAction;
        if (moneyRequired <= currentMoney && stocks.RecordBuy(sharesPerAction))
        {
            print("Bought " + company);
            CompanyShares[(int)company] += sharesPerAction;
            currentMoney -= moneyRequired;
        }
    }

    void SellShares(CompanyName company)
    {
        StockManager stocks = CompanyManager.S.GetStocks(company);
        float moneyGained = stocks.Price * sharesPerAction;
        if(CompanyShares[(int)company] >= sharesPerAction)
        {
            print("Sell " + company);
            stocks.RecordSell(sharesPerAction);
            CompanyShares[(int)company] -= sharesPerAction;
            currentMoney += moneyGained;
        }
    }
}
