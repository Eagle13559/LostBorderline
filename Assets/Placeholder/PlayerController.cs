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

        movement.x += Input.GetAxis("Horizontal");
        movement.y += Input.GetAxis("Vertical");

        _controller.move(movement * Time.deltaTime * movementSpeed);
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