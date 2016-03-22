using UnityEngine;
using System.Collections.Generic;

public class Controller : MonoBehaviour
{
	#region Unity Methods
	void Update()
	{
		Model.Instance.Tick();
		return;
	}
	#endregion
}