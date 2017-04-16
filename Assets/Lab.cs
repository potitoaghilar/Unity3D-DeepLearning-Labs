using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GeneticAlgorithm;
using UnityEditor;
using System.IO;
using System.Linq;

public class Lab : MonoBehaviour {
    
    public GameObject agent, point;
    System.Random random = new System.Random();
    GameObject[] agents;

    public GeneticController gController;
    public int[] brainStructure;
    public int genomes = 12, outputs;
    double[] fitness;
    double time = 20;

    public double bestFitness = 0, mutationProb = 0;

    void Start()
    {
        gController = new GeneticController(genomes, 60, brainStructure, outputs);
        agents = new GameObject[genomes];
        fitness = new double[genomes];
        gController.createGeneration();
        setEnvironment(49, 49);
        Debug.Log("Generation: " + gController.getGeneration());
    }

    void Update()
    {

        /*int actives = 0;
        foreach (GameObject a in agents)
            if (a.active)
                actives++;*/

        if (time <= 0)
        {
            time = 20;

            foreach (GameObject a in agents)
                a.GetComponent<Agent>().end();

            for (int i = 0; i < agents.Length; i++)
            {
                fitness[i] = agents[i].GetComponent<Agent>().fitness;
            }
            gController.setFitness(fitness);
            Debug.ClearDeveloperConsole();
            Debug.Log("Generation fitness: \n");
            foreach (double f in fitness)
            {
                Debug.Log(f + "\n");
            }
            bestFitness = fitness.Max();
            fitness = new double[genomes];
            gController.createGeneration();
            mutationProb = gController.mutationProb;
            setEnvironment(49, 49);
            Debug.Log("Generation: " + gController.getGeneration());
        }
        
        GameObject[] pts = GameObject.FindGameObjectsWithTag("point");
        for (int i = 0; i < 10 - pts.Length; i++)
        {
            float x = 50 * (float)random.NextDouble() - 50 / 2, y = 50 * (float)random.NextDouble() - 50 / 2;
            float minDistance = 100000;
            for (int o = 0; o < agents.Length; o++)
            {
                float dist = Vector3.Distance(new Vector3(x, 1.5f, y), agents[o].transform.position);
                if (dist < minDistance) minDistance = dist;
            }
            if (minDistance > 10)
            {
                GameObject p = Instantiate(point, new Vector3(x, 1.5f, y), Quaternion.identity);
                float dist = Vector3.Distance(Vector3.zero, p.transform.position);
                dist = Mathf.Pow(2, dist / 50);
                if (dist < 1) dist = 1;
                p.name = dist.ToString();
            }
        }

        time -= Time.deltaTime;

    }

    void setEnvironment(float x, float y)
    {
        foreach (GameObject g in agents) Destroy(g);
        foreach (GameObject g in GameObject.FindGameObjectsWithTag("point")) Destroy(g);
        foreach (GameObject g in GameObject.FindGameObjectsWithTag("neuron")) Destroy(g);
        for (int i = 0; i < genomes; i++)
        {
            //agents[i] = Instantiate(agent, new Vector3(0, 1.2f, 0), Quaternion.identity);
            agents[i] = Instantiate(agent, new Vector3(x * (float)random.NextDouble() - x / 2, 1.2f, y * (float)random.NextDouble() - y / 2), Quaternion.identity);
            agents[i].GetComponent<Agent>().gController = gController;
            agents[i].GetComponent<Agent>().genomeId = i;
            agents[i].GetComponent<Agent>().genome = gController.getGenome(i);
        }
        /*for (int i = 0; i < genomes; i++)
        {
            Instantiate(point, new Vector3(x * (float)random.NextDouble() - x / 2, 1.5f, y * (float)random.NextDouble() - y / 2), Quaternion.identity);
        }*/
    }

    /*void OnGUI()
    {
        if (GUI.Button(new Rect(10, 10, 200, 25), "Save generation"))
        {
            StreamWriter sw = new StreamWriter("Assets/generation.dat");
            sw.Write(gController.saveGeneration());
            sw.Close();
            AssetDatabase.Refresh();
        }
        if (GUI.Button(new Rect(10, 45, 200, 25), "Load generation"))
        {
            StreamReader sr = new StreamReader("Assets/generation.dat");
            gController.loadGeneration(sr.ReadToEnd());
            sr.Close();
        }
    }*/
}
