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

    // Button Color Fields
    public Color selectedButtonColor = Color.green;
    public Color unselectedButtonColor = Color.white;
    int currentCameraIndex = 0;
    Transform currentRocket;

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
                SelectCamera(i);
            }
        }

        for (int i = 0; i < cameraButtons.Count; i++)
        {
            cameraButtons[i].colors = new ColorBlock
            {
                normalColor = unselectedButtonColor,
                highlightedColor = unselectedButtonColor,
                pressedColor = unselectedButtonColor,
                selectedColor = unselectedButtonColor,
                disabledColor = unselectedButtonColor,
                colorMultiplier = 1f
            };
        }

        cameraButtons[currentCameraIndex].colors = new ColorBlock
        {
            normalColor = selectedButtonColor,
            highlightedColor = unselectedButtonColor,
            pressedColor = selectedButtonColor,
            selectedColor = selectedButtonColor,
            disabledColor = unselectedButtonColor,
            colorMultiplier = 1f
        };
    }

    public void SelectCamera(int index)
    {
        currentRocket = environments[index].transform.Find("SpaceX - Falcon 9");
        currentCameraIndex = index;
        camera.target = currentRocket;
    }

    public void ToggleAIControl()
    {
        if (currentRocket != null)
        {
            var rocketControls = currentRocket.GetComponent<rocket>();
            if (aiControl != null)
            {
                rocketControls.ToggleAIControl();
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
