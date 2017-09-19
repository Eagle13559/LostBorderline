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
            Vector3 movementDirection = Vector3.Normalize(_player.transform.position - gameObject.transform.position);
            //gameObject.transform.position += movementDirection * Time.deltaTime * _speed;
            _controller.move(movementDirection * Time.deltaTime * _speed);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "LevelWrapperR")
        {
            gameObject.transform.position = new Vector3(_warpLocationLeftX, gameObject.transform.position.y);
        }
        else if (other.tag == "LevelWrapperL")
        {
            gameObject.transform.position = new Vector3(_warpLocationRightX, gameObject.transform.position.y);
        }
        else if (other.tag == "LevelWrapperD")
        {
            gameObject.transform.position = new Vector3(gameObject.transform.position.x, _warpLocationUpY);
        }
        else if (other.tag == "LevelWrapperU")
        {
            gameObject.transform.position = new Vector3(gameObject.transform.position.x, _warpLocationDownY);
        }
        else if (other.tag == "Bullet")
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

        if (_health <= 0)
        {
            if (_smallerGuys != null)
            {
                for (int i = 0; i < 8; ++i)
                {
                    Vector3 position = new Vector3(Random.Range(-10.0f, 10.0f), Random.Range(-10.0f, 10.0f), 0);
                    Debug.Log(position);
                    position = Vector3.Normalize(position);
                    position *= 2;
                    Instantiate(_smallerGuys, gameObject.transform.position + position, Quaternion.identity);
                }
            }
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
