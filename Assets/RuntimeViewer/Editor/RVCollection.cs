using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using System.Reflection;
using System;

public class RVCollection : RVControlBase
{
    //Dictionary<string, RVControlBase> children = new Dictionary<string, RVControlBase>();
    List<RVControlBase> children = new List<RVControlBase>();
    string parentNameLabel;
    bool isFirstOpen = true;
    RVSettingData settingData;
    RVCStatus rvcStatus;


    public RVCollection(string parentNameLabel, string nameLabel, object data, int depth, RVVisibility rvVisibility)
         : base(nameLabel, data, depth, rvVisibility)
    {
        this.parentNameLabel = parentNameLabel;
    }

    public override void OnGUIUpdate(bool isRealtimeUpdate, RVSettingData settingData, RVCStatus rvcStatus)
    {
        this.settingData = settingData;
        this.rvcStatus = rvcStatus;
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("     ", GUILayout.Width(depth * RVControlBase.Indent_class));

        string rvcStatusName = this.parentNameLabel + this.NameLabel;
        if (rvcStatus.IsOpens.ContainsKey(rvcStatusName) == false)
            rvcStatus.IsOpens.Add(rvcStatusName, false);


        EditorGUILayout.BeginVertical();
        rvcStatus.IsOpens[rvcStatusName] = CollectionUI(rvcStatus.IsOpens[rvcStatusName], settingData);
        if (rvcStatus.IsOpens[rvcStatusName] == true)
            {
            if (isFirstOpen == true)
            {
                AnalyzeAndAddChildren();
                isFirstOpen = false;
            }
            else if (isRealtimeUpdate == true)
            {
                AnalyzeAndAddChildren();
            }

            foreach (var item in children)
            {
                item.OnGUIUpdate(isRealtimeUpdate, settingData, rvcStatus);
            }
        }
        EditorGUILayout.EndVertical();
        EditorGUILayout.EndHorizontal();
    }

    public override void OnDestroy()
    {
        foreach (var item in children)
        {
            //item.Value.OnDestroy();
            item.OnDestroy();
        }
        children.Clear();
    }

    protected virtual void AnalyzeAndAddChildren()
    {
        OnDestroy();
        if (data == null)
        {
            return;
        }

        Type t = data.GetType();

        if (RVHelper.IsCollection(t) == true)
        {
            children.AddRange(AnalyzeCollection(data, t));
        }
        else
        {
            children.AddRange(AnalyzeClass(data, t));
        }
    }

    RVControlBase CreateControl(object ob, string fieldName, RVVisibility rvVisibility)
    {
        RVControlBase b = null;

        if (IsNull(ob) == true)
        {
            b = new RVText(fieldName, null, depth + 1, rvVisibility);
        }
        else
        {
            Type t = ob.GetType();
            if (RVHelper.IsCanToStringDirently(t) == true)
            {
                b = new RVText(fieldName, ob, depth + 1, rvVisibility);
            }
            else
            {
                RVVisibility rvv = rvVisibility.GetCopy();
                if (RVHelper.IsCollection(t) == false)
                    rvv.RVType = RVVisibility.NameType.Class;
                //    Debug.Log("--------------" + this.NameLabel);
                b = new RVCollection(this.parentNameLabel + this.NameLabel, GetSpecialNameLabel(ob, fieldName), ob, depth + 1, rvv);
            }
        }

        return b;
    }

    bool IsForbidThis(object obj, string fieldName)
    {
        if (obj == null)
            return false;

        if (Application.isPlaying == false)
        {
            if (obj is MeshFilter)
                return true;
            if (obj is Renderer)
                return true;
            if (obj is Collider)
                return true;
        }

        if (RuntimeViewer.IsEnableForbidNames == true)
        {
            if (this.settingData.IsForbid(fieldName) == true)
                return true;
        }
        return false;
    }

    string GetSpecialNameLabel(object ob, string fieldName)
    {
        if (ob == null)
        {
            return fieldName;
        }
        else if (ob is UnityEngine.Object && (ob as UnityEngine.Object) != null)
        {
            fieldName += " : " + (ob as UnityEngine.Object).name;
        }
        else if (RVHelper.IsCollection(ob.GetType()) == true)
        {
            if (typeof(IDictionary).IsAssignableFrom(ob.GetType()) == true)
                fieldName += " : <dictionary>";
            else
                fieldName += " : <collection>";
        }

        return fieldName;
    }

    bool IsNull(object ob)
    {
        if (ob == null)
        {
            return true;
        }
        else if (ob is UnityEngine.Object && (ob as UnityEngine.Object) == null)
        {
            return true;
        }

        return false;
    }

    //所有集合判断,数组,字典,list等等
    List<RVControlBase> AnalyzeCollection(object ob, Type type)
    {
        List<RVControlBase> result = new List<RVControlBase>();

        if (typeof(IDictionary).IsAssignableFrom(type) == true)//是个字典
        {
            IDictionary dic = ob as IDictionary;
            foreach (DictionaryEntry item in dic)
            {
                string _key = "null";
                Type _type = null;

                if (IsNull(item.Key) == false)
                    _key = item.Key.ToString();
                if (IsNull(item.Value) == false)
                {
                    _type = item.Value.GetType();
                }

                RVVisibility rvv = new RVVisibility(RVVisibility.NameType.CollectionItem, _type);
                result.Add(CreateControl(item.Value, "[" + _key + "]", rvv));
            }
        }
        else if (typeof(ICollection).IsAssignableFrom(type) == true) //是个集合
        {
            foreach (var _v in (ICollection)ob)
            {
                string str = "item";
                Type _type = null;
                if (IsNull(_v) == false)
                {
                    _type = _v.GetType();
                    if(RVHelper.IsString(_type) == true || RVHelper.IsCanToStringDirently(_type) == true)
                        str = "item";
                    else if (RVHelper.IsNormalType(_type) == false )
                        str = _v.ToString();
                }

                RVVisibility rvv = new RVVisibility(RVVisibility.NameType.CollectionItem, _type);
                result.Add(CreateControl(_v, str, rvv));
            }
        }

        return result;
    }

    List<RVControlBase> AnalyzeClass(object data, Type thisType)
    {
        List<RVControlBase> result = new List<RVControlBase>();

        while (thisType.IsSubclassOf(typeof(object)))
        {
            FieldInfo[] fields = thisType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
            foreach (FieldInfo field in fields)
            {
                if (IsForbidThis(data, field.Name) == true)
                {
                    continue;
                }
                object value = field.GetValue(data);

                RVVisibility rvv = new RVVisibility();
                rvv.RVType = RVVisibility.NameType.Field;
                rvv.ValueType = field.FieldType;
                rvv.IsPrivate = field.IsPrivate;
                rvv.IsPublic = field.IsPublic;

                RVControlBase cb = CreateControl(value, field.Name, rvv);
                if (result.Contains(cb) == false)
                    result.Add(cb);
            }

            PropertyInfo[] properties = thisType.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
            foreach (PropertyInfo property in properties)
            {
                object value = null;
                try
                {
                    if (IsForbidThis(data, property.Name) == true)
                    {
                        continue;
                    }
                    value = property.GetValue(data, null);
                }
                catch
                {
                    value = null;
                }

                RVVisibility rvv = new RVVisibility();
                rvv.RVType = RVVisibility.NameType.Property;
                rvv.PropertyCanRead = property.CanRead;
                rvv.PropertyCanWrite = property.CanWrite;
                rvv.ValueType = property.PropertyType;

                RVControlBase cb = CreateControl(value, property.Name, rvv);
                if (result.Contains(cb) == false)
                    result.Add(cb);
            }

            thisType = thisType.BaseType;
        }
        //
        //result.Sort((a, b) =>
        //{
        //    return string.Compare(a.NameLabel, b.NameLabel, false, System.Globalization.CultureInfo.InvariantCulture);
        //});

        return result;
    }

    GUIStyle GetCollectionGUIStyle(RVSettingData settingData)
    {
        if (this.rvVisibility.RVType == RVVisibility.NameType.Class)
            return settingData.Get_name_class(EditorStyles.foldout);

        return settingData.Get_name_container(EditorStyles.foldout);
    }


    bool CollectionUI(bool isOpen, RVSettingData settingData)
    {
        bool isSelected = this.rvcStatus.IsSelected(this.NameLabel);

        GUIStyle guistyle = settingData.Get_name_container(EditorStyles.foldout);

        if (this.rvVisibility.RVType == RVVisibility.NameType.Class)
            guistyle = settingData.Get_name_class(EditorStyles.foldout);

        isOpen = EditorGUILayout.Foldout(isOpen, new GUIContent(this.NameLabel), guistyle);
        RVText.RightClickMenu(GUILayoutUtility.GetLastRect(), 1200, 16, settingData, "Copy", RVText.OnMenuClick_Copy, this.NameLabel, isSelected);

        return isOpen;
    }
}
