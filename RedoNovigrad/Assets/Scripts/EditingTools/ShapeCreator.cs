using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class ShapeCreator : MonoBehaviour
{
    // Building gen stu
    [SerializeField]
    public Vector3Int minSize = new Vector3Int(2,2,2);
    [SerializeField]
    public Vector3Int maxSize = new Vector3Int(4, 4, 4);
    [SerializeField]
    public GameObject[] randomWindows = new GameObject[5];
    [SerializeField]
    public GameObject defaultDoorway;
    [SerializeField]
    public GameObject defaultWall;
    [SerializeField]
    public GameObject defaultRoof;
    [SerializeField]
    public GameObject roofPiece;
    [SerializeField]
    public GameObject Koob;
    [SerializeField]
    public GameObject roofCenterPiece;
    [SerializeField]
    public float _roofOffset = -1.0f;

    public GameObject buildWallObject;

    // Map creator stuff
    [SerializeField] public List<RoadPoint> points = new List<RoadPoint>();
    [SerializeField] public float handleRadius = 0.5f;
    [SerializeField] public RoadPoint RoadPointpreFab;
    [SerializeField] public Material whiteMat;
    [SerializeField] public Material redMat;

    public List<List<RoadPoint>> NeigthborList = new List<List<RoadPoint>>();
    [SerializeField] public bool RemoveOverlapping = true;

    [System.NonSerialized] public GameObject CityParent;

    public void removeAllPoints()
    {
        for (int i = 0; i < points.Count; i++)
        {
            if (points[i] == null)
                continue;

            DestroyImmediate(points[i].gameObject);
            points[i] = null;
        }

        points = new List<RoadPoint>();
    }

    public void DeleteCity()
    {
        while(CityParent.transform.childCount > 0)
        {
            DestroyImmediate(CityParent.transform.GetChild(0).gameObject);
        }
    }

    public void removeNullObjects()
    {
        for (int i = 0; i < points.Count; i++)
        {
            if (points[i] == null)
                points.RemoveAt(i);
        }

    }

    public List<Vector3> GetVectorPoints()
    {
        List<Vector3> pointList = new List<Vector3>();

        for (int i = 0; i < points.Count; i++)
        {
            pointList.Add(points[i].transform.position);
        }

        return pointList;
    }

    public void removeNeigthborList()
    {
        NeigthborList = new List<List<RoadPoint>>();
    }

}
