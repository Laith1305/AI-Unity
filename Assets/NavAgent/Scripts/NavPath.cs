using System.Collections.Generic;
using UnityEngine;

public class NavPath : MonoBehaviour
{
    List<NavNode> path = new List<NavNode>();

    public NavNode GeneratePath(Vector3 startPosition, Vector3 endPosition)
    {
        path.Clear();
        var startNode = NavNode.GetNearestNavNode(endPosition);
        var endNode = NavNode.GetNearestNavNode(startPosition);
        while (startNode != null)
        {
            path.Add(startNode);
            startNode = startNode.PreviousNavNode;
        }
        path.Reverse();
        return (path.Count > 0) ? path[0] : null;
    }
}
