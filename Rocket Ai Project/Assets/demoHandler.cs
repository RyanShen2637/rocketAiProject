using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

public class demoHandler : MonoBehaviour
{
    public List<GameObject> environments = new List<GameObject>();
    public Slider slider;
    public orbitalCamera camera;
    public TextMeshProUGUI sliderValueText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        slider.onValueChanged.AddListener(delegate { SpawnEnvironments((int)slider.value); });
    }

    // Update is called once per frame
    void Update()
    {
        // Detect pressing keys 1-9
        for (int i = 0; i < 9; i++)
        {
            if (Input.GetKeyDown((KeyCode)(49 + i)))
            {
                // Debug.Log("Key " + (i + 1) + " pressed");
                // find the rocket child in the selected environment
                Transform rocket = environments[i].transform.Find("SpaceX - Falcon 9");
                // Debug.Log("Rocket: " + rocket.name); // Debug log to check the name of the rocket
                // assign it to the target field of the camera
                camera.target = rocket;
            }
        }
    }

    public void SpawnEnvironments(int amount)
    {
        sliderValueText.text = $"Environments: {amount}";
        for (int i = 0; i < 9; i++)
        {
            if (i < amount)
            {
                environments[i].SetActive(true);
            }
            else
            {
                environments[i].SetActive(false);
            }
        }
    }
}
