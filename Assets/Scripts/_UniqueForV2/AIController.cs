using UnityEngine;
using System.Collections;

public class AIController : MonoBehaviour {

	public enum AIBehavior {lowestBuyer = 0, size};

	public AIBehavior Behavior;
	public GameObject[] CompanyBooth = new GameObject[(int)CompanyName.size];

	NavMeshAgent agn;

	// Use this for initialization
	void Start () {
		agn = GetComponent<NavMeshAgent> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (Behavior == AIBehavior.lowestBuyer) {
			agn.destination = CompanyBooth[(int) findLowest()].transform.position;
		}
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
