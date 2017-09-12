using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Prime31;

public class PlayerController : MonoBehaviour
{
    // Where to go when the player has walked off the left side of the map
    [SerializeField]
    private float _warpLocationRightX = 10;
    // Where to go when the player has walked off the right side of the map
    [SerializeField]
    private float _warpLocationLeftX = -10;
    // How quickly the player moves while walking
    [SerializeField]
    private float _movementSpeed = 5;
    // How quickly the player rotates while in shooting mode
    [SerializeField]
    private float _rotationSpeed = 1;
    [SerializeField]
    private GameObject _bullet;
    // Used when figuring out hip fire direction, also changed with movement stick while in shoot mode
    private Vector3 _playerDirection;
    // True while player is holding left trigger, disables movement
    private bool _isShooting = false;
    // True for a time after pressing A, disables shooting
    private bool _isDashing = false;
    // Performs movement and collision so we don't have to think about it
    private CharacterController2D _controller;
    private bool _triggerHasBeenReleased = true;
    // The amount of time a dash takes
    private float _dashTime = 0.25f;
    // How long the player has been dashing for after activating a dash
    private float _dashTimer = 0f;
    // The amount of time an attack takes and the collider is active
    private float _attackTime = 0.1f;
    // How long the player has been attacking for after activating attack
    private float _attackTimer = 0f;
    [SerializeField]
    private float _dashSpeed = 12.5f;
    [SerializeField]
    private BoxCollider2D _attackCollider;

    private enum playerState
    {
        TAKINGDAMAGE,
        DASHING,
        ATTACKING,
        LANDING,
        FREE,
        DEAD,
        WINNING,
        SHOOTING
    }
    // The player can only be doing one thing at once
    private playerState _currentState = playerState.FREE;


    // Use this for initialization
    void Start()
    {
        _controller = gameObject.GetComponent<CharacterController2D>();
        _attackCollider = GameObject.Find("PlayerAttackCollider").GetComponent<BoxCollider2D>();
        _attackCollider.enabled = false;
        _playerDirection = new Vector3(1,0,0);
    }

    // Update is called once per frame
    void Update()
    {
        float shoot = Input.GetAxis("Fire1");
        float aim = Input.GetAxis("Fire2");
        bool dash = Input.GetButton("Dash");
        bool melee = Input.GetButton("Melee");

        if (_currentState == playerState.FREE)
        {
            if (dash)
                _currentState = playerState.DASHING;
            else if (melee)
                _currentState = playerState.ATTACKING;
            else if (aim > 0)
                _currentState = playerState.SHOOTING;
        }
        else if (aim == 0 && _currentState == playerState.SHOOTING)
            _currentState = playerState.FREE;

        if (_currentState == playerState.ATTACKING)
        {
            if (_attackTimer < _attackTime)
            {
                _attackTimer += Time.deltaTime;
                _attackCollider.enabled = true;
                //Debug.Log("SMACK");
            }
            else
            {
                _attackCollider.enabled = false;
                _currentState = playerState.FREE;
                _attackTimer = 0f;
            }
        }
        else if (_currentState == playerState.DASHING)
        {
            if (_dashTimer < _dashTime)
            {
                _dashTimer += Time.deltaTime;
                _controller.move(_playerDirection * Time.deltaTime * _dashSpeed);

                // No more actions possible while dashing
                return;
            }
            else
            {
                _currentState = playerState.FREE;
                _dashTimer = 0f;
            }
        }

        Vector3 playerInputDirection = Vector3.zero;

        playerInputDirection.x += Input.GetAxis("Horizontal");
        playerInputDirection.y += Input.GetAxis("Vertical");

        if (shoot > 0 && _triggerHasBeenReleased)
        {
            _triggerHasBeenReleased = false;
            GameObject bullet = Instantiate(_bullet, gameObject.transform.position, Quaternion.identity);
            bullet.GetComponent<BulletController>()._direction = Vector3.Normalize(_playerDirection);
        }
        else if (shoot == 0)
            _triggerHasBeenReleased = true;

        // If the player is changing directions, remember which way they ended up facing
        if (playerInputDirection.x != 0 || playerInputDirection.y != 0)
        {
            _playerDirection = playerInputDirection;
            //transform.RotateAround(new Vector3(1,0,0), new Vector3(0,0,1), Vector3.Angle(new Vector3(1, 0, 0), _playerDirection));
        }

        // If shooting, the player doesn't move
        if (_currentState != playerState.SHOOTING)
            _controller.move(playerInputDirection * Time.deltaTime * _movementSpeed);

        DrawLine(gameObject.transform.position, gameObject.transform.position + _playerDirection, Color.red, 0.05f);
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
    }

    void DrawLine(Vector3 start, Vector3 end, Color color, float duration = 0.2f)
    {
        GameObject myLine = new GameObject();
        myLine.transform.position = start;
        myLine.AddComponent<LineRenderer>();
        LineRenderer lr = myLine.GetComponent<LineRenderer>();
        lr.material = new Material(Shader.Find("Particles/Alpha Blended Premultiply"));
        lr.SetColors(color, color);
        lr.SetWidth(0.1f, 0.1f);
        lr.SetPosition(0, start);
        lr.SetPosition(1, end);
        GameObject.Destroy(myLine, duration);
    }
}