using System.Collections;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public class Generation : MonoBehaviour
{   
    public Button sendButton;

    public GameObject collectiblePrefab;
    public GameObject[] obstaclePrefabs;

    public Transform spawnArea;
    public Text scoreText;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        sendButton.interactable = false;
        StartCoroutine(SpawnCollectibles());
        UpdateScoreText("0");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator SpawnCollectibles()
    {
        while (true)
        {
            SpawnCollectible();
            yield return new WaitForSeconds(1f);
        }
    }

    void SpawnCollectible()
    {
        Vector2 randomPos = new Vector2(Random.Range(-7f, 7f), Random.Range(-3f, 3f));
        Instantiate(collectiblePrefab, randomPos, Quaternion.identity);
    }

    void UpdateScoreText(string score)
    {
        scoreText.text = "Score: " + score;
    }

}
