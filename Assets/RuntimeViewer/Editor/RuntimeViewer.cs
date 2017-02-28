using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;

/// <summary>
/// If you have any question. Feel free to email me! 
/// dsh0079@gmail.com or 710074758@qq.com.
/// </summary>
public class RuntimeViewer :  EditorWindow, IHasCustomMenu
{

    readonly string _ver = "2016.07.30 _ver 1.01";
    readonly int space_vertical = 8;

    public bool IsRealtimeUpdate = true;
    public static bool IsEnableForbidNames = true;

    Vector2 scrollPosition = Vector2.zero;
    GameObject nowSelectItem;
    string searchString = "";

    int nowSelectItemID =-1;
    //保存全部打开过的gameobject的开关状态, int对应GetInstanceID()
    Dictionary<int, RVCStatus> rvCStatusForAllObject = new Dictionary<int, RVCStatus>();

    //对应当前nowSelectItem上所有脚本
    Dictionary<RVCollection, RVCStatus> rvCollections = new Dictionary<RVCollection, RVCStatus>();

    RVSettingData settingData;


    void Awake()
    {
    }

    //绘制窗口时调用
    void OnGUI()
    {
        if(settingData == null)
        {
            settingData = Resources.Load<RVSettingData>("RVSettingData");
            if (settingData == null)
                Debug.LogError("RuntimeViewer -> error : can't find RVSettingData prefab ...");
        } 
        //ver text
        GUILayout.Space(space_vertical);
        GUILayout.Label(_ver);
        GUILayout.Space(space_vertical);


        GUI.color = new Color(1f, 1f, 0.8f);
        if (GUILayout.Button("-- Set Default Data --", GUILayout.Width(200)))
        {
            settingData.SetToDefault();
        }
        GUI.color = Color.white;

        GUILayout.Space(space_vertical);
        //bool control
        IsRealtimeUpdate = EditorGUILayout.Toggle("Is Open Realtime Update", IsRealtimeUpdate);

        // IsRealtimeUpdate = EditorGUILayout.Toggle("Is Open Realtime Update", IsRealtimeUpdate);
        bool b = IsEnableForbidNames;
        b = EditorGUILayout.Toggle("Is Enable Forbid List", b);
        if(IsEnableForbidNames != b)
        {
            Refresh();
        }
        IsEnableForbidNames = b;

        GUI.color = new Color(0.8f, 1f, 0.8f);
        if (GUILayout.Button("-- Refresh --", GUILayout.Width(200)))
        {
            Refresh();
        }
        GUI.color = Color.white;

        // search control
        GUILayout.Space(2);
        SearchBar();

        //content
        GUILayout.Space(space_vertical);
        UpdateNowSelectItem();

        EditorGUILayout.BeginHorizontal();
        if (nowSelectItem != null)
            GUILayout.Label("Select Item : " + nowSelectItem.name);
        else
            GUILayout.Label("Select Item : is null");

        FoldRVCStatus();
        EditorGUILayout.EndHorizontal();

        if (nowSelectItem != null)
        {
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            foreach (var item in rvCollections)
            {
                item.Value.SearchString = this.searchString.ToLower();
                item.Key.OnGUIUpdate(IsRealtimeUpdate, settingData, item.Value);
            }
            EditorGUILayout.EndScrollView();
        }
    }

    private void Refresh()
    {
        if (nowSelectItem != null)
        {
            rvCollections = CreateRVCollections();
            nowSelectItemID = nowSelectItem.GetInstanceID();
        }
    }

    void SearchBar()
    {
        GUILayout.BeginHorizontal(GUI.skin.FindStyle("Toolbar"));
        GUIStyle s = GUI.skin.FindStyle("ToolbarSeachTextField");
        searchString = GUILayout.TextField(searchString, s);

        Rect r = GUILayoutUtility.GetLastRect();
        r = new Rect(r.x - 10, r.y - 8, r.width, r.height);

        RVText.RightClickMenu(r, 1200, 36, settingData, "Paste",
            delegate (object obj)
            {
                if (EditorGUIUtility.systemCopyBuffer == null)
                    searchString = "";
                else
                    searchString = EditorGUIUtility.systemCopyBuffer;
            }
            , "", false);

        if (GUILayout.Button("", GUI.skin.FindStyle("ToolbarSeachCancelButton")))
        {
            searchString = "";// Remove focus if cleared
            GUI.FocusControl(null);
        }

        GUILayout.EndHorizontal();
    }

    //当窗口关闭时调用
    void OnDestroy()
    {
        if (rvCollections != null && rvCollections.Count > 0)
        {
            foreach (var item in rvCollections)
            {
                item.Key.OnDestroy();
            }
        }
    }

    void UpdateNowSelectItem()
    {
        if (locked == true)
            return;

        nowSelectItem = null;
        foreach (Transform t in Selection.transforms)
        {
            if (t != null && t.gameObject != null)
            {
                nowSelectItem = t.gameObject;
                break;
            }
        }

        if (nowSelectItem != null)
        {
            //切换物体
            if (nowSelectItemID != nowSelectItem.GetInstanceID())
            {
                rvCollections = CreateRVCollections();
                nowSelectItemID = nowSelectItem.GetInstanceID();
                this.searchString = "";
                scrollPosition = Vector2.zero;
            }
        }
    }

    void FoldRVCStatus()
    {
        if (nowSelectItem == null || rvCollections == null || rvCollections.Count == 0)
            return;

        if (GUILayout.Button("- fold all -", GUILayout.Width(80)))
        {
            foreach (var item in rvCollections)
            {
                if (item.Value == null || item.Value.IsOpens == null)
                    continue;
                item.Value.IsOpens.Clear();
            }
        }
    }

    Dictionary<RVCollection, RVCStatus> CreateRVCollections()
    {
        Dictionary<RVCollection, RVCStatus> result = new Dictionary<RVCollection, RVCStatus>();

        if(nowSelectItem != null)
        {
            Component[] c = RVHelper.GetComponent(nowSelectItem);
            if (c != null)
            {
                foreach (var item in c)
                {
                    if (item == null)
                        continue;
                    RVVisibility rvv = new RVVisibility();
                    rvv.RVType = RVVisibility.NameType.Class;

                    string rvcName = item.GetType().ToString().Replace("UnityEngine.", "");
                    if (IsContainsKey(result, rvcName) == true)
                        rvcName += "#"+item.GetInstanceID();

                    RVCollection rvc = new RVCollection(nowSelectItem.name, rvcName, item, 0, rvv);

                    RVCStatus rvCStatus = new RVCStatus();

                    if (rvCStatusForAllObject.ContainsKey(nowSelectItem.GetInstanceID()) == true)
                        rvCStatus = rvCStatusForAllObject[nowSelectItem.GetInstanceID()];
                    else
                        rvCStatusForAllObject.Add(nowSelectItem.GetInstanceID(), rvCStatus);

                    result.Add(rvc, rvCStatus);
                }
            }
        }
        return result;
    }

    bool IsContainsKey(Dictionary<RVCollection, RVCStatus> result, string keyName)
    {
        foreach (var item in result)
        {
            if (item.Key.NameLabel == keyName)
                return true;
        }
        return false;
    }

    #region window base thing ... 

    //更新
    void Update()
    {

    }

    //当窗口获得焦点时调用一次
    void OnFocus()
    {
    }

    //当窗口丢失焦点时调用一次
    void OnLostFocus()
    {
    }
    void OnInspectorUpdate()
    {
        this.Repaint();
    }

    [MenuItem("Window/RuntimeViewer")]
    static void AddWindow()
    {
        //创建窗口
        RuntimeViewer window = EditorWindow.GetWindow<RuntimeViewer>(false, "RuntimeViewer", true);
        window.Show();
    }

    #endregion

    #region lock thing ... 

    /// Keep local copy of lock button style for efficiency.
    [System.NonSerialized]
    GUIStyle lockButtonStyle;

    /// Indicates whether lock is toggled on/off.
    [System.NonSerialized]
    bool locked = false;

    /// <summary>
    /// Magic method which Unity detects automatically.
    /// </summary>
    /// <param name="position">Position of button.</param>
    void ShowButton(Rect position)
    {
        if (lockButtonStyle == null)
            lockButtonStyle = "IN LockButton";

        if (nowSelectItem == null)
        {
            locked = GUI.Toggle(position, false, GUIContent.none, lockButtonStyle);
        }
        else
        {
            locked = GUI.Toggle(position, locked, GUIContent.none, lockButtonStyle);
        }
    }

    /// <summary>
    /// Adds custom items to editor window context menu.
    /// </summary>
    /// <remarks>
    /// <para>This will only work for Unity 4.x+</para>
    /// </remarks>
    /// <param name="menu">Context menu.</param>
    void IHasCustomMenu.AddItemsToMenu(GenericMenu menu)
    {
        menu.AddItem(new GUIContent("Lock"), locked, () => {
            locked = !locked;
        });
    }
    #endregion
}


public class RVCStatus
{
    public Dictionary<string, bool> IsOpens = new Dictionary<string, bool>();
    public string SearchString = "";

    public bool IsSelected(string nameLabel)
    {
        if (nameLabel == null || SearchString == null || SearchString == "")
            return false;
        if (nameLabel.ToLower().Contains(SearchString) == true)
            return true;

        return false;
    }
}

