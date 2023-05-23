using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Default Skin", menuName = "Five Card Fighter/UI/Skin Data", order = 0)]
public class FCF_UI_Data : ScriptableObject
{
    #region General Enums
        [HideInInspector] public enum OperationType { 
            INSTANTIATE, UPDATE
        }
    #endregion

    public Sprite defaultRectangleFrame;
    public Sprite defaultSquareFrame;
    public Color frameColor;

    #region Button Data
        #region Button Enums
            [HideInInspector] public enum ButtonShape { 
                RECTANGLE, SQUARE
            }
            [HideInInspector] public enum ButtonFormat { 
                TEXT, ICON
            }
            [HideInInspector] public enum ButtonType {
                STANDARD, ACCEPT, CANCEL, WARNING, DOCUMENTATION
            }
            [HideInInspector] public enum ButtonContent { 
                DEFAULT, START, SELECT, NEXT, FORFEIT, HELP
            }
        #endregion

        #region Button Sprites
            [Header("Button Sprites")]
            public Sprite btnRectangleSprite;
            [Space(5)]
            public SpriteState btnRectangleState;
            [Space(10)]
            public Sprite btnSquareSprite;
            [Space(5)]
            public SpriteState btnSquareState;
            [Space(10)]
            public Sprite btnCircleSprite;
            [Space(5)]
            public SpriteState btnCircleState;
            [Space(20)]
        #endregion

        #region Button Icons
            [Header("Button Icons")]
            public Sprite btnDefaultIcon;
            [Space(5)]
            public Sprite btnStartIcon;
            [Space(5)]
            public Sprite btnSelectIcon;
            [Space(5)]
            public Sprite btnNextIcon;
            [Space(5)]
            public Sprite btnForfeitIcon;
            [Space(5)]
            public Sprite btnHelpIcon;
            [Space(20)]
        #endregion

        #region Button Colors
            [Header("Button Colors")]
            public Color btnStandardColor;
            [Space(5)]
            public Color btnAcceptColor;
            [Space(5)]
            public Color btnCancelColor;
            [Space(5)]
            public Color btnWarningColor;
            [Space(5)]
            public Color btnDocumentationColor;
            //[Space(20)]
        #endregion

        #region Button Methods
            public void ButtonHandler(GameObject button, Vector3 buttonSize, ButtonShape shape = ButtonShape.RECTANGLE, ButtonFormat format = ButtonFormat.TEXT, ButtonType type = ButtonType.STANDARD, ButtonContent content = ButtonContent.DEFAULT, OperationType op = OperationType.INSTANTIATE, string btnText = "", Sprite btnIcon = null) {

                if(button.transform.Find("Button Frame") == null) {
                        CreateFrameObject(button.transform);
                }
                if(btnIcon == null) { 
                    btnIcon = btnDefaultIcon;
                }

                ButtonShapeHandler(ref button, buttonSize, shape, op);
                ButtonFormatHandler(ref button, format, op, btnText);
                ButtonTypeHandler(ref button, type, op);
                ButtonContentHandler(ref button, content, format, op, btnText, btnIcon);
            }
            public void ButtonShapeHandler(ref GameObject button, Vector3 buttonSize, ButtonShape shape, OperationType op) { 
                var img = button.GetComponent<Image>();
                var btn = button.GetComponent<Button>();

                var frame = button.transform.Find("Button Frame").gameObject.GetComponent<Image>();

                switch(shape) { 
                    case ButtonShape.RECTANGLE:
                        if(buttonSize.Equals(new Vector3(0, 0, 0))) { 
                            button.transform.localScale = new Vector3(2f, 0.35f, 1);
                        } else button.transform.localScale = buttonSize;

                        img.sprite = btnRectangleSprite;
                        btn.spriteState = btnRectangleState;

                        frame.sprite = defaultRectangleFrame;
                        frame.color = frameColor;
                    break; 
                    case ButtonShape.SQUARE:
                        if(buttonSize.Equals(new Vector3(0, 0, 0))) { 
                            button.transform.localScale = new Vector3(0.75f, 0.75f, 1);
                        } else button.transform.localScale = buttonSize;

                        img.sprite = btnSquareSprite;
                        btn.spriteState = btnSquareState;

                        frame.sprite = defaultSquareFrame;
                        frame.color = frameColor;
                    break;
                }

                img.type = Image.Type.Sliced;
                btn.transition = Selectable.Transition.SpriteSwap;
                btn.targetGraphic = img;
            }
            public void ButtonFormatHandler(ref GameObject button, ButtonFormat format, OperationType op, string btnText) { 
                var textContainer = button.transform.Find("Text Container");
                var iconContainer = button.transform.Find("Icon Container");
                
                //Debug.Log(format.ToString());
                switch(format) { 
                    case ButtonFormat.TEXT:
                        if(textContainer == null) {
                            CreateTextObject(btnText, button.transform);
                            textContainer = button.transform.Find("Text Container");
                        } else textContainer.gameObject.SetActive(true);
                        if(iconContainer != null) { 
                            iconContainer.gameObject.SetActive(false);
                        }
                        var textObject = textContainer.GetComponent<TextMeshProUGUI>();

                        PositionTextObject(textContainer.gameObject, button.transform);
                        FinalizeTextObjectDetails(ref textObject, button.transform);
                    break; 
                    case ButtonFormat.ICON:
                        if (iconContainer == null) {
                            CreateIconObject(button.transform);
                            iconContainer = button.transform.Find("Icon Container");
                        } else iconContainer.gameObject.SetActive(true);
                        if(textContainer != null) { 
                            textContainer.gameObject.SetActive(false);
                        }
                        var rect = iconContainer.GetComponent<RectTransform>();
                        rect.sizeDelta = new Vector2(80, 15);

                        var iconObject = iconContainer.GetComponent<Image>();
                        iconObject.transform.position = button.transform.position;
                    break;
                }
            }
            public void ButtonTypeHandler(ref GameObject button, ButtonType type, OperationType op) {
                var img = button.GetComponent<Image>();

                switch(type) { 
                    case ButtonType.STANDARD:
                        img.color = btnStandardColor;
                    break; 
                    case ButtonType.ACCEPT:
                        img.color = btnAcceptColor;
                    break;
                    case ButtonType.CANCEL:
                        img.color = btnCancelColor;
                    break;
                    case ButtonType.WARNING:
                        img.color = btnWarningColor;
                    break;
                    case ButtonType.DOCUMENTATION:
                        img.color = btnDocumentationColor;
                    break;
                }
            }
            public void ButtonContentHandler(ref GameObject button, ButtonContent content, ButtonFormat format, OperationType op, string btnText, Sprite btnIcon) {
                
                TextMeshProUGUI txt = null;
                Image img = null;
                string textToEnter = "";

                switch(format) { 
                    case ButtonFormat.TEXT:
                        txt = button.transform.Find("Text Container").gameObject.GetComponent<TextMeshProUGUI>();
                    break;
                    case ButtonFormat.ICON:
                        img = button.transform.Find("Icon Container").gameObject.GetComponent<Image>();
                    break;
                }
                

                switch(content) { 
                    case ButtonContent.DEFAULT:
                        switch(format) { 
                            case ButtonFormat.ICON:
                                img.sprite = btnIcon;
                            break;
                            case ButtonFormat.TEXT:
                                if(btnText != "") {
                                    textToEnter = btnText;
                                }
                            break;
                        }
                    break; 
                    case ButtonContent.START:
                        switch(format) { 
                            case ButtonFormat.ICON:
                                img.sprite = btnStartIcon;
                            break;
                            case ButtonFormat.TEXT:
                                if(btnText == "") { 
                                    textToEnter = "Start";
                                }
                            break;
                        }
                    break;
                    case ButtonContent.SELECT:
                        switch(format) { 
                            case ButtonFormat.ICON:
                                img.sprite = btnSelectIcon;
                            break;
                            case ButtonFormat.TEXT:
                                if(btnText == "") { 
                                    textToEnter = "Select";
                                }
                            break;
                        }
                    break;
                    case ButtonContent.NEXT:
                        switch(format) { 
                            case ButtonFormat.ICON:
                                img.sprite = btnNextIcon;
                            break;
                            case ButtonFormat.TEXT:
                                if(btnText == "") { 
                                    textToEnter = "Next Round";
                                }
                            break;
                        }
                    break;
                    case ButtonContent.FORFEIT:
                        switch(format) { 
                            case ButtonFormat.ICON:
                                img.sprite = btnForfeitIcon;
                            break;
                            case ButtonFormat.TEXT:
                                if(btnText == "") { 
                                    textToEnter = "Forfeit";
                                }
                            break;
                        }
                    break;
                    case ButtonContent.HELP:
                        switch(format) { 
                            case ButtonFormat.ICON:
                                img.sprite = btnHelpIcon;
                            break;
                            case ButtonFormat.TEXT:
                                if(btnText == "") { 
                                    textToEnter = "Help";
                                }
                            break;
                        }
                    break;
                    default:
                        switch(format) { 
                            case ButtonFormat.ICON:
                                img.sprite = btnDefaultIcon;
                            break;
                            case ButtonFormat.TEXT:
                                if(btnText == "") { 
                                    textToEnter = btnText;
                                }
                            break;
                        }
                    break;
                }
                
                //Debug.Log(textToEnter);
                txt.SetText(textToEnter);
                if(format == ButtonFormat.ICON) { img.type = Image.Type.Sliced; }
                button.name = "FCF Button: " + txt.text;
            }

            private void CreateFrameObject(Transform parent) {
                var imgFrame = new GameObject("Button Frame");
                imgFrame.transform.localScale = parent.localScale;

                var frame = imgFrame.AddComponent<Image>();
                frame.GetComponent<RectTransform>().SetParent(parent);
                frame.transform.position = parent.position;
            }
            private void CreateTextObject(string btnText, Transform parent) {
                var txtContent = new GameObject("Text Container");
                txtContent.GetComponent<Transform>().SetParent(parent);
                
                var text = txtContent.AddComponent<TextMeshProUGUI>();
                text.transform.localScale = new Vector3(1, 1, 0);

                text.SetText(btnText);
            }
            private void CreateIconObject(Transform parent) {
                var iconContent = new GameObject("Icon Container");
                iconContent.transform.localScale = parent.localScale;

                var icon = iconContent.AddComponent<Image>();
                icon.GetComponent<RectTransform>().SetParent(parent);
                icon.transform.position = parent.position;
            }

            public void PositionTextObject(GameObject text, Transform parent) {
                var rect = text.GetComponent<RectTransform>();
                //rect.SetParent(parent);
                rect.sizeDelta = new Vector2(80, 50);
            }

            public void FinalizeTextObjectDetails(ref TextMeshProUGUI text, Transform parent) {
                text.enableAutoSizing = true;
                text.fontSizeMin = 10f;
                text.fontSizeMax = 75f;
                text.alignment = TextAlignmentOptions.Center;
                text.characterWidthAdjustment = 100;
                text.margin = new Vector4(0, 0, 0, 0);
                text.transform.position = parent.position + new Vector3(-0.5f, 0.5f, 0);
            }
        #endregion
    #endregion
}
