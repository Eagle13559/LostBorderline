using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BorderLinesController : MonoBehaviour {

    private List<levelWrapperScript> _borderLines;
    private List<bool> _states;

    // Use this for initialization
    void Start () {
        _borderLines.Add(GameObject.Find("levelWrapperL").GetComponent<levelWrapperScript>());
        _states.Add(_borderLines[0]._on);
        _borderLines.Add(GameObject.Find("levelWrapperU").GetComponent<levelWrapperScript>());
        _states.Add(_borderLines[1]._on);
        _borderLines.Add(GameObject.Find("levelWrapperR").GetComponent<levelWrapperScript>());
        _states.Add(_borderLines[2]._on);
        _borderLines.Add(GameObject.Find("levelWrapperD").GetComponent<levelWrapperScript>());
        _states.Add(_borderLines[3]._on);
    }
	
	// Update is called once per frame
	void Update () {
        bool clockwise = Input.GetButton("RotateBorderLinesClock");
        bool counterclockwise = Input.GetButton("RotateBorderLinesCoun");

        if (clockwise)
        {
            Debug.Log("clock");
        }
        else if (counterclockwise)
        {
            Debug.Log("coun");
        }
    }
}
