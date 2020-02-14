using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FouleCollision : MonoBehaviour
{

    BoxCollider2D hitbox;
    Animator anim;

    private void Start()
    {
        anim = GetComponent<Animator>();
        hitbox = GetComponent<BoxCollider2D>();
    }
    // Update is called once per frame
    void Update()
    {
        Collider2D[] enemy = new Collider2D[1];

        ContactFilter2D contactFilter = new ContactFilter2D
        {
            layerMask = LayerMask.GetMask("Enemy"),
            useLayerMask = true,
            useDepth = true,
            maxDepth = transform.position.z,
            minDepth = transform.position.z
        };


        if (hitbox.OverlapCollider(contactFilter, enemy) == 1)
        {
            if (!enemy[0].GetComponent<Enemy>().dead)
            {
                anim.SetTrigger("explosion");
            }
        }
    }

    public void RemoveFoule()
    {
        gameObject.SetActive(false);
        Invoke("ReactivateFoule", 6f);
    }

    private void ReactivateFoule()
    {
        gameObject.SetActive(true);
    }
}
