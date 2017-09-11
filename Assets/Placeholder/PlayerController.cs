using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Prime31;

public class PlayerController : MonoBehaviour
{

    float movementSpeed = 5;
    private CharacterController2D _controller;

    [SerializeField]
    private float _warpLocationRightX;
    [SerializeField]
    private float _warpLocationLeftX;



    // Use this for initialization
    void Start()
    {
        _controller = gameObject.GetComponent<CharacterController2D>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 movement = Vector3.zero;

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            movement.x -= movementSpeed;

        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            movement.x += movementSpeed;

        }
        if (Input.GetKey(KeyCode.UpArrow))
        {
            movement.y += movementSpeed;

        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            movement.y -= movementSpeed;

        }

        _controller.move(movement * Time.deltaTime);
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
}