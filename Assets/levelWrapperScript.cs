using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class levelWrapperScript : MonoBehaviour {

    [SerializeField]
    private bool _moveLeftRight = false;
    [SerializeField]
    private float _offsetDistance;
    public bool _on = true;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Vector3 otherPosition = collision.gameObject.transform.position;
        if (_moveLeftRight)
            collision.gameObject.transform.position = new Vector3(otherPosition.x + _offsetDistance, otherPosition.y, 1);
        else
            collision.gameObject.transform.position = new Vector3(otherPosition.x, otherPosition.y + _offsetDistance, 1);
    }
    public void setOnOrOff(bool newState_)
    {
        // TODO: turn on and off animation here
        _on = newState_;
    }
}
