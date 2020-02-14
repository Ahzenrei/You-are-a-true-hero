using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Werewolf : Enemy
{
    protected override void Start()
    {
        base.Start();
        Invoke("SelfDestroy", 30f);
    }

    protected override void Update()
    {
        base.Update();
        if (!dead)
        {
            Vector3 velocity = new Vector3(-enemySpeed, 0, 0);

            transform.Translate(velocity * Time.deltaTime);
        }
    }
}
