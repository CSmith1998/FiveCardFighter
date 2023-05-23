using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode()]
public class FCF_UI : MonoBehaviour
{
    [SerializeField]
    protected FCF_UI_Data SkinData;

    protected virtual void OnSkinUI() { }
    protected virtual void Start() { OnSkinUI(); }

    //protected virtual void Start() { }

    protected virtual void Update() { 
        if (Application.isEditor) { OnSkinUI(); } 
    }
}
