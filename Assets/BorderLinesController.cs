using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BorderLinesController : MonoBehaviour {

    private List<levelWrapperScript> _borderLines;
    private List<bool> _states;
    private bool _mustReleaseLBumper = false;
    private bool _mustReleaseRBumper = false;

    // Use this for initialization
    void Start () {
        _borderLines = new List<global::levelWrapperScript>();
        _states = new List<bool>();
        _borderLines.Add(GameObject.Find("levelWrapperL").GetComponent<levelWrapperScript>());
        _states.Add(_borderLines[0]._on);
        _borderLines.Add(GameObject.Find("levelWrapperU").GetComponent<levelWrapperScript>());
        _states.Add(_borderLines[1]._on);
        _borderLines.Add(GameObject.Find("levelWrapperR").GetComponent<levelWrapperScript>());
        _states.Add(_borderLines[2]._on);
        _borderLines.Add(GameObject.Find("levelWrapperD").GetComponent<levelWrapperScript>());
        _states.Add(_borderLines[3]._on);
        //Debug.Log(_states[0] + ", " + _states[1] + ", " + _states[2] + ", " + _states[3]);
    }
	
	// Update is called once per frame
	void Update () {
        bool clockwise = Input.GetButton("RotateBorderLinesClock");
        bool counterclockwise = Input.GetButton("RotateBorderLinesCoun");

        if (!clockwise)
            _mustReleaseRBumper = false;
        if (!counterclockwise)
            _mustReleaseLBumper = false;

        if ((clockwise && !_mustReleaseRBumper) || (counterclockwise && !_mustReleaseLBumper))
        {
            bool[] newStates = new bool[4];
            if (clockwise)
            {
                _mustReleaseRBumper = true;
                for (int i = 0; i < 4; ++i)
                    newStates[i] = _states[(i + 1) % 4];
                // Debug.Log("clock");
            }
            else if (counterclockwise)
            {
                _mustReleaseLBumper = true;
                newStates[0] = _states[3];
                for (int i = 1; i < 4; ++i)
                    newStates[i] = _states[i - 1];
                // Debug.Log("coun");
            }

            for (int i = 0; i < 4; ++i)
            {
                _states[i] = newStates[i];
                _borderLines[i].setOnOrOff(_states[i]);
            }
            //Debug.Log(_states[0] + ", " + _states[1] + ", " + _states[2] + ", " + _states[3]);
        }
    }
}
