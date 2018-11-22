using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class LevelGenerator : MonoBehaviour {

    public int LevelSize;
    
    public int nChunks;
	
	public DifficultyCurveParamsType DifficultyCurveParams;
	
	[Serializable]
	public struct DifficultyCurveParamsType
	{
		public float a;
		public float b;
		public float k;
	}

	public EvolutionParamsType EvolutionParams;
	
	[Serializable]
	public struct EvolutionParamsType
	{
		public int populationSize;
		public int nGroups;
		public int nIterations;
		public float mutProb;
	}

	public enum LevelElement
	{
		None = 0,
		Ogre = 1,
		Harpy = 2,
		Beast = 3,
		Skeleton = 4
	}

	public bool LevelReady = false;
    public LevelElement[] Level;
	
	// Use this for initialization
	void Start ()
	{
		StartCoroutine(GenerateLevel());
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	/*
	 * y = a*sin(k*x) + b*x
	 */
	private float DifficultyCurve(float x, float a, float b, float k)
	{
		return a * Mathf.Sin(k * x) + b * x;
	}

	private IEnumerator GenerateLevel()
	{
		Level = new LevelElement[LevelSize];
		for(var i = 0; i < nChunks; i++)
		{
			var dcParams = DifficultyCurveParams;
			var difficulty = DifficultyCurve(i, dcParams.a, dcParams.b, dcParams.k);
			var eParams = EvolutionParams;
			var chunkSize = LevelSize / nChunks;
			var chunk = GenerateLevelChunk(difficulty, LevelSize / nChunks, eParams.populationSize, eParams.nGroups,
				eParams.nIterations, eParams.mutProb);
			Array.Copy(chunk, 0, Level, i * chunkSize, chunkSize);
			yield return null;
		}
		LevelReady = true;
	}

	private class Chromosome
	{
		private LevelElement[] _levelElements;

		private float _fitness = -1;

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

		private Chromosome(LevelElement[] levelElements)
		{
			_levelElements = new LevelElement[levelElements.Length];
			Array.Copy(levelElements, _levelElements, levelElements.Length);
		}

		public static Chromosome GetRandom(int levelSize)
		{
			var nElements = Enum.GetNames(typeof(LevelElement)).Length;
			return new Chromosome(Enumerable.Repeat(0, levelSize)
				.Select(i => (LevelElement) Random.Range(0, nElements - 1)).ToArray());
		}

		public static Chromosome Mutate(Chromosome chromosome, float prob)
		{
			var nElements = Enum.GetNames(typeof(LevelElement)).Length;
			return new Chromosome(chromosome._levelElements.AsEnumerable()
				.Select(e => Random.value > prob ? (LevelElement) Random.Range(0, nElements - 1) : e).ToArray());
		}

		public static Chromosome OnePointCrossover(Chromosome first, Chromosome second)
		{
			var length = first.LevelElements.Length;
			var splitPoint = Random.Range(0, length);
			var child = new LevelElement[length];
			Array.Copy(first.LevelElements, child, splitPoint);
			Array.Copy(first.LevelElements, splitPoint, child, splitPoint, length-splitPoint);
			return new Chromosome(child);
		}

		public float GetFitness(float difficulty)
		{
			if (_fitness < 0)
				_fitness = 1 / (Mathf.Abs(Difficulty - difficulty));
			return _fitness;
		}
	}
	
	private LevelElement[] GenerateLevelChunk(float difficulty, int levelSize, int populationSize, int nGroups, int nIterations, float mutProb)
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
