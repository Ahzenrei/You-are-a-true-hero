using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Roseau : MonoBehaviour
{
    Animator anim;
    BoxCollider2D hitbox;
    Collider2D[] player;
    ContactFilter2D contactFilter;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        hitbox = GetComponentInParent<BoxCollider2D>();

        player = new Collider2D[1];

        contactFilter = new ContactFilter2D
        {
            layerMask = LayerMask.GetMask("Player"),
            useLayerMask = true,
            useDepth = true,
            maxDepth = transform.position.z + 2,
            minDepth = transform.position.z
        };
    }

    // Update is called once per frame
    void Update()
    {
        if (hitbox.OverlapCollider(contactFilter, player) == 1)
        {
            if(player[0].GetComponent<Animator>().GetFloat("speed") > 0.1f)
            {
                anim.SetTrigger("move");
            }
        }
    }
}
