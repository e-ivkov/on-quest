using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using Random = System.Random;
using GeneticSharp.Domain;
using GeneticSharp.Domain.Crossovers;
using GeneticSharp.Domain.Mutations;
using GeneticSharp.Domain.Populations;
using GeneticSharp.Domain.Selections;
using GeneticSharp.Domain.Terminations;
using GeneticSharp.Infrastructure.Framework.Threading;

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
    private GameMode generated = GameMode.None;

    public void Generate(GameMode gameMode)
    {
        if (gameMode == generated)
            return;
        _genTask = Task.Run(() => {
            switch (gameMode)
            {
                case GameMode.Game:
                    GenerateLevelGS();
                    Debug.Log("Generated Level");
                    break;
                case GameMode.Tutorial:
                    GenerateTutorial();
                    Debug.Log("Generated Tutorial");
                    break;
            }

            LevelReady = true;
        });
        generated = gameMode;
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    private void GenerateTutorial()
    {
        Level = new LevelElement[] { LevelElement.Beast, LevelElement.Harpy, LevelElement.Skeleton, LevelElement.Ogre, LevelElement.Restarter };
    }
	
	/*
	 * y = a*sin(k*x) + b*x
	 */
	private float DifficultyCurve(float x, float a, float b, float k)
	{
		return a * Mathf.Sin(k * x) + b * x;
	}

    private void GenerateLevelGS()
    {
        Level = new LevelElement[LevelSize];
        for (var i = 0; i < nChunks; i++)
        {
            var dcParams = DifficultyCurveParams;
            var difficulty = DifficultyCurve(i, dcParams.a, dcParams.b, dcParams.k);

            var eParams = EvolutionParams;
            var chunkSize = LevelSize / nChunks;

            var fitness = new LevelChunkFitness(difficulty);
            var chromosome = new LevelChunkChromosome(chunkSize);

            var crossover = new OnePointCrossover(chunkSize/2);
            var mutation = new UniformMutation(allGenesMutable : true);
            var selection = new RouletteWheelSelection();
            var population = new Population(50, 100, chromosome);

            var m_ga = new GeneticAlgorithm(population, fitness, selection, crossover, mutation)
            {
                Termination = new TimeEvolvingTermination(new TimeSpan(0,0,1))
            };

            m_ga.TaskExecutor = new ParallelTaskExecutor
            {
                MinThreads = 2,
                MaxThreads = 5
            };
            m_ga.Start();
            // Everty time a generation ends, we log the best solution.
            /*m_ga.GenerationRan += delegate
            {
                var f = m_ga.BestChromosome.Fitness;
                Debug.Log($"Generation: {m_ga.GenerationsNumber} - Fitness: ${f}");
            };*/
            var genes = m_ga.BestChromosome.GetGenes();
            for(int j = 0; j < chunkSize; j++)
            {
                Level[i * chunkSize + j] = (LevelElement)genes[j].Value;
            }
            Debug.Log($"Fitness: {m_ga.BestChromosome.Fitness}");
        }
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
