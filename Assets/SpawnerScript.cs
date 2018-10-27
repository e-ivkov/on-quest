using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerScript : MonoBehaviour
{
    public GameObject Skeleton;
    public GameObject Ogre;
    public GameObject Hound;
    public GameObject Oculothrax;
    public Transform SkeletonSpawnPosition;
    public Transform OgreSpawnPosition;
    public Transform HoundSpawnPosition;
    public Transform OculothraxSpawnPosition;

    public float Probability;
    public int SpawnPeriod;

    // Use this for initialization
    void Start()
    {
        StartCoroutine(Spawn());
    }

    IEnumerator Spawn()
    {
        while (enabled)
        {
            yield return new WaitForSeconds(SpawnPeriod);
            if ((Random.Range(0f, 1f) < Probability) &&
                GameObject.FindGameObjectWithTag("Player").GetComponent<KnightBehavior>().Running)
            {
                var rand = Random.Range(0f, 1f);
                if (rand >= 0 && rand < 0.25)
                {
                    Instantiate(Skeleton, SkeletonSpawnPosition.position, Quaternion.identity);
                }
                else if (rand >= 0.25 && rand < 0.5)
                {
                    Instantiate(Ogre, OgreSpawnPosition.position, Quaternion.identity);
                }
                else if (rand >= 0.5 && rand < 0.75)
                {
                    Instantiate(Hound, HoundSpawnPosition.position, Quaternion.identity);
                }
                else if (rand >= 0.75 && rand < 1)
                {
                    Instantiate(Oculothrax, OculothraxSpawnPosition.position, Quaternion.identity);
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
    }
}