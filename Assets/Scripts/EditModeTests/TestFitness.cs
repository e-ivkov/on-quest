using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using GeneticSharp.Domain.Chromosomes;

namespace Tests
{
    public class TestFitness
    {
        private const double _errorMargin = 0.001;
        // A Test behaves as an ordinary method
        [Test]
        public void TestEquality()
        {
            var fitness = new LevelChunkFitness(0);

            // Use the Assert class to test conditions
            var chromosome = new LevelChunkChromosome(10);
            Assert.LessOrEqual(Mathf.Abs((float)fitness.Evaluate(chromosome)
                - (float)fitness.Evaluate(chromosome.Clone())), _errorMargin);
        }

        /// <summary>
        /// Test Increase Lower Target
        /// </summary>
        [Test]
        public void TestIncreaseLT()
        {
            var fitness = new LevelChunkFitness(0);

            // Use the Assert class to test conditions
            var chromosome1 = new LevelChunkChromosome(2);
            chromosome1.ReplaceGenes(0, new Gene[] { new Gene(LevelElement.None), new Gene(LevelElement.None) });

            var chromosome2 = new LevelChunkChromosome(2);
            chromosome2.ReplaceGenes(0, new Gene[] { new Gene(LevelElement.None), new Gene(LevelElement.Beast) });

            Assert.Less(fitness.Evaluate(chromosome2), fitness.Evaluate(chromosome1));
        }

        /// <summary>
        /// Test Increase Higher Target
        /// </summary>
        [Test]
        public void TestIncreaseHT()
        {
            var fitness = new LevelChunkFitness(10);

            // Use the Assert class to test conditions
            var chromosome1 = new LevelChunkChromosome(2);
            chromosome1.ReplaceGenes(0, new Gene[] { new Gene(LevelElement.None), new Gene(LevelElement.None) });

            var chromosome2 = new LevelChunkChromosome(2);
            chromosome2.ReplaceGenes(0, new Gene[] { new Gene(LevelElement.None), new Gene(LevelElement.Beast) });

            Assert.Less(fitness.Evaluate(chromosome1), fitness.Evaluate(chromosome2));
        }
    }
}
