using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public enum CompanyDecisions { X = 0, Y , A , B, size, none};

public class UpdateDecisionPanel : MonoBehaviour {

    [Header("Player Objects")]
    public GameObject[] playersobj = new GameObject[4];
    private Player[] players = new Player[4];

    [Header("Player Indicators (Dynamically Found on Start)")]
    public Image[] indicators = new Image[4];

    [Header("Text Objects (Dynamically Found on Start)")]
    public Text Title;
    public Text[] ChoiceTextBoxes = new Text[(int)CompanyDecisions.size];

    // Use this for initialization
    void Start () {
        for(CompanyDecisions i = CompanyDecisions.X; i < CompanyDecisions.size; i++)
        {
            ChoiceTextBoxes[(int)i] = transform.FindChild("Choice_" + i + "/Choice_Text_" + i).GetComponent<Text>();
        }
        Title = transform.FindChild("TitleText").GetComponent<Text>();

        for(int i = 0; i < playersobj.Length; i++)
        {
            players[i] = playersobj[i].GetComponent<Player>();
            if(players[i] == null)
            {
                Debug.LogError("All player objects must have a player script component!");
                Destroy(this);
            }

            indicators[i] = transform.FindChild("PlayerIndicators/PlayerIndicator (" + i + ")").GetComponent<Image>();
        }
    }
	
	// Update is called once per frame
	void Update () {
        for(int i = 0; i < players.Length; i++)
        {
            if(players[i].selectedDecision < CompanyDecisions.size)
            {
                indicators[i].color = Color.green;
            } else
            {
                indicators[i].color = Color.white;
            }
        }
	}

    void UpdateDecisionChoices(StockEvent[] events)
    {
        for (CompanyDecisions i = CompanyDecisions.X; i < CompanyDecisions.size; i++)
        {
            //StockEvent needs public members or properties for this
            //ChoiceTextBoxes[(int)i].text = events[(int)i].eventName;
        }
    }
}
