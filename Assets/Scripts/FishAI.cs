using System.Collections.Generic;
using UnityEngine;

public class FishAI : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 4f;
    public float turnSpeed = 4f;

    [Header("Boids")]
    public float neighborRadius = 8f;
    public float separationDistance = 2f;
    public float cohesionWeight = 1f;
    public float alignmentWeight = 1f;
    public float separationWeight = 2f;

    [Header("Habitat")]
    public float habitatForce = 2f;

    [Header("Floor Avoidance")]
    public float floorY = 0f;
    public float floorAvoidanceForce = 6f;

    [Header("Feeding")]
    public float eatDistance = 2f;
    public float eatAmount = 5f;
    public float feedCooldown = 10f;

    [Header("AI Optimization")]
    public float aiUpdateInterval = 0.2f;

    float aiTimer;

    float lastFeedTime = -999f;

    Vector3 velocity;
    Vector3 target;

    Coral homeCoral;
    Coral targetCoral;

    void Start()
    {
        velocity = transform.forward;

        FishManager.Instance.RegisterFish(this);

        // randomize AI timer so fish don't update together
        aiTimer = Random.Range(0f, aiUpdateInterval);

        ChooseHabitat();
        PickNewTarget();
    }

    void OnDestroy()
    {
        if (FishManager.Instance != null)
            FishManager.Instance.UnregisterFish(this);
    }

    void Update()
    {
        aiTimer += Time.deltaTime;

        // expensive neighbor calculations
        if (aiTimer >= aiUpdateInterval)
        {
            aiTimer = Random.Range(0f, aiUpdateInterval);
            ApplyBoids();
        }

        // these must run every frame
        ApplyHabitatForce();
        ApplyFloorAvoidance();

        Move();
    }

    void ApplyBoids()
    {
        List<FishAI> fishes =
            FishManager.Instance.GetNeighbors(this, neighborRadius);

        Vector3 alignment = Vector3.zero;
        Vector3 cohesion = Vector3.zero;
        Vector3 separation = Vector3.zero;

        int count = 0;

        foreach (FishAI fish in fishes)
        {
            float dist = Vector3.Distance(
                transform.position,
                fish.transform.position
            );

            alignment += fish.velocity;
            cohesion += fish.transform.position;

            if (dist < separationDistance)
                separation += (transform.position - fish.transform.position);

            count++;
        }

        if (count > 0)
        {
            alignment /= count;
            cohesion = (cohesion / count) - transform.position;

            Vector3 boidForce =
                alignment * alignmentWeight +
                cohesion * cohesionWeight +
                separation * separationWeight;

            velocity += boidForce * Time.deltaTime;
        }
    }

    void ApplyHabitatForce()
    {
        if (homeCoral == null) return;

        Vector3 toHabitat =
            homeCoral.transform.position - transform.position;

        velocity += toHabitat.normalized *
                    habitatForce *
                    Time.deltaTime;
    }

    void ApplyFloorAvoidance()
    {
        if (transform.position.y < floorY + 1f)
        {
            velocity += Vector3.up *
                        floorAvoidanceForce *
                        Time.deltaTime;
        }
    }

    void Move()
    {
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
            speed *
            Time.deltaTime;

        float distance =
            Vector3.Distance(transform.position, target);

        if (targetCoral != null && distance < eatDistance)
        {
            if (Time.time - lastFeedTime >= feedCooldown)
            {
                if (targetCoral.CanFeed())
                {
                    targetCoral.BeginFeeding();

                    Debug.Log("Fish ate coral at: "
                        + targetCoral.transform.position);

                    targetCoral.FeedFish(eatAmount);

                    lastFeedTime = Time.time;

                    targetCoral.EndFeeding();

                    if (!targetCoral.HasFood())
                    {
                        ChooseHabitat();
                    }
                }
            }

            targetCoral = null;

            PickNewTarget();
        }

        if (distance < 2f)
        {
            PickNewTarget();
        }
    }

    void ChooseHabitat()
    {
        Coral[] corals = FindObjectsOfType<Coral>();

        if (corals.Length == 0) return;

        homeCoral =
            corals[Random.Range(0, corals.Length)];
    }

    void PickNewTarget()
    {
        if (homeCoral == null)
        {
            ChooseHabitat();
            if (homeCoral == null) return;
        }

        if (homeCoral.HasFood())
        {
            targetCoral = homeCoral;
            target = homeCoral.transform.position;
            return;
        }

        ChooseHabitat();

        if (homeCoral == null) return;

        Vector2 circle = Random.insideUnitCircle * homeCoral.habitatRadius;

        targetCoral = null;

        target = homeCoral.transform.position +
                 new Vector3(circle.x, Random.Range(2f, 6f), circle.y);
    }

    public void Die()
    {
        FishManager.Instance.UnregisterFish(this);
        Destroy(gameObject);
    }
}