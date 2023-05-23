using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class FlexibleUIInstance : Editor
{

    [MenuItem("GameObject/Flexible UI/Rectangle Button", priority = 0)]
    public static void AddButton()
    {
        Create("FCF RectButton");
    }

    [MenuItem("GameObject/Flexible UI/Square Button", priority = 0)]
    public static void AddButton1()
    {
        Create("FCF SquareButton");
    }

    static GameObject clickedObject;

    private static GameObject Create(string objectName)
    {
        GameObject instance = Instantiate(Resources.Load<GameObject>(objectName));
        instance.name = objectName;
        clickedObject = UnityEditor.Selection.activeObject as GameObject;
        if (clickedObject != null)
        {
            instance.transform.SetParent(clickedObject.transform, false);
        }
        return instance;
    }

}