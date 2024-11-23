using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class movement : Agent
{
    public float speed = 10;
    public GameObject target;
    public GameObject platform;
    public Material successMaterial;
    public Material failMaterial;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (this.transform.localPosition.y < platform.transform.localPosition.y) {
            platform.GetComponent<MeshRenderer>().material = failMaterial;
            SetReward(-1.0f);
            EndEpisode();
        }
    }

    public override void OnEpisodeBegin()
    {
        // reset our position
        this.transform.localPosition = new Vector3(Random.Range(-2.5f, 2.5f), 0, Random.Range(-2.5f, 2.5f));

        // move the target to a random position
        target.transform.localPosition = new Vector3(Random.Range(-2.5f, 2.5f), 0, Random.Range(-2.5f, 2.5f));
        
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // target and agent positions
        sensor.AddObservation(target.transform.localPosition);
        sensor.AddObservation(this.transform.localPosition);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        // move across the x and z axis
        float moveX = actions.ContinuousActions[0];
        float moveZ = actions.ContinuousActions[1];

        this.transform.localPosition += new Vector3(moveX, 0, moveZ) * Time.deltaTime * speed;
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var actions = actionsOut.ContinuousActions;
        actions[0] = Input.GetAxis("Horizontal");
        actions[1] = Input.GetAxis("Vertical");
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "target") {
            platform.GetComponent<MeshRenderer>().material = successMaterial;
            SetReward(1.0f);
            EndEpisode();
        }
    }


}
