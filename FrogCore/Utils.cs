using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using FrogCore.Ext;
using MonoMod.RuntimeDetour;
using UObject = UnityEngine.Object;
using System.Reflection;

namespace FrogCore
{
    public static class Utils
    {
        private static FieldInfo[] spriteDefinitionFields;
        private static Type spriteDefinitionType;

        public static Dictionary<string, tk2dSpriteAnimation> tk2dAnimations = new Dictionary<string, tk2dSpriteAnimation>();
        public static Dictionary<string, tk2dSpriteCollectionData> tk2dCollections = new Dictionary<string, tk2dSpriteCollectionData>();


        #region game object

        public static GameObject GOFromSprite(Sprite sprite, string name = "New GameObject")
        {
            GameObject go = new GameObject(name);
            go.AddComponent<SpriteRenderer>().sprite = sprite;
            return go;
        }

        #endregion

        #region tk2d

        public static Material CreateMaterialFromTextureTk2d(Texture tex, bool setdontdestroy = true)
        {
            Material mat = new Material(Shader.Find("tk2d/BlendVertexColor"));
            mat.mainTexture = tex;
            if (setdontdestroy)
            {
                mat.DontDestroyOnLoad();
                tex.DontDestroyOnLoad();
            }

            return mat;
        }

        public static tk2dSpriteCollectionData CreateTk2dSpriteCollection(Texture texture, string[] names, Rect[] rects, Vector2[] anchors, GameObject go)
        {
            if (texture != null)
            {
                texture.DontDestroyOnLoad();
                var spriteCollection = CreateFromTexture(go, texture, new tk2dSpriteCollectionSize(), new Vector2((float) texture.width, (float) texture.height), names, rects, null, anchors, new bool[6]);
                string text = "SpriteFromTexture " + texture.name;
                spriteCollection.spriteCollectionName = text;
                spriteCollection.spriteDefinitions[0].material.name = text;
                spriteCollection.spriteDefinitions[0].material.hideFlags = (HideFlags.HideInInspector | HideFlags.DontSaveInEditor | HideFlags.DontSaveInBuild | HideFlags.DontUnloadUnusedAsset);
                spriteCollection.DontDestroyOnLoad();
                return spriteCollection;
            }

            throw new NullReferenceException();
        }

        public static tk2dSpriteCollectionData CreateFromTexture(GameObject parentObject, Texture texture, tk2dSpriteCollectionSize size, Vector2 textureDimensions, string[] names, Rect[] regions, Rect[] trimRects, Vector2[] anchors, bool[] rotated)
        {
            tk2dSpriteCollectionData tk2dSpriteCollectionData = parentObject.GetOrAddComponent<tk2dSpriteCollectionData>();
            tk2dSpriteCollectionData.material = CreateMaterialFromTextureTk2d(texture);
            tk2dSpriteCollectionData.materials = new Material[]
            {
                tk2dSpriteCollectionData.material
            };
            tk2dSpriteCollectionData.textures = new Texture[]
            {
                texture
            };
            float scale = 2f * size.OrthoSize / size.TargetHeight;
            Rect trimRect = new Rect(0f, 0f, 0f, 0f);
            tk2dSpriteCollectionData.spriteDefinitions = new tk2dSpriteDefinition[regions.Length];
            for (int i = 0; i < regions.Length; i++)
            {
                if (trimRects != null)
                {
                    trimRect = trimRects[i];
                }
                else
                {
                    trimRect.Set(0f, 0f, regions[i].width, regions[i].height);
                }

                tk2dSpriteCollectionData.spriteDefinitions[i] = CreateDefinitionForRegionInTexture(names[i], textureDimensions, scale, regions[i], trimRect, anchors[i], false);
            }

            foreach (tk2dSpriteDefinition tk2dSpriteDefinition in tk2dSpriteCollectionData.spriteDefinitions)
            {
                tk2dSpriteDefinition.material = tk2dSpriteCollectionData.material;
            }

            return tk2dSpriteCollectionData;
        }

        public static tk2dSpriteDefinition CreateDefinitionForRegionInTexture(string name, Vector2 textureDimensions, float scale, Rect uvRegion, Rect trimRect, Vector2 anchor, bool rotated)
        {
            float height = uvRegion.height;
            float width = uvRegion.width;
            float x = textureDimensions.x;
            float y = textureDimensions.y;
            tk2dSpriteDefinition tk2dSpriteDefinition = new tk2dSpriteDefinition();
            tk2dSpriteDefinition.flipped = ((!rotated) ? tk2dSpriteDefinition.FlipMode.None : tk2dSpriteDefinition.FlipMode.TPackerCW);
            tk2dSpriteDefinition.extractRegion = false;
            tk2dSpriteDefinition.name = name;
            tk2dSpriteDefinition.colliderType = tk2dSpriteDefinition.ColliderType.Unset;
            Vector2 vector = new Vector2(0.001f, 0.001f);
            Vector2 vector2 = new Vector2((uvRegion.x + vector.x) / x, 1f - (uvRegion.y + uvRegion.height + vector.y) / y);
            Vector2 vector3 = new Vector2((uvRegion.x + uvRegion.width - vector.x) / x, 1f - (uvRegion.y - vector.y) / y);
            Vector2 a = new Vector2(trimRect.x - anchor.x, -trimRect.y + anchor.y);
            if (rotated)
            {
                a.y -= width;
            }

            a *= scale;
            Vector3 a2 = new Vector3(-anchor.x * scale, anchor.y * scale, 0f);
            Vector3 vector4 = a2 + new Vector3(trimRect.width * scale, -trimRect.height * scale, 0f);
            Vector3 a3 = new Vector3(0f, -height * scale, 0f);
            Vector3 vector5 = a3 + new Vector3(width * scale, height * scale, 0f);
            if (rotated)
            {
                tk2dSpriteDefinition.positions = new Vector3[]
                {
                    new Vector3(-vector5.y + a.x, a3.x + a.y, 0f),
                    new Vector3(-a3.y + a.x, a3.x + a.y, 0f),
                    new Vector3(-vector5.y + a.x, vector5.x + a.y, 0f),
                    new Vector3(-a3.y + a.x, vector5.x + a.y, 0f)
                };
                tk2dSpriteDefinition.uvs = new Vector2[]
                {
                    new Vector2(vector2.x, vector3.y),
                    new Vector2(vector2.x, vector2.y),
                    new Vector2(vector3.x, vector3.y),
                    new Vector2(vector3.x, vector2.y)
                };
            }
            else
            {
                tk2dSpriteDefinition.positions = new Vector3[]
                {
                    new Vector3(a3.x + a.x, a3.y + a.y, 0f),
                    new Vector3(vector5.x + a.x, a3.y + a.y, 0f),
                    new Vector3(a3.x + a.x, vector5.y + a.y, 0f),
                    new Vector3(vector5.x + a.x, vector5.y + a.y, 0f)
                };
                tk2dSpriteDefinition.uvs = new Vector2[]
                {
                    new Vector2(vector2.x, vector2.y),
                    new Vector2(vector3.x, vector2.y),
                    new Vector2(vector2.x, vector3.y),
                    new Vector2(vector3.x, vector3.y)
                };
            }

            tk2dSpriteDefinition.normals = new Vector3[0];
            tk2dSpriteDefinition.tangents = new Vector4[0];
            tk2dSpriteDefinition.indices = new int[]
            {
                0,
                3,
                1,
                2,
                3,
                0
            };
            Vector3 b = new Vector3(a2.x, vector4.y, 0f);
            Vector3 a4 = new Vector3(vector4.x, a2.y, 0f);
            tk2dSpriteDefinition.boundsData = new Vector3[]
            {
                (a4 + b) / 2f,
                a4 - b
            };
            tk2dSpriteDefinition.untrimmedBoundsData = new Vector3[]
            {
                (a4 + b) / 2f,
                a4 - b
            };
            tk2dSpriteDefinition.texelSize = new Vector2(scale, scale);
            return tk2dSpriteDefinition;
        }

        public static tk2dSpriteAnimation CloneTk2dAnimation(tk2dSpriteAnimation animation, string name = "")
        {
            if (string.IsNullOrEmpty(name))
                name = animation.name;
            tk2dSpriteAnimation newAnimation = UObject.Instantiate(animation);
            for (int i = 0; i < newAnimation.clips.Length; i++)
            {
                newAnimation.clips[i] = new tk2dSpriteAnimationClip(newAnimation.clips[i]);
            }

            tk2dAnimations.Add(name, newAnimation);
            newAnimation.name = name + " Sprite Animation";
            UObject.DontDestroyOnLoad(newAnimation);
            return newAnimation;
        }

        public static tk2dSpriteCollectionData CloneTk2dCollection(tk2dSpriteCollectionData collection, string name = "")
        {
            if (string.IsNullOrEmpty(name))
                name = collection.spriteCollectionName;
            tk2dSpriteCollectionData newCollection = UObject.Instantiate(collection);
            for (int i = 0; i < newCollection.materials.Length; i++)
            {
                newCollection.materials[i] = UObject.Instantiate(newCollection.materials[i]);
            }

            for (int i = 0; i < newCollection.spriteDefinitions.Length; i++)
            {
                tk2dSpriteDefinition definition = new tk2dSpriteDefinition();
                if (spriteDefinitionType == null) spriteDefinitionType = typeof(tk2dSpriteDefinition);
                if (spriteDefinitionFields == null) spriteDefinitionFields = spriteDefinitionType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                foreach (FieldInfo fi in spriteDefinitionFields)
                    fi.SetValue(definition, fi.GetValue(newCollection.spriteDefinitions[i]));
                definition.material = newCollection.materials[definition.materialId];
                newCollection.spriteDefinitions[i] = definition;
            }

            tk2dCollections.Add(name, newCollection);
            newCollection.spriteCollectionName = name;
            newCollection.name = name + " Sprite Collection";
            UObject.DontDestroyOnLoad(newCollection);
            return newCollection;
        }

        public static void SetCollection(this tk2dSpriteAnimation animation, tk2dSpriteCollectionData collection)
        {
            foreach (tk2dSpriteAnimationClip clip in animation.clips)
            foreach (tk2dSpriteAnimationFrame frame in clip.frames)
                frame.spriteCollection = collection;
        }

        public static void SetCollection(this tk2dSpriteAnimation animation, Dictionary<tk2dSpriteCollectionData, tk2dSpriteCollectionData> collections)
        {
            foreach (tk2dSpriteAnimationClip clip in animation.clips)
            foreach (tk2dSpriteAnimationFrame frame in clip.frames)
                if (collections.TryGetValue(frame.spriteCollection, out tk2dSpriteCollectionData collection))
                    frame.spriteCollection = collection;
        }

        public static void SetCollection(this tk2dSpriteAnimationClip clip, tk2dSpriteCollectionData collection)
        {
            foreach (tk2dSpriteAnimationFrame frame in clip.frames)
                frame.spriteCollection = collection;
        }

        public static void SetCollection(this tk2dSpriteAnimationClip clip, Dictionary<tk2dSpriteCollectionData, tk2dSpriteCollectionData> collections)
        {
            foreach (tk2dSpriteAnimationFrame frame in clip.frames)
                if (collections.TryGetValue(frame.spriteCollection, out tk2dSpriteCollectionData collection))
                    frame.spriteCollection = collection;
        }

        public static void SetCollection(this tk2dSpriteAnimationFrame frame, tk2dSpriteCollectionData collection)
        {
            frame.spriteCollection = collection;
        }

        public static void SetCollection(this tk2dSpriteAnimationFrame frame, Dictionary<tk2dSpriteCollectionData, tk2dSpriteCollectionData> collections)
        {
            if (collections.TryGetValue(frame.spriteCollection, out tk2dSpriteCollectionData collection))
                frame.spriteCollection = collection;
        }

        public static void SetTexture(this tk2dSpriteCollectionData collection, Texture2D tex)
        {
            collection.FirstValidDefinition.material.mainTexture = tex;
        }

        public static tk2dSpriteCollectionData CloneTk2dCollectionWithTexture(this tk2dSpriteCollectionData collection, Texture2D tex, string name = "")
        {
            tk2dSpriteCollectionData newcollection = CloneTk2dCollection(collection, name);
            SetTexture(newcollection, tex);
            return newcollection;
        }

        public static tk2dSpriteAnimation CloneTk2dAnimationWithCollection(this tk2dSpriteAnimation animation, tk2dSpriteCollectionData collection, string name = "")
        {
            tk2dSpriteAnimation newanimation = CloneTk2dAnimation(animation, name);
            SetCollection(newanimation, collection);
            return newanimation;
        }

        public static (tk2dSpriteAnimation, tk2dSpriteCollectionData) CloneTk2dAnimationWithCollection(this tk2dSpriteAnimation animation, string name = "")
        {
            tk2dSpriteAnimation newanimation = CloneTk2dAnimation(animation, name);
            tk2dSpriteCollectionData collection = newanimation.clips[0].frames[0].spriteCollection;
            tk2dSpriteCollectionData newcollection = CloneTk2dCollection(collection, name);
            SetCollection(newanimation, newcollection);
            return (newanimation, newcollection);
        }

        public static (tk2dSpriteAnimation, tk2dSpriteCollectionData) CloneTk2dAnimationWithCollectionSetTexture(this tk2dSpriteAnimation animation, Texture2D tex, string name = "")
        {
            tk2dSpriteAnimation newanimation = CloneTk2dAnimation(animation, name);
            tk2dSpriteCollectionData collection = newanimation.clips[0].frames[0].spriteCollection;
            tk2dSpriteCollectionData newcollection = CloneTk2dCollection(collection, name);
            SetCollection(newanimation, newcollection);
            SetTexture(newcollection, tex);
            return (newanimation, newcollection);
        }

        [Obsolete("Will be removed in later versions, use ApplyFramesAndFps")]
        public static void CloneFramesAndFps(this tk2dSpriteAnimation animation, tk2dSpriteAnimation other, Dictionary<string, string> move) => ApplyFramesAndFps(animation, other, move);

        public static void ApplyFramesAndFps(this tk2dSpriteAnimation animation, tk2dSpriteAnimation other, Dictionary<string, string> move)
        {
            foreach (KeyValuePair<string, string> pair in move)
            {
                tk2dSpriteAnimationClip sourceclip = animation.GetClipByName(pair.Key);
                tk2dSpriteAnimationClip newclip = other.GetClipByName(pair.Value);
                sourceclip.frames = newclip.frames;
                sourceclip.fps = newclip.fps;
            }
        }

        public static void ApplyFramesAndFps(this tk2dSpriteAnimation animation, tk2dSpriteAnimation other, Dictionary<string, string> move, bool clone)
        {
            if (!clone)
            {
                ApplyFramesAndFps(animation, other, move);
                return;
            }

            foreach (KeyValuePair<string, string> pair in move)
            {
                tk2dSpriteAnimationClip sourceclip = animation.GetClipByName(pair.Key);
                tk2dSpriteAnimationClip newclip = other.GetClipByName(pair.Value);
                sourceclip.frames = newclip.CloneFrames().ToArray();
                sourceclip.fps = newclip.fps;
            }
        }

        public static void ApplyFramesAndFps(this tk2dSpriteAnimation animation, tk2dSpriteAnimation other, Dictionary<string, string> move, int definitionOffset)
        {
            foreach (KeyValuePair<string, string> pair in move)
            {
                tk2dSpriteAnimationClip sourceclip = animation.GetClipByName(pair.Key);
                tk2dSpriteAnimationClip newclip = other.GetClipByName(pair.Value);
                sourceclip.frames = newclip.CloneFrames(definitionOffset).ToArray();
                sourceclip.fps = newclip.fps;
            }
        }

        public static List<tk2dSpriteAnimationClip> GetClips(this tk2dSpriteAnimation animation, string[] clips, bool clone = false)
        {
            List<tk2dSpriteAnimationClip> ret = new List<tk2dSpriteAnimationClip>();
            foreach (string name in clips)
            {
                ret.Add(animation.GetClipByName(name));
            }

            return ret;
        }

        public static Dictionary<string, tk2dSpriteAnimationClip> GetClipsDict(this tk2dSpriteAnimation animation, string[] clips, bool clone = false)
        {
            Dictionary<string, tk2dSpriteAnimationClip> ret = new Dictionary<string, tk2dSpriteAnimationClip>();
            foreach (string name in clips)
            {
                ret.Add(name, animation.GetClipByName(name));
            }

            return ret;
        }

        public static void SetFramesAndFps(this tk2dSpriteAnimation animation, List<tk2dSpriteAnimationClip> clips, Action<tk2dSpriteAnimationClip>? onSetClip = null)
        {
            foreach (tk2dSpriteAnimationClip newclip in clips)
            {
                tk2dSpriteAnimationClip sourceclip = animation.GetClipByName(newclip.name);
                sourceclip.frames = newclip.frames;
                sourceclip.fps = newclip.fps;
                if (onSetClip != null)
                    onSetClip.Invoke(sourceclip);
            }
        }

        public static void SetFramesAndFps(this tk2dSpriteAnimation animation, Dictionary<string, tk2dSpriteAnimationClip> clips, Action<tk2dSpriteAnimationClip>? setClip = null)
        {
            foreach (KeyValuePair<string, tk2dSpriteAnimationClip> pair in clips)
            {
                tk2dSpriteAnimationClip sourceclip = animation.GetClipByName(pair.Key);
                tk2dSpriteAnimationClip newclip = pair.Value;
                sourceclip.frames = newclip.frames;
                sourceclip.fps = newclip.fps;
                if (setClip != null)
                    setClip.Invoke(sourceclip);
            }
        }

        public static int AddTk2dSpriteDefinitions(this tk2dSpriteCollectionData collection, tk2dSpriteCollectionData addcollection, int startIndex = 0, int? endIndex = null)
        {
            int offset = collection.spriteDefinitions.Length - startIndex;
            List<tk2dSpriteDefinition> definitions = collection.spriteDefinitions.ToList();
            if (endIndex == null)
            {
                endIndex = addcollection.spriteDefinitions.Length;
            }

            for (int i = startIndex; i < endIndex; i++)
            {
                definitions.Add(addcollection.spriteDefinitions[i]);
            }

            collection.spriteDefinitions = definitions.ToArray();
            return offset;
        }

        public static List<tk2dSpriteAnimationClip> CloneClips(this tk2dSpriteAnimation animation)
        {
            List<tk2dSpriteAnimationClip> ret = new List<tk2dSpriteAnimationClip>();
            foreach (tk2dSpriteAnimationClip clip in animation.clips)
            {
                ret.Add(new tk2dSpriteAnimationClip(clip));
            }

            return ret;
        }

        public static List<tk2dSpriteAnimationClip> CloneClips(this tk2dSpriteAnimation animation, string[] clips)
        {
            List<tk2dSpriteAnimationClip> ret = new List<tk2dSpriteAnimationClip>();
            foreach (string clip in clips)
            {
                ret.Add(new tk2dSpriteAnimationClip(animation.GetClipByName(clip)));
            }

            return ret;
        }

        public static List<tk2dSpriteAnimationFrame> CloneFrames(this tk2dSpriteAnimationClip clip)
        {
            List<tk2dSpriteAnimationFrame> ret = new List<tk2dSpriteAnimationFrame>();
            foreach (tk2dSpriteAnimationFrame frame in clip.frames)
            {
                tk2dSpriteAnimationFrame newframe = new tk2dSpriteAnimationFrame();
                newframe.CopyFrom(frame);
                ret.Add(newframe);
            }

            return ret;
        }

        public static List<tk2dSpriteAnimationFrame> CloneFrames(this tk2dSpriteAnimationClip clip, int offset)
        {
            List<tk2dSpriteAnimationFrame> ret = new List<tk2dSpriteAnimationFrame>();
            foreach (tk2dSpriteAnimationFrame frame in clip.frames)
            {
                tk2dSpriteAnimationFrame newframe = new tk2dSpriteAnimationFrame();
                newframe.CopyFrom(frame);
                newframe.spriteId += offset;
                ret.Add(newframe);
            }

            return ret;
        }

        #endregion

        #region hook

        public static Hook InstanceHook<T>(string methodName, Action<Action<T>, T> HookMethod)
        {
            return new Hook(typeof(T).GetMethod(methodName), HookMethod.Method);
        }

        public static Hook InstanceHook<T, T2>(string methodName, Action<Action<T, T2>, T, T2> HookMethod)
        {
            return new Hook(typeof(T).GetMethod(methodName, new Type[] {typeof(T2)}), HookMethod.Method);
        }

        public static Hook InstanceHook<T, T2, T3>(string methodName, Action<Action<T, T2, T3>, T, T2, T3> HookMethod)
        {
            return new Hook(typeof(T).GetMethod(methodName, new Type[] {typeof(T2), typeof(T3)}), HookMethod.Method);
        }

        public static Hook InstanceHookRet<T, T2>(string methodName, Func<Func<T, T2>, T, T2> HookMethod)
        {
            return new Hook(typeof(T).GetMethod(methodName), HookMethod.Method);
        }

        public static Hook InstanceHookRet<T, T2, T3>(string methodName, Func<Func<T, T2, T3>, T, T2, T3> HookMethod)
        {
            return new Hook(typeof(T).GetMethod(methodName, new Type[] {typeof(T2)}), HookMethod.Method);
        }

        public static Hook ConstructorHook<T>(Action<Action<T>, T> HookMethod)
        {
            return new Hook(typeof(T).GetConstructor(new Type[] { }), HookMethod.Method);
        }

        public static Hook ConstructorHook<T, T2>(Action<Action<T, T2>, T, T2> HookMethod)
        {
            return new Hook(typeof(T).GetConstructor(new Type[] {typeof(T2)}), HookMethod.Method);
        }

        public static Hook ConstructorHook<T, T2, T3>(Action<Action<T, T2, T3>, T, T2, T3> HookMethod)
        {
            return new Hook(typeof(T).GetConstructor(new Type[] {typeof(T2), typeof(T3)}), HookMethod.Method);
        }

        #endregion
    }
}