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
    public float SpawnDistance;
    public float StartSpawnDistance;
    private LevelGenerator _levelGenerator;
    public GameObject Player;
    private float _nextDistance;

    // Use this for initialization
    void Start()
    {
        _nextDistance = StartSpawnDistance;
        StartCoroutine(Spawn());
        _levelGenerator = GetComponent<LevelGenerator>();
    }

    IEnumerator Spawn()
    {
        var enemyCounter = 0;
        while (enabled)
        {
            yield return new WaitUntil(() => Player.GetComponent<KnightBehavior>().Distance >= _nextDistance);
            _nextDistance = Player.GetComponent<KnightBehavior>().Distance + SpawnDistance;
            if (!Player.GetComponent<KnightBehavior>().Running ||
                !_levelGenerator.LevelReady || enemyCounter >= _levelGenerator.Level.Length) continue;
            switch (_levelGenerator.Level[enemyCounter])
            {
                 case LevelElement.Beast: Instantiate(Hound, HoundSpawnPosition.position, Quaternion.identity);
                        break;
                 case LevelElement.Harpy: Instantiate(Oculothrax, OculothraxSpawnPosition.position, Quaternion.identity);
                     break;
                 case LevelElement.Ogre: Instantiate(Ogre, OgreSpawnPosition.position, Quaternion.identity);
                     break;
                 case LevelElement.Skeleton: Instantiate(Skeleton, SkeletonSpawnPosition.position, Quaternion.identity);
                     break;
                case LevelElement.None:
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