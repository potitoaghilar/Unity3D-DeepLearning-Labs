using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GeneticAlgorithm;

public class Ctrl : MonoBehaviour {

    const float multiplier = .3f;
    public float speedFB = 0, speedLR = 0, headSpeed = 0;
    Rigidbody rigidbody;
    public GameObject eye_sensor;
    RaycastHit hit;
    Vector3 position, euler;

    public double time = 10;

    GeneticController gController;
    // 1.Distance 2.object_type
    public double[] inputs = new double[] { 0, 0 };
    public int currGenome = 0;
    int genomes = 12;
    double[] fitness;

    void Start() {
        gController = new GeneticController(genomes, inputs.Length, new int[] { 10, 20, 30 }, 3);
        fitness = new double[genomes];
        rigidbody = GetComponent<Rigidbody>();

        position = transform.position;
        euler = transform.eulerAngles;

        gController.createGeneration();
        Debug.Log("Generation: " + gController.getGeneration() + ", genome: 1/" + genomes);
    }

    void Update() {

        // Set inputs
        if (Physics.Raycast(eye_sensor.transform.position, transform.forward, out hit))
        {
            inputs[0] = hit.distance;
            if (hit.transform.tag == "point") inputs[1] = 1;
            else inputs[1] = 0;
        }

        double[] outputs = gController.executeGenome(currGenome, inputs);
        speedFB = (float)outputs[0];
        speedLR = (float)outputs[1];
        headSpeed = (float)outputs[2];

        // Apply motion
        rigidbody.AddForce(multiplier * speedFB * transform.forward, ForceMode.VelocityChange);
        rigidbody.AddForce(multiplier * speedLR * transform.right, ForceMode.VelocityChange);
        rigidbody.AddTorque(headSpeed * transform.up, ForceMode.VelocityChange);

        time -= Time.deltaTime;

        if (time <= 0)
        {
            if (currGenome < genomes - 1)
            {
                currGenome++;
                GameObject.Find("Point").transform.position = new Vector3(0, 1.146f, 2.16f);
                Debug.Log("Generation: " + gController.getGeneration() + ", genome: " + (currGenome + 1) + "/" + genomes);
            }
            else
            {
                gController.setFitness(fitness);
                currGenome = 0;
                fitness = new double[genomes];
                gController.createGeneration();
                Debug.Log("Generation: " + gController.getGeneration() + ", genome: 1/" + genomes);
            }
            transform.position = position;
            transform.eulerAngles = euler;
            time = 10;
        }
    }

    void onGUI()
    {
        if (GUI.Button(new Rect(10, 10, 200, 20), "Save genome")) {

        }
    }

    void OnTriggerEnter(Collider coll)
    {
        coll.transform.position = new Vector3(coll.transform.position.x, coll.transform.position.y, -coll.transform.position.z);
        fitness[currGenome] += 1;
    }
}
