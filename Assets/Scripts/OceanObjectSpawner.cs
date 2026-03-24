using UnityEngine;

public class OceanObjectSpawner : MonoBehaviour
{
    public Terrain terrain;

    [Header("Corals (40%)")]
    public GameObject[] coralPrefabs;
    public int coralCount = 200;

    [Header("Hiding Rocks (20%)")]
    public GameObject[] rockPrefabs;
    public int rockCount = 30;

    [Header("World Settings")]
    public Vector2 worldSize = new Vector2(200, 200);

    [Header("Spawn Settings")]
    public float minSpawnDistance = 4f;
    public LayerMask environmentLayer;

    void Start()
    {
        SpawnCorals();
        SpawnRocks();
    }

    void SpawnCorals()
    {
        int spawned = 0;
        int attempts = 0;

        while (spawned < coralCount && attempts < coralCount * 10)
        {
            attempts++;

            Vector3 randomPos = new Vector3(
                Random.Range(0, worldSize.x),
                100,
                Random.Range(0, worldSize.y)
            );

            RaycastHit hit;

            if (Physics.Raycast(randomPos, Vector3.down, out hit, 200))
            {
                if (Physics.CheckSphere(hit.point, minSpawnDistance, environmentLayer))
                    continue;

                GameObject coral = coralPrefabs[
                    Random.Range(0, coralPrefabs.Length)
                ];

                Quaternion rot =
                    Quaternion.FromToRotation(Vector3.up, hit.normal) *
                    Quaternion.Euler(0, Random.Range(0, 360), 0);

                GameObject obj = Instantiate(coral, hit.point, rot);

                Collider col = obj.GetComponentInChildren<Collider>();

                if (col != null)
                {
                    float offset = col.bounds.extents.y;
                    obj.transform.position += hit.normal * offset;
                }

                float scale = Random.Range(0.8f, 1.4f);
                obj.transform.localScale *= scale;

                spawned++;
            }
        }
    }

    void SpawnRocks()
    {
        int spawned = 0;
        int attempts = 0;

        while (spawned < rockCount && attempts < rockCount * 10)
        {
            attempts++;

            Vector3 randomPos = new Vector3(
                Random.Range(0, worldSize.x),
                100,
                Random.Range(0, worldSize.y)
            );

            RaycastHit hit;

            if (Physics.Raycast(randomPos, Vector3.down, out hit, 200))
            {
                if (Physics.CheckSphere(hit.point, minSpawnDistance, environmentLayer))
                    continue;

                GameObject rock = rockPrefabs[
                    Random.Range(0, rockPrefabs.Length)
                ];

                Quaternion rot =
                    Quaternion.FromToRotation(Vector3.up, hit.normal) *
                    Quaternion.Euler(0, Random.Range(0, 360), 0);

                GameObject obj = Instantiate(rock, hit.point, rot);

                Collider col = obj.GetComponentInChildren<Collider>();

                if (col != null)
                {
                    float offset = col.bounds.extents.y;
                    obj.transform.position += hit.normal * offset;
                }

                float scale = Random.Range(0.9f, 1.6f);
                obj.transform.localScale *= scale;

                spawned++;
            }
        }
    }
}