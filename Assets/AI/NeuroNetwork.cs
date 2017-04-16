using System.Linq;
using UnityEngine;

namespace GeneticAlgorithm
{
    // Considering the output layer as the result of last layer of HLs
    public class NeuroNetwork : MonoBehaviour
    {
        
        // Network params
        private int input_nodes, output_nodes, hidden_layers_count;
        private int[] neurons_per_layer;

        // Network structure as array of perceptrons
        private Perceptron[] perceptrons;
        public double[] n_values;

        public NeuroNetwork(int input_nodes, int[] neurons_per_layer, int output_nodes, sbyte[] genome)
        {
            // Set parameters of network
            this.input_nodes = input_nodes;
            this.output_nodes = output_nodes;
            this.hidden_layers_count = neurons_per_layer.Length + 1; // Adds 1 because of output layer
            this.neurons_per_layer = neurons_per_layer.Concat(new int[] { output_nodes }).ToArray(); // Concat the out layer to hidden layer

            // Create network structure
            createNetwork(genome);
        }

        private void createNetwork(sbyte[] genome)
        {
            perceptrons = new Perceptron[neurons_per_layer.Sum()];
            n_values = new double[input_nodes + neurons_per_layer.Sum()];

            // Indexes
            int currLayer = 0, curr_perceptron_all_layers = 0, curr_perceptron_this_layer = 0;

            for (int i = 0; i < genome.Length;)
            {
                if (curr_perceptron_this_layer > neurons_per_layer[currLayer] - 1)
                {
                    curr_perceptron_this_layer = 0;
                    currLayer++;
                }

                int input_length;
                if (currLayer == 0) input_length = input_nodes;
                else input_length = neurons_per_layer[currLayer - 1];
                
                // Set perceptron weights and bias
                sbyte[] weights = new sbyte[input_length];
                
                for (int w = 0; w < weights.Length; w++)
                    weights[w] = genome[i++];
                sbyte bias = genome[i++];
                /*if (currLayer == hidden_layers_count - 1) bias = 0;*/
                perceptrons[curr_perceptron_all_layers] = new Perceptron(weights, bias);

                curr_perceptron_this_layer++;
                curr_perceptron_all_layers++;
            }
        }

        // Elaborate input signals through NeuralNetwork
        public double[] elaborate(double[] datas)
        {

            if (showNet)
            {
                for (int a = 0; a < datas.Length; a++)
                {
                    inputs_n[a].GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", new Color(0, 0, Mathf.Abs((float)(datas[a] < .1 ? .1f : datas[a]))));
                    n_values[a] = double.Parse(datas[a].ToString("F2"));
                    for (int j = 0; j < neurons_per_layer[0]; j++)
                    {
                        if (Mathf.Abs((float)datas[a]) > .1 && perceptrons[j].getWeight(a) < 0)
                            Debug.DrawLine(inputs_n[a].transform.position, neurons[j].transform.position, new Color(Mathf.Abs((float)perceptrons[j].getOutput()), 0, 0));
                        else if (Mathf.Abs((float)datas[a]) > .1 && perceptrons[j].getWeight(a) > 0)
                            Debug.DrawLine(inputs_n[a].transform.position, neurons[j].transform.position, new Color(0, Mathf.Abs((float)perceptrons[j].getOutput()), 0));
                    }
                }
            }

            // Elaborate datas within Hidden Layers
            int p_index = 0;
            for (int i = 0; i < hidden_layers_count; i++)
            {
                for (int o = 0; o < neurons_per_layer[i]; o++)
                {
                    double[] inputs;
                    if (i == 0) inputs = datas;
                    else inputs = getPerceptronsOutputs(i - 1);
                    perceptrons[p_index].execute_perceptron(inputs);

                    if (showNet)
                    {
                        neurons[p_index].GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", new Color(0, 0, (float)(Mathf.Abs((float)perceptrons[p_index].getOutput()) < .1 ? .1f : Mathf.Abs((float)perceptrons[p_index].getOutput()))));
                        n_values[datas.Length + p_index] = double.Parse(perceptrons[p_index].getOutput().ToString("F2"));
                        if (i > 0)
                            for (int j = 0; j < neurons_per_layer[i - 1]; j++)
                            {
                                if(Mathf.Abs((float)perceptrons[p_index].getOutput()) > .1 && perceptrons[p_index].getWeight(j) < 0) 
                                    Debug.DrawLine(neurons[p_index].transform.position, neurons[p_index - o - neurons_per_layer[i - 1] + j].transform.position, new Color(Mathf.Abs((float)perceptrons[p_index].getOutput()), 0, 0));
                                else if(Mathf.Abs((float)perceptrons[p_index].getOutput()) > .1 && perceptrons[p_index].getWeight(j) > 0)
                                    Debug.DrawLine(neurons[p_index].transform.position, neurons[p_index - o - neurons_per_layer[i - 1] + j].transform.position, new Color(0, Mathf.Abs((float)perceptrons[p_index].getOutput()), 0));
                            }
                    }

                    p_index++;
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

        // Static function for fast calculation of a genome length
        public static int calculateGenomeLength(int input_length, int[] neurons_per_layer, int output_legth)
        {
            int genome_length = 0;
            for (int i = 0; i < neurons_per_layer.Length; i++)
            {
                if (i == 0) genome_length += neurons_per_layer[i] * (input_length + 1);
                else genome_length += neurons_per_layer[i] * (neurons_per_layer[i - 1] + 1);
            }
            genome_length += output_legth * (neurons_per_layer[neurons_per_layer.Length - 1] + 1);
            return genome_length;
        }

        GameObject[] inputs_n, neurons;
        bool showNet = false;
        public void print(Material neuronMaterial)
        {
            inputs_n = new GameObject[input_nodes];
            for (int i = 0; i < input_nodes; i++)
            {
                inputs_n[i] = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                inputs_n[i].transform.position = new Vector3(-20, 50, ((input_nodes / 2 - i) * 3));
                inputs_n[i].GetComponent<MeshRenderer>().material = neuronMaterial;
                inputs_n[i].tag = "neuron";
                inputs_n[i].name = i.ToString();
            }

            neurons = new GameObject[neurons_per_layer.Sum()];
            int n = 0;
            for (int l = 0; l < neurons_per_layer.Length; l++)
            {
                for (int nn = 0; nn < neurons_per_layer[l]; nn++)
                {
                    neurons[n] = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    neurons[n].transform.position = new Vector3(l * 20, 50, ((neurons_per_layer[l] / 2 - nn) * 10));
                    neurons[n].GetComponent<MeshRenderer>().material = neuronMaterial;
                    neurons[n].tag = "neuron";
                    neurons[n].name = (n + input_nodes).ToString();
                    n++;
                }
            }

            showNet = true;
        }
    }
}
