using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bat : Enemy
{
    [SerializeField]
    float maxY = 7f;
    [SerializeField]
    float minY = 6f;

    bool goingUp = true;

    protected override void Update()
    {
        base.Update();

        Vector3 velocity = new Vector3(-enemySpeed, 0, 0);

        if (dead)
        {
            velocity.x = 0;
            velocity.y = -4;

        } else if (goingUp)
        {
            if (transform.position.y > maxY)
            {
                velocity.y = -2f;
                goingUp = false;
            } else
            {
                velocity.y = 2f;
            }
        } else
        {
            if (transform.position.y < minY)
            {
                velocity.y = 2f;
                goingUp = true;
            }
            else
            {
                velocity.y = -2f;
            }
        }

        transform.Translate(velocity * Time.deltaTime);
    }
}
