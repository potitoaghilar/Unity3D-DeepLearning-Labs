using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GeneticAlgorithm;

public class Lab2 : MonoBehaviour {
    
    public GameObject agent, point;
    System.Random random = new System.Random();
    GameObject[] agents;

    GeneticController gController;
    public int[] brainStructure;
    public int genomes = 12, outputs;
    double[] fitness;

    void Start()
    {
        gController = new GeneticController(genomes, 2, brainStructure, outputs);
        agents = new GameObject[genomes];
        fitness = new double[genomes];

        setEnvironment(49, 49);

        gController.createGeneration();
        Debug.Log("Generation: " + gController.getGeneration());
    }

    void Update()
    {

        int actives = 0;
        foreach (GameObject a in agents)
            if (a.active)
                actives++;

        if (actives == 0)
        {
            for (int i = 0; i < agents.Length; i++)
            {
                fitness[i] = agents[i].GetComponent<Agent>().fitness;
            }
            gController.setFitness(fitness);
            fitness = new double[genomes];
            gController.createGeneration();
            setEnvironment(49, 49);
            Debug.Log("Generation: " + gController.getGeneration());
        }

    }

    void setEnvironment(float x, float y)
    {
        foreach (GameObject g in agents) Destroy(g);
        foreach (GameObject g in GameObject.FindGameObjectsWithTag("point")) Destroy(g);
        for (int i = 0; i < genomes; i++)
        {
            agents[i] = Instantiate(agent, new Vector3(x * (float)random.NextDouble() - x / 2, 1.2f, - 5), Quaternion.Euler(0, 360 * (float)random.NextDouble(), 0));
            agents[i].GetComponent<Agent>().gController = gController;
            agents[i].GetComponent<Agent>().genomeId = i;
        }
        for (int i = 0; i < genomes * 2; i++)
        {
            Instantiate(point, new Vector3(x * (float)random.NextDouble() - x / 2, 1.2f, y * (float)random.NextDouble() / 2), Quaternion.identity);
        }
    }
}
