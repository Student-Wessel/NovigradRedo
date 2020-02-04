using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ShapeCreator))]
public class ShapeEditor : Editor
{
    private ShapeCreator shapeCreator;
    private RoadPoint lastSelected = null;

    private List<HousePrefabRedo> houseList = null;

    public List<Vector3> points;
    bool needsRepaint;
    void OnEnable()
    {
        shapeCreator = target as ShapeCreator;

        if (shapeCreator.CityParent == null)
        {
            if (GameObject.FindGameObjectsWithTag("CityParent").Length > 0)
            {
                shapeCreator.CityParent = GameObject.FindGameObjectsWithTag("CityParent")[0];
            } else
            {
                shapeCreator.CityParent = new GameObject();
                shapeCreator.CityParent.tag = "CityParent";
                shapeCreator.CityParent.name = "City Parent";
            }
        }

        Undo.undoRedoPerformed += UndoCall;
    }

    void OnSceneGUI()
    {
        Event guiEvent = Event.current;

        if (guiEvent.type == EventType.Repaint)
        {
            DrawDots();
        }
        else if (guiEvent.type == EventType.Layout)
        {
            HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
        }
        else
        {
            HandleInput(guiEvent);

            if (needsRepaint)
                HandleUtility.Repaint();
        }
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        var controller = target as ShapeCreator;

        base.OnInspectorGUI();

        if (GUILayout.Button("Remove all dots"))
        {
            shapeCreator.removeAllPoints();
            shapeCreator.NeigthborList = new List<List<RoadPoint>>();
            shapeCreator.removeNeigthborList();
            HandleUtility.Repaint();
        }

        if (GUILayout.Button("Remove city"))
        {
            shapeCreator.DeleteCity();
        }

        GUILayout.Label("Generate options", EditorStyles.boldLabel);

        if (GUILayout.Button("Generate houses"))
        {
            shapeCreator.DeleteCity();

            generateRoads();
            generateHouses();
            PlaceCityElements();
            SetAllCityBuildingsStatic();
        }
    }

    void generateRoads()
    {
        for (int i = 0; i < shapeCreator.NeigthborList.Count; i++)
        {
            Vector3 point = shapeCreator.NeigthborList[i][0].transform.position;
            Vector3 nextPoint = shapeCreator.NeigthborList[i][1].transform.position;
            Vector3 pointBetween = (nextPoint + point) * 0.5f;
            float distBetween = Vector3.Distance(point, nextPoint);

            Vector3 roadNormal = nextPoint - point;

            roadNormal.Normalize();

            CreateRoad(pointBetween, point, distBetween, shapeCreator.CityParent);
        }
    }

    void CreateRoad(Vector3 roadPoint, Vector3 startPoint, float roadLength, GameObject p_parent)
    {
        GameObject roadPart = Instantiate(shapeCreator.path);
        //roadPart.transform.localScale = new Vector3(1.9f, 0.1f, roadLength + 1.1f);
        roadPart.transform.localScale += new Vector3(0, 0, roadLength + 0.6f);
        roadPart.transform.position = roadPoint;
        roadPart.transform.LookAt(startPoint);
        roadPart.name = "Road part";
        roadPart.transform.SetParent(p_parent.transform);
    }


    void PlaceCityElements()
    {
        System.Random rand = new System.Random();
        PlaceWells(rand);
        PlaceLanterns(rand);
    }


    void PlaceWells(System.Random rand)
    {
        int totalPoint = shapeCreator.points.Count;
        int wellAmout = totalPoint / 10;

        for (int i = 0; i < wellAmout; i++)
        {
            GameObject well = Instantiate(shapeCreator.wellPreFab);
            well.transform.position = shapeCreator.points[rand.Next(totalPoint)].transform.position;
            well.transform.SetParent(shapeCreator.CityParent.transform);
        }
    }

    void PlaceLanterns(System.Random rand)
    {
        int totalPoint = shapeCreator.points.Count;
        int lanternsAmount = totalPoint / 3;

        for (int i = 0; i < lanternsAmount; i++)
        {
            int randomIndex = rand.Next(shapeCreator.NeigthborList.Count);

            GameObject lantern = Instantiate(shapeCreator.lanternPreFab);
            lantern.transform.position = shapeCreator.NeigthborList[randomIndex][0].transform.position;
            lantern.transform.LookAt(shapeCreator.NeigthborList[randomIndex][1].transform.position);
            lantern.transform.position += lantern.transform.right;
            lantern.transform.Rotate(0, -90, 0);
            lantern.transform.SetParent(shapeCreator.CityParent.transform);
        }
    }

    void DrawDots()
    {
        if (shapeCreator == null)
            return;

        if (shapeCreator.NeigthborList == null)
            return;

        for (int i = 0; i < shapeCreator.NeigthborList.Count; i++)
        {
            RoadPoint pointA = shapeCreator.NeigthborList[i][0];
            RoadPoint pointB = shapeCreator.NeigthborList[i][1];

            if (pointA == null || pointB == null)
                return;
            
            Handles.DrawDottedLine(pointA.transform.position, pointB.transform.position, 4);
        }
    }

    private RoadPoint GetRoadPoint(Ray ray)
    {
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
            if (hit.collider.gameObject.tag == "RoadPoint")
            {
                return hit.collider.GetComponent<RoadPoint>();
            }
            else
                return null;
        else
            return null;
    }

    private RoadPoint createRoadPoint(Vector3 pos)
    {
        RoadPoint newPoint = (RoadPoint)PrefabUtility.InstantiatePrefab(shapeCreator.RoadPointpreFab);
        Undo.RegisterCreatedObjectUndo(newPoint.gameObject, "Point created");
        newPoint.transform.position = pos;
        newPoint.transform.localScale = new Vector3(shapeCreator.handleRadius * 0.1f, shapeCreator.handleRadius * 0.1f, shapeCreator.handleRadius * 0.1f);
        if (lastSelected != null)
        {
            AddNeightbors(newPoint);
        }
        makeLastSelected(newPoint);
        newPoint.transform.SetParent(shapeCreator.transform);
        shapeCreator.points.Add(newPoint);
        needsRepaint = true;
        return newPoint;
    }


    private void makeLastSelected(RoadPoint last)
    {
        if (lastSelected == last)
            return;

        if (lastSelected != null)
        {
            lastSelected.gameObject.GetComponent<Renderer>().sharedMaterial = shapeCreator.whiteMat;
            Undo.RecordObject(lastSelected, "Change last selected");
        }

        lastSelected = last;
        lastSelected.gameObject.GetComponent<Renderer>().sharedMaterial = shapeCreator.redMat;
    }

    private void UndoCall()
    {
        while (DoIHaveANullNeightbor())
        {
            for (int i = 0; i < shapeCreator.NeigthborList.Count; i++)
            {
                if (shapeCreator.NeigthborList[i][0] == null || shapeCreator.NeigthborList[i][1] == null)
                {
                    shapeCreator.NeigthborList.RemoveAt(i);
                    break;
                }
            }
        }

        shapeCreator.removeNullObjects();
        lastSelected = null;
    }

    private bool DoIHaveANullNeightbor()
    {
        for (int i = 0; i < shapeCreator.NeigthborList.Count; i++)
            if (shapeCreator.NeigthborList[i][0] == null || shapeCreator.NeigthborList[i][1] == null)
                return true;

        return false;
    }

    private void AddNeightbors(RoadPoint pointA)
    {
        if (lastSelected == null)
            return;

        if (lastSelected == pointA)
            return;

        for (int i = 0; i < shapeCreator.NeigthborList.Count; i++)
        {
            if (shapeCreator.NeigthborList[i].Contains(lastSelected) && shapeCreator.NeigthborList[i].Contains(pointA))
            {
                return;
            }
        }

        List<RoadPoint> connection = new List<RoadPoint>();

        connection.Add(pointA);
        connection.Add(lastSelected);

        shapeCreator.NeigthborList.Add(connection);
    }



    private void generateHouses()
    {
        houseList = new List<HousePrefabRedo>();
        for (int i = 0; i < shapeCreator.NeigthborList.Count; i++)
        {
            Vector3 point = shapeCreator.NeigthborList[i][0].transform.position;
            Vector3 nextPoint = shapeCreator.NeigthborList[i][1].transform.position;
            Vector3 pointBetween = (nextPoint + point) * 0.5f;

            Vector3 roadNormal = nextPoint - point;
            Vector3 roadNormalInverse = nextPoint - point;

            roadNormal = new Vector3(-roadNormal.z, roadNormal.y, roadNormal.x);
            roadNormalInverse = new Vector3(roadNormalInverse.z, roadNormalInverse.y, -roadNormalInverse.x);

            roadNormal.Normalize();
            roadNormalInverse.Normalize();

            System.Random rand = new System.Random();
            Vector3Int randomSize = new Vector3Int(rand.Next(shapeCreator.minSize.x, shapeCreator.maxSize.x), rand.Next(shapeCreator.minSize.y, shapeCreator.maxSize.y), rand.Next(shapeCreator.minSize.z, shapeCreator.maxSize.z));
            Vector3Int randomSize2 = new Vector3Int(rand.Next(shapeCreator.minSize.x, shapeCreator.maxSize.x), rand.Next(shapeCreator.minSize.y, shapeCreator.maxSize.y), rand.Next(shapeCreator.minSize.z, shapeCreator.maxSize.z));

            Vector3 housePos = pointBetween + (roadNormal * (randomSize.z + 1));
            Vector3 housePos2 = pointBetween + (roadNormalInverse * (randomSize2.z + 1));

            GenerateBuilding(housePos, new Vector3(0, 0, 0), pointBetween, randomSize, i + 1);
            GenerateBuilding(housePos2, new Vector3(0, 0, 0), pointBetween, randomSize2, (i + 1) * -1);
        }
        if (shapeCreator.RemoveOverlapping) 
            DeleteOverlappingHouses();
    }

    void DeleteOverlappingHouses()
    {
        while (AreThereHousesOverlapping())
        {

            for (int i = 0; i < houseList.Count; i++)
            {
                GameObject houseA = houseList[i].gameObject;
                for (int j = 0; j < houseList.Count; j++)
                {
                    GameObject houseB = houseList[j].gameObject;

                    if (houseA.name == houseB.name)
                    {
                        continue;
                    }

                    Bounds boundA = new Bounds();
                    Bounds boundB = new Bounds();

                    boundA.center = houseA.transform.position;
                    boundB.center = houseB.transform.position;
                    boundA.extents = houseA.GetComponent<BoxCollider>().bounds.extents;
                    boundB.extents = houseB.GetComponent<BoxCollider>().bounds.extents;

                    if (boundA.Intersects(boundB))
                    {
                        DestroyImmediate(houseB);
                        houseList.RemoveAt(j);
                        break;
                    }

                }
            }
        }
    }

    private bool AreThereHousesOverlapping()
    {
        for (int i = 0; i < houseList.Count; i++)
        {
            GameObject houseA = houseList[i].gameObject;
            for (int j = 0; j < houseList.Count; j++)
            {
                GameObject houseB = houseList[j].gameObject;

                if (houseA.name == houseB.name)
                    continue;


                Bounds boundA = new Bounds();
                Bounds boundB = new Bounds();

                boundA.center = houseA.transform.position;
                boundB.center = houseB.transform.position;
                boundA.extents = houseA.GetComponent<BoxCollider>().bounds.extents;
                boundB.extents = houseB.GetComponent<BoxCollider>().bounds.extents;                

                if (boundA.Intersects(boundB))
                    return true;

            }
        }

        return false;
    }

    private void SetAllCityBuildingsStatic()
    {
        foreach (Transform child in shapeCreator.CityParent.transform)
        {
            SetObjectStaticAndChilderen(child);
        }
    }

    private void SetObjectStaticAndChilderen(Transform thisObject)
    {
        thisObject.gameObject.isStatic = true;
        foreach(Transform child in thisObject)
        {
            SetObjectStaticAndChilderen(child);
        }
    }

    // Tom's building gen V2


    // If you want to create a random sized building you pass in a Vector3Int that is (0,0,0)
    private void GenerateBuilding(Vector3 startPos, Vector3 startRot,Vector3 middlePoint, Vector3Int buildSize,int index)
    {
        System.Random rand = new System.Random();

        int sizeX;
        int sizeZ;
        int floors;
        int windows = rand.Next(4, 7);

        if (buildSize.x == 0 && buildSize.y == 0 && buildSize.z == 0)
        {
            sizeX = rand.Next(shapeCreator.minSize.x, shapeCreator.maxSize.x);
            sizeZ = rand.Next(shapeCreator.minSize.z, shapeCreator.maxSize.z);
            floors = rand.Next(shapeCreator.minSize.y, shapeCreator.maxSize.y);
        }
        else
        {
            sizeX = buildSize.x;
            sizeZ = buildSize.z;
            floors = buildSize.y;
        }

        GameObject randomWindow = shapeCreator.randomWindows[rand.Next(shapeCreator.randomWindows.Length)];

        HousePrefabRedo myParent = Instantiate(shapeCreator.houseContainer);
        myParent.minSize = shapeCreator.minSize;
        myParent.maxSize = shapeCreator.maxSize;
        GameObject roof = new GameObject("Roof");
        GameObject Walls = new GameObject("Walls");
        BoxCollider houseCollider = (BoxCollider)myParent.gameObject.AddComponent(typeof(BoxCollider));
        roof.transform.SetParent(myParent.transform);
        Walls.transform.SetParent(myParent.transform);

        shapeCreator.buildWallObject = shapeCreator.defaultWall;
        for (int floor = 0; floor < floors; floor++)
        {
            for (int x = 0; x < sizeX; x++)
            {
                randomWindow = shapeCreator.randomWindows[rand.Next(shapeCreator.randomWindows.Length)];

                if (floors == 0) { shapeCreator.buildWallObject = shapeCreator.defaultDoorway; }
                if (floors == 0 && x % 2 == 0)
                {
                    shapeCreator.buildWallObject = randomWindow;
                }

                // A door needs to be somewhere placed in here
                if (shapeCreator.buildWallObject != null)
                {
                    // We need to place a door somewhere on the first floor.
                    if (floor == 0)
                    {
                        // We place the door as the first object
                        if (x == 0)
                        {
                            GameObject WallX = Instantiate(shapeCreator.buildWallObject);
                            GameObject WallZ = Instantiate(shapeCreator.defaultDoorway);
                            WallX.transform.transform.localPosition = new Vector3(x, floor, 0);
                            WallX.transform.localRotation = Quaternion.Euler(0, 180, 0);

                            WallZ.transform.transform.localPosition = new Vector3(x, floor, sizeZ);
                            WallZ.transform.localRotation = Quaternion.Euler(0, 0, 0);

                            WallX.transform.SetParent(Walls.transform);
                            WallZ.transform.SetParent(Walls.transform);
                        } 
                        else
                        {
                            GameObject WallX = Instantiate(shapeCreator.buildWallObject);
                            GameObject WallZ = Instantiate(shapeCreator.buildWallObject);
                            WallX.transform.transform.localPosition = new Vector3(x, floor, 0);
                            WallX.transform.localRotation = Quaternion.Euler(0, 180, 0);

                            WallZ.transform.transform.localPosition = new Vector3(x, floor, sizeZ);
                            WallZ.transform.localRotation = Quaternion.Euler(0, 0, 0);

                            WallX.transform.SetParent(Walls.transform);
                            WallZ.transform.SetParent(Walls.transform);
                        }
                    }
                    else
                    {
                        GameObject WallX = Instantiate(shapeCreator.buildWallObject);
                        GameObject WallZ = Instantiate(shapeCreator.buildWallObject);
                        WallX.transform.transform.localPosition = new Vector3(x, floor, 0);
                        WallX.transform.localRotation = Quaternion.Euler(0, 180, 0);

                        WallZ.transform.transform.localPosition = new Vector3(x, floor, sizeZ);
                        WallZ.transform.localRotation = Quaternion.Euler(0, 0, 0);

                        WallX.transform.SetParent(Walls.transform);
                        WallZ.transform.SetParent(Walls.transform);
                    }
                }


            }

            for (int z = 0; z < sizeZ; z++)
            {
                if (floors == 0) { shapeCreator.buildWallObject = shapeCreator.defaultDoorway; }
                if (floor > 0 && z % 2 == 0)
                {
                    shapeCreator.buildWallObject = randomWindow;
                }
                if (shapeCreator.buildWallObject != null)
                {
                    GameObject WallX = Instantiate(shapeCreator.buildWallObject);
                    GameObject WallZ = Instantiate(shapeCreator.buildWallObject);
                    WallX.transform.localPosition = new Vector3(0 - 0.5f, floor, z + 0.5f);
                    WallX.transform.localRotation = Quaternion.Euler(0, 270, 0);

                    WallZ.transform.localPosition = new Vector3(sizeX - 0.5f, floor, z + 0.5f);
                    WallZ.transform.localRotation = Quaternion.Euler(0, 90, 0);

                    WallX.transform.SetParent(Walls.transform);
                    WallZ.transform.SetParent(Walls.transform);
                }
                else { throw new ArgumentException("Parameter cannot be null", "NullPointerException"); }



            }
        }

        //for (int x = 0; x < sizeX; x++)
        //{
        //    for (int z = 0; z < sizeZ; z++)
        //    {
        //        GameObject Roof = Instantiate(defaultRoof, new Vector3(x, floors, z + 0.5f), Quaternion.Euler(0, 0, 0));
        //        Roof.transform.SetParent(myParent.transform);
        //    }
        //}


        //Go through the longest side of the house twice, for each side you place roofs
        //Decrease size by 1 each side and increase y axis coordinate by one

        for (int x = 0; x < sizeX; x++)
        {
            for (int z = 0; z < sizeZ; z++)
            {
                if (sizeX > sizeZ)
                {
                    
                    // is even
                    if (sizeX % 2 == 0)
                    {
                        if (z == 0)
                        {
                            GameObject Roof = Instantiate(shapeCreator.roofPiece);
                            roof.transform.localPosition = new Vector3(x, floors, z);
                            roof.transform.localRotation = Quaternion.Euler(0, 180, 0);
                            Roof.transform.SetParent(roof.transform);
                        }
                        if (z > sizeZ - 2)
                        {
                            GameObject Roof = Instantiate(shapeCreator.roofPiece);
                            roof.transform.localPosition = new Vector3(x, floors, z - shapeCreator._roofOffset);
                            roof.transform.localRotation = Quaternion.Euler(0, 0, 0);

                            Roof.transform.SetParent(roof.transform);
                        }
                        else if (z < sizeZ / 2 && z > 0)
                        {
                            GameObject Block = Instantiate(shapeCreator.Koob);
                            Block.transform.localPosition = new Vector3(x, floors, z + 0.5f);
                            Block.transform.localRotation = Quaternion.Euler(0, 0, 0);
                            Block.transform.SetParent(roof.transform);

                            GameObject Roof = Instantiate(shapeCreator.roofPiece);
                            Roof.transform.localPosition = new Vector3(x, floors + 1, z);
                            Roof.transform.localRotation = Quaternion.Euler(0, 180, 0);
                            Roof.transform.SetParent(roof.transform);
                        }
                        else if (z >= sizeZ / 2 && z < sizeZ - 1)
                        {
                            GameObject Block = Instantiate(shapeCreator.Koob);
                            Block.transform.localPosition = new Vector3(x, floors, z + 0.5f);
                            Block.transform.localRotation = Quaternion.Euler(0, 0, 0);
                            Block.transform.SetParent(roof.transform);

                            GameObject Roof = Instantiate(shapeCreator.roofPiece);
                            Roof.transform.localPosition = new Vector3(x, floors + 1, z - shapeCreator._roofOffset);
                            Roof.transform.localRotation = Quaternion.Euler(0, 0, 0);
                            Roof.transform.SetParent(roof.transform);
                        }
                    }
                    // is odd
                    else
                    {
                        if (z == 0)
                        {
                            GameObject Roof = Instantiate(shapeCreator.roofPiece);
                            Roof.transform.localPosition = new Vector3(x, floors, z);
                            Roof.transform.localRotation = Quaternion.Euler(0, 180, 0);
                            Roof.transform.SetParent(roof.transform);
                        }
                        if (z > sizeZ - 2)
                        {
                            GameObject Roof = Instantiate(shapeCreator.roofPiece);
                            Roof.transform.localPosition = new Vector3(x, floors, z - shapeCreator._roofOffset);
                            Roof.transform.localRotation = Quaternion.Euler(0, 0, 0);
                            Roof.transform.SetParent(roof.transform);
                        }
                        else if (z < sizeZ / 2 && z > 0)
                        {
                            GameObject Block = Instantiate(shapeCreator.Koob);
                            Block.transform.localPosition = new Vector3(x, floors, z + 0.5f);
                            Block.transform.localRotation = Quaternion.Euler(0, 0, 0);
                            Block.transform.SetParent(roof.transform);
                        }
                        else if (z >= sizeZ / 2 && z < sizeZ - 1)
                        {
                            GameObject Block = Instantiate(shapeCreator.Koob);
                            Block.transform.localPosition = new Vector3(x, floors, z + 0.5f);
                            Block.transform.localRotation = Quaternion.Euler(0, 0, 0);
                            Block.transform.SetParent(roof.transform);

                            GameObject roofCenter = Instantiate(shapeCreator.roofCenterPiece);
                            roofCenter.transform.localPosition = new Vector3(x, floors + 1, z + 0.5f);
                            roofCenter.transform.localRotation = Quaternion.Euler(0, 90, 0);
                            roofCenter.transform.SetParent(roof.transform);
                        }
                    }
                }

                else if (sizeZ > sizeX)
                {

                    if (sizeZ % 2 == 1)
                    {
                        if (z == 0)
                        {
                            GameObject Roof = Instantiate(shapeCreator.roofPiece);
                            Roof.transform.localPosition = new Vector3(x, floors, z);
                            Roof.transform.localRotation = Quaternion.Euler(0, 180, 0);
                            Roof.transform.SetParent(roof.transform);
                        }
                        if (z > sizeZ - 2)
                        {
                            GameObject Roof = Instantiate(shapeCreator.roofPiece);
                            Roof.transform.localPosition = new Vector3(x, floors, z - shapeCreator._roofOffset);
                            Roof.transform.localRotation = Quaternion.Euler(0, 0, 0);
                            Roof.transform.SetParent(roof.transform);
                        }
                        else if (z < sizeZ / 2 && z > 0)
                        {
                            GameObject Block = Instantiate(shapeCreator.Koob);
                            Block.transform.localPosition = new Vector3(x, floors, z + 0.5f);
                            Block.transform.localRotation = Quaternion.Euler(0, 0, 0);
                            Block.transform.SetParent(roof.transform);
                        }
                        else if (z >= sizeZ / 2 && z < sizeZ - 1)
                        {
                            GameObject Block = Instantiate(shapeCreator.Koob);
                            Block.transform.localPosition = new Vector3(x, floors, z + 0.5f);
                            Block.transform.localRotation = Quaternion.Euler(0, 0, 0);
                            Block.transform.SetParent(roof.transform);

                            GameObject roofCenter = Instantiate(shapeCreator.roofCenterPiece);
                            roofCenter.transform.localPosition = new Vector3(x, floors + 1, z + 0.5f);
                            roofCenter.transform.localRotation = Quaternion.Euler(0, 90, 0);
                            roofCenter.transform.SetParent(roof.transform);
                        }
                    }

                    else if (sizeZ % 2 == 0)
                    {
                        if (z == 0)
                        {
                            GameObject Roof = Instantiate(shapeCreator.roofPiece);
                            Roof.transform.localPosition = new Vector3(x, floors, z);
                            Roof.transform.localRotation = Quaternion.Euler(0, 180, 0);
                            Roof.transform.SetParent(roof.transform);
                        }
                        if (z > sizeZ - 2)
                        {
                            GameObject Roof = Instantiate(shapeCreator.roofPiece);
                            Roof.transform.localPosition = new Vector3(x, floors, z - shapeCreator._roofOffset);
                            Roof.transform.localRotation = Quaternion.Euler(0, 0, 0);
                            Roof.transform.SetParent(roof.transform);
                        }
                        else if (z < sizeZ / 2 && z > 0)
                        {
                            GameObject Block = Instantiate(shapeCreator.Koob);
                            Block.transform.localPosition = new Vector3(x, floors, z + 0.5f);
                            Block.transform.localRotation = Quaternion.Euler(0, 0, 0);
                            Block.transform.SetParent(roof.transform);

                            GameObject Roof = Instantiate(shapeCreator.roofPiece);
                            Roof.transform.localPosition = new Vector3(x, floors + 1, z);
                            Roof.transform.localRotation = Quaternion.Euler(0, 180, 0);
                            Roof.transform.SetParent(roof.transform);
                        }
                        else if (z >= sizeZ / 2 && z < sizeZ - 1)
                        {
                            GameObject Block = Instantiate(shapeCreator.Koob);
                            Block.transform.localPosition = new Vector3(x, floors, z + 0.5f);
                            Block.transform.localRotation = Quaternion.Euler(0, 0, 0);
                            Block.transform.SetParent(roof.transform);

                            GameObject Roof = Instantiate(shapeCreator.roofPiece);
                            Roof.transform.localPosition = new Vector3(x, floors + 1, z - shapeCreator._roofOffset);
                            Roof.transform.localRotation = Quaternion.Euler(0, 0, 0);
                            Roof.transform.SetParent(roof.transform);
                        }
                    }

                }
                else if (sizeZ == sizeX)
                {
                    if (sizeZ % 2 == 1 || sizeX % 2 == 1)
                    {
                        if (z == 0)
                        {
                            GameObject Roof = Instantiate(shapeCreator.roofPiece);
                            Roof.transform.localPosition = new Vector3(x, floors, z);
                            Roof.transform.localRotation = Quaternion.Euler(0, 180, 0);
                            Roof.transform.SetParent(roof.transform);
                        }
                        if (z > sizeZ - 2)
                        {
                            GameObject Roof = Instantiate(shapeCreator.roofPiece);
                            Roof.transform.localPosition = new Vector3(x, floors, z - shapeCreator._roofOffset);
                            Roof.transform.localRotation = Quaternion.Euler(0, 0, 0);
                            Roof.transform.SetParent(roof.transform);
                        }
                        else if (z < sizeZ / 2 && z > 0)
                        {
                            GameObject Block = Instantiate(shapeCreator.Koob);
                            Block.transform.localPosition = new Vector3(x, floors, z + 0.5f);
                            Block.transform.localRotation = Quaternion.Euler(0,0,0);
                            Block.transform.SetParent(roof.transform);
                        }
                        else if (z >= sizeZ / 2 && z < sizeZ - 1)
                        {
                            GameObject Block = Instantiate(shapeCreator.Koob);
                            Block.transform.localPosition = new Vector3(x, floors, z + 0.5f);
                            Block.transform.localRotation = Quaternion.Euler(0, 0, 0);
                            Block.transform.SetParent(roof.transform);

                            GameObject roofCenter = Instantiate(shapeCreator.roofCenterPiece);
                            roofCenter.transform.localPosition = new Vector3(x, floors + 1, z + 0.5f);
                            roofCenter.transform.localRotation = Quaternion.Euler(0, 90, 0);
                            roofCenter.transform.SetParent(roof.transform);
                        }
                    }
                    else if (sizeZ % 2 == 0 || sizeX % 2 == 0)
                    {
                        if (z == 0)
                        {
                            GameObject Roof = Instantiate(shapeCreator.roofPiece);
                            Roof.transform.localPosition = new Vector3(x, floors, z);
                            Roof.transform.localRotation = Quaternion.Euler(0, 180, 0);
                            Roof.transform.SetParent(roof.transform);
                        }
                        if (z > sizeZ - 2)
                        {
                            GameObject Roof = Instantiate(shapeCreator.roofPiece);
                            Roof.transform.localPosition = new Vector3(x, floors, z - shapeCreator._roofOffset);
                            Roof.transform.localRotation = Quaternion.Euler(0, 0, 0);
                            Roof.transform.SetParent(roof.transform);
                        }
                        else if (z < sizeZ / 2 && z > 0)
                        {
                            GameObject Block = Instantiate(shapeCreator.Koob);
                            Block.transform.localPosition = new Vector3(x, floors, z + 0.5f);
                            Block.transform.localRotation = Quaternion.Euler(0, 0, 0);
                            Block.transform.SetParent(roof.transform);

                            GameObject Roof = Instantiate(shapeCreator.roofPiece);
                            Roof.transform.localPosition = new Vector3(x, floors, z + 0.5f);
                            Roof.transform.localRotation = Quaternion.Euler(0, 0, 0);
                            Roof.transform.SetParent(roof.transform);
                        }
                        else if (z >= sizeZ / 2 && z < sizeZ - 1)
                        {
                            GameObject Block = Instantiate(shapeCreator.Koob);
                            Block.transform.localPosition = new Vector3(x, floors, z + 0.5f);
                            Block.transform.localRotation = Quaternion.Euler(0, 0, 0);
                            Block.transform.SetParent(roof.transform);

                            GameObject Roof = Instantiate(shapeCreator.roofPiece);
                            Roof.transform.localPosition = new Vector3(x, floors + 1, z - shapeCreator._roofOffset);
                            Roof.transform.localRotation = Quaternion.Euler(0, 0, 0);
                            Roof.transform.SetParent(roof.transform);
                        }
                    }
                }
            }
        }

        myParent.transform.position = startPos;
        myParent.transform.LookAt(middlePoint);
        myParent.transform.position += new Vector3(0, 0.5f, 0);

        myParent.transform.SetParent(shapeCreator.CityParent.transform);

        houseCollider.size = new Vector3(sizeX, floors + 1, sizeZ);
        houseCollider.center = new Vector3((((float)sizeX) / 2)-0.5f, ((float)floors) / 2, ((float)sizeZ) / 2);

        houseList.Add(myParent);
    }


    void HandleInput(Event guiEvent)
    {
        Ray mouseRay = HandleUtility.GUIPointToWorldRay(guiEvent.mousePosition);
        float drawPlaneHeight = shapeCreator.terrainHeight;
        float distToDrawPlane = (drawPlaneHeight - mouseRay.origin.y) / mouseRay.direction.y;
        Vector3 mousePosition = mouseRay.GetPoint(distToDrawPlane);

        if (guiEvent.type == EventType.MouseDown && guiEvent.button == 0 && guiEvent.modifiers == EventModifiers.None)
        {
            HandleLeftMouseDown(mousePosition, GetRoadPoint(mouseRay));
        }

        if (guiEvent.type == EventType.MouseUp && guiEvent.button == 0 && guiEvent.modifiers == EventModifiers.None)
        {
            HandleLeftMouseUp(mousePosition, GetRoadPoint(mouseRay));
        }

        if (guiEvent.type == EventType.MouseDrag && guiEvent.button == 0 && guiEvent.modifiers == EventModifiers.None)
        {
            HandleLeftMouseDrag(mousePosition, GetRoadPoint(mouseRay));
        }

        if (guiEvent.type == EventType.MouseDown && guiEvent.button == 1 && guiEvent.modifiers == EventModifiers.None)
        {
            HandleRightMouseDown(mousePosition, GetRoadPoint(mouseRay));
        }

        if (guiEvent.type == EventType.MouseUp && guiEvent.button == 1 && guiEvent.modifiers == EventModifiers.None)
        {
            HandleRightMouseUp(mousePosition, GetRoadPoint(mouseRay));
        }

        if (guiEvent.type == EventType.MouseDrag && guiEvent.button == 1 && guiEvent.modifiers == EventModifiers.None)
        {
            HandleRightMouseDrag(mousePosition, GetRoadPoint(mouseRay));
        }


    }

    void HandleLeftMouseDown(Vector3 mousePosition, RoadPoint roadPoint)
    {
        RoadPoint roadPt;

        // We didn't click on a point so we add a new point
        if (roadPoint == null)
        {
            roadPt = createRoadPoint(mousePosition);
        }
        else
        {
            roadPt = roadPoint;
        }

        if (roadPt != lastSelected)
        {
            AddNeightbors(roadPt);
        }

        makeLastSelected(roadPt);
    }

    void HandleLeftMouseUp(Vector3 mousePosition, RoadPoint roadPoint)
    {

    }

    void HandleLeftMouseDrag(Vector3 mousePosition, RoadPoint roadPoint)
    {
        if (lastSelected == null)
            return;


        lastSelected.transform.position = mousePosition;
    }

    void UpdateMouseOverInfo(Vector3 mousePosition, RoadPoint roadPoint)
    {

    }

    void HandleRightMouseDown(Vector3 mousePosition, RoadPoint roadPoint)
    {
        if (lastSelected == null)
            return;

        lastSelected.gameObject.GetComponent<Renderer>().sharedMaterial = shapeCreator.whiteMat;
        lastSelected = null;
    }

    void HandleRightMouseUp(Vector3 mousePosition, RoadPoint roadPoint)
    {

    }

    void HandleRightMouseDrag(Vector3 mousePosition, RoadPoint roadPoint)
    {

    }

    

}
