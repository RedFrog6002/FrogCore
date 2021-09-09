using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UObject = UnityEngine.Object;
using System.Text;
using UnityEngine;
using System.Collections;
using System.IO;
using FrogCore;

namespace FrogCore.Ext
{
    public static class Extensions
    {
		#region cache
		internal static GameObject StaticGo;
        internal static tk2dSpriteCollectionData cacheSpriteCollection;
        internal static List<int> cacheTk2dSprites = new List<int>();
        #endregion

        #region u object
		public static void DontDestroyOnLoad(this UObject o)
        {
			UObject.DontDestroyOnLoad(o);
        }
        public static void Destroy(this UObject o)
        {
            UObject.Destroy(o);
        }
        public static void DestroyImmediate(this UObject o)
        {
            UObject.DestroyImmediate(o);
        }
        public static void DestroyObject(this UObject o)
        {
            UObject.Destroy(o);
        }
        public static void DestroyObject(this UObject o, float time)
        {
            UObject.Destroy(o, time);
        }
        #endregion

        #region tk2d sprite
        public static void ReplaceSpritesheet(this tk2dSprite sprite, Texture tex, int sheetindex = 0)
        {
			sprite.Collection.ReplaceSpritesheet(tex, sheetindex);
        }
        public static tk2dSpriteAnimation CloneTk2dAnimation(this tk2dSpriteAnimator animator, string name = "", bool replaceOrig = true)
        {
            tk2dSpriteAnimation newAnimation = Utils.CloneTk2dAnimation(animator.Library, name);
            if (replaceOrig)
                animator.Library = newAnimation;
            return newAnimation;
        }
        public static tk2dSpriteCollectionData CloneTk2dCollection(this tk2dSprite sprite, string name = "", bool replaceOrig = true)
        {
            tk2dSpriteCollectionData newCollection = Utils.CloneTk2dCollection(sprite.Collection, name);
            if (replaceOrig)
                sprite.Collection = newCollection;
            return newCollection;
        }
        #endregion

        #region tk2d sprite animator
        public static void ReplaceSpritesheet(this tk2dSpriteAnimator anim, Texture tex, int sheetindex = 0)
        {
			anim.Library.clips.First((tk2dSpriteAnimationClip ac) => ac.frames != null && ac.frames.Length > 0).frames[0].spriteCollection.ReplaceSpritesheet(tex, sheetindex);
        }
        #endregion

        #region tk2d sprite collection data
		public static void ReplaceSpritesheet(this tk2dSpriteCollectionData data, Texture tex, int sheetindex = 0)
        {
			tex.DontDestroyOnLoad();
			if (data.textures.Length > sheetindex)
				data.textures[sheetindex] = tex;
			if (data.materials.Length > sheetindex)
				data.materials[sheetindex].mainTexture = tex;
			else
				data.material.mainTexture = tex;
		}
        #endregion

		#region game object
        public static T GetOrAddComponent<T>(this GameObject go) where T : Component
        {
            return go.GetComponent<T>() ?? go.AddComponent<T>();
        }
		public static void SetSprite(this GameObject go, Sprite s, bool pauseAnimation = false)
        {
            if (pauseAnimation && go.GetComponent<Animation>())
                go.GetComponent<Animation>().Stop();
            if (pauseAnimation && go.GetComponent<Animator>())
                go.GetComponent<Animator>().StopPlayback();
            if (pauseAnimation && go.GetComponent<tk2dSpriteAnimator>())
                go.GetComponent<tk2dSpriteAnimator>().Stop();
            if (go.GetComponent<SpriteRenderer>())
            {
                var rend = go.GetComponent<SpriteRenderer>();
                rend.sprite = s;
            }
            if (go.GetComponent<tk2dSprite>())
            {
                var rend = go.GetComponent<tk2dSprite>();
                if (!cacheSpriteCollection)
                {
					if (!StaticGo)
                    {
						StaticGo = new GameObject("FrogCore Manager");
						StaticGo.DontDestroyOnLoad();
                    }
					cacheSpriteCollection = Utils.CreateTk2dSpriteCollection(
                        s.texture,
                        s.GetHashCode().ToString().CreateArray(),
                        new Rect(0f, 0f, s.texture.width, s.texture.height).CreateArray(),
						s.pivot.CreateArray(),
                        //new Vector2(s.texture.width / 2f, s.texture.height / 2f).CreateArray(),
                        StaticGo);
					cacheTk2dSprites.Add(s.GetHashCode());
                }
                else if (!cacheTk2dSprites.Contains(s.GetHashCode()))
				{
					Material mat = Utils.CreateMaterialFromTextureTk2d(s.texture);
					tk2dSpriteDefinition tk2DSpriteDefinition = Utils.CreateDefinitionForRegionInTexture(
						s.GetHashCode().ToString(),
						new Vector2(s.texture.width, s.texture.height),
						0.01f,
						new Rect(0f, 0f, s.texture.width, s.texture.height),
						new Rect(0f, 0f, 0f, 0f),
						s.pivot,
						//new Vector2(s.texture.width / 2f, s.texture.height / 2f),
						false);
					tk2DSpriteDefinition.material = mat;
					cacheSpriteCollection.materials = cacheSpriteCollection.materials.Append(mat).ToArray();
					cacheSpriteCollection.textures = cacheSpriteCollection.textures.Append(s.texture).ToArray();
					cacheSpriteCollection.spriteDefinitions = cacheSpriteCollection.spriteDefinitions.Append(tk2DSpriteDefinition).ToArray();
					cacheTk2dSprites.Add(s.GetHashCode());
                }
				rend.SetSprite(cacheSpriteCollection, s.GetHashCode());
            }
        }
		public static void PlayAnimation(this GameObject go, string name)
        {
			if (go.GetComponent<Animation>())
				go.GetComponent<Animation>().Play(name);
			if (go.GetComponent<Animator>())
				go.GetComponent<Animator>().Play(name);
			if (go.GetComponent<tk2dSpriteAnimator>())
				go.GetComponent<tk2dSpriteAnimator>().Play(name);
		}
        #endregion

        #region enemy death effects
        public static void SetJournalEntry(this EnemyDeathEffects ede, JournalHelper jh)
        {
            try
            {
                string s = "CustomJournal" + jh.entrynumber;
                Modding.ReflectionHelper.SetField<EnemyDeathEffects, string>(ede, "playerDataName", s);
            }
            catch (Exception e)
            {
                Log("Journal Extentions", e);
            }
        }
        public static void RecordJournalEntry(this EnemyDeathEffects ede)
        {
            try
            {
                var recordmethod = typeof(EnemyDeathEffects).GetMethod("RecordKillForJournal", BindingFlags.NonPublic | BindingFlags.Instance);
                recordmethod.Invoke(ede, new object[] { });
            }
            catch (Exception e)
            {
                Log("Journal Extentions", e);
            }
        }
        #endregion

        #region other
        public static T[] CreateArray<T>(this T obj)
        {
			return new T[] { obj };
        }
        #endregion

        #region coroutines
		public static Coroutine StartCoroutine(IEnumerator enumerator)
        {
			if (!StaticGo)
            {
				StaticGo = new GameObject("FrogCore Manager");
				StaticGo.DontDestroyOnLoad();
            }
			return StaticGo.GetOrAddComponent<NonBouncer>().StartCoroutine(enumerator);
        }
		public static void StopCoroutine(Coroutine coroutine)
        {
			StaticGo.GetOrAddComponent<NonBouncer>().StopCoroutine(coroutine);
		}
        #endregion

        #region IEnumberable
        public static IEnumerable<T> Insert<T>(this IEnumerable<T> enumerable, int index, T obj)
        {
            IEnumerator<T> enumerator = enumerable.GetEnumerator();
            int i = 0;
            while (enumerator.MoveNext())
            {
                if (index == i)
                    yield return obj;
                yield return enumerator.Current;
                i++;
            }
            if (index == i)
                yield return obj;
        }
        #endregion

        #region logging
        public static void Log(object o)
        {
            Modding.Logger.Log("[FrogCore] - " + o);
        }
        public static void Log(string s)
        {
            Modding.Logger.Log("[FrogCore] - " + s);
        }

        public static void Log(string from, object o)
        {
            Modding.Logger.Log("[FrogCore]:[" + from + "] - " + o);
        }
        public static void Log(string from, string s)
        {
            Modding.Logger.Log("[FrogCore]:[" + from + "] - " + s);
        }

        internal static void Log(string[] fromlist, object o)
        {
            string from = "";
            foreach (string list in fromlist)
                from += ":[" + list + "]";
            Modding.Logger.Log("[FrogCore]" + from + " - " + o);
        }
        internal static void Log(string[] fromlist, string s)
        {
            string from = "";
            foreach (string list in fromlist)
                from += ":[" + list + "]";
            Modding.Logger.Log("[FrogCore]" + from + " - " + s);
        }
        #endregion
    }
}
