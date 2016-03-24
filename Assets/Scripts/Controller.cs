using UnityEngine;
using System.Collections.Generic;

public class Controller : MonoBehaviour
{
    [Header("Game Status")]
    public GamePhases currentPhase;
    public GamePhases prevPhase;

    [Header("Config")]
    public GameObject StockGraph;
    public GameObject BusinessPhasePanel;
    public GameObject[] playersobj = new GameObject[4];

    private Player[] players = new Player[4];
    private CompanyName currCompanyPhase = CompanyName.none;

    #region Properties
    bool allPlayersMadeBusinessChoice
    {
        get
        {
            for (int i = 0; i < players.Length; i++)
            {
                if (players[i].selectedDecision >= CompanyDecisions.size)
                {
                    return false;
                }
            }
            return true;
        }
    }
    #endregion

    #region Unity Methods
    void Awake()
    {
        for(int i = 0;  i < 4; i++)
        {
            Model.Instance.AddTrader();
        }

        for (int i = 0; i < playersobj.Length; i++)
        {
            players[i] = playersobj[i].GetComponent<Player>();
            if (players[i] == null)
            {
                Debug.LogError("All player objects must have a player script component!");
                Destroy(this);
            }
        }
    }

	void Update()
	{
		Model.Instance.Tick();

        ManageGameState();
        ManageUnityObjects();
		return;
	}
	#endregion

    void ManageUnityObjects()
    {
        switch(Model.Instance.gamePhase)
        {
            case GamePhases.Business:
                StockGraph.SetActive(true);
                BusinessPhasePanel.SetActive(true);
                break;

            case GamePhases.Market:
                StockGraph.SetActive(true);
                BusinessPhasePanel.SetActive(false);
                break;

            default:
                StockGraph.SetActive(true);
                BusinessPhasePanel.SetActive(false);
                break;
        }
    }

    void ManageGameState()
    {
        prevPhase = currentPhase;
        currentPhase = Model.Instance.gamePhase;

        //state machine
        switch(Model.Instance.gamePhase)
        {
            case GamePhases.Business:
                if (prevPhase != GamePhases.Business)
                {
                    currCompanyPhase = CompanyName.A;
                    BusinessPhasePanel.GetComponent<UpdateDecisionPanel>().UpdateDecisionChoices();
                }

                BusinessPhasePanel.GetComponent<UpdateDecisionPanel>().Title.text =
                    "Shareholders, Decide on the Future of Company " + currCompanyPhase;

                if (allPlayersMadeBusinessChoice)
                {
                    //record selections
                    RecordPlayerSelections();

                    //advance to next company
                    currCompanyPhase++;
                    ClearPlayerBusinessChoices();
                    BusinessPhasePanel.GetComponent<UpdateDecisionPanel>().UpdateDecisionChoices();
                }
                
                if (currCompanyPhase >= CompanyName.size)
                {
                    print("Advancing to next day!");
                    currCompanyPhase = CompanyName.none;
                    Model.Instance.gamePhase = GamePhases.Market;
                }
                break;

            default:
                break;
        }
    }

    void ClearPlayerBusinessChoices()
    {
        for (int i = 0; i < players.Length; i++)
        {
            players[i].ResetBusinessDecision();
        }
    }

    void RecordPlayerSelections()
    {
        for (int i = 0; i < players.Length; i++)
        {

            if(EventLibrary.getEventsCurrent()[(int)players[i].selectedDecision] == null)
            {
                print("!1");
            }

            if(Model.Instance.stocks[(int)currCompanyPhase] == null)
            {
                print("!2");
            }

            if (players[i].getPlayerStockData(currCompanyPhase) == 0)
            {
                print("!3");
            }

            Model.Instance.MarketEvent(
                EventLibrary.getEventsCurrent()[(int)players[i].selectedDecision],
                Model.Instance.stocks[(int)currCompanyPhase],
                players[i].getPlayerStockData(currCompanyPhase)
                );
        }
    }
}