using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    static Animator anim;

    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    public static void ShakeCamera(bool hit)
    {
        if (hit)
        {
            anim.SetTrigger("bigShake");
        } else
        {
            anim.SetTrigger("shake");
        }
    }
}
