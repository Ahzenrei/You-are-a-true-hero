using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    float cameraSpeed = 1;
    [SerializeField]
    float xOffset = 0;
    [SerializeField]
    float yOffset = 2;
    [SerializeField]
    float zOffset = -2.5f;

    public BoxCollider2D hitbox;

    public MoveFoule foule;

    private void Update()
    {
        Vector3 cameraVelocity = new Vector3(cameraSpeed, 0, 0) * Time.deltaTime;

        Collider2D[] player = new Collider2D[1];

        ContactFilter2D contactFilter = new ContactFilter2D
        {
            layerMask = LayerMask.GetMask("Player"),
            useLayerMask = true
        };


        if (hitbox.OverlapCollider(contactFilter, player) == 1)
        {
            player[0].transform.Translate(cameraVelocity);
        }

        transform.Translate(cameraVelocity);

    }

    public void MoveCamera (Vector3 newPosition)
    {
        newPosition.x += xOffset;

        if (newPosition.x < transform.position.x)
        {
            newPosition.x = transform.position.x;
        }

        transform.position = new Vector3(newPosition.x, yOffset, newPosition.z + zOffset);

        foule.Move(newPosition.x);
    }
}
