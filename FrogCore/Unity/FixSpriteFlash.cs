using System;
using System.Reflection;
using UnityEngine;
using System.Collections;

namespace FrogCore.Unity;

public class FixSpriteFlash : MonoBehaviour
{
    public FixSpriteFlashType flashType;
    public Func<Texture> TextureFunc;
    public MaterialPropertyBlock block;
    public IEnumerator Start()
    {
        switch (flashType)
        {
            case FixSpriteFlashType.SpriteRenderer:
                SpriteRenderer myRend = GetComponent<SpriteRenderer>();
                TextureFunc = () => myRend.sprite.texture;
                break;
        }
        yield return null;
#if UNITY
#else
        while (block == null)
        {
            block = typeof(SpriteFlash).GetField("block", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(GetComponent<SpriteFlash>()) as MaterialPropertyBlock;
            yield return null;
        }
#endif
    }
    public void Update()
    {
        if (TextureFunc != null && block != null)
            block.SetTexture("_MainTex", TextureFunc());
    }
}
public enum FixSpriteFlashType
{
    Undefined = 0,
    SpriteRenderer = 1
}