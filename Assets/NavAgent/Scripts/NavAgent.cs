using UnityEngine;

public class NavAgent : AIAgent
{
    [SerializeField] NavPath navPath;
    [SerializeField] Movement movement;
    [SerializeField, Range(1, 20)] float rotationRate = 1;

    public NavNode TargetNode { get; set; } = null;




    void Start()
    {
        TargetNode = navPath != null;
        
            navPath.GeneratePath(TargetNode.transform.position, transform.position);

       
    }

    // Update is called once per frame
    void Update()
    {
        if(TargetNode != null)
        {
            Vector3 direction = TargetNode.transform.position;
            Vector3 force = direction.normalized * movement.maxForce;

            movement.ApplyForce(force);
        }   


        if(movement.Velocity.sqrMagnitude > 0)
        {
            var targetRotation = Quaternion.LookRotation(movement.Velocity.normalized);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationRate * Time.deltaTime);


        }
    }


    public void OnEnterNavNode(NavNode navNode)
    {

        if(navNode == TargetNode)
        {
            if(navPath != null)
            {
                //get next navnode in path
            }
            else
            {
                TargetNode = navNode.Neighbors[Random.Range(0, navNode.Neighbors.Count)];

            }


        }
    }
}
