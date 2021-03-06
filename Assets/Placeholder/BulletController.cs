﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour {

    public Vector3 _direction;
    [SerializeField]
    private float _speed = 1;
    // Where to go when the player has walked off the left side of the map
    [SerializeField]
    private float _warpLocationRightX = 10;
    // Where to go when the player has walked off the right side of the map
    [SerializeField]
    private float _warpLocationLeftX = -10;
    [SerializeField]
    private float _warpLocationUpY = 6;
    [SerializeField]
    private float _warpLocationDownY = -6;
    [SerializeField]
    private float _timer = 5f;
    private float _timeAlive = 0f;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        transform.Translate(_direction * Time.deltaTime * _speed);
        _timeAlive += Time.deltaTime;
        if (_timeAlive > _timer)
            Destroy(gameObject);
	}

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Wall")
        {
            Destroy(gameObject);
        }
    }
}
