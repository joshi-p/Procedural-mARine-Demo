using System.Collections;
using UnityEngine;

public class Coral : MonoBehaviour
{
    public float foodAmount = 100f;
    public float habitatRadius = 15f;

    [Header("Feeding Capacity")]
    public int maxFeedingFish = 3;
    int currentFeedingFish = 0;

    [Header("Debug")]
    public Material eatingMaterial;

    Renderer rend;
    Material originalMaterial;

    void Start()
    {
        rend = GetComponentInChildren<Renderer>();
        originalMaterial = rend.material;
    }

    public bool HasFood()
    {
        return foodAmount > 0;
    }

    public bool CanFeed()
    {
        return currentFeedingFish < maxFeedingFish;
    }

    public void BeginFeeding()
    {
        currentFeedingFish++;
    }

    public void EndFeeding()
    {
        currentFeedingFish = Mathf.Max(0, currentFeedingFish - 1);
    }

    public void FeedFish(float amount)
    {
        foodAmount -= amount;

        Debug.Log("Coral remaining food: " + foodAmount);

        StartCoroutine(FlashRed());

        if (foodAmount <= 0)
        {
            Debug.Log("Coral depleted!");
            Destroy(gameObject);
        }
    }

    IEnumerator FlashRed()
    {
        if (rend == null) yield break;

        rend.material = eatingMaterial;

        yield return new WaitForSeconds(5f);

        rend.material = originalMaterial;
    }
}