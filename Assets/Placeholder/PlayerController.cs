using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Prime31;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    private const float SCREENWIDTH = 32f;
    private const float SCREENHEIGHT = 18f;
    [SerializeField]
    private GameObject _camera;
    public int damage = 1;
    // Use so the player can hoild down dash button.
    private bool canDash = true;
    // Used to create the dash bar
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
    private float _chargedAttackTime = 0.5f;
    // How long the player has been attacking for after activating attack
    private float _attackTimer = 0f;
    [SerializeField]
    private float _dashSpeed = 12.5f;
    [SerializeField]
    private BoxCollider2D _attackCollider;
    private int _playerHealth = 100;
    [SerializeField]
    private int _ammo = 100;
    public int bulletDamage = 1;
    private float _meleeCharge = 0;
    [SerializeField]
    private float _meleeChargeTime = 0.5f;
    [SerializeField]
    private float _meleeChargePauseTime = 0.15f;
    [SerializeField]
    private int _enemyDamage = 5;
    private float _damageTime = 0.5f;
    private float _damageTimer = 0;
    [SerializeField]
    private float _damageSpeed = 10f;
    private bool _isTakingDamage = false;
    private SpriteRenderer _sprite;
    private bool _dashReleased = true;
    private float _dashRepeatBuffer = 1f;
    private bool _mustReleaseDash = false;
    [SerializeField]
    private float _dashRepeatBufferStartingTime = 1f;
    [SerializeField]
    private float _dashCoolDownTime = 2f;

    private enum playerState
    {
        TAKINGDAMAGE,
        DASHING,
        ATTACKING,
        LANDING,
        FREE,
        DEAD,
        WINNING,
        SHOOTING,
        CHARGEDMELEE
    }
    // The player can only be doing one thing at once
    private playerState _currentState = playerState.FREE;


    // Use this for initialization
    void Start()
    {
        _dashRepeatBuffer = _dashRepeatBufferStartingTime;
        _controller = gameObject.GetComponent<CharacterController2D>();
        _attackCollider.enabled = false;
        _playerDirection = new Vector3(1, 0, 0);
        _sprite = gameObject.GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        float shoot = Input.GetAxis("Fire1");
        float aim = Input.GetAxis("Fire2");
        bool dash = Input.GetButton("Dash");
        bool melee = Input.GetButton("Melee");

        if (!dash) _mustReleaseDash = false;

        if (_isTakingDamage)
        {
            _damageTimer += Time.deltaTime;
            _sprite.enabled = !_sprite.enabled;
            if (_damageTimer >= _damageTime)
            {
                _isTakingDamage = false;
                _damageTimer = 0;
                _sprite.enabled = true;
            }
        }

        // TODO: this is hack, it appears when A is pressed it shoots AND dashes, need to investigate
        if (dash) shoot = 0;
        if (melee) { aim = 0; _meleeCharge += Time.deltaTime; }
        else _meleeCharge = 0;

        if (_meleeCharge > _meleeChargeTime)
        {
            _currentState = playerState.CHARGEDMELEE;
            _meleeCharge = 0;
        }
        else if (_meleeCharge > _meleeChargePauseTime)
            return;

        if (_currentState == playerState.FREE)
        {
            if (dash && canDash)
            {
                _mustReleaseDash = true;
                _currentState = playerState.DASHING;
            }
            else if (melee)
                _currentState = playerState.ATTACKING;
            else if (aim > 0)
                _currentState = playerState.SHOOTING;
        }
        else if (aim == 0 && _currentState == playerState.SHOOTING)
            _currentState = playerState.FREE;

        if (_currentState == playerState.CHARGEDMELEE)
        {
            if (_attackTimer < _chargedAttackTime)
            {
                _attackTimer += Time.deltaTime;
                _attackCollider.enabled = true;
                damage = 5;
                return;
            }
            else
            {
                _attackCollider.enabled = false;
                _currentState = playerState.FREE;
                _attackTimer = 0f;
                damage = 1;
                _meleeCharge = 0;
            }
        }
        else if (_currentState == playerState.ATTACKING)
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
            _dashTimer += Time.deltaTime;
            if (_dashTimer < _dashTime)
            {
                _controller.move(_playerDirection * Time.deltaTime * _dashSpeed);
                // HACK: if the player presses dash again while dashing, stop the dash
                if (dash && !_mustReleaseDash)
                    _dashTimer = 100f;

                // No more actions possible while dashing
                return;
            }
            else
            {
                if (_dashTime + _dashRepeatBuffer > _dashTimer)
                {
                    // If the player has tried to dash again and they still have a chance to
                    if (dash && !_mustReleaseDash)
                    {
                        _dashTimer = 0f;
                        if (_dashRepeatBuffer > 0.03f)
                            _dashRepeatBuffer *= 0.66f;
                        _dashRepeatBuffer *= 2;
                        _dashRepeatBuffer /= 3;
                        Debug.Log(_dashRepeatBuffer);
                        // HACK: copied and pasted movement code to get it working
                        //  The player is able to change directions before dashing again
                        Vector3 dir = Vector3.zero;

                        dir.x += Input.GetAxis("Horizontal");
                        dir.y += Input.GetAxis("Vertical");
                        dir = Vector3.Normalize(dir);
                        dir = SnapToNearestMovementAxis(dir);

                        // If the player is changing directions, remember which way they ended up facing
                        if (dir.x != 0 || dir.y != 0)
                        {
                            _playerDirection = dir;
                        }
                        // END HACK

                        _controller.move(_playerDirection * Time.deltaTime * _dashSpeed);

                        _mustReleaseDash = true;

                        // No more actions possible while dashing
                        return;
                    }
                }
                else
                {
                    _currentState = playerState.FREE;
                    _dashTimer = 0f;
                    dashTime = 0;
                    _dashRepeatBuffer = _dashRepeatBufferStartingTime;
                }
            }
        }

        Vector3 playerInputDirection = Vector3.zero;

        playerInputDirection.x += Input.GetAxis("Horizontal");
        playerInputDirection.y += Input.GetAxis("Vertical");
        playerInputDirection = Vector3.Normalize(playerInputDirection);
        //Debug.Log(playerInputDirection.x + ", " + playerInputDirection.y);

        // Shooting
        if (shoot > 0 && _triggerHasBeenReleased && _ammo > 0)
        {
            _ammo--;
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
            if (!_isTakingDamage)
                _controller.move(SnapToNearestMovementAxis(movementAxis) * Time.deltaTime * _movementSpeed);
            else
                _controller.move(SnapToNearestMovementAxis(movementAxis) * Time.deltaTime * _damageSpeed);
            _attackCollider.gameObject.transform.position = gameObject.transform.position + movementAxis;
            DrawLine(gameObject.transform.position, gameObject.transform.position + movementAxis, Color.red, 0.05f);
        }
        else
            DrawLine(gameObject.transform.position, gameObject.transform.position + playerInputDirection, Color.red, 0.05f);

        if (_currentState == playerState.ATTACKING)
            DrawLine(new Vector3(gameObject.transform.position.x-0.2f, gameObject.transform.position.y - 0.2f, gameObject.transform.position.z), new Vector3(gameObject.transform.position.x - 0.2f, gameObject.transform.position.y - 0.2f, gameObject.transform.position.z) + playerInputDirection, Color.blue, 0.05f);
        else if (_currentState == playerState.CHARGEDMELEE)
            DrawLine(new Vector3(gameObject.transform.position.x - 0.2f, gameObject.transform.position.y - 0.2f, gameObject.transform.position.z), new Vector3(gameObject.transform.position.x - 0.2f, gameObject.transform.position.y - 0.2f, gameObject.transform.position.z) + playerInputDirection*2, Color.blue, 0.05f);

        if (dashTime < _dashCoolDownTime)
        {
            canDash = false;
            dashTime += Time.deltaTime;
            DrawDashBar(dashTime, SCREENHEIGHT / 2 - 0.6f);
        }
        else if (dashTime > _dashCoolDownTime)
        {
            canDash = true;
            DrawDashBar(dashTime, SCREENHEIGHT / 2 - 0.6f);
        }
        DrawHealthBar(SCREENHEIGHT / 2 - 0.2f);
        DrawAmmoBar(SCREENHEIGHT / 2 - 0.4f);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "LevelWrapperR")
        {
            _ammo += 5;
            if (_ammo > 100) _ammo = 100;
        }
        else if (other.tag == "LevelWrapperL")
        {
            _ammo += 5;
            if (_ammo > 100) _ammo = 100;
        }
        else if (other.tag == "LevelWrapperD")
        {
            _ammo += 5;
            if (_ammo > 100) _ammo = 100;
        }
        else if (other.tag == "LevelWrapperU")
        {
            _ammo += 5;
            if (_ammo > 100) _ammo = 100;
        }
        else if (other.tag == "PlayerBullet")
        {
            Destroy(other.gameObject);
            _playerHealth -= bulletDamage;
            _isTakingDamage = true;
            if (_playerHealth <= 0)
                _currentState = playerState.DEAD;
        }
        else if (other.tag == "EnemyBullet")
        {
            Destroy(other.gameObject);
            _playerHealth -= bulletDamage;
            _isTakingDamage = true;
            if (_playerHealth <= 0)
                _currentState = playerState.DEAD;
        }
        else if (other.tag == "Enemy")
        {
            _playerHealth -= _enemyDamage;
            _isTakingDamage = true;
            if (_playerHealth <= 0)
                _currentState = playerState.DEAD;
        }
        else if (other.tag == "Ammo")
        {
            Debug.Log("Hi I'm some ammo");
            Destroy(other.gameObject);
            _ammo += 15;
            if (_ammo > 100) _ammo = 100;
        }
        else if (other.tag == "Door")
        {
            int scene = SceneManager.GetActiveScene().buildIndex;
            scene++;
            SceneManager.LoadScene(scene);
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
        Vector3 returnDirection = new Vector3(0, 0, 0);

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

    void DrawHealthBar(double y)
    {
        float barLocX = SCREENWIDTH / 2 * -1 + 0.2f;
        Vector3 healthBarStart = new Vector3(barLocX, (float)y, 0);
        int healthBarLength = 5;
        Vector3 healthBarEnd = new Vector3(healthBarStart.x + healthBarLength * _playerHealth / 100f, healthBarStart.y, healthBarStart.z);
        DrawLine(healthBarStart, healthBarEnd, Color.red);
    }

    void DrawDashBar(float length, double y)
    {
        float barLocX = SCREENWIDTH / 2 * -1 + 0.2f;
        Vector3 DashBarStart = new Vector3(barLocX, (float)y, 0);
        float DashBarLength = length;
        Vector3 DashBarEnd = new Vector3(DashBarStart.x + (length), DashBarStart.y, DashBarStart.z);
        DrawLine(DashBarStart, DashBarEnd, Color.cyan);
    }

    void DrawAmmoBar(double y)
    {
        float barLocX = SCREENWIDTH / 2 * -1 + 0.2f;
        Vector3 ammoBarStart = new Vector3(barLocX, (float)y, 0);
        int ammoBarLength = 5;
        Vector3 ammoBarEnd = new Vector3(ammoBarStart.x + ammoBarLength * _ammo / 100f, ammoBarStart.y, ammoBarStart.z);
        DrawLine(ammoBarStart, ammoBarEnd, Color.green);
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