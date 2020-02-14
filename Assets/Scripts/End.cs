using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class End : MonoBehaviour
{
    BoxCollider2D hitbox;

    private void Start()
    {
        hitbox = GetComponent<BoxCollider2D>();
    }
    // Update is called once per frame
    void Update()
    {
        Collider2D[] player = new Collider2D[1];

        ContactFilter2D contactFilter = new ContactFilter2D
        {
            layerMask = LayerMask.GetMask("Player"),
            useLayerMask = true,
        };


        if (hitbox.OverlapCollider(contactFilter, player) == 1)
        {
            SceneManager.LoadScene("TrueEnd");
        }
    }
}
