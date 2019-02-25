using UnityEngine;
using System.Collections;
using GeneticSharp.Domain.Fitnesses;
using GeneticSharp.Domain.Chromosomes;

public class LevelChunkFitness : IFitness
{
    public float TargetDifficulty
    {
        get;
        private set;
    }

    public LevelChunkFitness(float targetDifficulty)
    {
        TargetDifficulty = targetDifficulty;
    }

    public double Evaluate(IChromosome chromosome)
    {
        var difficulty = 0;
        var genes = chromosome.GetGenes();
        for (var i = 0; i < genes.Length - 1; i++)
        {
            var type1 = ((LevelElement)genes[i].Value).GetElementType();
            var type2 = ((LevelElement)genes[i+1].Value).GetElementType();
            if (type1 == type2 && type1 > 0)
                difficulty += 2;
            else if (Mathf.Abs(type1 - type2) == 1 && type1 > 0 && type2 > 0)
                difficulty += 3;
            else if (type2 - type1 >= 1)
                difficulty += 1;
        }
        return -(Mathf.Abs(TargetDifficulty - difficulty));
    }
}
