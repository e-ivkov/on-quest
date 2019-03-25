using UnityEngine;
using System.Collections;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Randomizations;


public class LevelChunkChromosome : ChromosomeBase
{
    private readonly int enemyTypesCount = System.Enum.GetNames(typeof(LevelElement)).Length-1; // length - Restarter

    public LevelChunkChromosome(int length) : base(length)
    {
        var enemies = RandomizationProvider.Current.GetInts(length, 0, enemyTypesCount);
        for (int i = 0; i < enemies.Length; i++)
        {
            ReplaceGene(i, new Gene((LevelElement)enemies[i]));
        }
    }

    public override IChromosome CreateNew() => new LevelChunkChromosome(Length);

    public override Gene GenerateGene(int geneIndex) => new Gene((LevelElement)RandomizationProvider.Current.GetInt(0, enemyTypesCount));
}

