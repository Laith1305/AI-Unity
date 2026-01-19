using UnityEngine;

public abstract class Movement : MonoBehaviour
{

    public float maxSpeed = 10f;
    public float maxForce = 10f;

    public virtual Vector3 Velocity { get;  set; }
    public virtual Vector3 Acceleration { get;  set; }
    public virtual Vector3 Direction { get { return Velocity.normalized; } }

    public abstract void ApplyForce(Vector3 force);
}