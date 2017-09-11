using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour {

    public Vector3 _direction;
    [SerializeField]
    private float _speed = 1;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        transform.Translate(_direction * Time.deltaTime * _speed);
	}

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "LevelWrapperR" || other.tag == "LevelWrapperL" || other.tag == "Wall")
        {
            Destroy(gameObject);
        }
    }
}
