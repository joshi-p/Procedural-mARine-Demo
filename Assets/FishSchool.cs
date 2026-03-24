using UnityEngine;

public class FishSchool : MonoBehaviour
{
    public GameObject fishPrefab;
    public int fishCount = 200;
    public float radius = 20f;

    MeshFilter[] meshFilters;

    Matrix4x4[] matrices;

    void Start()
    {
        // Only grab objects that actually have meshes
        meshFilters = fishPrefab.GetComponentsInChildren<MeshFilter>();

        matrices = new Matrix4x4[fishCount];

        for (int i = 0; i < fishCount; i++)
        {
            Vector3 pos =
                fishPrefab.transform.position +
                Random.insideUnitSphere * radius;

            Quaternion rot = fishPrefab.transform.rotation;
            Vector3 scale = fishPrefab.transform.lossyScale;

            matrices[i] = Matrix4x4.TRS(pos, rot, scale);
        }
    }

    void Update()
    {
        for (int i = 0; i < fishCount; i++)
        {
            foreach (MeshFilter mf in meshFilters)
            {
                MeshRenderer mr = mf.GetComponent<MeshRenderer>();

                if (mr == null) continue;

                Matrix4x4 childOffset =
                    Matrix4x4.TRS(
                        mf.transform.localPosition,
                        mf.transform.localRotation,
                        mf.transform.localScale
                    );

                Graphics.DrawMesh(
                    mf.sharedMesh,
                    matrices[i] * childOffset,
                    mr.sharedMaterial,
                    0
                );
            }
        }
    }
}