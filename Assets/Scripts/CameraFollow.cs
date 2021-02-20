using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    private Transform playerTransform;
    private Vector2 cameraOffset;

    [Range(0.01f, 1.0f)]
    public float SmoothFactor = 1f;
    // Start is called before the first frame update
    void Start()
    {
        playerTransform = GameManager.instance.player.transform;
        cameraOffset = playerTransform.position - playerTransform.position;
        Debug.Log(cameraOffset);
    }

    // Update is called once per frame
    void LateUpdate()
    {
        Vector2 playerpos = playerTransform.position;
        Vector2 newPos = playerpos + cameraOffset;
        Vector3 orig = new Vector3(playerTransform.position.x, playerTransform.position.y, 0);
        Vector3 result = new Vector3(newPos.x, newPos.y, 0);
        transform.position = Vector3.Slerp(orig, result, SmoothFactor);
    }
}
