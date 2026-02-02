using System;
using UnityEngine;
using System.Collections.Generic;

public class NavNode : MonoBehaviour
{
    [SerializeField] protected List<NavNode> neighbors;


    public List<NavNode> Neighbors { get { return neighbors; } set { neighbors = value; } }
    public float Cost { get; set; } = 0;
    public NavNode PreviousNavNode { get; set; } = null;

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.TryGetComponent<NavAgent>(out NavAgent navAgent))
        {
            navAgent.OnEnterNavNode(this);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.TryGetComponent<NavAgent>(out NavAgent navAgent))
        {
            navAgent.OnEnterNavNode(this);
        }
    }

    public static NavNode[] GetAllNavNodes()
    {
        return FindObjectsByType<NavNode>(FindObjectsSortMode.None);
    }


    internal static NavNode GetRandomNavNode()
    {
        var navNodes = GetAllNavNodes();
        return (navNodes.Length == 0) ? null : navNodes[UnityEngine.Random.Range(0, navNodes.Length)]; 

    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        foreach (var neighbor in neighbors)
        {
            
            
            Gizmos.DrawLine(transform.position, neighbor.transform.position);
            
        }
    }


    public static NavNode GetNearestNavNode(Vector3 position)
    {
        NavNode nearestNavNode = null;
        var navNodes = GetAllNavNodes();
        foreach(var navNode in navNodes)
        {
            if (nearestNavNode == null)
            {
                nearestNavNode = navNode;
            }
            else
            {
                float currentDistance = Vector3.Distance(Vector3.zero, nearestNavNode.transform.position);
                float newDistance = Vector3.Distance(Vector3.zero, navNode.transform.position);
                if (newDistance < currentDistance)
                {
                    nearestNavNode = navNode;
                }
            }
        }
        return nearestNavNode;
    }

    public static void ResetNavNodes()
    {
        var navNodes = GetAllNavNodes();
        foreach (var navNode in navNodes)
        {
            navNode.Cost = 0;
            navNode.PreviousNavNode = null;
        }
    }


    public static void CreatePath(NavNode navNode, List<NavNode> path)
    {
        //add nodes to path in reverse order
        while (navNode.PreviousNavNode != null)
        {
            path.Add(navNode);
            navNode = navNode.PreviousNavNode;
        }

        //path is in reverse order (end is first), reverse
        path.Reverse();


    }

}

