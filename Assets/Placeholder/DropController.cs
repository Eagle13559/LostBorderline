using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropController : MonoBehaviour {
    [SerializeField]
    private float _timer = 4f;          // max lifetime
    private float _panicTime = 3f;      // start blinking after 3 seconds
    private float _timeAlive = 0f;      // current lifetime
    private SpriteRenderer _sprite;     // sprite
    private bool _isPanicked = false;   // start blinking if true



    // Use this for initialization
    void Start()
    {
        _sprite = gameObject.GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        _timeAlive += Time.deltaTime;
        if (_timeAlive > _panicTime)
            _isPanicked = true;
        if (_isPanicked)
            _sprite.enabled = !_sprite.enabled;
        if (_timeAlive > _timer)
            Destroy(gameObject);
    }
}
