using UnityEngine;
using static UnityEngine.UIElements.UxmlAttributeDescription;

public class AutonomousAgent : AIAgent
{
    [SerializeField] Movement movement;
    [SerializeField] Perception seekPerception;
    [SerializeField] Perception fleePerception;
    [SerializeField] Perception flockPerception;

    [Header("Wander")]
    [SerializeField] float wanderRadius = 1f;
    [SerializeField] float wanderDistance = 1f;
    [SerializeField] float wanderDisplacement = 10f;

    [Header("Flock")]
    [SerializeField, Range(0, 5)] float cohesionWeight = 1;
    [SerializeField, Range(0, 5)] float separationWeight = 1;
    [SerializeField, Range(0, 5)] float alignmentWeight = 1;
    [SerializeField, Range(0, 5)] float separationRadius = 1;

    [Header("Obsticles")]
    [SerializeField] Perception obstaclePerception;
    [SerializeField, Range(0, 5)] float obstacleSeparationRadius = 1;

    float wanderAngle = 0.0f;


    void Start()
    {
        // random within circle degrees (0 - 360)
        wanderAngle = Random.Range(0f, 360f);
    }
    void Update()
    {
        bool hasTarget = false;

        // SEEK
        if (seekPerception != null)
        {
            var gameObjects = seekPerception.GetGameObjects();
            if (gameObjects.Length > 0)
            {
                hasTarget = true;
                Vector3 force = Seek(gameObjects[0]);
                movement.ApplyForce(force);
            }
        }

        // FLEE
        if (fleePerception != null)
        {
            var gameObjects = fleePerception.GetGameObjects();
            if (gameObjects.Length > 0)
            {
                hasTarget = true;
                Vector3 force = Flee(gameObjects[0]);
                movement.ApplyForce(force);
            }
        }

        // FLOCKS
        if (flockPerception != null)
        {
            var neighbors = flockPerception.GetGameObjects();

            if (neighbors.Length > 0)
            {
                movement.ApplyForce(Cohesion(neighbors) * cohesionWeight);
                movement.ApplyForce(Separation(neighbors, separationRadius) * separationWeight);
                movement.ApplyForce(Alignment(neighbors) * alignmentWeight);
            }
        }

        if (obstaclePerception != null && obstaclePerception.GetGameObjectInDirection(transform.forward) != null)
        {
            Vector3 openDirection = Vector3.zero;
            if (obstaclePerception.GetOpenDirection(ref openDirection))
            {
                hasTarget = true;
                movement.ApplyForce(GetStereingForce(openDirection) * separationWeight);
            }
        }

        // WANDER (only if no seek or flee)
        if (!hasTarget)
        {
            Vector3 force = Wander();
            movement.ApplyForce(force);
        }

        // World wrap
        transform.position = Utilities.Wrap(
            transform.position,
            new Vector3(-20, 0, -20),
            new Vector3(20, 0, 20)
        );

        // Face movement direction
        if (movement.Velocity.sqrMagnitude > 0.01f)
        {
            transform.rotation = Quaternion.LookRotation(movement.Velocity);
        }
    }


    // Seek behavior implementation
    Vector3 Seek(GameObject go)
    {
        Vector3 direction = go.transform.position - transform.position;
        Vector3 force = GetStereingForce(direction);
        return force;
    }
    // Flee behavior implementation
    Vector3 Flee(GameObject go)
    {
        Vector3 direction = transform.position - go.transform.position;
        Vector3 force = GetStereingForce(direction);
        return force;
    }

    private Vector3 Cohesion(GameObject[] neighbors)
    {
        Vector3 positions = Vector3.zero;
        foreach (var neighbor in neighbors)
        {
            positions += neighbor.transform.position;
        }

        Vector3 center = positions / neighbors.Length;

        Vector3 direction = center - transform.position;

        Vector3 force = GetStereingForce(direction);

        return force;
    }

    private Vector3 Separation(GameObject[] neighbors, float radius)
    {
        Vector3 separation = Vector3.zero;

        foreach (var neighbor in neighbors)
        {
            Vector3 direction = (transform.position - neighbor.transform.position);
            float distance = direction.magnitude;

            // check if within separation radius
            if (distance > 0.0f && distance < radius)
            {
                // closer neighbors push harder
                separation += direction * (1 / distance);
            }
        }

        // steer towards the separation direction
        Vector3 force = (separation.sqrMagnitude > 0) ? GetStereingForce(separation) : Vector3.zero;


        return force;
    }


    private Vector3 Alignment(GameObject[] neighbors)
    {
        Vector3 velocities = Vector3.zero;

        foreach (var neighbor in neighbors)
        {
            // get AutonomousAgent component safely
            if (neighbor.TryGetComponent<AutonomousAgent>(out AutonomousAgent agent))
            {
                velocities += agent.movement.Velocity;
            }
        }

        // get the average velocity of the neighbors
        Vector3 averageVelocity = velocities / neighbors.Length;

        // steer towards the average velocity
        Vector3 force = GetStereingForce(averageVelocity);

        return force;
    }


    private Vector3 Wander()
    {
        // randomly adjust the wander angle within displacement range
        wanderAngle += Random.Range(-wanderDisplacement, wanderDisplacement);

        // calculate point on the wander circle
        Quaternion rotation = Quaternion.AngleAxis(wanderAngle, Vector3.up);
        Vector3 pointOnCircle = rotation * (Vector3.forward * wanderRadius);
        // project the wander circle in front of the agent
        Vector3 circleCenter = movement.Velocity.normalized * wanderDistance;

        // steer toward the target point
        Vector3 target = circleCenter + pointOnCircle;
        Vector3 force = GetStereingForce(target);

        // Debug visualization (optional but recommended)
        Debug.DrawLine(transform.position, transform.position + circleCenter, Color.blue);
        Debug.DrawLine(transform.position, transform.position + target, Color.red);

        return force;
    }

    // Calculate the steering force towards a desired direction
    public Vector3 GetStereingForce(Vector3 direction)
    {
        Vector3 desire = direction.normalized * movement.maxSpeed;
        Vector3 steer = desire - movement.Velocity;
        Vector3 force = Vector3.ClampMagnitude(steer, movement.maxForce);

        return force;
    }

}