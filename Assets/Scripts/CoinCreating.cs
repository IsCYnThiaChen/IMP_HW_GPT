using UnityEngine;
using System.Collections;

public class CoinCreating : MonoBehaviour
{
    public GameObject[] collectiblePrefabs;
    public float spawnInterval = 1f;

    void Start()
    {
        StartCoroutine(SpawnRoutine());
    }

    IEnumerator SpawnRoutine()
    {
        while (true)
        {
            SpawnCollectible();
            yield return new WaitForSeconds(spawnInterval);
        }
    }
    void SpawnCollectible()
    {
        if (collectiblePrefabs.Length == 0) return;

        Vector2 spawnPos = new Vector2(Random.Range(-7f, 7f), Random.Range(-3f, 3f));
        int index = Random.Range(0, collectiblePrefabs.Length);

        GameObject prefabToSpawn = collectiblePrefabs[index];
        Instantiate(prefabToSpawn, spawnPos, Quaternion.identity); // Never destroy this prefab!
    }
}
