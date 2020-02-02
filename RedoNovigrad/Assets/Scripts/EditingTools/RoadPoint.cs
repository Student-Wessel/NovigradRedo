using System.Collections.Generic;
using UnityEngine;

public class RoadPoint : MonoBehaviour
{
    public List<RoadPoint> neightborList;
    public Vector3 pos;
    public bool isSelected = false;
    public GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);

    public RoadPoint(Vector3 c_pos, RoadPoint c_previousSelected = null)
    {
        pos = c_pos;
        sphere.transform.position = c_pos;
        if (c_previousSelected != null)
        {
            AddNeightbor(c_previousSelected);
        }
        sphere.transform.tag = "RoadPoint";
        neightborList = new List<RoadPoint>();
    }

    public void AddNeightbor(RoadPoint neightbor)
    {
        if (!neightborList.Contains(neightbor))
            neightborList.Add(neightbor);
    }

    public void RemoveNeightbor(RoadPoint neightbor)
    {
        if (neightborList.Contains(neightbor))
            neightborList.Remove(neightbor);
    }

}
