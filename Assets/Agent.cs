using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GeneticAlgorithm;
using System.Threading;

public class Agent : MonoBehaviour {

    const float multiplier = .3f;
    public float back = 0, rotate = 0;
    Rigidbody rigidbody;
    public GameObject eye_sensor;
    public Material neuronMaterial;
    RaycastHit hit;

    double health = 20, startTime, points = 0;

    public int genomeId = -1;
    public GeneticController gController;
    public double[] inputs, outputs;
    public double fitness;

    public LayerMask lm;

    const int rays = 20;
    const float deg = 90;

    void Start() {
        Physics.IgnoreLayerCollision(8, 8, true);
        rigidbody = gameObject.AddComponent<Rigidbody>();
        rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
        startTime = Time.time;
        inputs = new double[rays * 2];
        outputs = new double[1];

        if (genomeId == 0) gController.printANetwork(genomeId, neuronMaterial);
    }

    void Update() {

        if (health <= 0)
        {
            gameObject.active = false;
            fitness = points + health / 10;
        }
        else
        {
            // Set inputs
            for (int i = 0; i < rays * 2; i += 2)
            {
                if (Physics.Raycast(eye_sensor.transform.position, Quaternion.Euler(0, -deg / 2 + i * (deg / rays) / 2, 0) * transform.forward, out hit, 15, lm))
                {
                    Color color = Color.red;

                    if (hit.transform.tag == "point")
                    {
                        inputs[i] = hit.distance / 15;
                        inputs[i + 1] = 0;
                        color = Color.green;
                    }
                    else if (hit.transform.tag == "wall")
                    {
                        inputs[i] = 0;
                        inputs[i + 1] = hit.distance / 15;
                    }


                    Debug.DrawLine(eye_sensor.transform.position, hit.point, color);
                }
                else
                {
                    inputs[i] = 0;
                    inputs[i + 1] = 0;
                }
            }

            outputs = gController.executeGenome(genomeId, inputs);
            rotate = (float)outputs[0];

            transform.position += 10 * transform.forward * Time.deltaTime;
            transform.Rotate(0, 10 * (rotate * 2 - 1), 0);

        }

        health -= Time.deltaTime;
    }

    void OnTriggerEnter(Collider coll)
    {
        Destroy(coll.gameObject);
        points += 1;
    }

    void OnCollisionEnter(Collision coll)
    {
        if (coll.gameObject.tag == "wall")
        {
            points -= 2;
            health -= 2;
        }
    }
}
