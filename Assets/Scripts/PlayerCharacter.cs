using UnityEngine;
using System.Collections;

public class PlayerCharacter : MonoBehaviour {

    public int playerNum;
    private PlayerControls controls;
    private Rigidbody body;

    [Header("Config")]
    public GameObject playerPanel;
    public float movementSpeed = 1f;
    float lerpSpeed = 1f;

	// Use this for initialization
	void Start () {
        controls = new PlayerControls(playerNum);
        body = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
	    if(controls.Update())
        {
            MovementControls();
        }
	}

    void MovementControls()
    {
        Vector3 vel = Vector3.zero;
        Vector2 dir = Vector2.ClampMagnitude(controls.Input.LeftStick.Vector, 1f) * movementSpeed;

        vel = new Vector3(dir.x, 0f, dir.y);
        body.velocity = Vector3.Lerp(body.velocity, vel, lerpSpeed);

        Debug.DrawRay(body.transform.position, body.velocity * 5f, Color.blue);
    }
}
