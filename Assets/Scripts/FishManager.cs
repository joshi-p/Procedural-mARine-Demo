using System.Collections.Generic;
using UnityEngine;

public class FishManager : MonoBehaviour
{
    public static FishManager Instance;

    public List<FishAI> fishes = new List<FishAI>();

    void Awake()
    {
        Instance = this;
    }

    public void RegisterFish(FishAI fish)
    {
        if (!fishes.Contains(fish))
            fishes.Add(fish);
    }

    public void UnregisterFish(FishAI fish)
    {
        if (fishes.Contains(fish))
            fishes.Remove(fish);
    }

    public List<FishAI> GetNeighbors(FishAI fish, float radius)
    {
        List<FishAI> neighbors = new List<FishAI>();

        foreach (FishAI other in fishes)
        {
            if (other == fish)
                continue;

            float dist = Vector3.Distance(
                fish.transform.position,
                other.transform.position
            );

            if (dist < radius)
                neighbors.Add(other);
        }

        return neighbors;
    }
}