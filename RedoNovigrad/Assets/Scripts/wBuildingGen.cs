using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class wBuildingGen : MonoBehaviour
{

    [SerializeField] private int bl_width = 3;
    [SerializeField] private int bl_depth = 3;
    [SerializeField] private int bl_height = 3;

    [SerializeField] private GameObject wall_prefab;

    void Start()
    {
        

        for (int i_height = 0; i_height < bl_height; i_height++)
        {
            // 
            GameObject setTransfrom = new GameObject();
            setTransfrom.transform.position = new Vector3(0, i_height, 0);
            setTransfrom.transform.rotation = Quaternion.Euler(0, 90, 0);

            for (int i_width = 0; i_width < bl_width; i_width++)
            {
                setTransfrom.transform.localPosition = new Vector3(i_width-0.5f, i_height, 0 - 0.5f);
                setTransfrom.transform.localRotation = Quaternion.Euler(0, -180, 0);
                GameObject wall = Instantiate(wall_prefab);
                wall.transform.position = setTransfrom.transform.position;
                wall.transform.rotation = setTransfrom.transform.rotation;
            }

            for (int i_depth = 0; i_depth < bl_depth; i_depth++)
            {
                setTransfrom.transform.localPosition = new Vector3(bl_width, i_height, i_depth);
                setTransfrom.transform.localRotation = Quaternion.Euler(0, 270, 0);
                GameObject wall = Instantiate(wall_prefab);
                wall.transform.position = setTransfrom.transform.position;
                wall.transform.rotation = setTransfrom.transform.rotation;
            }

            for (int i_width = 0; i_width < bl_width; i_width++)
            {
                setTransfrom.transform.localPosition = new Vector3(i_width, i_height, bl_depth);
                setTransfrom.transform.localRotation = Quaternion.Euler(0, 0, 0);
                GameObject wall = Instantiate(wall_prefab);
                wall.transform.position = setTransfrom.transform.position;
                wall.transform.rotation = setTransfrom.transform.rotation;
            }

            for (int i_depth = 0; i_depth < bl_depth; i_depth++)
            {
                setTransfrom.transform.localPosition = new Vector3(0, i_height, i_depth);
                setTransfrom.transform.localRotation = Quaternion.Euler(0, 270, 0);
                GameObject wall = Instantiate(wall_prefab);
                wall.transform.position = setTransfrom.transform.position;
                wall.transform.rotation = setTransfrom.transform.rotation;
            }
        }
    }

    void Update()
    {
        
    }


}
