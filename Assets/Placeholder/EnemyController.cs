using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour {

    [SerializeField]
    private int _health = 50;

    //private BoxCollider2D _attackCollider;
    // The amount of time an attack takes and the collider is active
    //private float _attackTime = 0.1f;
    // How long the player has been attacking for after activating attack
    //private float _attackTimer = 0f;

    // Player object to get player position
    private Transform _playerPos;

    private float _moveSpeed = 0.2f;

	// Use this for initialization
	void Start ()
    {
        //_attackCollider = GameObject.Find("Enemy1AttackCollider").GetComponent<BoxCollider2D>();
        //_attackCollider.enabled = false;
        _playerPos = GameObject.Find("Player").transform;
    }
	
	// Update is called once per frame
	void Update ()
    {
        transform.LookAt(_playerPos);
        if (Vector3.Distance(transform.position,_playerPos.position) >= 0.0f)
        {
            transform.position += transform.forward * _moveSpeed * Time.deltaTime;
        }
        Vector3 pos = transform.position;
        pos.z = 0.0f;
        transform.position = pos;
    }

    void MeleeAttack()
    {

    }
}
