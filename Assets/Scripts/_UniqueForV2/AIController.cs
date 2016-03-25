using UnityEngine;
using System.Collections;

public class AIController : MonoBehaviour {

	public enum AIState {decision, getStock, buyStock, sellStock, size};
	public enum AIBehavior {lowestBuyer = 0, size};
	public enum AIBuyType {smallBuyer =0, bigBuyer, size};
	NavMeshAgent agn;

	[Header("OBJECTS TO GO TO")]
	public GameObject[] CompanyBooth = new GameObject[(int)CompanyName.size];
	public GameObject buyBooth;
	public GameObject sellBooth;

	[Header("AI BEHAVIOR")]
	public AIState state;
	public AIBehavior behavior;
	public AIBuyType buyType;
	public int roundsDecision; // How long must a stock be optimal before picking it up
	public CompanyName currConsiderCompany;
	public int howLongCompanyConsidered;

	[Header("INVENTORY")]
	public int numStockOnHand;
	public GameObject stockStack;

	// Use this for initialization
	void Start () {
		state = AIState.decision;

		howLongCompanyConsidered = 0;

		agn = GetComponent<NavMeshAgent> ();
		numStockOnHand = 0;
	}
	
	// Update is called once per frame
	void Update () {
		if (state == AIState.decision) {
			CompanyName newConsiderCompany = CompanyName.none;

			if (behavior == AIBehavior.lowestBuyer) {
				newConsiderCompany = findLowest ();
				agn.SetDestination(CompanyBooth [(int)newConsiderCompany].transform.position);
			}
			print (reachedDestination ());

			// For changing states
			decisionToBuy(newConsiderCompany);

			currConsiderCompany = newConsiderCompany;
		} 
		else if (state == AIState.buyStock) {
			agn.SetDestination (buyBooth.transform.position);

			if (reachedDestination ()) {
				print ("I REACHED THE BUY BOOTH");
				state = AIState.decision;
			}
		}
	}

	void decisionToBuy(CompanyName newCompany) {
		if (reachedDestination ()) {
			if (currConsiderCompany == newCompany) {
				howLongCompanyConsidered++;
			} else {
				howLongCompanyConsidered = 0;
			}

			if (howLongCompanyConsidered >= roundsDecision) {
				state = AIState.buyStock;
				howLongCompanyConsidered = 0;
			}
		}
	}

	bool reachedDestination() {
		// http://answers.unity3d.com/questions/324589/how-can-i-tell-when-a-navmesh-has-reached-its-dest.html
		float dist= agn.remainingDistance;

		print (dist);

		// Arrived
		if (dist != Mathf.Infinity && agn.pathStatus == NavMeshPathStatus.PathComplete && dist <= 1f) {
			return true;
		}
		return false;
	}

	CompanyName findLowest() {
		float lowestPrice = Model.Instance.stocks [(int)CompanyName.A].Price;
		CompanyName lowestPriceCom = CompanyName.A;
		for (CompanyName i = CompanyName.B; i != CompanyName.size; i++) {
			if (lowestPrice > Model.Instance.stocks [(int)i].Price) {
				lowestPrice = Model.Instance.stocks [(int)i].Price;
				lowestPriceCom = i;
			}
		}
		return lowestPriceCom;
	}
}
