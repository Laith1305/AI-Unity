using UnityEngine;
using System.Collections.Generic;
using System.ComponentModel;

public class RaycastPerception : Perception
{
    public override GameObject[] GetGameObjects()
    {
        List<GameObject> result = new List<GameObject>();
        Vector3[] directions = new Vector3[1];
        directions[0] = Vector3.forward;

        foreach (var direction in directions)
        {
            Ray ray = new Ray(transform.position, transform.rotation * direction);
            if (Physics.Raycast(ray, out RaycastHit raycastHit, maxDistance, layerMask))
            {
                if (raycastHit.collider.gameObject == gameObject) continue;
                
                if(tagName == "" || raycastHit.collider.CompareTag(tagName))
                {
                    result.Add(raycastHit.collider.gameObject);
                    Debug.DrawRay(ray.origin, ray.direction * raycastHit.distance, Color.red);
                }
            }else
            {
                Debug.DrawRay(ray.origin, ray.direction * maxDistance, Color.green);
            }
        }

        return result.ToArray();
    }
}
