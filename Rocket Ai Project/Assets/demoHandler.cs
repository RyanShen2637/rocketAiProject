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

    [Header("Experiment Fields")]
    public TMP_InputField gravityInputField;
    public TMP_InputField dragInputField;
    public TMP_InputField massInputField;
    public TMP_InputField thrustInputField;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        slider.onValueChanged.AddListener(delegate { SpawnEnvironments((int)slider.value); });
        slider.value = 9;
        sliderValueText.text = $"Environments: {slider.value}";
        SelectCamera(currentCameraIndex);

        gravityInputField.onValueChanged.AddListener(delegate { OnGravityChanged(); });
        dragInputField.onValueChanged.AddListener(delegate { OnDragChanged(); });
        massInputField.onValueChanged.AddListener(delegate { OnMassChanged(); });
        thrustInputField.onValueChanged.AddListener(delegate { OnThrustChanged(); });
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

    public void OnGravityChanged()
    {
        // Change the global gravity value
        if (float.TryParse(gravityInputField.text, out float gravityValue))
        {
            Physics.gravity = new Vector3(0, -gravityValue, 0);
            Debug.Log($"Gravity set to: {Physics.gravity}");
        }
        else
        {
            Debug.LogError("Invalid gravity value entered.");
        }
    }

    public void OnDragChanged()
    {
        // change the drag of all rocket rigidbodies
    }

    public void OnMassChanged()
    {
        // change the mass of all rocket rigidbodies
    }

    public void OnThrustChanged()
    {
        // change the mainThrust of all rocket scripts
    }

    public void SelectCamera(int index)
    {
        currentRocket = environments[index].transform.Find("SpaceX - Falcon 9");
        currentCameraIndex = index;
        
        // AI Rocket 1 --> AI Rocket 2
        // - AI Rocket 1: (aiControlled = true, selectedCamera = true) --> (aiControlled = true, selectedCamera = false)
        // - AI Rocket 2: (aiControlled = true, selectedCamera = false) --> (aiControlled = true, selectedCamera = true)

        // Manual Rocket 1 --> Manual Rocket 2
        // - Manual Rocket 1: (aiControlled = false, selectedCamera = true) --> (aiControlled = true, selectedCamera = false)
        // - Manual Rocket 2: (aiControlled = true, selectedCamera = false) --> (aiControlled = false, selectedCamera = true)

        currentRocket.GetComponent<rocket>().aiControlled = camera.target.GetComponent<rocket>().aiControlled;
        camera.target.GetComponent<rocket>().aiControlled = true;

        camera.target.GetComponent<rocket>().selectedCamera = false;
        currentRocket.GetComponent<rocket>().selectedCamera = true;
        
        camera.target = currentRocket;
    }

    public void ToggleAIControl()
    {
        Debug.Log("Toggling AI Control: ", currentRocket);
        if (currentRocket != null)
        {
            var rocketControls = currentRocket.GetComponent<rocket>();
            rocketControls?.ToggleAIControl();
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
