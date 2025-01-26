using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FrogCore.Unity;

[CreateAssetMenu(fileName = "NewAnim", menuName = "Sprite Animation/Sprite Animation")]
public class SpriteAnimation : ScriptableObject
{
    public Sprite[] frames = new Sprite[0];
    public float fps = 15f;
    public SpriteAnimationLoopType loopType;
    public int loopStart = 0;
}
