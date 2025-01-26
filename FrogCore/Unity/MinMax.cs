using UnityEngine;
using System;

namespace FrogCore.Unity;

public abstract class MinMax {}

[Serializable]
public class FloatMinMax : MinMax
{
    public FloatMinMax() {}
    public FloatMinMax(float min, float max) {this.min = min; this.max = max;}

    public float min = 0f;
    public float max = 1f;

    public float GetRandom() => UnityEngine.Random.Range(min, max);
}

[Serializable]
public class IntMinMax : MinMax
{
    public IntMinMax() {}
    public IntMinMax(int min, int max) {this.min = min; this.max = max;}

    public int min = 0;
    public int max = 1;

    public int GetRandom() => UnityEngine.Random.Range(min, max + 1);
}

[Serializable]
public class Vector2MinMax : MinMax
{
    public Vector2MinMax() {}
    public Vector2MinMax(Vector2 min, Vector2 max) {this.min = min; this.max = max;}

    public Vector2 min = new Vector2(0f, 0f);
    public Vector2 max = new Vector2(1f, 1f);

    public Vector2 GetRandom() => new Vector2(UnityEngine.Random.Range(min.x, max.x), UnityEngine.Random.Range(min.y, max.y));
}