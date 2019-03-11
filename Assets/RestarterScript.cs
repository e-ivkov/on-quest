using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RestarterScript : MonoBehaviour
{
    public const float Velocity = 0.71f;
    private float _playerVelocity;

    // Update is called once per frame
    void Update()
    {
        _playerVelocity = GameObject.FindGameObjectWithTag("Player").GetComponent<KnightBehavior>().Speed;
        if (GameObject.FindGameObjectWithTag("Player").GetComponent<KnightBehavior>().Running)
            transform.Translate(Time.deltaTime * Velocity * _playerVelocity * Vector3.left);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
