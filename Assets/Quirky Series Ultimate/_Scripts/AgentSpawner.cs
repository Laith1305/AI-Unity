using UnityEngine;
using UnityEngine.InputSystem;
public class AgentSpawner : AIAgent
{
    [SerializeField] Movement movement;
    [SerializeField] Perception seekPerception;
    [SerializeField] Perception fleePerception;

    void Start()
    {


    }
    void Update()
    {
        if(seekPerception != null)
        {


            var gameObjects = seekPerception.GetGameObjects();
            if (gameObjects.Length > 0)
            {
                Vector3 force = Seek(gameObjects[0]);
                movement.ApplyForce(force);
            }
        }

        if(fleePerception != null)
        {

            var gameObjects = fleePerception.GetGameObjects();
            if (gameObjects.Length > 0)
            {
                Vector3 force = Seek(gameObjects[0]);
                movement.ApplyForce(force);
            }
        }
        //foreach (var go in gameObjects)
        //{
        //    Debug.DrawLine(transform.position, go.transform.position, Color.red);
        

        movement.ApplyForce(transform.forward);
        transform.position = Utilities.Wrap(transform.position, new Vector3(-150, -150, -150), new Vector3(150, 150, 150));

    }

    Vector3 Seek(GameObject go)
    {
        Vector3 direction = go.transform.position - transform.position;
        Vector3 force = GetSteeringForce(direction);
        return force;
    }
    Vector3 Flee(GameObject go)
    {
        Vector3 direction = transform.position - go.transform.position;
        Vector3 force = GetSteeringForce(direction);
        return force;
    }

    public Vector3 GetSteeringForce(Vector3 direction)
    {
        Vector3 desire = direction.normalized * movement.maxSpeed;
        Vector3 steer = desire - movement.Velocity;
        Vector3 force = Vector3.ClampMagnitude(steer, movement.maxForce);

        return force;
    }

}