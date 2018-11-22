using System;
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
    private LevelGenerator _levelGenerator;

    // Use this for initialization
    void Start()
    {
        StartCoroutine(Spawn());
        _levelGenerator = GetComponent<LevelGenerator>();
    }

    IEnumerator Spawn()
    {
        var enemyCounter = 0;
        while (enabled)
        {
            yield return new WaitForSeconds(SpawnPeriod);
            if (!GameObject.FindGameObjectWithTag("Player").GetComponent<KnightBehavior>().Running ||
                !_levelGenerator.LevelReady || enemyCounter >= _levelGenerator.Level.Length) continue;
            switch (_levelGenerator.Level[enemyCounter])
            {
                 case LevelGenerator.LevelElement.Beast: Instantiate(Hound, HoundSpawnPosition.position, Quaternion.identity);
                        break;
                 case LevelGenerator.LevelElement.Harpy: Instantiate(Oculothrax, OculothraxSpawnPosition.position, Quaternion.identity);
                     break;
                 case LevelGenerator.LevelElement.Ogre: Instantiate(Ogre, OgreSpawnPosition.position, Quaternion.identity);
                     break;
                 case LevelGenerator.LevelElement.Skeleton: Instantiate(Skeleton, SkeletonSpawnPosition.position, Quaternion.identity);
                     break;
                case LevelGenerator.LevelElement.None:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            enemyCounter++;
        }
    }

    // Update is called once per frame
    void Update()
    {
    }
}