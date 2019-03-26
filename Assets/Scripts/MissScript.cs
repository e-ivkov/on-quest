using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissScript : MonoBehaviour
{
    private KnightBehavior _knight;

    public void Start()
    {
        _knight = GameObject.FindGameObjectWithTag("Player").GetComponent<KnightBehavior>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player"))
            return;
        var enemy = gameObject.GetComponentInParent<SkeletonScript>();
        if (_knight.GameMode == GameMode.Game)
        {
            if (enemy.CheckPlayerAction(collision.gameObject))
            {
                var progress = _knight.gameObject.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime % 1;
                var difficulty = 1/progress;
                Debug.Log(difficulty);
                Miss();
            }
        }
        else if(_knight.GameMode == GameMode.Tutorial)
        {
            _knight.ShowTutorialMessage(enemy, this);
        }
    }

    public void Miss()
    {
        var animator = gameObject.GetComponentInParent<Animator>();
        animator.SetTrigger("miss");
    }
}
