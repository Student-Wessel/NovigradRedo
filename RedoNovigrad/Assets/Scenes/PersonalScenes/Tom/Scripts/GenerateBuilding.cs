using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateBuilding : MonoBehaviour
{
    [SerializeField]
    private GameObject[] randomWindows = new GameObject[5];
    [SerializeField]
    private GameObject defaultDoorway;
    [SerializeField]
    private GameObject defaultWall;
    [SerializeField]
    private GameObject defaultRoof;
    [SerializeField]
    private GameObject roofPiece;    
    [SerializeField]
    private GameObject Koob;

    GameObject buildWallObject;

    


    // Start is called before the first frame update
    void Start()
    {
        System.Random rand = new System.Random();
        int sizeX = rand.Next(2, 4);
        int sizeZ = rand.Next(2, 4);
        int floors = rand.Next(2, 4);
        int windows = rand.Next(4, 7);
        GameObject randomWindow = randomWindows[rand.Next(randomWindows.Length)];        

        GameObject myParent = new GameObject("House");
        GameObject roof = new GameObject("Roof");
        GameObject Walls = new GameObject("Walls");
        BoxCollider houseCollider = (BoxCollider)myParent.gameObject.AddComponent(typeof(BoxCollider));
        houseCollider.center = myParent.transform.position + new Vector3(sizeX / 2, (floors + 1) / 2, sizeZ / 2);
        houseCollider.size = new Vector3(sizeX, floors + 1, sizeZ);
        roof.transform.SetParent(myParent.transform);
        Walls.transform.SetParent(myParent.transform);

        myParent.transform.position = new Vector3(0, 0 ,0);
        myParent.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));


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

                if (buildWallObject != null)
                {
                    
                    GameObject WallX = Instantiate(buildWallObject, new Vector3(x, floor, 0), Quaternion.Euler(0, 180, 0));
                    GameObject WallZ = Instantiate(buildWallObject, new Vector3(x, floor, sizeZ), Quaternion.Euler(0, 0, 0));
                    WallX.transform.SetParent(Walls.transform);
                    WallZ.transform.SetParent(Walls.transform);
                }


            }

            for(int z = 0; z < sizeZ; z++)
            {
                if (floors == 0) { buildWallObject = defaultDoorway; }
                if (floor > 0 && z % 2 == 0) 
                {
                    buildWallObject = randomWindow;
                }
                if (buildWallObject != null)
                {

                    GameObject WallX = Instantiate(buildWallObject, new Vector3(0 - 0.5f, floor, z + 0.5f), Quaternion.Euler(0, 270, 0));
                    GameObject WallZ = Instantiate(buildWallObject, new Vector3(sizeX - 0.5f, floor, z + 0.5f), Quaternion.Euler(0, 90, 0));
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

                    if (sizeX % 2 == 1)
                    {
                        if (z == 0)
                        {
                            GameObject Roof = Instantiate(roofPiece, new Vector3(x, floors, z + 0.5f), Quaternion.Euler(0, 180, 0));
                            Roof.transform.SetParent(roof.transform);
                        }
                        if (z > sizeZ - 2)
                        {
                            GameObject Roof = Instantiate(roofPiece, new Vector3(x, floors, z + 0.5f), Quaternion.Euler(0, 0, 0));
                            Roof.transform.SetParent(roof.transform);
                        }
                        else if (z < sizeZ / 2 && z > 0)
                        {
                            GameObject Block = Instantiate(Koob, new Vector3(x, floors, z + 0.5f), Quaternion.Euler(0, 0, 0));
                            Block.transform.SetParent(roof.transform);
                        }
                        else if (z >= sizeZ / 2 && z < sizeZ - 1)
                        {
                            GameObject Block = Instantiate(Koob, new Vector3(x, floors, z + 0.5f), Quaternion.Euler(0, 0, 0));
                            Block.transform.SetParent(roof.transform);
                        }
                    }else if (sizeX % 2 == 0)
                    {
                        if (z == 0)
                        {
                            GameObject Roof = Instantiate(roofPiece, new Vector3(x, floors, z + 0.5f), Quaternion.Euler(0, 180, 0));
                            Roof.transform.SetParent(roof.transform);
                        }
                        if (z > sizeZ - 2)
                        {
                            GameObject Roof = Instantiate(roofPiece, new Vector3(x, floors, z + 0.5f), Quaternion.Euler(0, 0, 0));
                            Roof.transform.SetParent(roof.transform);
                        }
                        else if (z < sizeZ / 2 && z > 0)
                        {
                            GameObject Block = Instantiate(Koob, new Vector3(x, floors, z + 0.5f), Quaternion.Euler(0, 0, 0));
                            Block.transform.SetParent(roof.transform);
                            GameObject Roof = Instantiate(roofPiece, new Vector3(x, floors + 1, z + 0.5f), Quaternion.Euler(new Vector3(0, 180, 0)));
                            Roof.transform.SetParent(roof.transform);
                        }
                        else if (z >= sizeZ / 2 && z < sizeZ - 1)
                        {
                            GameObject Block = Instantiate(Koob, new Vector3(x, floors, z + 0.5f), Quaternion.Euler(0, 0, 0));
                            Block.transform.SetParent(roof.transform);
                            GameObject Roof = Instantiate(roofPiece, new Vector3(x, floors + 1, z + 0.5f), Quaternion.Euler(0, 0, 0));
                            Roof.transform.SetParent(roof.transform);
                        }
                    }





                }
                else if (sizeZ > sizeX)
                {

                    if (sizeZ % 2 == 1)
                    {
                        if (z == 0)
                        {
                            GameObject Roof = Instantiate(roofPiece, new Vector3(x, floors, z + 0.5f), Quaternion.Euler(0, 180, 0));
                            Roof.transform.SetParent(roof.transform);
                        }
                        if (z > sizeZ - 2)
                        {
                            GameObject Roof = Instantiate(roofPiece, new Vector3(x, floors, z + 0.5f), Quaternion.Euler(0, 0, 0));
                            Roof.transform.SetParent(roof.transform);
                        }
                        else if (z < sizeZ / 2 && z > 0)
                        {
                            GameObject Block = Instantiate(Koob, new Vector3(x, floors, z + 0.5f), Quaternion.Euler(0, 0, 0));
                            Block.transform.SetParent(roof.transform);
                        }
                        else if (z >= sizeZ / 2 && z < sizeZ - 1)
                        {
                            GameObject Block = Instantiate(Koob, new Vector3(x, floors, z + 0.5f), Quaternion.Euler(0, 0, 0));
                            Block.transform.SetParent(roof.transform);
                        }
                    }else if (sizeZ % 2 == 0)
                    {
                        if (z == 0)
                        {
                            GameObject Roof = Instantiate(roofPiece, new Vector3(x, floors, z + 0.5f), Quaternion.Euler(0, 180, 0));
                            Roof.transform.SetParent(roof.transform);
                        }
                        if (z > sizeZ - 2)
                        {
                            GameObject Roof = Instantiate(roofPiece, new Vector3(x, floors, z + 0.5f), Quaternion.Euler(0, 0, 0));
                            Roof.transform.SetParent(roof.transform);
                        }
                        else if (z < sizeZ / 2 && z > 0)
                        {
                            GameObject Block = Instantiate(Koob, new Vector3(x, floors, z + 0.5f), Quaternion.Euler(0, 0, 0));
                            Block.transform.SetParent(roof.transform);
                            GameObject Roof = Instantiate(roofPiece, new Vector3(x, floors + 1, z + 0.5f), Quaternion.Euler(new Vector3(0, 180, 0)));
                            Roof.transform.SetParent(roof.transform);
                        }
                        else if (z >= sizeZ / 2 && z < sizeZ - 1)
                        {
                            GameObject Block = Instantiate(Koob, new Vector3(x, floors, z + 0.5f), Quaternion.Euler(0, 0, 0));
                            Block.transform.SetParent(roof.transform);
                            GameObject Roof = Instantiate(roofPiece, new Vector3(x, floors + 1, z + 0.5f), Quaternion.Euler(0, 0, 0));
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
                            GameObject Roof = Instantiate(roofPiece, new Vector3(x, floors, z + 0.5f), Quaternion.Euler(0, 180, 0));
                            Roof.transform.SetParent(roof.transform);
                        }
                        if (z > sizeZ - 2)
                        {
                            GameObject Roof = Instantiate(roofPiece, new Vector3(x, floors, z + 0.5f), Quaternion.Euler(0, 0, 0));
                            Roof.transform.SetParent(roof.transform);
                        }
                        else if (z < sizeZ / 2 && z > 0)
                        {
                            GameObject Block = Instantiate(Koob, new Vector3(x, floors, z + 0.5f), Quaternion.Euler(0, 0, 0));
                            Block.transform.SetParent(roof.transform);
                        }
                        else if (z >= sizeZ / 2 && z < sizeZ - 1)
                        {
                            GameObject Block = Instantiate(Koob, new Vector3(x, floors, z + 0.5f), Quaternion.Euler(0, 0, 0));
                            Block.transform.SetParent(roof.transform);
                        }
                    }
                    else if (sizeZ % 2 == 0 || sizeX % 2 == 0)
                    {
                        if (z == 0)
                        {
                            GameObject Roof = Instantiate(roofPiece, new Vector3(x, floors, z + 0.5f), Quaternion.Euler(0, 180, 0));
                            Roof.transform.SetParent(roof.transform);
                        }
                        if (z > sizeZ - 2)
                        {
                            GameObject Roof = Instantiate(roofPiece, new Vector3(x, floors, z + 0.5f), Quaternion.Euler(0, 0, 0));
                            Roof.transform.SetParent(roof.transform);
                        }
                        else if (z < sizeZ / 2 && z > 0)
                        {
                            GameObject Block = Instantiate(Koob, new Vector3(x, floors, z + 0.5f), Quaternion.Euler(0, 0, 0));
                            Block.transform.SetParent(roof.transform);
                            GameObject Roof = Instantiate(roofPiece, new Vector3(x, floors + 1, z + 0.5f), Quaternion.Euler(new Vector3(0, 180, 0)));
                            Roof.transform.SetParent(roof.transform);
                        }
                        else if (z >= sizeZ / 2 && z < sizeZ - 1)
                        {
                            GameObject Block = Instantiate(Koob, new Vector3(x, floors, z + 0.5f), Quaternion.Euler(0, 0, 0));
                            Block.transform.SetParent(roof.transform);
                            GameObject Roof = Instantiate(roofPiece, new Vector3(x, floors + 1, z + 0.5f), Quaternion.Euler(0, 0, 0));
                            Roof.transform.SetParent(roof.transform);
                        }
                    }


                }
            }
        }


    }



    // Update is called once per frame
    void Update()
    {

    }
}
