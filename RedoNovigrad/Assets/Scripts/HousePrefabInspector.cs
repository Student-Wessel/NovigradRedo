using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(HousePrefabRedo))]
public class HousePrefabInspector : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        HousePrefabRedo prefab = (HousePrefabRedo)target;

        if(GUILayout.Button("Regenerate building"))
        {
            prefab.NewBuilding();
        }
    }
}
