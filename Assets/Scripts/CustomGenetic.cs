using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using Random = System.Random;

public class CustomGenetic
{

    public class Chromosome
    {
        private LevelElement[] _levelElements;

        private float _fitness = 1;

        public LevelElement[] LevelElements
        {
            get { return _levelElements; }
        }

        /*
		 * None -> None = 0
		 * Any -> None = 0
		 * None -> Any = 1
		 * Ogre <-> Skeleton = 2 Type 1
		 * Harpy <-> Beast = 2 Type 2
		 * Type 1 <-> Type 2 = 3
		 */
        public float Difficulty
        {
            get
            {
                var difficulty = 0;
                for (var i = 0; i < _levelElements.Length - 1; i++)
                {
                    var type1 = GetElementType(_levelElements[i]);
                    var type2 = GetElementType(_levelElements[i + 1]);
                    if (type1 == type2 && type1 > 0)
                        difficulty += 2;
                    else if (Mathf.Abs(type1 - type2) == 1 && type1 > 0 && type2 > 0)
                        difficulty += 3;
                    else if (type2 - type1 >= 1)
                        difficulty += 1;
                }

                return difficulty;
            }
        }

        public static int GetElementType(LevelElement element)
        {
            switch (element)
            {
                case LevelElement.Ogre:
                case LevelElement.Skeleton:
                    return 1;
                case LevelElement.Harpy:
                case LevelElement.Beast:
                    return 2;
            }
            return 0;
        }

        public Chromosome(LevelElement[] levelElements)
        {
            _levelElements = new LevelElement[levelElements.Length];
            Array.Copy(levelElements, _levelElements, levelElements.Length);
        }

        public static Chromosome GetRandom(int levelSize)
        {
            var nElements = Enum.GetNames(typeof(LevelElement)).Length;
            var rand = new Random();
            return new Chromosome(Enumerable.Repeat(0, levelSize)
                .Select(i => (LevelElement)rand.Next(nElements)).ToArray());
        }

        public static Chromosome Mutate(Chromosome chromosome, float prob)
        {
            var nElements = Enum.GetNames(typeof(LevelElement)).Length;
            var rand = new Random();
            return new Chromosome(chromosome._levelElements.AsEnumerable()
                .Select(e => rand.NextDouble() > prob ? (LevelElement)rand.Next(nElements) : e).ToArray());
        }

        public static Chromosome OnePointCrossover(Chromosome first, Chromosome second)
        {
            var length = first.LevelElements.Length;
            var rand = new Random();
            var splitPoint = rand.Next(length);
            var child = new LevelElement[length];
            Array.Copy(first.LevelElements, child, splitPoint);
            Array.Copy(first.LevelElements, splitPoint, child, splitPoint, length - splitPoint);
            return new Chromosome(child);
        }

        public float GetFitness(float difficulty)
        {
            if (_fitness > 0)
                _fitness = -(Mathf.Abs(Difficulty - difficulty));
            return _fitness;
        }
    }

    public static LevelElement[] GenerateLevelChunk(float difficulty, int levelSize, int populationSize, int nGroups, int nIterations, float mutProb)
    {

        var population = Enumerable.Repeat(0, populationSize)
            .Select(i => Chromosome.GetRandom(levelSize));
        for (var i = 0; i < nIterations; i++)
        {
            population = Partition(population, populationSize / nGroups).SelectMany(partition =>
            {
                var ordered = partition.OrderByDescending(ch => ch.GetFitness(difficulty));
                var first = ordered.First();
                var child = Chromosome.OnePointCrossover(ordered.First(), ordered.ElementAt(1));
                child = Chromosome.Mutate(child, mutProb);
                return ordered.Select(ch => ch == first ? child : ch);
            });
        }

        var best = population.OrderByDescending(ch => ch.GetFitness(difficulty)).First();
        Debug.Log(best.GetFitness(difficulty));
        return best.LevelElements;
    }

    private static IEnumerable<IEnumerable<T>> Partition<T>(IEnumerable<T> items, int partitionSize)
    {
        List<T> partition = new List<T>(partitionSize);
        foreach (T item in items)
        {
            partition.Add(item);
            if (partition.Count == partitionSize)
            {
                yield return partition;
                partition = new List<T>(partitionSize);
            }
        }
        // Cope with items.Count % partitionSize != 0
        if (partition.Count > 0) yield return partition;
    }
}
