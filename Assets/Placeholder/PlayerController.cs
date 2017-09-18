﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Prime31;

public class PlayerController : MonoBehaviour
{
    // Use so the player can hoild down dash button.
    [SerializeField]
    private bool canDash = true;
    // Used to create the dash bar
    [SerializeField]
    private float dashTime = 0;
    // Where to go when the player has walked off the left side of the map
    [SerializeField]
    private float _warpLocationRightX = 10;
    // Where to go when the player has walked off the right side of the map
    [SerializeField]
    private float _warpLocationLeftX = -10;
    [SerializeField]
    private float _warpLocationUpY = 7f;
    [SerializeField]
    private float _warpLocationDownY = -7f;
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
    private BoxCollider2D _attackCollider;
    private int _playerHealth = 100;
    [SerializeField]
    private int _bulletDamage = 10;

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
            dashTime = 0;
            canDash = false;
            if ((_dashTimer < _dashTime) && canDash)
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
        Vector3.Normalize(_playerDirection);

        if (shoot > 0 && _triggerHasBeenReleased)
        {
            _triggerHasBeenReleased = false;
            GameObject bullet = Instantiate(_bullet, gameObject.transform.position + _playerDirection, Quaternion.identity);
            bullet.GetComponent<BulletController>()._direction = _playerDirection;
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
        {
            Vector3 movementAxis = SnapToNearestMovementAxis(playerInputDirection);
            _controller.move(SnapToNearestMovementAxis(movementAxis) * Time.deltaTime * _movementSpeed);
            _attackCollider.gameObject.transform.position = gameObject.transform.position + movementAxis;
            DrawLine(gameObject.transform.position, gameObject.transform.position + movementAxis, Color.red, 0.05f);
        }
        else
            DrawLine(gameObject.transform.position, gameObject.transform.position + playerInputDirection, Color.red, 0.05f);

        if(dashTime < 2f)
        {
            canDash = false;
            dashTime += Time.deltaTime;
            DrawDashBar(dashTime);
        }
        else if(dashTime > 2f)
        {
            canDash = true;
            DrawDashBar(dashTime);
        }
        DrawHealthBar();
        
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
            _playerHealth -= _bulletDamage;
            if (_playerHealth == 0)
                _currentState = playerState.DEAD;
        }
    }

    // The player is allowed to move in the following 8 directions:
    //  (1,0,0)
    //  (0.7,0.7,0)
    //  (0,1,0)
    //  (-0.7,0.7,0)
    //  (-1,0,0)
    //  (-0.7,-0.7,0)
    //  (0,-1,0)
    //  (0.7,-0.7,0)
    //      NOTE: assumed input vector is normalized
    Vector3 SnapToNearestMovementAxis(Vector3 inputDirection)
    {
        Vector3 returnDirection = new Vector3(0,0,0);

        // Check to see if x is closer to 0, 1, or 0.7
        if (Mathf.Abs(1f - Mathf.Abs(inputDirection.x)) < Mathf.Abs(0.7f - Mathf.Abs(inputDirection.x)))
            returnDirection.x = 1;
        else if ((Mathf.Abs(inputDirection.x)) < Mathf.Abs(0.7f - Mathf.Abs(inputDirection.x)))
            returnDirection.x = 0;
        else
            returnDirection.x = 0.7f;

        // Ensure the sign matches
        if (inputDirection.x < 0)
            returnDirection.x *= -1;

        // Check to see if x is closer to 0, 1, or 0.7
        if (Mathf.Abs(1f - Mathf.Abs(inputDirection.y)) < Mathf.Abs(0.7f - Mathf.Abs(inputDirection.y)))
            returnDirection.y = 1;
        else if ((Mathf.Abs(inputDirection.y)) < Mathf.Abs(0.7f - Mathf.Abs(inputDirection.y)))
            returnDirection.y = 0;
        else
            returnDirection.y = 0.7f;

        // Ensure the sign matches
        if (inputDirection.y < 0)
            returnDirection.y *= -1;

        return returnDirection;
    }

    void DrawHealthBar()
    {
        Vector3 healthBarStart = new Vector3(-8,4,0);
        int healthBarLength = 5;
        Vector3 healthBarEnd = new Vector3(healthBarStart.x+healthBarLength * _playerHealth / 100f, healthBarStart.y, healthBarStart.z);
        DrawLine(healthBarStart, healthBarEnd , Color.red);
    }

    void DrawDashBar(float length)
    {
        Vector3 DashBarStart = new Vector3(-8, 4.3f, 0);
        float DashBarLength = length;
        Vector3 DashBarEnd = new Vector3(DashBarStart.x + (length) , DashBarStart.y, DashBarStart.z);
        DrawLine(DashBarStart, DashBarEnd, Color.cyan);
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