using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelController : MonoBehaviour {
    private GameObject _door;
    private BoxCollider2D _doorCollider;
    private SpriteRenderer _doorRenderer;
    private bool _playerFinished = false;

	// Use this for initialization
	void Start () {
        _door = GameObject.Find("Door");
        _doorCollider = _door.gameObject.GetComponent<BoxCollider2D>();
        _doorRenderer = _door.gameObject.GetComponent<SpriteRenderer>();
        _doorCollider.enabled = false;
        _doorRenderer.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!_playerFinished)
        {
            if (GameObject.Find("SplitEnemy") == null)
            {
                if (GameObject.Find("SplitEnemySmall(Clone)") == null)
                {
                    if (GameObject.Find("TeleportEnemy") == null)
                    {
                        _doorCollider.enabled = true;
                        _doorRenderer.enabled = true;
                        _playerFinished = true;
                    }
                }

            }
        }
    }
}
