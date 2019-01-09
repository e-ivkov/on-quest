using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissScript : MonoBehaviour
{

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") &&
            gameObject.GetComponentInParent<SkeletonScript>().CheckPlayerAction(collision.gameObject))
        {
            var animator = gameObject.GetComponentInParent<Animator>();
            animator.SetTrigger("miss");
        }
    }
}
