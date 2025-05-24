using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

public class demoHandler : MonoBehaviour
{
    public List<GameObject> environments = new List<GameObject>();
    public List<Button> cameraButtons = new List<Button>();
    public Slider slider;
    public orbitalCamera camera;
    public TextMeshProUGUI sliderValueText;
    int currentCameraIndex = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        slider.onValueChanged.AddListener(delegate { SpawnEnvironments((int)slider.value); });
        slider.value = 9;
        sliderValueText.text = $"Environments: {slider.value}";
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
                currentCameraIndex = i;

                // // Set the button corresponding to the index to be selected
                // for (int j = 0; j < cameraButtons.Count; j++)
                // {
                //     if (i == j){
                //         cameraButtons[j].Select();
                //     }
                //     else
                //     {
                //         cameraButtons[j].OnDeselect(null);
                //     }
                // }


                // Debug.Log("Rocket: " + rocket.name); // Debug log to check the name of the rocket
                // assign it to the target field of the camera
                camera.target = rocket;
            }
        }

        cameraButtons[currentCameraIndex].Select();
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
