using UnityEngine;
using System;

namespace FrogCore.Unity;

[Serializable]
public class ScriptableObjectWrapper<T> : IScriptableObjectWrapper where T : ScriptableObject
{
    private bool _loaded;
    private T _obj;
    private string _bundleName;
    private string _path;

    public string bundleName {get => _bundleName; private set => _bundleName = value;}
    public string path {get => _path; private set => _path = value;}

    public static AssetBundle GetBundle(string name) => null;

    public T Obj {get => GetScriptableObject() as T;}
    
    public ScriptableObject GetScriptableObject(bool force = false)
    {
        if (!force && _loaded)
            return _obj;

        _loaded = true;
        if (!string.IsNullOrEmpty(bundleName))
            _obj = GetBundle(bundleName).LoadAsset<T>(path);
        else
            _obj = Resources.Load<T>(path);
        return _obj;
    }

    public void SetScriptableObject(ScriptableObject obj, string path, string bundleName, bool force = false)
    {
        if (obj == _obj)
        {
            _path = path;
            _bundleName = bundleName;
        }
        else if (obj)
        {
            if (!(obj is T tObj))
                throw new Exception("Invalid ScriptableObject Type");
            _path = path;
            _bundleName = bundleName;
            _obj = tObj;
            _loaded = true;
        }
        else
        {
            _path = path;
            _bundleName = bundleName;
            _loaded = false;
            if (force)
                GetScriptableObject(true);
        }
    }

    public Type GetObjectType() => typeof(T);
}

public interface IScriptableObjectWrapper
{
    string bundleName {get;}
    string path {get;}

    Type GetObjectType();

    ScriptableObject GetScriptableObject(bool force = false);

    void SetScriptableObject(ScriptableObject obj, string path, string bundleName, bool force = false);
}