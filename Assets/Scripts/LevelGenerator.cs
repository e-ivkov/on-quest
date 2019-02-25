using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using Random = System.Random;
using GeneticSharp.Domain;

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

	

	public bool LevelReady = false;
    public LevelElement[] Level;
	private Task _genTask;
	
	// Use this for initialization
	void Start ()
	{
		_genTask = Task.Run(() => {
            GenerateLevelCustom();
            LevelReady = true;
        });
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

	private void GenerateLevelCustom()
	{
		Level = new LevelElement[LevelSize];
		for(var i = 0; i < nChunks; i++)
		{
			var dcParams = DifficultyCurveParams;
			var difficulty = DifficultyCurve(i, dcParams.a, dcParams.b, dcParams.k);
			var eParams = EvolutionParams;
			var chunkSize = LevelSize / nChunks;
			var chunk = CustomGenetic.GenerateLevelChunk(difficulty, LevelSize / nChunks, eParams.populationSize, eParams.nGroups,
				eParams.nIterations, eParams.mutProb);
            var chromosome = new CustomGenetic.Chromosome(chunk);
            var retDifficulty = chromosome.Difficulty;
			Array.Copy(chunk, 0, Level, i * chunkSize, chunkSize);
		}
	}

	
}
