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

    GameObject buildWallObject;





    // Start is called before the first frame update
    void Start()
    {
        System.Random rand = new System.Random(1);
        int sizeX = rand.Next(3, 6);
        int sizeZ = rand.Next(3, 6);
        int floors = rand.Next(2, 10);
        int windows = rand.Next(4, 10);
        GameObject randomWindow = randomWindows[rand.Next(randomWindows.Length)];



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
                    Instantiate(buildWallObject, new Vector3(x, floor * 0.6f, 0), Quaternion.Euler(0, 0, 0));
                    Instantiate(buildWallObject, new Vector3(x, floor * 0.6f, sizeZ), Quaternion.Euler(0, 0, 0));
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

                    Instantiate(buildWallObject, new Vector3(0, floor * 0.6f, z), Quaternion.Euler(0, 90, 0));
                    Instantiate(buildWallObject, new Vector3(sizeX, floor * 0.6f, z), Quaternion.Euler(0, 90, 0));
                }
                else { throw new ArgumentException("Parameter cannot be null", "NullPointerException"); }



            }
        }

        for (int x = 0; x < sizeX; x++)
        {
            for (int z = 0; z < sizeZ; z++)
            {
                Instantiate(defaultRoof, new Vector3(x, floors * 0.6f, z), Quaternion.Euler(0, 0, 0));
            }
        }

    }


    // Update is called once per frame
    void Update()
    {

    }
}
