using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NeuralNetwork
{
    // Considering the output layer as the result of last layer of HLs
    public class NeuroNetwork
    {
        
        // Network params
        private int input_nodes, output_nodes, hidden_layers_count;
        private int[] neurons_per_layer;
        Random random;

        // Network structure as array of perceptrons
        private Perceptron[] perceptrons;

        public NeuroNetwork(int input_nodes, int[] neurons_per_layer, int output_nodes, Random random, Perceptron[] perceptrons = null)
        {
            // Set parameters of network
            this.input_nodes = input_nodes;
            this.output_nodes = output_nodes;
            this.hidden_layers_count = neurons_per_layer.Length + 1; // Adds 1 becouse of output layer
            this.neurons_per_layer = neurons_per_layer.Concat(new int[] { output_nodes }).ToArray(); // Concat the out layer to hidden layer
            this.random = random;

            // Create network structure
            createNetwork(perceptrons);
        }

        private void createNetwork(Perceptron[] ps)
        {
            if (perceptrons == null)
            {
                perceptrons = new Perceptron[neurons_per_layer.Sum()];
                int p_index = 0;
                for (int i = 0; i < hidden_layers_count; i++)
                {
                    // Check if is last layer (output layer)
                    bool isOutputLayer = false;
                    if (i == hidden_layers_count - 1) isOutputLayer = true;

                    for (int p = 0; p < neurons_per_layer[i]; p++)
                    {
                        int input_nodes;
                        if (i == 0) input_nodes = this.input_nodes;
                        else input_nodes = neurons_per_layer[i - 1];
                        perceptrons[p_index++] = new Perceptron(input_nodes, random, isOutputLayer);
                    }
                }
            }
            else
            {
                perceptrons = ps;
            }
        }

        // Elaborate input signals through NeuralNetwork
        public double[] elaborate(double[] datas)
        {

            // Elaborate datas within Hidden Layers
            int p_index = 0;
            for (int i = 0; i < hidden_layers_count; i++)
            {
                for (int o = 0; o < neurons_per_layer[i]; o++)
                {
                    double[] inputs;
                    if (i == 0) inputs = datas;
                    else inputs = getPerceptronsOutputs(i - 1);
                    perceptrons[p_index++].execute_perceptron(inputs);
                }
            }

            // Return result elaborated from NeuralNetwork
            double[] result = new double[output_nodes];
            for (int i = output_nodes - 1; i >= 0; i--)
            {
                result[i] = perceptrons[p_index - i - 1].getOutput();
            }
            return result.Reverse().ToArray();
        }

        // Get perceptions outputs from prevoius layer
        private double[] getPerceptronsOutputs(int layer_id)
        {
            double[] outputs = new double[neurons_per_layer[layer_id]];
            int p_index = 0;
            for (int i = 0; i < layer_id; i++)
            {
                p_index += neurons_per_layer[i];
            }
            for (int i = 0; i < neurons_per_layer[layer_id]; i++)
            {
                outputs[i] = perceptrons[p_index++].getOutput();
            }
            return outputs;
        }

        // Set weights and biases of network
        public void setPerceptrons(Perceptron[] perceptrons)
        {
            for (int i = 0; i < perceptrons.Length; i++)
                this.perceptrons[i] = perceptrons[i];
        }
        public void setPerceptrons(Perceptron[] perceptrons1, Perceptron[] perceptrons2)
        {
            for (int i = 0; i < perceptrons1.Length; i++)
                perceptrons[i] = perceptrons1[i];

            for (int i = perceptrons1.Length; i < perceptrons2.Length; i++)
                perceptrons[i] = perceptrons2[i];
        }

        // Get weights and biases of network
        public Perceptron[] getPerceptrons()
        {
            return perceptrons;
        }

        // Split genome to cross-over in genetic controller
        public object[] crossOverSplit()
        {
            int p1_count = (int)Math.Ceiling(perceptrons.Length / 2.0), p2_count = perceptrons.Length - p1_count;
            Perceptron[] p1 = new Perceptron[p1_count], p2 = new Perceptron[p2_count];
            for (int i = 0; i < p1_count; i++)
                p1[i] = perceptrons[i];
            for (int i = p1_count; i < p2_count; i++)
                p2[i] = perceptrons[i];
            return new object[] { p1, p2 };
        }

        // Apply mutations to perceptrons - changing weights and biases
        public void applyMutations(Random random)
        {
            for (int i = 0; i < perceptrons.Length; i++)
            {
                perceptrons[i].mutate(random);
            }
        }

    }
}
