using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using System;

public class RVText : RVControlBase
{
    bool isSelected = false;
    public RVText(string nameLabel, object data, int depth, RVVisibility rvVisibility)
        : base(nameLabel, data, depth, rvVisibility)
    {
        Init();
    }

    void Init()
    {
    }

    public override void OnGUIUpdate(bool isRealtimeUpdate, RVSettingData settingData, RVCStatus rvcStatus)
    {
        string s = GetValueString(this.data);

        isSelected = rvcStatus.IsSelected(this.NameLabel);
        if (isSelected == false)
            isSelected = rvcStatus.IsSelected(s);

        GUIStyle guiStyle = SelectNameGUIStyle(settingData);
        GUIStyle value_guiStyle = SelectValueGUIStyle(settingData);

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("     ", GUILayout.Width(depth * RVControlBase.Indent_field));

        EditorGUILayout.LabelField(this.NameLabel + " :", guiStyle, GUILayout.Width(GetWidth(this.NameLabel, settingData)));
        CopyMenu(GUILayoutUtility.GetLastRect(), settingData, this.NameLabel, s);

        if (this.data == null && this.rvVisibility.ValueTypeIsString() == false)
            EditorGUILayout.LabelField(s, settingData.Get_value_null());
        else
            EditorGUILayout.LabelField(s, value_guiStyle);

        EditorGUILayout.EndHorizontal();
    }

    public override void OnDestroy()
    {
    }

    GUIStyle SelectValueGUIStyle(RVSettingData settingData)
    {
        GUIStyle _nowStyle = settingData.Get_value_others();

        if (RVHelper.IsString(this.rvVisibility.ValueType) == true)
        {
            _nowStyle = settingData.Get_value_string();
        }
        else if (RVHelper.IsEnum(this.rvVisibility.ValueType) == true)
        {
            _nowStyle = settingData.Get_value_enum();
        }
        else if (RVHelper.IsNormalType(this.rvVisibility.ValueType) == true)
        {
            _nowStyle = settingData.Get_value_digital();
        }

        return _nowStyle;
    }

    GUIStyle SelectNameGUIStyle(RVSettingData settingData)
    {
        GUIStyle _nowStyle = settingData.Get_default();
        if (this.rvVisibility.RVType == RVVisibility.NameType.Field)
        {
            if (this.rvVisibility.IsPublic == true)
                _nowStyle = settingData.Get_name_public();
            else
                _nowStyle = settingData.Get_name_private();
        }
        else if (this.rvVisibility.RVType == RVVisibility.NameType.CollectionItem)
        {
            _nowStyle = settingData.Get_name_collection_item();
        }
        else if (this.rvVisibility.RVType == RVVisibility.NameType.Property)
        {
            _nowStyle = settingData.Get_name_property();
        }


        return _nowStyle;
    }

    string GetValueString(object data)
    {
        if (data != null)
        {
            if (this.rvVisibility.ValueTypeIsString() == true)
                return "\"" + this.data.ToString() + "\"";
            else
                return this.data.ToString();
        }
        else
        {
            if (this.rvVisibility.ValueTypeIsString() == true)
                return "\"\"";
            else
                return "null";
        }
    }

    float GetWidth(string str, RVSettingData settingData)
    {
        int count = 2;
        int bigCount = 0;
        foreach (char item in str)
        {
            if (item >= 'a' && item <= 'z')
                count++;
            else
                bigCount++;
        }
        return count * settingData.LowercaseLength + bigCount * settingData.UppercaseLength;
    }

    void CopyMenu(Rect rect, RVSettingData settingData, string name, string value)
    {
        Event currentEvent = Event.current;
        Rect contextRect = new Rect(rect.x, rect.y, 1200, 16);
        if (isSelected == true)
            EditorGUI.DrawRect(contextRect, settingData.bgColor_selected);
        else
            EditorGUI.DrawRect(contextRect, new Color(0, 0, 0, 0));

        if (currentEvent.type == EventType.ContextClick)
        {
            Vector2 mousePos = currentEvent.mousePosition;
            if (contextRect.Contains(mousePos))
            {
                // Now create the menu, add items and show it
                GenericMenu menu = new GenericMenu();
                menu.AddItem(new GUIContent("Copy name"), false, delegate (object obj)
                {
                    EditorGUIUtility.systemCopyBuffer = obj.ToString();
                }, name);
                menu.AddItem(new GUIContent("Copy value"), false, delegate (object obj)
                {
                    EditorGUIUtility.systemCopyBuffer = obj.ToString();
                }, value);
                menu.ShowAsContext();
                currentEvent.Use();
            }
        }
    }

    public static void RightClickMenu(Rect rect, float width, float height, RVSettingData settingData, string menuName, GenericMenu.MenuFunction2 onClick, string text, bool isSelected)
    {
        Event currentEvent = Event.current;
        Rect contextRect = new Rect(rect.x, rect.y, width, height);
        if (isSelected == true)
            EditorGUI.DrawRect(contextRect, settingData.bgColor_selected);
        else
            EditorGUI.DrawRect(contextRect, new Color(0, 0, 0, 0));

        if (currentEvent.type == EventType.ContextClick)
        {
            Vector2 mousePos = currentEvent.mousePosition;
            if (contextRect.Contains(mousePos))
            {
                // Now create the menu, add items and show it
                GenericMenu menu = new GenericMenu();
                menu.AddItem(new GUIContent(menuName), false, onClick, text);
                menu.ShowAsContext();
                currentEvent.Use();
            }
        }
    }

    public static void OnMenuClick_Copy(object obj)
    {
        EditorGUIUtility.systemCopyBuffer = obj.ToString();
    }

}

public class RVVisibility
{
    public enum NameType
    {
        None,
        Field,
        Property,
        CollectionItem,
        Class,
    }
    public NameType RVType = RVVisibility.NameType.None;

    public Type ValueType;

    public RVVisibility()
    {

    }

    public RVVisibility(NameType n, Type v )
    {
        this.RVType = n;
        this.ValueType = v;
    }

    public bool ValueTypeIsString()
    {
        if (ValueType == null)
            return false;
        return RVHelper.IsString(ValueType);
    }

    public bool IsPublic = false;
    public bool IsPrivate = false;

    public bool PropertyCanRead = false;
    public bool PropertyCanWrite = false;

    public RVVisibility GetCopy()
    {
        return this.MemberwiseClone() as RVVisibility;
    }

}
