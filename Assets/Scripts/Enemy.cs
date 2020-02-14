using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class Enemy : MonoBehaviour
{
    [SerializeField]
    public float scoreValue = 50f;
    [SerializeField]
    protected float enemySpeed = 5f;

    public bool dead = false;

    Collider2D[] player;
    ContactFilter2D contactFilter;

    protected BoxCollider2D hitbox;
    protected Animator anim;
    private SpriteRenderer sprite;

    protected virtual void Start()
    {

        player = new Collider2D[1];

        contactFilter = new ContactFilter2D
        {
            layerMask = LayerMask.GetMask("Player"),
            useLayerMask = true,
            useDepth = true,
            maxDepth = transform.position.z,
            minDepth = transform.position.z
        };

        sprite = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        hitbox = GetComponent<BoxCollider2D>();
        Invoke("SelfDestroy", 30f);
        scoreValue += (50 - (transform.position.z/10)*25);

        if (transform.position.z == 0)
        {
            sprite.sortingLayerName = "FirstPath";
        }
        else if (transform.position.z == Player.widthLevel)
        {
            sprite.sortingLayerName = "SecondPath";
        }
        else
        {
            sprite.sortingLayerName = "ThirdPath";
        }
    }

    protected virtual void Update()
    {
        if (!dead)
        {
            if (hitbox.OverlapCollider(contactFilter, player) == 1)
            {
                player[0].GetComponent<Player>().GetHit();
            }
        }
    }

    public void Kill()
    {
        Score.AddScore(scoreValue);
        dead = true;
        if(anim != null)
        {
            anim.SetTrigger("death");
        } else
        {
            Invoke("SelfDestroy", 0f);
        }
    } 

    private void SelfDestroy()
    {
        Destroy(gameObject);
    }
}
