using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;

public class RVControlBase
{
    public int ID = -1; //只用于第一层,RuntimeViewer脚本中
    public readonly int depth = 0;

    public string NameLabel { private set; get; }
    protected object data = null;

    //层缩进长度,其实值为 Indent * depth
    public static readonly int Indent_class = 5;

    public static readonly int Indent_field = 5;

    protected RVVisibility rvVisibility;

    public RVControlBase(string nameLabel, object data, int depth, RVVisibility rvVisibility)
    {
        this.NameLabel = nameLabel;
        this.data = data;
        this.depth = depth;
        this.rvVisibility = rvVisibility;
    }

    public virtual void OnGUIUpdate(bool isRealtimeUpdate, RVSettingData settingData, RVCStatus rvcStatus)
    {
    }

    public virtual void OnDestroy()
    {

    }

    public override bool Equals(object obj)
    {
        if (obj is RVControlBase == false)
            return false;
        return this.NameLabel.Equals(((RVControlBase)obj).NameLabel);
    }

    public override int GetHashCode()
    {
        return this.NameLabel.GetHashCode();
    }
}
