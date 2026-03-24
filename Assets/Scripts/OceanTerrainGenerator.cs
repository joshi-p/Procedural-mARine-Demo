using UnityEngine;

public class OceanTerrainGenerator : MonoBehaviour
{
    public Terrain terrain;

    public float noiseScale = 0.03f;
    public float heightMultiplier = 8f;

    void Start()
    {
        GenerateTerrain();
    }

    void GenerateTerrain()
    {
        TerrainData data = terrain.terrainData;

        int resolution = data.heightmapResolution;

        float[,] heights = new float[resolution, resolution];

        for (int x = 0; x < resolution; x++)
        {
            for (int y = 0; y < resolution; y++)
            {
                float xCoord = x * noiseScale;
                float yCoord = y * noiseScale;

                float noise = Mathf.PerlinNoise(xCoord, yCoord);

                heights[x, y] = noise * heightMultiplier / data.size.y;
            }
        }

        data.SetHeights(0, 0, heights);
    }
}