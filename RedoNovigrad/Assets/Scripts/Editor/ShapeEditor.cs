using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ShapeCreator))]
public class ShapeEditor : Editor
{
    private ShapeCreator shapeCreator;
    private RoadPoint lastSelected = null;

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
            generateRoads();
            generateAllHouses();
        }
    }

    void generateAllHouses()
    {
        for (int i = 0; i < shapeCreator.points.Count; i++)
        {

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
        GameObject roadPart = GameObject.CreatePrimitive(PrimitiveType.Cube);
        roadPart.transform.localScale = new Vector3(1.9f, 0.1f, roadLength + 1.1f);
        roadPart.transform.position = roadPoint;
        roadPart.transform.LookAt(startPoint);
        roadPart.name = "Road part";
        roadPart.transform.SetParent(p_parent.transform);
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

    void HandleInput(Event guiEvent)
    {
        Ray mouseRay = HandleUtility.GUIPointToWorldRay(guiEvent.mousePosition);
        float drawPlaneHeight = 0;
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
