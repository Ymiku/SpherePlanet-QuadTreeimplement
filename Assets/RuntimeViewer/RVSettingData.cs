using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RVSettingData : MonoBehaviour
{
    public Color bgColor_selected = Color.black;

    public Color name_public = Color.black;
    public Color name_private = Color.black;
    public Color name_collection = Color.black;
    public Color name_collection_onSelect = Color.black;
    public Color name_collection_item = Color.black;
    public Color name_property = Color.black;
    public Color name_class = Color.black;
    public Color name_class_onSelect = Color.black;

    //public Color textColor_property;
    public float LowercaseLength = 7.5f;
    public float UppercaseLength = 9f;
    public Color value_string = Color.black;
    public Color value_digital = Color.black;
    public Color value_null = Color.black;
    public Color value_enum = Color.black;
    public Color value_others = Color.black;

    public FontStyle FontStyle = FontStyle.Bold;
    public FontStyle ValueFontStyle = FontStyle.Bold;
    public string[] ForbidNames;
    public string[] ForbidNamesIfContains;


    public GUIStyle Get_default()
    {
        GUIStyle style = new GUIStyle();
        style.fontStyle = ValueFontStyle;
        return style;
    }

    public GUIStyle Get_name_public()
    {
        GUIStyle style = new GUIStyle();
        style.normal.textColor = name_public;
        style.fontStyle = FontStyle;
        //  style.normal.background = GerTexture2D(bgColor_public);

        return style;
    }
    public GUIStyle Get_name_private()
    {
        GUIStyle style = new GUIStyle();
        style.normal.textColor = name_private;
        //     style.normal.background = GerTexture2D(bgColor_private);
        style.fontStyle = FontStyle;
        return style;
    }
    public GUIStyle Get_name_container(GUIStyle g)
    {
        GUIStyle foldoutStyle = new GUIStyle(g);
        foldoutStyle.normal.textColor = name_collection;
        foldoutStyle.focused.textColor = name_collection;
        foldoutStyle.active.textColor = name_collection_onSelect;
        foldoutStyle.hover.textColor = name_collection_onSelect;
        foldoutStyle.onNormal.textColor = name_collection_onSelect;
        foldoutStyle.onActive.textColor = name_collection_onSelect;
        foldoutStyle.onFocused.textColor = name_collection_onSelect;
        foldoutStyle.onHover.textColor = name_collection_onSelect;
        foldoutStyle.fontStyle = FontStyle;
        return foldoutStyle;
    }
    public GUIStyle Get_name_collection_item()
    {
        GUIStyle style = new GUIStyle();
        style.normal.textColor = name_collection_item;
        style.fontStyle = FontStyle;
        return style;
    }
    public GUIStyle Get_name_property()
    {
        GUIStyle style = new GUIStyle();
        style.normal.textColor = name_property;
        style.fontStyle = FontStyle;
        return style;
    }
    public GUIStyle Get_name_class(GUIStyle g)
    {
        GUIStyle foldoutStyle = new GUIStyle(g);
        foldoutStyle.normal.textColor = name_class;
        foldoutStyle.focused.textColor = name_class;
        foldoutStyle.active.textColor = name_class_onSelect;
        foldoutStyle.hover.textColor = name_class_onSelect;
        foldoutStyle.onNormal.textColor = name_class_onSelect;
        foldoutStyle.onActive.textColor = name_class_onSelect;
        foldoutStyle.onFocused.textColor = name_class_onSelect;
        foldoutStyle.onHover.textColor = name_class_onSelect;
        foldoutStyle.fontStyle = FontStyle;
        return foldoutStyle;
    }

    public GUIStyle Get_value_others()
    {
        GUIStyle style = new GUIStyle();
        style.normal.textColor = value_others;
        style.fontStyle = ValueFontStyle;
        return style;
    }
    public GUIStyle Get_value_string()
    {
        GUIStyle style = new GUIStyle();
        style.normal.textColor = value_string;
        style.fontStyle = ValueFontStyle;
        return style;
    }
    public GUIStyle Get_value_digital()
    {
        GUIStyle style = new GUIStyle();
        style.normal.textColor = value_digital;
        style.fontStyle = ValueFontStyle;
        return style;
    }
    public GUIStyle Get_value_null()
    {
        GUIStyle style = new GUIStyle();
        style.normal.textColor = value_null;
        style.fontStyle = ValueFontStyle;
        return style;
    }
    public GUIStyle Get_value_enum()
    {
        GUIStyle style = new GUIStyle();
        style.normal.textColor = value_enum;
        style.fontStyle = ValueFontStyle;
        return style;
    }

    public bool IsForbid(string _name)
    {
        if (_name == null)
            return false;
        if (ForbidNames != null)
        {
            for (int i = 0; i < ForbidNames.Length; i++)
            {
                if (string.IsNullOrEmpty(ForbidNames[i]) == true)
                    continue;

                if (ForbidNames[i] == _name)
                    return true;
            }
        }

        if (ForbidNamesIfContains != null)
        {
            for (int i = 0; i < ForbidNamesIfContains.Length; i++)
            {
                if (string.IsNullOrEmpty(ForbidNamesIfContains[i]) == true)
                    continue;

                if (_name.Contains(ForbidNamesIfContains[i]) == true)
                    return true;
            }
        }
        return false;
    }

    public void SetToDefault()
    {
        bgColor_selected = new Color(255f / 255f, 237f / 255f, 0f / 255f, 75f/255f);
        name_public = new Color(0f / 255f, 2f / 255f, 244f / 255f, 1f);
        name_private = new Color(19f / 255f, 73f / 255f, 199f / 255f, 1f);
        name_collection = new Color(1f / 255f, 103f / 255f, 19f / 255f, 1f);
        name_collection_onSelect = new Color(0f / 255f, 131f / 255f, 29f / 255f, 1f);
        name_collection_item = new Color(62f / 255f, 116f / 255f, 242f / 255f, 1f);
        name_property = new Color(146f / 255f, 73f / 255f, 0f / 255f, 1f);
        name_class = new Color(0f / 255f, 94f / 255f, 103f / 255f, 1f);
        name_class_onSelect = new Color(0f / 255f, 141f / 255f, 154f / 255f, 1f);

        LowercaseLength = 7.5f;
        UppercaseLength = 9f;
        value_others = new Color(23f / 255f, 23f / 255f, 23f / 255f, 1f);
        value_string = new Color(154f / 255f, 49f / 255f, 5f / 255f, 1f);
        value_digital = new Color(56f / 255f, 56f / 255f, 56f / 255f, 1f);
        value_null = new Color(189f / 255f, 28f / 255f, 28f / 255f, 1f);
        value_enum = new Color(185f / 255f, 0f / 255f, 188f / 255f, 1f);
        this.FontStyle = FontStyle.Bold;
        this.ValueFontStyle = FontStyle.BoldAndItalic;

        List<string> list = new List<string>();
        list.Add("useGUILayout");
        list.Add("isActiveAndEnabled");
        list.Add("rigidbody");
        list.Add("rigidbody2D");
        list.Add("camera");
        list.Add("light");
        list.Add("animation");
        list.Add("constantForce");
        list.Add("audio");
        list.Add("guiText");
        list.Add("guiElement");
        list.Add("guiTexture");
        list.Add("hingeJoint");
        list.Add("particleEmitter");
        list.Add("particleSystem");
        list.Add("collider");
        list.Add("collider2D");
        list.Add("hideFlags");
        list.Add("worldToLocalMatrix");
        list.Add("localToWorldMatrix");
        list.Add("hasChanged");
        list.Add("guiText");
        list.Add("guiElement");
        list.Add("guiTexture");
        list.Add("collider");
        list.Add("collider2D");
        list.Add("hingeJoint");
        list.Add("particleEmitter");
        list.Add("UnityEngine");
        list.Add("hideFlags");
        list.Add("isStatic");
        list.Add("rigidbody");
        list.Add("rigidbody2D");
        list.Add("camera");
        list.Add("light");
        list.Add("constantForce");
        list.Add("audio");
        ForbidNames = list.ToArray();

        List<string> list2 = new List<string>();
        list2.Add("k__BackingField");
        ForbidNamesIfContains = list2.ToArray();
    }
}
