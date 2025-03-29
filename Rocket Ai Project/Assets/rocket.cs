using System;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class rocket : Agent
{
    public trainingStats stats;
    public float mainThrust = 10000f;
    public float sideThrust = 500f;
    public float rotationThrust = 500f;
    public float spinThrust = 500f;
    public Transform target;
    public Material successMaterial;
    public Material failMaterial;

    private Rigidbody rb;

    private float startTime;
    private float startHeight;
    private float targetVelocity = 0.5f;
    private float lastHeight = 500f;
    private float lowestHeight = 500f;
    private float lastDistance = 1000f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        //Set fail material for failed run
        // if (this.transform.localPosition.y < target.transform.localPosition.y){
        //     target.GetComponent<MeshRenderer>().material = failMaterial;
        //     EndEpisode();
        // }
    }

    void FixedUpdate()
    {
        // Check if the rocket missed the target
        if (this.transform.localPosition.y < target.transform.localPosition.y - 10f)
        {
            stats.missCount++;
            Fail();
        }

        // Check if the rocket is too far from the target
        if (Vector3.Distance(this.transform.localPosition, target.localPosition) > 1000f)
        {
            stats.farCount++;
            Fail(-1.5f);
        }

        // Check if the rocket is rising
        if (this.transform.localPosition.y > lastHeight + 5f)
        {
            stats.riseCount++;
            Fail(-7f);
            // AddReward(-0.02f);
        }
        lastHeight = this.transform.localPosition.y;

        // Check if the rocket is getting closer to the target
        if (Vector3.Distance(this.transform.localPosition, target.localPosition) >= lastDistance)
        {
            AddReward(-0.02f);
        }
        lastDistance = Vector3.Distance(this.transform.localPosition, target.localPosition);

        // If the rocket rises too much, fail
        // if (this.transform.localPosition.y > lowestHeight + 50f)
        // {
        //     stats.riseCount++;
        //     Fail(-2f);
        // }
        // lowestHeight = Mathf.Min(lowestHeight, this.transform.localPosition.y);
    }

    void Fail(float punishment = -1f)
    {
        if (GetCumulativeReward() > 0f) {
            AddReward(-1f * GetCumulativeReward());
        }

        AddReward(punishment);

        Debug.Log($"Total punishment: {GetCumulativeReward()}");
        target.GetComponent<MeshRenderer>().material = failMaterial;

        if (stats != null)
        {
            stats.failures++;
        }

        EndEpisode();
    }

    void Success()
    {
        if (GetCumulativeReward() < 0f) {
            AddReward(-1f * GetCumulativeReward());
        }

        float time = Time.time - startTime;
        Debug.Log($"Landed in {time} seconds");

        if (time < 0.5f) {
            time = 0.5f;
        }

        float additionalReward = (startHeight * 0.1f) / time;

        if (additionalReward > 100f)
        {
            additionalReward = 100f;
        }

        // Formula: (40 * (x^-0.5)) - 5; and then clamped between 0 and 10
        float bullseyeCoefficient = Mathf.Clamp(40.0f * ((float)Math.Pow(Vector3.Distance(this.transform.localPosition, target.localPosition), -0.5f)) - 5f, 0f, 10f);

        Debug.Log($"Distance from center: {Vector3.Distance(this.transform.localPosition, target.localPosition)}");
        Debug.Log($"Bullseye coefficient: {bullseyeCoefficient}");

        Debug.Log($"Adding reward: 10 + {additionalReward} + {bullseyeCoefficient}");

        AddReward(10f + additionalReward + bullseyeCoefficient);

        // Print the total reward accumulated
        Debug.Log($"Total reward: {GetCumulativeReward()}");

        target.GetComponent<MeshRenderer>().material = successMaterial;

        if (stats != null)
        {
            stats.successes++;
        }

        lastHeight = 1000f;

        EndEpisode();
    }

    public override void OnEpisodeBegin()
    {
        startHeight = UnityEngine.Random.Range(300f, 700f);
        lowestHeight = startHeight;
        lastHeight = startHeight;
        lastDistance = Vector3.Distance(this.transform.localPosition, target.localPosition);
        //Reset rocket localPosition
        this.transform.localPosition = new Vector3(0, startHeight, 0);

        // Reset rocket velocity
        rb.linearVelocity = new Vector3(0, 0, 0);

        // Reset rocket angular velocity
        rb.angularVelocity = new Vector3(0, 0, 0);

        // Reset rocket rotation
        this.transform.localRotation = Quaternion.Euler(0, 0, 0);

        // Reset target localPosition
        target.localPosition = new Vector3(UnityEngine.Random.Range(-150f, 150f), 0, UnityEngine.Random.Range(-150f, 150f));

        // Set the start time
        startTime = Time.time;

    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition); // 3
        sensor.AddObservation(transform.localRotation); // 4
        sensor.AddObservation(target.transform.localPosition); // 3
        sensor.AddObservation(targetVelocity); // 1
        sensor.AddObservation(rb.linearVelocity.magnitude); // 1

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


        // Translation Validation Layer
        float x_distance = transform.localPosition.x - target.transform.localPosition.x;
        float z_distance = transform.localPosition.z - target.transform.localPosition.z;

        float x_velocity = rb.linearVelocity.x;
        float z_velocity = rb.linearVelocity.z;

        // translation 1 = W pressed; z goes up
        // translation 2 = W+D pressed; z goes up and x goes up
        // translation 3 = D pressed; x goes up
        // translation 4 = S+D pressed; z goes down and x goes up
        // translation 5 = S pressed; z goes down
        // translation 6 = S+A pressed; z goes down and x goes down
        // translation 7 = A pressed; x goes down
        // translation 8 = W+A pressed; z goes up and x goes down
        
        // bool useDiagonals = true;
        // if (x_distance < 10 || z_distance < 10) {
        //     useDiagonals = false;
        // }

        char x_thruster = ' ';
        char z_thruster = ' ';

        if (transform.localPosition.x > target.transform.localPosition.x) {
            // objectively, A needs to be pressed to bring x_distance down
            x_thruster = 'A';

            if (x_velocity < -10) {
                x_thruster = 'D';
            }
        } else {
            // objectively, D needs to be pressed to bring x_distance up
            x_thruster = 'D';

            if (x_velocity > 10) {
                x_thruster = 'A';
            }
        }

        if (transform.localPosition.z > target.transform.localPosition.z) {
            // objectively, S needs to be pressed to bring z_distance down
            z_thruster = 'S';

            if (z_velocity < -10) {
                z_thruster = 'W';
            }
        } else {
            // objectively, W needs to be pressed to bring z_distance up
            z_thruster = 'W';

            if (z_velocity > 10) {
                z_thruster = 'S';
            }
        }

        if (x_thruster == 'A' && z_thruster == 'W') {
            // translation = 8; // northwest
        } else if (x_thruster == 'D' && z_thruster == 'W') {
            // translation = 2; // northeast
        } else if (x_thruster == 'D' && z_thruster == 'S') {
            // translation = 4; // southeast
        } else if (x_thruster == 'A' && z_thruster == 'S') {
            // translation = 6; // southwest
        }

        if (translation == 8) {
            if (Mathf.Abs(x_distance) < 10 && Mathf.Abs(x_velocity) < 10) {
                if (translation == 8 || translation == 2) {
                    // translation = 1; // north
                } else if (translation == 6 || translation == 4) {
                    // translation = 5; // south
                }
            } else if (Mathf.Abs(z_distance) < 10 && Mathf.Abs(z_velocity) < 10) {
                if (translation == 8 || translation == 6) {
                    //translation = 7; // west
                } else if (translation == 2 || translation == 4) {
                    //translation = 3; // east
                }
            }


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

        // Vertical Validation Layer
        float y_distance = transform.localPosition.y - target.transform.localPosition.y;
        float y_velocity = rb.linearVelocity.y;

        float horizontal_magnitude = Mathf.Sqrt(Mathf.Pow(transform.localPosition.x - target.transform.localPosition.x, 2) + Mathf.Pow(transform.localPosition.z - target.transform.localPosition.z, 2));
        float distance_coefficient = 1/75f * horizontal_magnitude + 1;
        // Debug.Log($"{Mathf.Pow(y_distance, 2)} becomes {1/2000f * Mathf.Pow(y_distance, 2)}");
        float current_falling_limit = (1/distance_coefficient) * 1/600f * Mathf.Pow(y_distance, 2) + targetVelocity;
        // Debug.Log($"Current falling limit: {-current_falling_limit}, Current y velocity: {y_velocity}, Current y distance: {y_distance}");
        if (rb.linearVelocity.y >= 0) {
            actions[0] = 0;
        } else {
            if (y_velocity < -current_falling_limit) {
                actions[0] = 1;
            }
        }

        // Translation Validation Layer
        float x_distance = transform.localPosition.x - target.transform.localPosition.x;
        float z_distance = transform.localPosition.z - target.transform.localPosition.z;

        float x_velocity = rb.linearVelocity.x;
        float z_velocity = rb.linearVelocity.z;

        // translation 1 = W pressed; z goes up
        // translation 2 = W+D pressed; z goes up and x goes up
        // translation 3 = D pressed; x goes up
        // translation 4 = S+D pressed; z goes down and x goes up
        // translation 5 = S pressed; z goes down
        // translation 6 = S+A pressed; z goes down and x goes down
        // translation 7 = A pressed; x goes down
        // translation 8 = W+A pressed; z goes up and x goes down
        
        // bool useDiagonals = true;
        // if (x_distance < 10 || z_distance < 10) {
        //     useDiagonals = false;
        // }

        char x_thruster = ' ';
        char z_thruster = ' ';

        if (transform.localPosition.x > target.transform.localPosition.x) {
            // objectively, A needs to be pressed to bring x_distance down
            x_thruster = 'A';

            if (x_velocity < ((Mathf.Abs(x_distance) > 25f) ? -10 : -1)) {
                x_thruster = 'D';
            }
        } else {
            // objectively, D needs to be pressed to bring x_distance up
            x_thruster = 'D';

            if (x_velocity > ((Mathf.Abs(x_distance) > 25f) ? 10 : 1)) {
                x_thruster = 'A';
            }
        }

        if (transform.localPosition.z > target.transform.localPosition.z) {
            // objectively, S needs to be pressed to bring z_distance down
            z_thruster = 'S';

            if (z_velocity < ((Mathf.Abs(x_distance) > 25f) ? -10 : -1)) {
                z_thruster = 'W';
            }
        } else {
            // objectively, W needs to be pressed to bring z_distance up
            z_thruster = 'W';

            if (z_velocity > ((Mathf.Abs(z_distance) > 25f) ? 10 : 1)) {
                z_thruster = 'S';
            }
        }

        if (x_thruster == 'A' && z_thruster == 'W') {
            actions[1] = 8; // northwest
        } else if (x_thruster == 'D' && z_thruster == 'W') {
            actions[1] = 2; // northeast
        } else if (x_thruster == 'D' && z_thruster == 'S') {
            actions[1] = 4; // southeast
        } else if (x_thruster == 'A' && z_thruster == 'S') {
            actions[1] = 6; // southwest
        }

        if (actions[1] == 8) {
            if (Mathf.Abs(x_distance) < 10 && Mathf.Abs(x_velocity) < 10) {
                if (actions[1] == 8 || actions[1] == 2) {
                    actions[1] = 1; // north
                } else if (actions[1] == 6 || actions[1] == 4) {
                    actions[1] = 5; // south
                }
            } else if (Mathf.Abs(z_distance) < 10 && Mathf.Abs(z_velocity) < 10) {
                if (actions[1] == 8 || actions[1] == 6) {
                    actions[1] = 7; // west
                } else if (actions[1] == 2 || actions[1] == 4) {
                    actions[1] = 3; // east
                }
            }


        }
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "target"){

            // Check the speed of the rocket or if the rocket isn't upright
            Debug.Log($"Speed: {rb.linearVelocity.y}");

            if (rb.linearVelocity.y > targetVelocity && stats != null) {
                stats.crashCount++;
            }

            if (rb.linearVelocity.y > targetVelocity || Mathf.Abs(this.transform.localRotation.z) > 2f)
            {
                Fail();
            }
            else
            {
                Success();
            }
        }
    }
}
