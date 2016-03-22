using UnityEngine;
using System.Collections.Generic;

public class Controller : MonoBehaviour
{
    [Header("Game Status")]
    public GamePhases currentPhase;

    [Header("Config")]
    public GameObject StockGraph;
    public GameObject BusinessPhasePanel;
    public GameObject[] playersobj = new GameObject[4];
    private Player[] players = new Player[4];

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
        // for debugging purposes, making the game status viewable in the inspector
        currentPhase = Model.Instance.gamePhase;

        //state machine
        switch(Model.Instance.gamePhase)
        {
            case GamePhases.Business:
                if(allPlayersMadeBusinessChoice)
                {
                    print("Advancing to next day!");
                    Model.Instance.gamePhase = GamePhases.Market;
                }
                break;

            default:
                break;
        }
    }
}