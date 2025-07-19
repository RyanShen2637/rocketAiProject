using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class demoHandler : MonoBehaviour
{
    [Header("God Variables")]
    public double timeScale = 1.0; // Default time scale
    float currentTimeScale = 1.0f; // Current time scale
    [Header("UI Elements")]
    public GameObject pausePanel;
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
    private float defaultGravityValue = 9.81f; // Default gravity value
    public TMP_InputField dragInputField;
    private float defaultDragValue = 0.001293f; // Default drag value
    public TMP_InputField massInputField;
    private float defaultMassValue = 549054f; // Default mass value
    public TMP_InputField thrustInputField;
    private float defaultThrustValue = 10000000f; // Default thrust value

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Time.timeScale = 1f; // Ensure the game is running at normal speed
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
        if (timeScale != currentTimeScale)
        {
            Time.timeScale = (float)timeScale;
            currentTimeScale = (float)timeScale;
        }

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

        // Toggle pause panel with Escape key
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (pausePanel.activeSelf)
            {
                pausePanel.SetActive(false);
                Time.timeScale = 1f; // Resume the game
            }
            else
            {
                pausePanel.SetActive(true);
                Time.timeScale = 0f; // Pause the game
            }
        }
    }

    // Scene management methods
    public void ReturnToTitle()
    {
        SceneManager.LoadScene("Title Scene");
    }
    public void QuitGame()
    {
        Application.Quit();
    }

    public void OnGravityChanged()
    {
        // Change the global gravity value
        if (gravityInputField.text == "")
        {
            Physics.gravity = new Vector3(0, -defaultGravityValue, 0);
            Debug.Log($"Gravity set to: {Physics.gravity}");
        }
        else if (float.TryParse(gravityInputField.text, out float gravityValue))
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
        if (dragInputField.text == "")
        {
            foreach (var env in environments)
            {
                var rockets = env.GetComponentsInChildren<rocket>();
                foreach (var rocket in rockets)
                {
                    var rb = rocket.GetComponent<Rigidbody>();
                    if (rb != null)
                    {
                        rb.linearDamping = defaultDragValue;
                        Debug.Log($"Drag set to: {defaultDragValue} for rocket in {env.name}");
                    }
                }
            }
        }
        else if (float.TryParse(dragInputField.text, out float dragValue)) {
            foreach (var env in environments)
            {
                var rockets = env.GetComponentsInChildren<rocket>();
                foreach (var rocket in rockets)
                {
                    var rb = rocket.GetComponent<Rigidbody>();
                    if (rb != null)
                    {
                        rb.linearDamping = dragValue;
                        Debug.Log($"Drag set to: {dragValue} for rocket in {env.name}");
                    }
                }
            }
        }
    }

    public void OnMassChanged()
    {
        // change the mass of all rocket rigidbodies (int)
        if (massInputField.text == "")
        {
            foreach (var env in environments)
            {
                var rockets = env.GetComponentsInChildren<rocket>();
                foreach (var rocket in rockets)
                {
                    var rb = rocket.GetComponent<Rigidbody>();
                    if (rb != null)
                    {
                        rb.mass = defaultMassValue;
                        Debug.Log($"Mass set to: {defaultMassValue} for rocket in {env.name}");
                    }
                }
            }
        }
        else if (int.TryParse(massInputField.text, out int massValue))
        {
            foreach (var env in environments)
            {
                var rockets = env.GetComponentsInChildren<rocket>();
                foreach (var rocket in rockets)
                {
                    var rb = rocket.GetComponent<Rigidbody>();
                    if (rb != null)
                    {
                        rb.mass = massValue;
                        Debug.Log($"Mass set to: {massValue} for rocket in {env.name}");
                    }
                }
            }
        }
    }

    public void OnThrustChanged()
    {
        // change the mainThrust of all rocket scripts
        if (thrustInputField.text == "")
        {
            foreach (var env in environments)
            {
                var rockets = env.GetComponentsInChildren<rocket>();
                foreach (var rocket in rockets)
                {
                    rocket.mainThrust = defaultThrustValue;
                    Debug.Log($"Thrust set to: {defaultThrustValue} for rocket in {env.name}");
                }
            }
        }
        else if (int.TryParse(thrustInputField.text, out int thrustValue))
        {
            foreach (var env in environments)
            {
                var rockets = env.GetComponentsInChildren<rocket>();
                foreach (var rocket in rockets)
                {
                    rocket.mainThrust = thrustValue;
                    Debug.Log($"Thrust set to: {thrustValue} for rocket in {env.name}");
                }
            }
        }
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
