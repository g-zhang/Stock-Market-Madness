using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MascotController : MonoBehaviour {

	public enum mascotState {intro, idle, exit, size};

	public mascotState state;

	[Header("Body Parts")]
	public Image Head;
	public Image Body;
	public Image Arm;

	[Header("Init Values")]
	public float XPos;
	public bool facingLeft;

	[Header("Movement Speeds")]
	public float introExitMoveSpeed;

	Vector3 startPos;
	Vector3 endPos;
	// Use this for initialization
	void Start () {
		startPos = new Vector3 (XPos, -400f, 0f);
		endPos = new Vector3 (XPos, -150f, 0f);
		transform.localPosition = startPos;
		state = mascotState.intro;
	}
	
	// Update is called once per frame
	void Update () {
		// lol toggles between facing left or right
		if ((facingLeft && transform.localScale.x < 0) || (!facingLeft && transform.localScale.x > 0)) {
			transform.localScale = new Vector3 (transform.localScale.x * -1f, transform.localScale.y, transform.localScale.z);
		}

		if (state == mascotState.intro) {
			startPos = new Vector3 (XPos, -400f, 0f);
			endPos = new Vector3 (XPos, -150f, 0f);
			popIn ();
		} else if (state == mascotState.idle) {

		} else if (state == mascotState.exit) {
			startPos = new Vector3 (XPos, -150f, 0f);
			endPos = new Vector3 (XPos, -400f, 0f);
		}
	}

	void popIn() {
		print ("IN POPIN");

		float totalU = endPos.y - startPos.y;
		float currU = (transform.localPosition.y - startPos.y) / totalU;

		transform.localPosition = Vector3.Lerp (transform.localPosition, endPos, Time.deltaTime * introExitMoveSpeed);
	}
}
