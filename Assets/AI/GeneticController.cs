using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NeuralNetwork;

namespace GeneticAlgorithm
{
    public class GeneticController
    {

        // GA params
        private int genomes_count, current_generation = 0;
        private static Random random = new Random();
        NeuroNetwork[] genomes;
        double[] fitness;
        const int solution_selection_for_crossover = 2; // Not edit

        // NeuroNetwork params
        private int neuro_input_nodes, neuro_output_nodes;
        private int[] neurons_per_layer;
        

        public GeneticController(int genomes_count, int neuro_input_nodes, int[] neurons_per_layer, int neuro_output_nodes)
        {
            // Set GA params
            this.genomes_count = genomes_count;
            this.neuro_input_nodes = neuro_input_nodes;
            this.neurons_per_layer = neurons_per_layer;
            this.neuro_output_nodes = neuro_output_nodes;
        }

        public void createGeneration()
        {
            if (current_generation == 0)
            {
                // Generate random genomes on first generation
                genomes = new NeuroNetwork[genomes_count];
                fitness = new double[genomes_count];

                for (int i = 0; i < genomes.Length; i++)
                {
                    genomes[i] = new NeuroNetwork(neuro_input_nodes, neurons_per_layer, neuro_output_nodes, random);
                }
            }
            else
            {
                // Generate new solutions trying to improve them
                improveGeneration();
            }

            current_generation++;
        }

        private void improveGeneration()
        {
            // Selecting better solutions
            Dictionary<NeuroNetwork, double> genomes_dictionary = new Dictionary<NeuroNetwork, double>();
            for (int i = 0; i < genomes_count; i++)
                genomes_dictionary.Add(genomes[i], fitness[i]);
            genomes_dictionary = genomes_dictionary.OrderByDescending(key => key.Value).Take(solution_selection_for_crossover).ToDictionary(pair => pair.Key, pair => pair.Value);

            // Crente new genomes array
            NeuroNetwork[] newGenomes = new NeuroNetwork[genomes_count];

            // Keep original 2 better solutions
            newGenomes[0] = genomes_dictionary.Keys.First();
            newGenomes[1] = genomes_dictionary.Keys.Last();

            // Cross-over solutions
            NeuroNetwork[] hybrids = crossover(newGenomes[0], newGenomes[1]);
            for (int i = 0; i < hybrids.Length; i++)
            {
                newGenomes[i + 2] = hybrids[i]; // Add crossed over solutions to new generation
            }

            // Mutated solutions
            for (int i = hybrids.Length + 2; i < newGenomes.Length; i++)
            {
                int selection = random.Next(0, 4);
                Perceptron[] old_ps = newGenomes[selection].getPerceptrons();
                Perceptron[] new_ps = new Perceptron[old_ps.Length];
                for (int o = 0; o < new_ps.Length; o++)
                {
                    new_ps[o] = new Perceptron(old_ps[o].getWeights(), old_ps[o].getBias());
                }

                newGenomes[i] = new NeuroNetwork(neuro_input_nodes, neurons_per_layer, neuro_output_nodes, random, new_ps);
                newGenomes[i].applyMutations(random);
            }

            // Replace old generation with new one
            genomes = newGenomes;
        }

        private NeuroNetwork[] crossover(NeuroNetwork genome1, NeuroNetwork genome2)
        {
            NeuroNetwork newGenome1 = new NeuroNetwork(neuro_input_nodes, neurons_per_layer, neuro_output_nodes, random),
                         newGenome2 = new NeuroNetwork(neuro_input_nodes, neurons_per_layer, neuro_output_nodes, random);

            object[] genome1_perceptrons = genome1.crossOverSplit(), genome2_perceptrons = genome2.crossOverSplit();

            newGenome1.setPerceptrons((Perceptron[])genome1_perceptrons[0], (Perceptron[])genome2_perceptrons[1]);
            newGenome2.setPerceptrons((Perceptron[])genome2_perceptrons[0], (Perceptron[])genome1_perceptrons[1]);

            return new NeuroNetwork[] { newGenome1, newGenome2 };
        }

        public void resetGenerations()
        {
            genomes = null;
            fitness = null;
        }

        public int getGeneration()
        {
            return current_generation;
        }

        public double[] executeGenome(int genomeId, double[] inputs)
        {
            return genomes[genomeId].elaborate(inputs);
        }

        public double[][] executeGeneration(double[] inputs)
        {
            double[][] result = new double[genomes_count][];
            for (int i = 0; i < genomes_count; i++)
            {
                result[i] = genomes[i].elaborate(inputs);
            }
            return result;
        }

        public void setFitness(double[] fitness_values)
        {
            fitness = fitness_values;
        }

    }
}
