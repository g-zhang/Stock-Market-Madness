using UnityEngine;
using System.Collections.Generic;

public class Controller : MonoBehaviour
{
	#region Unity Methods
    void Awake()
    {
        for(int i = 0;  i < 4; i++)
        {
            Model.Instance.AddTrader();
        }
    }

	void Update()
	{
		Model.Instance.Tick();
		return;
	}
	#endregion
}