using System.Collections.Generic;
using UnityEngine;

public class OceanGenerator : MonoBehaviour
{
    public int totalZones = 50;

    public Vector3 zoneSize = new Vector3(40, 40, 40);

    public GameObject sandPrefab;
    public GameObject coralPrefab;
    public GameObject hidingPrefab;

    public Vector3 worldSize = new Vector3(200, 120, 200);

    void Start()
    {
        GenerateOcean();
    }

    void GenerateOcean()
    {
        int sandCount = Mathf.RoundToInt(totalZones * 0.4f);
        int coralCount = Mathf.RoundToInt(totalZones * 0.4f);
        int hidingCount = totalZones - sandCount - coralCount;

        List<OceanZoneType> zones = new List<OceanZoneType>();

        for (int i = 0; i < sandCount; i++) zones.Add(OceanZoneType.Sand);
        for (int i = 0; i < coralCount; i++) zones.Add(OceanZoneType.Coral);
        for (int i = 0; i < hidingCount; i++) zones.Add(OceanZoneType.Hiding);

        Shuffle(zones);

        int xCount = Mathf.FloorToInt(worldSize.x / zoneSize.x);
        int yCount = Mathf.FloorToInt(worldSize.y / zoneSize.y);
        int zCount = Mathf.FloorToInt(worldSize.z / zoneSize.z);

        int index = 0;

        for (int x = 0; x < xCount; x++)
        {
            for (int y = 0; y < yCount; y++)
            {
                for (int z = 0; z < zCount; z++)
                {
                    if (index >= zones.Count)
                        return;

                    Vector3 pos = new Vector3(
                        x * zoneSize.x - worldSize.x / 2 + zoneSize.x / 2,
                        y * zoneSize.y - worldSize.y / 2 + zoneSize.y / 2,
                        z * zoneSize.z - worldSize.z / 2 + zoneSize.z / 2
                    );

                    SpawnZone(zones[index], pos);

                    index++;
                }
            }
        }
    }

    void SpawnZone(OceanZoneType type, Vector3 pos)
    {
        GameObject prefab = null;

        switch (type)
        {
            case OceanZoneType.Sand:
                prefab = sandPrefab;
                break;

            case OceanZoneType.Coral:
                prefab = coralPrefab;
                break;

            case OceanZoneType.Hiding:
                prefab = hidingPrefab;
                break;
        }

        if (prefab == null)
        {
            Debug.LogError("Prefab missing for zone type: " + type);
            return;
        }

        GameObject zone = Instantiate(prefab, pos, Quaternion.identity, transform);

        zone.transform.localScale = zoneSize;

        OceanZone zoneComponent = zone.GetComponent<OceanZone>();
        zoneComponent.Size = zoneSize;

        zone.name = type.ToString() + "_Zone";
    }

    void Shuffle(List<OceanZoneType> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int rand = Random.Range(i, list.Count);
            (list[i], list[rand]) = (list[rand], list[i]);
        }
    }
}