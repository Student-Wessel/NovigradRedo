using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class wBuildingGen : MonoBehaviour
{

    [SerializeField] private int bl_width = 3;
    [SerializeField] private int bl_depth = 3;
    [SerializeField] private int bl_height = 3;

    [SerializeField] private Vector3 startingPoint = new Vector3(0, 0, 0);
    [SerializeField] private Vector3 startRotation = new Vector3(0,0,0);

    [SerializeField] private GameObject wall_prefab;

    void Start()
    {
        for (int i_height = 0; i_height < bl_height; i_height++)
        {
            // 
            GameObject setTransfrom = new GameObject();
            GameObject thisParent = new GameObject();
            thisParent.transform.position = startingPoint;
            thisParent.transform.rotation = Quaternion.Euler(startRotation);

            // Setting all walls infront from the beginning to the left
            for (int i_width = 0; i_width < bl_width; i_width++)
            {
                float x = -i_width + (bl_width / 2);
                float y = i_height + 0.5f;
                float z = 0 - (bl_depth / 2);

                Vector3 pos = new Vector3(x, y, z);

                setTransfrom.transform.localPosition = pos;
                setTransfrom.transform.localRotation = Quaternion.Euler(0, 180, 0);
                GameObject wall = Instantiate(wall_prefab);
                wall.transform.SetParent(thisParent.transform);
                wall.transform.localPosition = setTransfrom.transform.position;
                wall.transform.localRotation = setTransfrom.transform.rotation;
            }


            // We need to turn 90 degrees
            for (int i_depth = 0; i_depth < bl_depth; i_depth++)
            {
                float x = -((bl_width - 1) + 0.45f) + (bl_width / 2);
                float y = i_height + 0.5f;
                float z = i_depth + 0.5f - (bl_depth / 2);

                Vector3 pos = new Vector3(x, y, z);

                setTransfrom.transform.localRotation = Quaternion.Euler(0, -270, 0);
                setTransfrom.transform.localPosition = pos;
                GameObject wall = Instantiate(wall_prefab);
                wall.transform.SetParent(thisParent.transform);
                wall.transform.localPosition = setTransfrom.transform.position;
                wall.transform.localRotation = setTransfrom.transform.rotation;
            }

            // Setting all walls infront from the beginning to the left
            for (int i_width = 0; i_width < bl_width; i_width++)
            {
                float x = -i_width + (bl_width / 2);
                float y = i_height + 0.5f;
                float z = bl_depth - (bl_depth / 2);

                Vector3 pos = new Vector3(x, y, z);

                setTransfrom.transform.localPosition = pos;
                setTransfrom.transform.localRotation = Quaternion.Euler(0, 180, 0);
                GameObject wall = Instantiate(wall_prefab);
                wall.transform.SetParent(thisParent.transform);
                wall.transform.localPosition = setTransfrom.transform.position;
                wall.transform.localRotation = setTransfrom.transform.rotation;
            }


            // We need to turn 90 degrees
            for (int i_depth = 0; i_depth < bl_depth; i_depth++)
            {
                float x = 0.45f + (bl_width / 2);
                float y = i_height + 0.5f;
                float z = i_depth + 0.5f - (bl_depth / 2);

                Vector3 pos = new Vector3(x, y, z);

                setTransfrom.transform.localRotation = Quaternion.Euler(0, -270, 0);
                setTransfrom.transform.localPosition = pos;
                GameObject wall = Instantiate(wall_prefab);
                wall.transform.SetParent(thisParent.transform);
                wall.transform.localPosition = setTransfrom.transform.position;
                wall.transform.localRotation = setTransfrom.transform.rotation;
            }
        }
    }

    public static void CreateHouse(Vector3Int buildingSize,Vector3 startingPoint, Vector3 startRotation, GameObject wallPrefab)
    {
        int bl_width = buildingSize.x;
        int bl_height = buildingSize.y;
        int bl_depth = buildingSize.z;

        for (int i_height = 0; i_height < bl_height; i_height++)
        {
            // 
            GameObject setTransfrom = new GameObject();
            GameObject thisParent = new GameObject();
            thisParent.transform.position = startingPoint;
            thisParent.transform.rotation = Quaternion.Euler(startRotation);

            // Setting all walls infront from the beginning to the left
            for (int i_width = 0; i_width < bl_width; i_width++)
            {
                float x = -i_width + (bl_width / 2);
                float y = i_height + 0.5f;
                float z = 0 - (bl_depth / 2);

                Vector3 pos = new Vector3(x, y, z);

                setTransfrom.transform.localPosition = pos;
                setTransfrom.transform.localRotation = Quaternion.Euler(0, 180, 0);
                GameObject wall = Instantiate(wallPrefab);
                wall.transform.SetParent(thisParent.transform);
                wall.transform.localPosition = setTransfrom.transform.position;
                wall.transform.localRotation = setTransfrom.transform.rotation;
            }


            // We need to turn 90 degrees
            for (int i_depth = 0; i_depth < bl_depth; i_depth++)
            {
                float x = -((bl_width - 1) + 0.45f) + (bl_width / 2);
                float y = i_height + 0.5f;
                float z = i_depth + 0.5f - (bl_depth / 2);

                Vector3 pos = new Vector3(x, y, z);

                setTransfrom.transform.localRotation = Quaternion.Euler(0, 270, 0);
                setTransfrom.transform.localPosition = pos;
                GameObject wall = Instantiate(wallPrefab);
                wall.transform.SetParent(thisParent.transform);
                wall.transform.localPosition = setTransfrom.transform.position;
                wall.transform.localRotation = setTransfrom.transform.rotation;
            }

            // Setting all walls infront from the beginning to the left
            for (int i_width = 0; i_width < bl_width; i_width++)
            {
                float x = -i_width + (bl_width / 2);
                float y = i_height + 0.5f;
                float z = bl_depth - (bl_depth / 2);

                Vector3 pos = new Vector3(x, y, z);

                setTransfrom.transform.localPosition = pos;
                setTransfrom.transform.localRotation = Quaternion.Euler(0, 180, 0);
                GameObject wall = Instantiate(wallPrefab);
                wall.transform.SetParent(thisParent.transform);
                wall.transform.localPosition = setTransfrom.transform.position;
                wall.transform.localRotation = setTransfrom.transform.rotation;
            }


            // We need to turn 90 degrees
            for (int i_depth = 0; i_depth < bl_depth; i_depth++)
            {
                float x = 0.45f + (bl_width / 2);
                float y = i_height + 0.5f;
                float z = i_depth + 0.5f - (bl_depth / 2);

                Vector3 pos = new Vector3(x, y, z);

                setTransfrom.transform.localRotation = Quaternion.Euler(0, 270, 0);
                setTransfrom.transform.localPosition = pos;
                GameObject wall = Instantiate(wallPrefab);
                wall.transform.SetParent(thisParent.transform);
                wall.transform.localPosition = setTransfrom.transform.position;
                wall.transform.localRotation = setTransfrom.transform.rotation;
            }
        }
    }

    void Update()
    {
        
    }


}
