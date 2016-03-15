using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum CompanyName { A, B, C, size, none }

public class CompanyManager : MonoBehaviour {

    public static CompanyManager S;

    public GameObject[] stockObjects = new GameObject[(int)CompanyName.size];
    StockManager[] companyStocks = new StockManager[(int)CompanyName.size];

    [Header("Status")]
    public float[] currentPrices = new float[(int)CompanyName.size];

    void Awake()
    {
        S = this;

        for (CompanyName cname = CompanyName.A; cname < CompanyName.size; cname++)
        {
            StockManager cstock = stockObjects[(int)cname].GetComponentInChildren<StockManager>();
            if (cstock != null)
            {
                companyStocks[(int)cname] = cstock;
            }
            else
            {
                Debug.LogError("All stock objects must have a StockManager component script!");
                Destroy(this);
            }
        }
    }

    // Use this for initialization
    void Start () {
  
	}

    public StockManager GetStocks(CompanyName name)
    {
        if(name < CompanyName.size && name >= CompanyName.A)
        {
            return companyStocks[(int)name];
        }
        return null;
    }
	
	// Update is called once per frame
	void Update () {
        //for (CompanyName cname = CompanyName.A; cname < CompanyName.size; cname++)
        //{
        //    currentPrices[(int)cname] = GetStocks(cname).Price;
        //}
    }
}
