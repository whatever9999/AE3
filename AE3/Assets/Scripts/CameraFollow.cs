using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    private Transform player;
    private float cameraZPos;

    private void Start()
    {
        player = GameObject.Find("Isilion").transform;
        cameraZPos = transform.position.z;
    }

    private void Update()
    {
        Vector3 newPosition = player.position;
        newPosition.z = cameraZPos;
        transform.position = newPosition;
    }
}
