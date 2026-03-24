using UnityEngine;

public class HidingSpot : MonoBehaviour
{
    public bool occupied = false;

    public Transform eggPoint;

    public bool IsAvailable()
    {
        return !occupied;
    }

    public void Occupy()
    {
        occupied = true;
    }
}