using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Prime31;

public class SplitEnemyController : MonoBehaviour {

    [SerializeField]
    private int _health = 5;
    [SerializeField]
    private float _speed = 0.5f;
    [SerializeField]
    private float _warpLocationRightX = 10;
    // Where to go when the player has walked off the right side of the map
    [SerializeField]
    private float _warpLocationLeftX = -10;
    [SerializeField]
    private float _warpLocationUpY = 7f;
    [SerializeField]
    private float _warpLocationDownY = -7f;
    [SerializeField]
    private float _viewRange = 5f;
    private float _damageTime = 0.5f;
    private float _damageTimer = 0;
    private GameObject _player;
    // Performs movement and collision so we don't have to think about it
    private CharacterController2D _controller;
    private PlayerController _playerController;
    private bool _takingDamage = false;
    private SpriteRenderer _sprite;
    [SerializeField]
    private GameObject _smallerGuys;
    [SerializeField]
    private GameObject _ammoDrop;
    private bool _patrolOutgoing = true;
    private float _patrolTime = 2f;
    private float _patrolWaitTime = 2f;
    private float _patrolTimer = 0;

    // Use this for initialization
    void Start () {
        _player = GameObject.Find("Player");
        _controller = gameObject.GetComponent<CharacterController2D>();
        _playerController = _player.GetComponent<PlayerController>();
        _sprite = gameObject.GetComponent<SpriteRenderer>();
    }
	
	// Update is called once per frame
	void Update () {
        if (_takingDamage)
        {
            _damageTimer += Time.deltaTime;
            _sprite.enabled = !_sprite.enabled;
            if (_damageTimer >= _damageTime)
            {
                _takingDamage = false;
                _damageTimer = 0;
                _sprite.enabled = true;
            }
        }
        else
        {
            Vector3 distanceToPlayer = _player.transform.position - gameObject.transform.position;
            if (distanceToPlayer.magnitude < _viewRange)
            {
                Vector3 movementDirection = Vector3.Normalize(distanceToPlayer);
                //gameObject.transform.position += movementDirection * Time.deltaTime * _speed;
                _controller.move(movementDirection * Time.deltaTime * _speed);
            }
            else
            {
                Vector3 patrolDir = new Vector3(1, 0, 0);
                _patrolTimer += Time.deltaTime;
                if (_patrolTimer < _patrolTime)
                {
                    if (_patrolOutgoing)
                        _controller.move(patrolDir * Time.deltaTime * _speed / 2f);
                    else
                        _controller.move(-1 * patrolDir * Time.deltaTime * _speed / 2f);
                }
                else if (_patrolTimer >= _patrolTime + _patrolWaitTime)
                {
                    _patrolOutgoing = !_patrolOutgoing;
                    _patrolTimer = 0;
                }
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
    if (other.tag == "PlayerBullet")
        {
            Destroy(other.gameObject);
            _health -= _playerController.bulletDamage;
            _takingDamage = true;
        }
        else if (other.tag == "PlayerSword")
        {
            _health -= _playerController.damage;
            
            _takingDamage = true;
        }

        if (_takingDamage)
        {
            if (_smallerGuys != null)
            {
                if (_health > 1)
                    gameObject.transform.localScale = new Vector3(gameObject.transform.localScale.x*4/5, gameObject.transform.localScale.y * 4 / 5, 1);
                Vector3 position = new Vector3(Random.Range(-10.0f, 10.0f), Random.Range(-10.0f, 10.0f), 0);
                Debug.Log(position);
                position = Vector3.Normalize(position);
                position *= 2;
                Instantiate(_smallerGuys, gameObject.transform.position + position, Quaternion.identity);
            }
        }

        if (_health <= 0)
        {
            Vector3 t = gameObject.transform.position;
            Destroy(gameObject);
            // ammo drop is 25%
            int dropChance = Random.Range(1, 5); // random integer from 1 to 4
            if (dropChance == 1)
            {
                Instantiate(_ammoDrop, t, Quaternion.identity);
            }
        }
    }
}
