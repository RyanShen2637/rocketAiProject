using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class rocket : Agent
{
    public float mainThrust = 10000f;
    public float sideThrust = 500f;
    public float rotationThrust = 500f;
    public float spinThrust = 500f;
    public Transform target;
    public Material successMaterial;
    public Material failMaterial;

    private Rigidbody rb;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void OnEpisodeBegin()
    {

    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.position); // 3
        sensor.AddObservation(transform.rotation); // 4
        sensor.AddObservation(target.position); // 3

    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        // discrete action space
        int mainThruster = actions.DiscreteActions[0];
        int translation = actions.DiscreteActions[1];
        int rotation = actions.DiscreteActions[2];
        int spin = actions.DiscreteActions[3];

        // control main thruster
        if (mainThruster == 1)
        {
            rb.AddRelativeForce(Vector3.up * mainThrust);
        }

        // control translation
        if (translation == 1) // north
        {
            rb.AddRelativeForce(Vector3.forward * sideThrust);
        } else if (translation == 2) // northeast
        {
            rb.AddRelativeForce(Vector3.forward * sideThrust);
            rb.AddRelativeForce(Vector3.right * sideThrust);
        } else if (translation == 3) // east
        {
            rb.AddRelativeForce(Vector3.right * sideThrust);
        } else if (translation == 4) // southeast
        {
            rb.AddRelativeForce(Vector3.back * sideThrust);
            rb.AddRelativeForce(Vector3.right * sideThrust);
        } else if (translation == 5) // south
        {
            rb.AddRelativeForce(Vector3.back * sideThrust);
        } else if (translation == 6) // southwest
        {
            rb.AddRelativeForce(Vector3.back * sideThrust);
            rb.AddRelativeForce(Vector3.left * sideThrust);
        } else if (translation == 7) // west
        {
            rb.AddRelativeForce(Vector3.left * sideThrust);
        } else if (translation == 8) // northwest
        {
            rb.AddRelativeForce(Vector3.forward * sideThrust);
            rb.AddRelativeForce(Vector3.left * sideThrust);
        }

        // control rotation
        if (rotation == 1) // tilt north
        {
            rb.AddRelativeTorque(Vector3.right * rotationThrust);
        } else if (rotation == 2) // tilt northeast
        {
            rb.AddRelativeTorque(Vector3.right * rotationThrust);
            rb.AddRelativeTorque(Vector3.forward * rotationThrust);
        } else if (rotation == 3) // tilt east
        {
            rb.AddRelativeTorque(Vector3.forward * rotationThrust);
        } else if (rotation == 4) // tilt southeast
        {
            rb.AddRelativeTorque(Vector3.forward * rotationThrust);
            rb.AddRelativeTorque(Vector3.left * rotationThrust);
        } else if (rotation == 5) // tilt south
        {
            rb.AddRelativeTorque(Vector3.left * rotationThrust);
        } else if (rotation == 6) // tilt southwest
        {
            rb.AddRelativeTorque(Vector3.left * rotationThrust);
            rb.AddRelativeTorque(Vector3.back * rotationThrust);
        } else if (rotation == 7) // tilt west
        {
            rb.AddRelativeTorque(Vector3.back * rotationThrust);
        } else if (rotation == 8) // tilt northwest
        {
            rb.AddRelativeTorque(Vector3.back * rotationThrust);
            rb.AddRelativeTorque(Vector3.right * rotationThrust);
        }

        // control spin
        if (spin == 1) // spin clockwise
        {
            rb.AddRelativeTorque(Vector3.up * spinThrust);
        } else if (spin == 2) // spin counterclockwise
        {
            rb.AddRelativeTorque(Vector3.down * spinThrust);
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var actions = actionsOut.DiscreteActions;

        // main thruster control with spacebar
        // ternary expression: condition ? true : false
        actions[0] = Input.GetKey(KeyCode.Space) ? 1 : 0;

        // translation control with WASD
        if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.D))
        {
            actions[1] = 2;
        } else if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.A))
        {
            actions[1] = 8;
        } else if (Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.A))
        {
            actions[1] = 6;
        } else if (Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.D))
        {
            actions[1] = 4;
        } else if (Input.GetKey(KeyCode.W))
        {
            actions[1] = 1;
        } else if (Input.GetKey(KeyCode.D))
        {
            actions[1] = 3;
        } else if (Input.GetKey(KeyCode.S))
        {
            actions[1] = 5;
        } else if (Input.GetKey(KeyCode.A))
        {
            actions[1] = 7;
        } else
        {
            actions[1] = 0;
        }

        // rotation control with arrow keys
        if (Input.GetKey(KeyCode.UpArrow) && Input.GetKey(KeyCode.RightArrow))
        {
            actions[2] = 2;
        } else if (Input.GetKey(KeyCode.UpArrow) && Input.GetKey(KeyCode.LeftArrow))
        {
            actions[2] = 8;
        } else if (Input.GetKey(KeyCode.DownArrow) && Input.GetKey(KeyCode.LeftArrow))
        {
            actions[2] = 6;
        } else if (Input.GetKey(KeyCode.DownArrow) && Input.GetKey(KeyCode.RightArrow))
        {
            actions[2] = 4;
        } else if (Input.GetKey(KeyCode.UpArrow))
        {
            actions[2] = 1;
        } else if (Input.GetKey(KeyCode.RightArrow))
        {
            actions[2] = 3;
        } else if (Input.GetKey(KeyCode.DownArrow))
        {
            actions[2] = 5;
        } else if (Input.GetKey(KeyCode.LeftArrow))
        {
            actions[2] = 7;
        } else
        {
            actions[2] = 0;
        }

        // spin control with Q and E
        if (Input.GetKey(KeyCode.Q))
        {
            actions[3] = 1;
        } else if (Input.GetKey(KeyCode.E))
        {
            actions[3] = 2;
        } else
        {
            actions[3] = 0;
        }
    }

    public void OnCollisionEnter(Collision collision)
    {
        
    }
}
