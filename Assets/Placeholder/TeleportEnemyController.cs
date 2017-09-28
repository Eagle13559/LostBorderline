using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Prime31;

public class TeleportEnemyController : MonoBehaviour
{

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
    private float _safeDistance = 2.2f;
    private float _damageTime = 0.5f;
    private float _damageTimer = 0;
    private GameObject _player;
    // Performs movement and collision so we don't have to think about it
    private CharacterController2D _controller;
    private PlayerController _playerController;
    private bool _takingDamage = false;
    private SpriteRenderer _sprite;
    [SerializeField]
    private GameObject _ammoDrop;
    private float _shootTime = 2f;
    private float _shootTimer = 0;
    [SerializeField]
    private GameObject _bullet;
    private Vector3[] teleportLocations;
    private int currentLocation;
    private bool _panicked = false;
    private float _teleportWindow = 0;
    private float _teleportTime = 0.2f;

    // Use this for initialization
    void Start()
    {
        _player = GameObject.Find("Player");
        _controller = gameObject.GetComponent<CharacterController2D>();
        _playerController = _player.GetComponent<PlayerController>();
        _sprite = gameObject.GetComponent<SpriteRenderer>();
        currentLocation = 0;
        teleportLocations = new Vector3[7];
        teleportLocations[0] = new Vector3(-2.7f, -0.44f, 0);
        teleportLocations[1] = new Vector3(1.49f, -.056f, 0);
        teleportLocations[2] = new Vector3(-10.7f, 5.0f, 0);
        teleportLocations[3] = new Vector3(-0.7f, 4.0f, 0);
        teleportLocations[4] = new Vector3(13.3f, 3.3f, 0);
        teleportLocations[5] = new Vector3(11.6f, -1.75f, 0);
        teleportLocations[6] = new Vector3(-14.7f, -3.0f, 0);
    }

    // Update is called once per frame
    void Update()
    {
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
            Vector3 movementDirection = Vector3.Normalize(distanceToPlayer);
            if (distanceToPlayer.magnitude < _safeDistance)
            {
                _panicked = true;
            }
            else
            {
                _shootTimer += Time.deltaTime;
                if (_shootTimer >= _shootTime)
                {
                    GameObject bullet = Instantiate(_bullet, gameObject.transform.position + movementDirection, Quaternion.identity);
                    bullet.GetComponent<BulletController>()._direction = movementDirection;
                    _shootTimer = 0;
                }
            }
        }
        if (_panicked)
        {
            _teleportWindow += Time.deltaTime;
            if (_teleportWindow > _teleportTime)
                Teleport();
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

        if(other.tag=="Wall")
        {
            _takingDamage = true;
        }
        else if (other.tag == "PlayerSword")
        {
            _health -= _playerController.damage;
            _takingDamage = true;
        }
        else if(other.tag == "Wall")
        {
            Teleport(); 
        }

        if (_takingDamage)
        {
            Teleport();
        }

        if (_health <= 0)
        {
            Vector3 t = gameObject.transform.position;
            Destroy(gameObject);
            // ammo drop is 10%
            int dropChance = Random.Range(1, 11); // random integer from 1 to 10
            if (dropChance == 1)
            {
                Instantiate(_ammoDrop, t, Quaternion.identity);
            }
        }
    }

    void Teleport()
    {
        currentLocation++;
        int newLoc = Random.Range(0, 8);
        if (newLoc == currentLocation) newLoc++;
        transform.position = teleportLocations[newLoc];
        _panicked = false;
        _teleportWindow = 0f;
    }
}
