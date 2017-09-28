using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class levelWrapperScript : MonoBehaviour {

    [SerializeField]
    private bool _moveLeftRight = false;
    [SerializeField]
    private float _offsetDistance;
    public bool _on = true;
    [SerializeField]
    private GameObject _barrier;
    private GameObject _player;
    private const float SCREENWIDTH = 32f;
    private const float SCREENHEIGHT = 18f;
    private SpriteRenderer _renderer;

    // Use this for initialization
    void Start () {
        _barrier.SetActive(!_on);
        _player = GameObject.Find("Player");
        _renderer = gameObject.GetComponent<SpriteRenderer>();
        if (_on)
            _renderer.color = Color.green;
        else
            _renderer.color = Color.red;
    }
	
	// Update is called once per frame
	void Update () {
        // Turn of walls when player is outside of camera viewport
        Vector3 playerPos = _player.gameObject.transform.position;
        if (playerPos.x < -1 * SCREENWIDTH / 2 || playerPos.x > SCREENWIDTH / 2 || playerPos.y < -1 * SCREENHEIGHT / 2 || playerPos.y > SCREENHEIGHT / 2)
             _barrier.SetActive(false);
        else
            _barrier.SetActive(!_on);
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
        _barrier.SetActive(!_on);
        if (_on)
            _renderer.color = Color.green;
        else
            _renderer.color = Color.red;
    }
}
