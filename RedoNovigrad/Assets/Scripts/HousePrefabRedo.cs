using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class HousePrefabRedo : MonoBehaviour
{

    // Building gen stu
    [SerializeField]
    public Vector3Int minSize = new Vector3Int(2, 2, 2);
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


    public void NewBuilding()
    {
        RemoveBuilding();

        System.Random rand = new System.Random();
        Vector3Int randomSize = new Vector3Int(rand.Next(minSize.x, maxSize.x), rand.Next(minSize.y, maxSize.y), rand.Next(minSize.z, maxSize.z));

        

        ReGenerateBuilding(transform.position, transform.rotation.eulerAngles, new Vector3(), randomSize);
    }

    private void RemoveBuilding()
    {
        DestroyImmediate(gameObject.GetComponent<BoxCollider>());

        while(gameObject.transform.childCount != 0)
        {
            DestroyImmediate(gameObject.transform.GetChild(0).gameObject);
        }
    }
    
    private void ReGenerateBuilding(Vector3 startPos, Vector3 startRot, Vector3 middlePoint, Vector3Int buildSize, int index = int.MaxValue)
    {
        System.Random rand = new System.Random();

        int sizeX;
        int sizeZ;
        int floors;
        int windows = rand.Next(4, 7);

        sizeX = buildSize.x;
        sizeZ = buildSize.z;
        floors = buildSize.y;
        

        GameObject randomWindow = randomWindows[rand.Next(randomWindows.Length)];

        GameObject myParent = new GameObject();
        myParent.transform.position = new Vector3(0, 0, 0);
        myParent.name = "ReGeneratedHouse";
        GameObject roof = new GameObject("Roof");
        GameObject Walls = new GameObject("Walls");
        BoxCollider houseCollider = (BoxCollider)myParent.gameObject.AddComponent(typeof(BoxCollider));
        roof.transform.SetParent(myParent.transform);
        Walls.transform.SetParent(myParent.transform);

        buildWallObject = defaultWall;
        for (int floor = 0; floor < floors; floor++)
        {
            for (int x = 0; x < sizeX; x++)
            {
                randomWindow = randomWindows[rand.Next(randomWindows.Length)];

                if (floors == 0) { buildWallObject = defaultDoorway; }
                if (floors == 0 && x % 2 == 0)
                {
                    buildWallObject = randomWindow;
                }

                // A door needs to be somewhere placed in here
                if (buildWallObject != null)
                {
                    // We need to place a door somewhere on the first floor.
                    if (floor == 0)
                    {
                        // We place the door as the first object
                        if (x == 0)
                        {
                            GameObject WallX = Instantiate(buildWallObject);
                            GameObject WallZ = Instantiate(defaultDoorway);
                            WallX.transform.transform.localPosition = new Vector3(x, floor, 0);
                            WallX.transform.localRotation = Quaternion.Euler(0, 180, 0);

                            WallZ.transform.transform.localPosition = new Vector3(x, floor, sizeZ);
                            WallZ.transform.localRotation = Quaternion.Euler(0, 0, 0);

                            WallX.transform.SetParent(Walls.transform);
                            WallZ.transform.SetParent(Walls.transform);
                        }
                        else
                        {
                            GameObject WallX = Instantiate(buildWallObject);
                            GameObject WallZ = Instantiate(buildWallObject);
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
                        GameObject WallX = Instantiate(buildWallObject);
                        GameObject WallZ = Instantiate(buildWallObject);
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
                if (floors == 0) { buildWallObject = defaultDoorway; }
                if (floor > 0 && z % 2 == 0)
                {
                    buildWallObject = randomWindow;
                }
                if (buildWallObject != null)
                {
                    GameObject WallX = Instantiate(buildWallObject);
                    GameObject WallZ = Instantiate(buildWallObject);
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
                            GameObject Roof = Instantiate(roofPiece);
                            roof.transform.localPosition = new Vector3(x, floors, z);
                            roof.transform.localRotation = Quaternion.Euler(0, 180, 0);
                            Roof.transform.SetParent(roof.transform);
                        }
                        if (z > sizeZ - 2)
                        {
                            GameObject Roof = Instantiate(roofPiece);
                            roof.transform.localPosition = new Vector3(x, floors, z - _roofOffset);
                            roof.transform.localRotation = Quaternion.Euler(0, 0, 0);

                            Roof.transform.SetParent(roof.transform);
                        }
                        else if (z < sizeZ / 2 && z > 0)
                        {
                            GameObject Block = Instantiate(Koob);
                            Block.transform.localPosition = new Vector3(x, floors, z + 0.5f);
                            Block.transform.localRotation = Quaternion.Euler(0, 0, 0);
                            Block.transform.SetParent(roof.transform);

                            GameObject Roof = Instantiate(roofPiece);
                            Roof.transform.localPosition = new Vector3(x, floors + 1, z);
                            Roof.transform.localRotation = Quaternion.Euler(0, 180, 0);
                            Roof.transform.SetParent(roof.transform);
                        }
                        else if (z >= sizeZ / 2 && z < sizeZ - 1)
                        {
                            GameObject Block = Instantiate(Koob);
                            Block.transform.localPosition = new Vector3(x, floors, z + 0.5f);
                            Block.transform.localRotation = Quaternion.Euler(0, 0, 0);
                            Block.transform.SetParent(roof.transform);

                            GameObject Roof = Instantiate(roofPiece);
                            Roof.transform.localPosition = new Vector3(x, floors + 1, z - _roofOffset);
                            Roof.transform.localRotation = Quaternion.Euler(0, 0, 0);
                            Roof.transform.SetParent(roof.transform);
                        }
                    }
                    // is odd
                    else
                    {
                        if (z == 0)
                        {
                            GameObject Roof = Instantiate(roofPiece);
                            Roof.transform.localPosition = new Vector3(x, floors, z);
                            Roof.transform.localRotation = Quaternion.Euler(0, 180, 0);
                            Roof.transform.SetParent(roof.transform);
                        }
                        if (z > sizeZ - 2)
                        {
                            GameObject Roof = Instantiate(roofPiece);
                            Roof.transform.localPosition = new Vector3(x, floors, z - _roofOffset);
                            Roof.transform.localRotation = Quaternion.Euler(0, 0, 0);
                            Roof.transform.SetParent(roof.transform);
                        }
                        else if (z < sizeZ / 2 && z > 0)
                        {
                            GameObject Block = Instantiate(Koob);
                            Block.transform.localPosition = new Vector3(x, floors, z + 0.5f);
                            Block.transform.localRotation = Quaternion.Euler(0, 0, 0);
                            Block.transform.SetParent(roof.transform);
                        }
                        else if (z >= sizeZ / 2 && z < sizeZ - 1)
                        {
                            GameObject Block = Instantiate(Koob);
                            Block.transform.localPosition = new Vector3(x, floors, z + 0.5f);
                            Block.transform.localRotation = Quaternion.Euler(0, 0, 0);
                            Block.transform.SetParent(roof.transform);

                            GameObject roofCenter = Instantiate(roofCenterPiece);
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
                            GameObject Roof = Instantiate(roofPiece);
                            Roof.transform.localPosition = new Vector3(x, floors, z);
                            Roof.transform.localRotation = Quaternion.Euler(0, 180, 0);
                            Roof.transform.SetParent(roof.transform);
                        }
                        if (z > sizeZ - 2)
                        {
                            GameObject Roof = Instantiate(roofPiece);
                            Roof.transform.localPosition = new Vector3(x, floors, z - _roofOffset);
                            Roof.transform.localRotation = Quaternion.Euler(0, 0, 0);
                            Roof.transform.SetParent(roof.transform);
                        }
                        else if (z < sizeZ / 2 && z > 0)
                        {
                            GameObject Block = Instantiate(Koob);
                            Block.transform.localPosition = new Vector3(x, floors, z + 0.5f);
                            Block.transform.localRotation = Quaternion.Euler(0, 0, 0);
                            Block.transform.SetParent(roof.transform);
                        }
                        else if (z >= sizeZ / 2 && z < sizeZ - 1)
                        {
                            GameObject Block = Instantiate(Koob);
                            Block.transform.localPosition = new Vector3(x, floors, z + 0.5f);
                            Block.transform.localRotation = Quaternion.Euler(0, 0, 0);
                            Block.transform.SetParent(roof.transform);

                            GameObject roofCenter = Instantiate(roofCenterPiece);
                            roofCenter.transform.localPosition = new Vector3(x, floors + 1, z + 0.5f);
                            roofCenter.transform.localRotation = Quaternion.Euler(0, 90, 0);
                            roofCenter.transform.SetParent(roof.transform);
                        }
                    }

                    else if (sizeZ % 2 == 0)
                    {
                        if (z == 0)
                        {
                            GameObject Roof = Instantiate(roofPiece);
                            Roof.transform.localPosition = new Vector3(x, floors, z);
                            Roof.transform.localRotation = Quaternion.Euler(0, 180, 0);
                            Roof.transform.SetParent(roof.transform);
                        }
                        if (z > sizeZ - 2)
                        {
                            GameObject Roof = Instantiate(roofPiece);
                            Roof.transform.localPosition = new Vector3(x, floors, z - _roofOffset);
                            Roof.transform.localRotation = Quaternion.Euler(0, 0, 0);
                            Roof.transform.SetParent(roof.transform);
                        }
                        else if (z < sizeZ / 2 && z > 0)
                        {
                            GameObject Block = Instantiate(Koob);
                            Block.transform.localPosition = new Vector3(x, floors, z + 0.5f);
                            Block.transform.localRotation = Quaternion.Euler(0, 0, 0);
                            Block.transform.SetParent(roof.transform);

                            GameObject Roof = Instantiate(roofPiece);
                            Roof.transform.localPosition = new Vector3(x, floors + 1, z);
                            Roof.transform.localRotation = Quaternion.Euler(0, 180, 0);
                            Roof.transform.SetParent(roof.transform);
                        }
                        else if (z >= sizeZ / 2 && z < sizeZ - 1)
                        {
                            GameObject Block = Instantiate(Koob);
                            Block.transform.localPosition = new Vector3(x, floors, z + 0.5f);
                            Block.transform.localRotation = Quaternion.Euler(0, 0, 0);
                            Block.transform.SetParent(roof.transform);

                            GameObject Roof = Instantiate(roofPiece);
                            Roof.transform.localPosition = new Vector3(x, floors + 1, z - _roofOffset);
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
                            GameObject Roof = Instantiate(roofPiece);
                            Roof.transform.localPosition = new Vector3(x, floors, z);
                            Roof.transform.localRotation = Quaternion.Euler(0, 180, 0);
                            Roof.transform.SetParent(roof.transform);
                        }
                        if (z > sizeZ - 2)
                        {
                            GameObject Roof = Instantiate(roofPiece);
                            Roof.transform.localPosition = new Vector3(x, floors, z - _roofOffset);
                            Roof.transform.localRotation = Quaternion.Euler(0, 0, 0);
                            Roof.transform.SetParent(roof.transform);
                        }
                        else if (z < sizeZ / 2 && z > 0)
                        {
                            GameObject Block = Instantiate(Koob);
                            Block.transform.localPosition = new Vector3(x, floors, z + 0.5f);
                            Block.transform.localRotation = Quaternion.Euler(0, 0, 0);
                            Block.transform.SetParent(roof.transform);
                        }
                        else if (z >= sizeZ / 2 && z < sizeZ - 1)
                        {
                            GameObject Block = Instantiate(Koob);
                            Block.transform.localPosition = new Vector3(x, floors, z + 0.5f);
                            Block.transform.localRotation = Quaternion.Euler(0, 0, 0);
                            Block.transform.SetParent(roof.transform);

                            GameObject roofCenter = Instantiate(roofCenterPiece);
                            roofCenter.transform.localPosition = new Vector3(x, floors + 1, z + 0.5f);
                            roofCenter.transform.localRotation = Quaternion.Euler(0, 90, 0);
                            roofCenter.transform.SetParent(roof.transform);
                        }
                    }
                    else if (sizeZ % 2 == 0 || sizeX % 2 == 0)
                    {
                        if (z == 0)
                        {
                            GameObject Roof = Instantiate(roofPiece);
                            Roof.transform.localPosition = new Vector3(x, floors, z);
                            Roof.transform.localRotation = Quaternion.Euler(0, 180, 0);
                            Roof.transform.SetParent(roof.transform);
                        }
                        if (z > sizeZ - 2)
                        {
                            GameObject Roof = Instantiate(roofPiece);
                            Roof.transform.localPosition = new Vector3(x, floors, z - _roofOffset);
                            Roof.transform.localRotation = Quaternion.Euler(0, 0, 0);
                            Roof.transform.SetParent(roof.transform);
                        }
                        else if (z < sizeZ / 2 && z > 0)
                        {
                            GameObject Block = Instantiate(Koob);
                            Block.transform.localPosition = new Vector3(x, floors, z + 0.5f);
                            Block.transform.localRotation = Quaternion.Euler(0, 0, 0);
                            Block.transform.SetParent(roof.transform);

                            GameObject Roof = Instantiate(roofPiece);
                            Roof.transform.localPosition = new Vector3(x, floors, z + 0.5f);
                            Roof.transform.localRotation = Quaternion.Euler(0, 0, 0);
                            Roof.transform.SetParent(roof.transform);
                        }
                        else if (z >= sizeZ / 2 && z < sizeZ - 1)
                        {
                            GameObject Block = Instantiate(Koob);
                            Block.transform.localPosition = new Vector3(x, floors, z + 0.5f);
                            Block.transform.localRotation = Quaternion.Euler(0, 0, 0);
                            Block.transform.SetParent(roof.transform);

                            GameObject Roof = Instantiate(roofPiece);
                            Roof.transform.localPosition = new Vector3(x, floors + 1, z - _roofOffset);
                            Roof.transform.localRotation = Quaternion.Euler(0, 0, 0);
                            Roof.transform.SetParent(roof.transform);
                        }
                    }
                }
            }
        }

        myParent.transform.position = gameObject.transform.position;
        myParent.transform.rotation = gameObject.transform.rotation;
        myParent.transform.SetParent(gameObject.transform);
        //myParent.transform.LookAt(middlePoint);
        //myParent.transform.position += new Vector3(0, 0.5f, 0);

        houseCollider.size = new Vector3(sizeX, floors + 1, sizeZ);
        houseCollider.center = new Vector3((((float)sizeX) / 2) - 0.5f, ((float)floors) / 2, ((float)sizeZ) / 2);

    }
}
