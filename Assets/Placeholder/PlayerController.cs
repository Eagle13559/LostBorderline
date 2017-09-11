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
    private bool _isShooting;
    // Performs movement and collision so we don't have to think about it
    private CharacterController2D _controller;
    private bool _triggerHasBeenReleased;


    // Use this for initialization
    void Start()
    {
        _controller = gameObject.GetComponent<CharacterController2D>();
        _isShooting = false;
        _triggerHasBeenReleased = true;
    }

    // Update is called once per frame
    void Update()
    {
        // Check for player aiming
        float aim = Input.GetAxis("Fire2");
        if (aim > 0)
        {
            _isShooting = true;
        }
        else
        {
            _isShooting = false;
        }
        // Check for player shooting (Check Axis not Button for robustness between systems)
        float fire = Input.GetAxis("Fire1");
        if (fire > 0 && _triggerHasBeenReleased)
        {
            _triggerHasBeenReleased = false;
            GameObject bullet = Instantiate(_bullet, gameObject.transform.position, Quaternion.identity);
            bullet.GetComponent<BulletController>()._direction = Vector3.Normalize(_playerDirection);
        }
        else if (fire == 0)
            _triggerHasBeenReleased = true;

        Vector3 playerInputDirection = Vector3.zero;

        playerInputDirection.x += Input.GetAxis("Horizontal");
        playerInputDirection.y += Input.GetAxis("Vertical");

        // If the player is changing directions, remember which way they ended up facing
        if (playerInputDirection.x != 0 || playerInputDirection.y != 0)
            _playerDirection = playerInputDirection;

        // If the player is in shoot mode, don't perform the move
        if (!_isShooting)
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