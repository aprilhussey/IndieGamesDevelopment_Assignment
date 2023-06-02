using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    Camera mainCam;
    
    public int posX;
    public int posY;
    int posZ = -10;

    void Awake()
    {
		mainCam = Camera.main;
	}

    void MoveCamera(int x, int y)
    {
        mainCam.transform.position = transform.position = new Vector3(x, y, posZ);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            MoveCamera(posX, posY);
        }
    }
}
