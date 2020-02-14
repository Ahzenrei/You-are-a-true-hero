using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveFoule : MonoBehaviour
{
    [SerializeField]
    float xOffset = -2;
    [SerializeField]
    float yOffset = 2;
    [SerializeField]
    float zOffset = 0;

    public void Move (float xPosition)
    {
        transform.position = new Vector3(xPosition + xOffset, yOffset, zOffset);
    }
}
