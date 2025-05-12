using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections;
using System.Text;

public class ChatGPTColorFetcher : MonoBehaviour
{
    public string openAIKey = "";
        //;
    public InputField moodInput;
    public Button sendButton;
    public GameObject ball;
    public Text responseText;

    private Renderer ballRenderer;

    public GameObject moodBallPrefab;
    private GameObject currentMoodBall;

    public GameObject[] obstaclePrefabs;

    void Start()
    {
        sendButton.onClick.AddListener(OnSendMood);
        ballRenderer = ball.GetComponent<Renderer>();
    }

    void OnSendMood()
    {
        string mood = moodInput.text;
        currentMoodBall = CreateMoodBall(); // Step 1: Create the ball first
        StartCoroutine(SendMoodToChatGPT(mood));
    }

    IEnumerator SendMoodToChatGPT(string mood)
    {
        string prompt = $"What color best represents the mood '{mood}'? Just return the RGB values in this format: R,G,B (for example: 255,100,50). No extra text.";
        string jsonData = "{\"model\": \"gpt-3.5-turbo\", \"messages\": [{\"role\": \"user\", \"content\": \"" + prompt + "\"}]}";

        UnityWebRequest request = new UnityWebRequest("https://api.openai.com/v1/chat/completions", "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();

        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", "Bearer " + openAIKey);
        Debug.Log("Full API response:\n" + request.downloadHandler.text);


        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string result = request.downloadHandler.text;
            string colorName = ParseColorFromResponse(result);

            Debug.Log("Full API response:\n" + result);
            Debug.Log("ChatGPT returned color: " + colorName);

            responseText.text = "Color: " + colorName;
            ApplyColorToBall(colorName);
        }
        else
        {
            Debug.Log("Error: " + request.error);
            responseText.text = "API Error: " + request.error;
        }
    }

    string ParseColorFromResponse(string json)
    {
        int contentIndex = json.IndexOf("\"content\":");
        if (contentIndex == -1) return "255,255,255";

        int start = json.IndexOf("\"", contentIndex + 10) + 1;
        int end = json.IndexOf("\"", start);

        if (start == -1 || end == -1) return "255,255,255";

        string rgbString = json.Substring(start, end - start);
        return rgbString.Trim();
    }
    GameObject CreateMoodBall()
    {
        Vector2 spawnPos = new Vector2(Random.Range(-7f, 7f), 4f); // spawn above screen
        GameObject newBall = Instantiate(moodBallPrefab, spawnPos, Quaternion.identity);
        SpawnRandomObstacle();
        return newBall;
    }
    void ApplyColorToMoodBall(GameObject moodBall, Color color)
    {
        SpriteRenderer renderer = moodBall.GetComponent<SpriteRenderer>();
        if (renderer != null)
        {
            renderer.color = color;
        }

        Rigidbody2D rb = moodBall.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            PhysicsMaterial2D bounceMat = new PhysicsMaterial2D
            {
                bounciness = 1f,
                friction = 0f
            };
            rb.sharedMaterial = bounceMat;
        }

        Debug.Log($"Applied color to mood ball: {color}");
    }

    void ApplyColorToBall(string rgbString)
    {
        string[] parts = rgbString.Split(',');

        if (parts.Length == 3 &&
            int.TryParse(parts[0], out int r) &&
            int.TryParse(parts[1], out int g) &&
            int.TryParse(parts[2], out int b))
        {
            Color newColor = new Color(r / 255f, g / 255f, b / 255f);
            ballRenderer.material.color = newColor;

            if (currentMoodBall != null)
            {
                ApplyColorToMoodBall(currentMoodBall, newColor);
            }

            Debug.Log($"Applied RGB color: R={r}, G={g}, B={b}");
        }
        else
        {
            Debug.Log("Failed to parse RGB values, using white.");
            ballRenderer.material.color = Color.white;

            if (currentMoodBall != null)
            {
                ApplyColorToMoodBall(currentMoodBall, Color.white);
            }
        }
    }

    void SpawnRandomObstacle()
    {
        if (obstaclePrefabs.Length == 0) return;

        Vector2 spawnPos = new Vector2(Random.Range(-7f, 7f), Random.Range(-3f, 3f));
        Quaternion randomRot = Quaternion.Euler(0, 0, Random.Range(0f, 360f));
        int index = Random.Range(0, obstaclePrefabs.Length);

        Instantiate(obstaclePrefabs[index], spawnPos, randomRot);
    }
}

