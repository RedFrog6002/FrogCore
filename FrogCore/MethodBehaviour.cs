using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace FrogCore
{
    public class MethodBehaviour : MonoBehaviour
    {
        public Action<GameObject> AwakeMethod;
        public Action<GameObject> StartMethod;
        public Action<GameObject> OnEnableMethod;
        public Action<GameObject> OnDisableMethod;
        public Action<GameObject> UpdateMethod;
        public Action<GameObject> FixedUpdateMethod;
        public Action<GameObject> LateUpdateMethod;
        public Action<GameObject, Collider2D> OnTriggerEnter2DMethod;
        public Action<GameObject, Collider2D> OnTriggerExit2DMethod;
        public Action<GameObject, Collider2D> OnTriggerStay2DMethod;
        public Action<GameObject, Collision2D> OnCollisionEnter2DMethod;
        public Action<GameObject, Collision2D> OnCollisionExit2DMethod;
        public Action<GameObject, Collision2D> OnCollisionStay2DMethod;
        private Action Call(Action<GameObject> method)
        {
            if (method != null) return () => { method(gameObject); };
            return () => { };
        }
        private Action<T1> Call<T1>(Action<GameObject, T1> method)
        {
            if (method != null) return (t1) => { method(gameObject, t1); };
            return (_) => { };
        }
        private void Awake() => Call(AwakeMethod);
        private void Start() => Call(StartMethod);
        private void OnEnable() => Call(OnDisableMethod);
        private void OnDisable() => Call(OnDisableMethod);
        private void Update() => Call(UpdateMethod);
        private void FixedUpdate() => Call(FixedUpdateMethod);
        private void LateUpdate() => Call(LateUpdateMethod);
        private void OnTriggerEnter2D(Collider2D col) => Call(OnTriggerEnter2DMethod);
        private void OnTriggerExit2D(Collider2D col) => Call(OnTriggerExit2DMethod);
        private void OnTriggerStay2D(Collider2D col) => Call(OnTriggerStay2DMethod);
        private void OnCollisionEnter2D(Collision2D col) => Call(OnCollisionEnter2DMethod);
        private void OnCollisionExit2D(Collision2D col) => Call(OnCollisionExit2DMethod);
        private void OnCollisionStay2D(Collision2D col) => Call(OnCollisionStay2DMethod);
    }
}
