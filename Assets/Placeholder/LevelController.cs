using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelController : MonoBehaviour {
    [SerializeField]
    GameObject _enemy;
    [SerializeField]
    GameObject _door;
    [SerializeField]
    GameObject _camera;
    private int lvlCount;
    private bool newRoom;
	// Use this for initialization
	void Start () {
        lvlCount = 0;
        newRoom = true;
    }
	
	// Update is called once per frame
	void Update () {

        switch (lvlCount)
        {
            case 0:
                if (GameObject.Find("SplitEnemy1.1") == null)
                {
                    if (GameObject.Find("SplitEnemySmall(Clone)") == null)
                    {
                        _door.transform.localScale = new Vector3(3, .7f, 0);
                        Instantiate(_door, new Vector3(0, 5, 0), Quaternion.identity);
                        lvlCount++;
                        break;
                    }

                }
                break;
            case 1:
                if(newRoom)
                {
                    Instantiate(_enemy, new Vector3(-7, 10, 0), Quaternion.identity);
                    Instantiate(_enemy, new Vector3(5, 12, 0), Quaternion.identity);
                    newRoom = false;
                }
                
                if (GameObject.Find("SplitEnemy(Clone)") == null)
                {
                    if (GameObject.Find("SplitEnemySmall(Clone)") == null)
                    {
                        _door.transform.localScale = new Vector3(3, .7f, 0);
                        Instantiate(_door, new Vector3(0, 15, 0), Quaternion.identity);
                        lvlCount++;
                        newRoom = true;
                        break;
                    }

                }
                break;
            case 2:
                if(newRoom)
                {
                    Instantiate(_enemy, new Vector3(-8, 23, 0), Quaternion.identity);
                    Instantiate(_enemy, new Vector3(0, 23, 0), Quaternion.identity);
                    Instantiate(_enemy, new Vector3(8, 23, 0), Quaternion.identity);
                    Instantiate(_enemy, new Vector3(0, 21, 0), Quaternion.identity);
                    newRoom = false;
                }
                
                if (GameObject.Find("SplitEnemy(Clone)") == null)
                {
                    if (GameObject.Find("SplitEnemySmall(Clone)") == null)
                    {
                        _door.transform.localScale = new Vector3(3, .7f, 0);
                        Instantiate(_door, new Vector3(0, 15, 0), Quaternion.identity);
                        lvlCount++;
                        newRoom = true;
                        break;
                    }

                }
                break;

        }	
	}
}
