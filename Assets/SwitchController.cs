using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchController : MonoBehaviour {
    public bool state = false;
    private SpriteRenderer _renderer;

	// Use this for initialization
	void Start () {
        _renderer = gameObject.GetComponent<SpriteRenderer>();

    }
	
	// Update is called once per frame
	void Update () {
        if (state)
            _renderer.color = Color.green;
        else
            _renderer.color = Color.red;
	}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "PlayerBullet" || collision.tag == "PlayerAttackCollider")
            state = !state;
    }
}
