using UnityEngine;

public abstract class Perception : MonoBehaviour
{

    [SerializeField] string info;

    [SerializeField] protected string tagName;
    [SerializeField] protected LayerMask layerMask = Physics.AllLayers;
    [SerializeField] protected float maxDistance = 5;
    [SerializeField, Range(0, 180)] protected float maxAngle;


    public abstract GameObject[] GetGameObjects();


    
}
