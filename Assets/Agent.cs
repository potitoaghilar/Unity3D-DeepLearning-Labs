using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GeneticAlgorithm;
using System.Threading;

public class Agent : MonoBehaviour {

    const float multiplier = .3f;
    public float speed = 0, rotate = 0;
    Rigidbody rigidbody;
    public GameObject eye_sensor;
    public Material neuronMaterial;
    RaycastHit hit;
    public sbyte[] genome;

    double health = 1, startTime, points = 0;

    public int genomeId = -1;
    public GeneticController gController;
    public double[] inputs, outputs;
    public double fitness;

    public LayerMask lm;

    const int rays = 20;
    const float deg = 120;

    void Start() {
        //Physics.IgnoreLayerCollision(8, 8, true);
        rigidbody = gameObject.AddComponent<Rigidbody>();
        rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
        startTime = Time.time;
        inputs = new double[rays * 3];
        outputs = new double[1];

        if (genomeId == 0) gController.printANetwork(genomeId, neuronMaterial);
    }

    void Update() {
        
        for (int i = 0; i < rays * 3; i += 3)
        {
            if (Physics.Raycast(eye_sensor.transform.position, Quaternion.Euler(0, -deg / 2 + i * (deg / rays) / 2, 0) * transform.forward, out hit, 15, lm))
            {
                Color color = Color.red;

                if (hit.transform.tag == "point")
                {
                    inputs[i] = double.Parse(hit.transform.name) / 2;
                    inputs[i + 1] = 0;
                    inputs[i + 2] = 0;
                    color = Color.green;
                }
                else if (hit.transform.tag == "wall")
                {
                    inputs[i] = 0;
                    inputs[i + 1] = hit.distance / 15;
                    inputs[i + 2] = 0;
                }
                else if (hit.transform.tag == "agent")
                {
                    inputs[i] = 0;
                    inputs[i + 1] = 0;
                    inputs[i + 2] = hit.distance / 50;
                    color = Color.blue;
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
        speed = (float)outputs[0];
        rotate = (float)outputs[1];

        transform.position += speed * 10 * transform.forward * Time.deltaTime;
        transform.Rotate(0, 50 * (rotate - .5f), 0);
    }

    void OnTriggerEnter(Collider coll)
    {
        points += double.Parse(coll.gameObject.name);
        Destroy(coll.gameObject);
    }

    void OnCollisionEnter(Collision coll)
    {
        if (coll.gameObject.tag == "wall")
        {
            health -= .2;
        }
    }

    public void end()
    {
        fitness = 5 * points + 5 * health;
    }
}
