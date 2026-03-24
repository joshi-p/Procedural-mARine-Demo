using System.Collections.Generic;
using UnityEngine;

public class OctopusAI : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 2.5f;
    public float huntSpeed = 4f;
    public float turnSpeed = 3f;

    [Header("Hunting")]
    public float huntRadius = 25f;
    public float eatDistance = 1.5f;

    [Header("Wandering")]
    public float wanderRadius = 10f;

    Vector3 velocity;
    Vector3 target;

    FishAI targetFish;

    void Start()
    {
        velocity = transform.forward;

        PickRandomTarget();

        Debug.Log("Octopus AI started");
    }

    void Update()
    {
        FindFish();
        Move();

        // draw path to target for debugging
        Debug.DrawLine(transform.position, target, Color.magenta);
    }

    void FindFish()
    {
        if (targetFish != null) return;

        if (FishManager.Instance == null) return;

        List<FishAI> fishes = FishManager.Instance.fishes;

        float closestDist = Mathf.Infinity;

        foreach (FishAI fish in fishes)
        {
            if (fish == null) continue;

            float dist = Vector3.Distance(
                transform.position,
                fish.transform.position
            );

            if (dist < huntRadius && dist < closestDist)
            {
                closestDist = dist;
                targetFish = fish;
            }
        }

        if (targetFish != null)
        {
            target = targetFish.transform.position;

            Debug.Log("🐙 Octopus spotted fish");
        }
    }

    void Move()
    {
        float currentSpeed = speed;

        if (targetFish != null)
        {
            target = targetFish.transform.position;
            currentSpeed = huntSpeed; // speed boost when hunting
        }

        Vector3 dir =
            (target - transform.position).normalized;

        velocity += dir * Time.deltaTime;

        velocity = Vector3.ClampMagnitude(velocity, 1f);

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            Quaternion.LookRotation(velocity),
            turnSpeed * Time.deltaTime
        );

        transform.position +=
            transform.forward *
            currentSpeed *
            Time.deltaTime;

        float distance =
            Vector3.Distance(transform.position, target);

        if (targetFish != null && distance < eatDistance)
        {
            Debug.Log("🐙 Octopus ate fish");

            targetFish.Die();

            targetFish = null;

            PickRandomTarget();
        }

        if (distance < 2f && targetFish == null)
        {
            PickRandomTarget();
        }
    }

    void PickRandomTarget()
    {
        Vector2 circle = Random.insideUnitCircle * wanderRadius;

        target = transform.position +
                 new Vector3(
                     circle.x,
                     Random.Range(-1f, 1f),
                     circle.y
                 );
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, huntRadius);
    }
}