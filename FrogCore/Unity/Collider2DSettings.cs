using UnityEngine;
using System;

namespace FrogCore.Unity;

[Serializable]
[Collider2DType(typeof(Collider2D))]
public class Collider2DSettings 
{
    [Collider2DMatch("m_Offset")]
    public Vector2 offset = Vector2.zero;
    [Collider2DMatch("m_IsTrigger")]
    public bool isTrigger = false;

    public virtual void Apply(Collider2D col)
    {
        col.offset = offset;
        col.isTrigger = isTrigger;
    }

    public virtual void Set(Collider2D col)
    {
        offset = col.offset;
        isTrigger = col.isTrigger;
    }

    public static T EnsureCorrectCollider<T>(Collider2D col)
    {
        if (col is T typeCol)
            return typeCol;
        throw new Exception("Wrong Collider Type");
    }
}

[Serializable]
[Collider2DType(typeof(BoxCollider2D))]
public class BoxCollider2DSettings : Collider2DSettings
{
    [Collider2DMatch("m_Size")]
    public Vector2 size = Vector2.zero;

    public override void Apply(Collider2D col)
    {
        BoxCollider2D box = EnsureCorrectCollider<BoxCollider2D>(col);

        box.size = size;

        base.Apply(col);
    }

    public override void Set(Collider2D col)
    {
        BoxCollider2D box = EnsureCorrectCollider<BoxCollider2D>(col);

        size = box.size;

        base.Set(col);
    }
}

[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public class Collider2DTypeAttribute : Attribute
{
    public Type type;

    public Collider2DTypeAttribute(Type type) => this.type = type;
}

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
public class Collider2DMatchAttribute : Attribute
{
    public string match;

    public Collider2DMatchAttribute(string match) => this.match = match;
}