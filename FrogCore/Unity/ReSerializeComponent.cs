using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace FrogCore.Unity;

public class ReSerializeComponent : MonoBehaviour
{
    public string componentTypeToReSerialize;
    public string json;

    public virtual void Awake()
    {
        Debug.Log(json);
        
        DeserializeTo(json);
    }

    public string SerializeFrom()
    {
        MonoBehaviour componentToReSerialize = null;
        foreach (MonoBehaviour component in GetComponents<MonoBehaviour>())
        {
            if (component.GetType().Name == componentTypeToReSerialize)
            {
                componentToReSerialize = component;
                break;
            }
        }
        JsonSerializerSettings settings = new JsonSerializerSettings() {ContractResolver = new IgnoreUnityPropertiesResolver(), NullValueHandling = NullValueHandling.Ignore, Formatting = Formatting.Indented, Converters = new List<JsonConverter>() {new ClearListConverter()}};//, new UnityStructConverter()}});
        string s = JsonConvert.SerializeObject(componentToReSerialize, settings);
        return s;
    }

    public void DeserializeTo(string json)
    {
        MonoBehaviour componentToReSerialize = null;
        foreach (MonoBehaviour component in GetComponents<MonoBehaviour>())
        {
            if (component.GetType().Name == componentTypeToReSerialize)
            {
                componentToReSerialize = component;
                break;
            }
        }
        JsonSerializerSettings settings = new JsonSerializerSettings() {ContractResolver = new IgnoreUnityPropertiesResolver(), NullValueHandling = NullValueHandling.Ignore, Formatting = Formatting.Indented, Converters = new List<JsonConverter>() {new ClearListConverter()}};//, new UnityStructConverter()}});
        JsonConvert.PopulateObject(json, componentToReSerialize, settings);
    }
}

public class IgnoreUnityPropertiesResolver : DefaultContractResolver
{
    protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
    {
        IList<JsonProperty> allProps;
        if (type.IsSubclassOf(typeof(UnityEngine.Object)) || (!string.IsNullOrEmpty(type.Namespace) && type.Namespace.Contains("UnityEngine")))
            allProps = CreateReflectionPropertiesForUnityType(type);
        else
            allProps = base.CreateProperties(type, MemberSerialization.Fields);

        return allProps.Where(FilterUnityProperties).ToList();
    }

    private bool FilterUnityProperties(JsonProperty p)
    {
        List<Type> types = new List<Type>() {p.PropertyType};

        if (types[0].IsConstructedGenericType)
        {
            foreach (Type t in types[0].GenericTypeArguments)
            {
                types.Add(t);
            }
        }

        if (types[0].IsArray)
            types.Add(types[0].GetElementType());
        
        foreach (Type type in types)
        {
            if (type == typeof(IntPtr))
                return false;
            if (type.IsSubclassOf(typeof(UnityEngine.Object)))
                return false;
        }

        return true;
    }

    private List<JsonProperty> CreateReflectionPropertiesForUnityType(Type type)
    {
        List<JsonProperty> properties = new List<JsonProperty>();
        PropertyInfo[] cachedRealProperties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);;

        foreach (FieldInfo fi in type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
        {
            MemberInfo mi = null;
            
            string subName = fi.Name.ToLower();
            if (subName.StartsWith("m_"))
                subName = subName.Substring(2);

            mi = cachedRealProperties.FirstOrDefault(p => p.Name.ToLower() == subName);
            if (mi == null)
                mi = fi;
            
            properties.Add(CreateProperty(mi, MemberSerialization.Fields));
        }

        return properties;
    }
}

public class ClearListConverter : JsonConverter
{
    public override bool CanConvert(Type objectType)
    {
        return typeof(IList).IsAssignableFrom(objectType) && objectType.IsGenericType;
    }

    public override bool CanRead {get => true;}

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        var list = existingValue as IList;
        if (list == null)
            list = Activator.CreateInstance(objectType) as IList;
        list.Clear();

        Type elementType = list.GetType().GenericTypeArguments[0];

        var array = JArray.Load(reader);
        foreach (var item in array)
        {
            list.Add(item.ToObject(elementType, serializer));
        }

        return list;
    }

    public override bool CanWrite {get => false;}

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        serializer.Serialize(writer, value);
    }
}