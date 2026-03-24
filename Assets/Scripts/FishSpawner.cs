using UnityEngine;
using System.Collections;

public class FishSpawner : MonoBehaviour
{
    public GameObject fishPrefab;
    public int fishCount = 50;

    void Start()
    {
        StartCoroutine(SpawnFishWhenCoralReady());
    }

    IEnumerator SpawnFishWhenCoralReady()
    {
        Coral[] corals = FindObjectsOfType<Coral>();

        // wait until coral exists
        while (corals.Length == 0)
        {
            Debug.Log("Waiting for coral to spawn...");
            yield return null;

            corals = FindObjectsOfType<Coral>();
        }

        Debug.Log("Coral detected. Spawning fish.");

        for (int i = 0; i < fishCount; i++)
        {
            Coral coral = corals[Random.Range(0, corals.Length)];

            Vector2 circle = Random.insideUnitCircle * coral.habitatRadius;

            Vector3 pos = coral.transform.position +
                new Vector3(circle.x, Random.Range(2f, 6f), circle.y);

            Instantiate(fishPrefab, pos, Quaternion.identity);
        }
    }
}