using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

using ButtonShape = FCF_UI_Data.ButtonShape;
using ButtonFormat = FCF_UI_Data.ButtonFormat;
using ButtonType = FCF_UI_Data.ButtonType;
using ButtonContent = FCF_UI_Data.ButtonContent;

using OperationType = FCF_UI_Data.OperationType;
using Unity.VisualScripting;

[RequireComponent(typeof(Button)), 
    RequireComponent(typeof(Image)),
    System.Serializable]
public class FCF_UI_Button : FCF_UI
{
    protected Button btn;
    protected Image img;

    protected GameObject txtContent;
    protected GameObject iconContent;
    protected GameObject imgFrame;
    protected TextMeshProUGUI text;
    protected Image icon;
    protected Image frame;

    protected string previousName;

    [SerializeField] protected ButtonShape shape;
    [SerializeField] protected ButtonFormat format;
    [SerializeField] protected ButtonType type;
    [SerializeField] protected ButtonContent content;

    [SerializeField] protected Vector3 buttonSize;

    [SerializeField, InspectorLabel("Button Text [Optional] (TEXT type ONLY)")] protected string btnText;
    [SerializeField, InspectorLabel("Button Image [Optional] (ICON type ONLY)")] protected Sprite btnIcon;

    protected override void Start() {
        //InitializeScript();
        if(btnText != null) {
            if(btnIcon != null) {
                SkinData.ButtonHandler(gameObject, buttonSize, shape, format, type, content, OperationType.INSTANTIATE, btnText, btnIcon);
            } else SkinData.ButtonHandler(gameObject, buttonSize, shape, format, type, content, OperationType.INSTANTIATE, btnText);
        } else {
            if(btnIcon != null) {
                SkinData.ButtonHandler(gameObject, buttonSize, shape, format, type, content, OperationType.INSTANTIATE, "", btnIcon);
            } else SkinData.ButtonHandler(gameObject, buttonSize, shape, format, type, content, OperationType.INSTANTIATE);
        }
    }

    protected override void Update() {
        base.Update();
    }

    protected override void OnSkinUI() {
        SkinData.ButtonHandler(gameObject, buttonSize, shape, format, type, content, OperationType.UPDATE, btnText);
    }
}
