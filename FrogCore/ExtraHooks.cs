using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrogCore
{
    /// <summary>
    /// DON'T MESS WITH THIS PLEASE.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class EventMethod<T> where T : Delegate
    {
        public EventMethod() { }
        public EventMethod(T method)
        {
            _method = method;
        }
        public Delegate[] GetInvokationList() => _method.GetInvocationList();
        internal T _method;
        public void Add(T method)
        {
            _method = (T)Delegate.Combine(_method, method);
        }
        public void Remove(T method)
        {
            _method = (T)Delegate.Remove(_method, method);
        }
        public static EventMethod<T> operator +(EventMethod<T> em, T method) => new EventMethod<T>((em != null && em._method != null) ? (T)Delegate.Combine(em._method, method) : method);
        public static EventMethod<T> operator -(EventMethod<T> em, T method) => new EventMethod<T>((em != null && em._method != null) ? (T)Delegate.Remove(em._method, method) : null);
    }
    /// <summary>
    /// DON'T MESS WITH THIS PLEASE.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class EventDictionary<T> where T : Delegate
    {
        private Dictionary<string, EventMethod<T>> privateDict = new Dictionary<string, EventMethod<T>>();
        internal EventMethod<T> this[string name]
        {
            get
            {
                if (!privateDict.ContainsKey(name))
                    privateDict.Add(name, new EventMethod<T>());
                return privateDict[name];
            }
            set
            {
                if (privateDict.ContainsKey(name))
                    privateDict[name] = value;
                else
                    privateDict.Add(name, value);
            }
        }
        public bool HasEventFor(string name) => privateDict.ContainsKey(name) && privateDict[name]._method != null;
        public void Add(string s, T method)
        {
            privateDict[s].Add(method);
        }
        public void Remove(string s, T method)
        {
            privateDict[s].Remove(method);
        }
    }
    /// <summary>
    /// BROKEN! (unless it's not broken) USE AT YOUR OWN RISK!
    /// </summary>
    public static class ExtraHooks
    {
        static ExtraHooks()
        {
            On.PlayMakerFSM.Awake += FsmAwake;
        }
        public delegate void FsmAwakeHandler(PlayMakerFSM fsm);
        public static EventDictionary<FsmAwakeHandler> OnFsmAwake = new EventDictionary<FsmAwakeHandler>();
        private static void FsmAwake(On.PlayMakerFSM.orig_Awake orig, PlayMakerFSM self)
        {
            orig(self);
            if (OnFsmAwake.HasEventFor(self.FsmName))
                foreach (Delegate d in OnFsmAwake[self.FsmName].GetInvokationList())
                    d.DynamicInvoke(self);
        }
    }
}
