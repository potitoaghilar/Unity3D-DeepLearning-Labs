using System;

namespace GeneticAlgorithm
{
    public class Perceptron
    {

        private double[] weights;
        private double bias, output;

        public Perceptron(sbyte[] weights, sbyte bias)
        {
            // Initialize perceptron with weights and bias
            this.weights = new double[weights.Length];
            for (int i = 0; i < weights.Length; i++)
                this.weights[i] = weights[i] / (double)128;
            this.bias = bias / (double)128;
        }

        // Main perceptron elaborator
        public void execute_perceptron(double[] input)
        {
            output = sigmoid(dot_product(input, weights) + bias);
        }

        public double getOutput()
        {
            return output;
        }

        // Dot product
        private double dot_product(double[] vector1, double[] vector2)
        {
            double result = 0;
            for (int i = 0; i < vector1.Length; i++)
            {
                result += vector1[i] * vector2[i];
            }
            return result;
        }

        // Activator functions
        private double sigmoid(double x)
        {
            return 1 / (1 + Math.Exp(-x));
        }
        private int binary(double x)
        {
            if (x >= 0) return 1;
            else return 0;
        }
        private double tanh(double x)
        {
            return Math.Tanh(x);
        }

    }
}
