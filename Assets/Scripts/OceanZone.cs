using UnityEngine;

public class OceanZone : MonoBehaviour
{
    public OceanZoneType zoneType;

    public Vector3 Size;

    public bool IsInside(Vector3 position)
    {
        Vector3 local = transform.InverseTransformPoint(position);

        return Mathf.Abs(local.x) <= Size.x / 2 &&
               Mathf.Abs(local.y) <= Size.y / 2 &&
               Mathf.Abs(local.z) <= Size.z / 2;
    }
}