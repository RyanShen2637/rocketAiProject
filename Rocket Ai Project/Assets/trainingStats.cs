using UnityEngine;
using TMPro;

public class trainingStats : MonoBehaviour
{
    public TextMeshProUGUI statsText;

    public int failures;
    public int successes;

    public int missCount;
    public int farCount;
    public int riseCount;
    public int crashCount;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        statsText.text = $"Successes: {successes}\n" +
            $"Failures: {failures}\n" +
            $"Total: {successes + failures}\n" +
            $"Accuracy: {(successes / ((successes > 0 || failures > 0) ?(successes + failures * 1.0f) : 1.0f)) * 100}%\n" +
            $"Crashes: {crashCount}\n" +
            $"Misses: {missCount}\n" +
            $"Fars: {farCount}\n" +
            $"Rises: {riseCount}\n";
    }
}
