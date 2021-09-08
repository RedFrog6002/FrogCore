using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FrogCore;
using FrogCore.Ext;
using HutongGames;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using PMFSM = PlayMakerFSM;

namespace FrogCore.Fsm
{
    #region newActions
    public class CustomCallMethod : FsmStateAction
    {
        readonly Action<FsmStateAction> call;
        public CustomCallMethod(Action<FsmStateAction> method)
        {
            call = method;
        }
        public CustomCallMethod(Action method)
        {
            call = _ => method();
        }
        public override void OnEnter()
        {
            call(this);
            Finish();
        }
    }
    public class CustomCallCoroutine : FsmStateAction
    {
        readonly Func<FsmStateAction, IEnumerator> call;
        public CustomCallCoroutine(Func<FsmStateAction, IEnumerator> coroutine)
        {
            call = coroutine;
        }
        public CustomCallCoroutine(Func<IEnumerator> coroutine)
        {
            call = _ => coroutine();
        }
        public override void OnEnter()
        {
            StartCoroutine(Coroutine());
        }

        private IEnumerator Coroutine()
        {
            yield return call(this);
            Finish();
        }
    }
    #endregion
    public static class FsmUtil
    {
        #region fsmStates
        public static FsmState GetState(this PMFSM fsm, string statename)
        {
            return fsm.FsmStates.FirstOrDefault(fsmstate => fsmstate.Name == statename);
        }
        public static FsmState GetState(this PMFSM fsm, int stateindex)
        {
            return fsm.FsmStates[stateindex];
        }
        public static void RemoveState(this PMFSM fsm, int stateindex)
        {
            List<FsmState> states = fsm.Fsm.States.ToList();
            states.RemoveAt(stateindex);
            fsm.Fsm.States = states.ToArray();
        }
        public static void RemoveState(this PMFSM fsm, string statename)
        {
            fsm.Fsm.States = fsm.Fsm.States.Where(state => state.Name != statename).ToArray();
        }
        public static FsmState CreateState(this PMFSM fsm, string statename)
        {
            FsmState state = new FsmState(fsm.Fsm) { Name = statename };
            fsm.Fsm.States = fsm.Fsm.States.Append(state).ToArray();
            return state;
        }
        public static FsmState CreateState(this PMFSM fsm, string statename, Action action)
        {
            FsmState state = new FsmState(fsm.Fsm) { Name = statename, Actions = new FsmStateAction[] { new CustomCallMethod(action) } };
            fsm.Fsm.States = fsm.Fsm.States.Append(state).ToArray();
            return state;
        }
        public static FsmState CreateState(this PMFSM fsm, string statename, Action<FsmStateAction> action)
        {
            FsmState state = new FsmState(fsm.Fsm) { Name = statename, Actions = new FsmStateAction[] { new CustomCallMethod(action) } };
            fsm.Fsm.States = fsm.Fsm.States.Append(state).ToArray();
            return state;
        }
        public static FsmState CreateState(this PMFSM fsm, string statename, Func<IEnumerator> coroutine)
        {
            FsmState state = new FsmState(fsm.Fsm) { Name = statename, Actions = new FsmStateAction[] { new CustomCallCoroutine(coroutine) } };
            fsm.Fsm.States = fsm.Fsm.States.Append(state).ToArray();
            return state;
        }
        public static FsmState CreateState(this PMFSM fsm, string statename, Func<FsmStateAction, IEnumerator> coroutine)
        {
            FsmState state = new FsmState(fsm.Fsm) { Name = statename, Actions = new FsmStateAction[] { new CustomCallCoroutine(coroutine) } };
            fsm.Fsm.States = fsm.Fsm.States.Append(state).ToArray();
            return state;
        }
        public static List<FsmState> CreateStates(this PMFSM fsm, string[] statenames)
        {
            List<FsmState> newstates = new List<FsmState>();
            for (int i = 0; i < statenames.Length; i++)
                newstates.Add(CreateState(fsm, statenames[i]));
            return newstates;
        }
        public static List<FsmState> CreateStates(this PMFSM fsm, string[] statenames, Action[] actions)
        {
            List<FsmState> newstates = new List<FsmState>();
            for (int i = 0; i < statenames.Length; i++)
                newstates.Add(CreateState(fsm, statenames[i], actions[i]));
            return newstates;
        }
        public static List<FsmState> CreateStates(this PMFSM fsm, string[] statenames, Action<FsmStateAction>[] actions)
        {
            List<FsmState> newstates = new List<FsmState>();
            for (int i = 0; i < statenames.Length; i++)
                newstates.Add(CreateState(fsm, statenames[i], actions[i]));
            return newstates;
        }
        public static List<FsmState> CreateStates(this PMFSM fsm, string[] statenames, Func<IEnumerator>[] coroutines)
        {
            List<FsmState> newstates = new List<FsmState>();
            for (int i = 0; i < statenames.Length; i++)
                newstates.Add(CreateState(fsm, statenames[i], coroutines[i]));
            return newstates;
        }
        public static List<FsmState> CreateStates(this PMFSM fsm, string[] statenames, Func<FsmStateAction, IEnumerator>[] coroutines)
        {
            List<FsmState> newstates = new List<FsmState>();
            for (int i = 0; i < statenames.Length; i++)
                newstates.Add(CreateState(fsm, statenames[i], coroutines[i]));
            return newstates;
        }
        public static FsmState CopyState(this FsmState state, string newstate)
        {
            FsmState statenew = new FsmState(state) { Name = newstate };
            for (int i = 0; i < state.Transitions.Length; i++)
            {
                statenew.Transitions[i].ToFsmState = state.Transitions[i].ToFsmState;
            }
            state.Fsm.States = state.Fsm.States.Append(statenew).ToArray();
            return state;
        }
        public static FsmState CopyState(this PMFSM fsm, FsmState copystate, string newstate)
        {
            FsmState state = new FsmState(copystate) { Name = newstate };
            for (int i = 0; i < state.Transitions.Length; i++)
            {
                state.Transitions[i].ToFsmState = copystate.Transitions[i].ToFsmState;
            }
            fsm.Fsm.States = fsm.Fsm.States.Append(state).ToArray();
            return state;
        }
        public static FsmState CopyState(this PMFSM fsm, string copystate, string newstate)
        {
            return CopyState(fsm, fsm.GetState(copystate), newstate);
        }
        #endregion

        #region fsmActions
        public static FsmStateAction GetAction(this PMFSM fsm, string statename, int actionindex)
        {
            return fsm.GetState(statename).GetAction(actionindex);
        }
        public static FsmStateAction GetAction(this PMFSM fsm, int stateindex, int actionindex)
        {
            return fsm.GetState(stateindex).GetAction(actionindex);
        }
        public static T GetAction<T>(this PMFSM fsm, string statename) where T : FsmStateAction
        {
            return fsm.GetState(statename).GetAction<T>();
        }
        public static T GetAction<T>(this PMFSM fsm, string statename, int actionindex) where T : FsmStateAction
        {
            return (T)GetAction(fsm, statename, actionindex);
        }
        public static T GetAction<T>(this PMFSM fsm, int stateindex, int actionindex) where T : FsmStateAction
        {
            return (T)GetAction(fsm, stateindex, actionindex);
        }
        public static FsmStateAction GetAction(this FsmState state, int actionindex)
        {
            return state.Actions[actionindex];
        }
        public static T GetAction<T>(this FsmState state) where T : FsmStateAction
        {
            return (T)state.Actions.FirstOrDefault(action => action.GetType() == typeof(T));
        }
        public static T GetAction<T>(this FsmState state, int actionindex) where T : FsmStateAction
        {
            return (T)state.Actions[actionindex];
        }
        public static void RemoveAction(this PMFSM fsm, int stateindex, int actionindex)
        {
            FsmState state = fsm.GetState(stateindex);
            List<FsmStateAction> actions = state.Actions.ToList();
            actions.RemoveAt(actionindex);
            state.Actions = actions.ToArray();
        }
        public static void RemoveAction(this PMFSM fsm, string statename, int actionindex)
        {
            FsmState state = fsm.GetState(statename);
            List<FsmStateAction> actions = state.Actions.ToList();
            actions.RemoveAt(actionindex);
            state.Actions = actions.ToArray();
        }
        public static void RemoveAction<T>(this PMFSM fsm, int stateindex) where T : FsmStateAction
        {
            FsmState state = fsm.GetState(stateindex);
            state.Actions = state.Actions.Where(action => action.GetType() != typeof(T)).ToArray();
        }
        public static void RemoveAction<T>(this PMFSM fsm, string statename) where T : FsmStateAction
        {
            FsmState state = fsm.GetState(statename);
            state.Actions = state.Actions.Where(action => action.GetType() != typeof(T)).ToArray();
        }
        public static void RemoveAction(this FsmState state, int actionindex)
        {
            List<FsmStateAction> actions = state.Actions.ToList();
            actions.RemoveAt(actionindex);
            state.Actions = actions.ToArray();
        }
        public static void RemoveAction<T>(this FsmState state) where T : FsmStateAction
        {
            state.Actions = state.Actions.Where(action => action.GetType() != typeof(T)).ToArray();
        }
        public static void AddAction(this PMFSM fsm, int stateindex, FsmStateAction action)
        {
            action.Fsm = fsm.Fsm;
            FsmState state = fsm.GetState(stateindex);
            state.Actions = state.Actions.Append(action).ToArray();
        }
        public static void AddAction(this PMFSM fsm, string statename, FsmStateAction action)
        {
            action.Fsm = fsm.Fsm;
            FsmState state = fsm.GetState(statename);
            state.Actions = state.Actions.Append(action).ToArray();
        }
        public static void AddAction(this FsmState state, FsmStateAction action)
        {
            action.Fsm = state.Fsm;
            state.Actions = state.Actions.Append(action).ToArray();
        }
        public static void InsertAction(this PMFSM fsm, int stateindex, int actionindex, FsmStateAction action)
        {
            action.Fsm = fsm.Fsm;
            FsmState state = fsm.GetState(stateindex);
            state.Actions = state.Actions.Insert(actionindex, action).ToArray();
        }
        public static void InsertAction(this PMFSM fsm, string statename, int actionindex, FsmStateAction action)
        {
            action.Fsm = fsm.Fsm;
            FsmState state = fsm.GetState(statename);
            state.Actions = state.Actions.Insert(actionindex, action).ToArray();
        }
        public static void InsertAction(this FsmState state, int actionindex, FsmStateAction action)
        {
            action.Fsm = state.Fsm;
            state.Actions = state.Actions.Insert(actionindex, action).ToArray();
        }
        public static void AddMethod(this PMFSM fsm, string statename, Action method)
        {
            FsmState state = fsm.GetState(statename);
            state.Actions = state.Actions.Append(new CustomCallMethod(method) { Fsm = fsm.Fsm }).ToArray();
        }
        public static void AddMethod(this FsmState state, Action method)
        {
            state.Actions = state.Actions.Append(new CustomCallMethod(method) { Fsm = state.Fsm }).ToArray();
        }
        public static void InsertMethod(this PMFSM fsm, int stateindex, int actionindex, Action method)
        {
            FsmState state = fsm.GetState(stateindex);
            state.Actions = state.Actions.Insert(actionindex, new CustomCallMethod(method) { Fsm = fsm.Fsm }).ToArray();
        }
        public static void InsertMethod(this PMFSM fsm, string statename, int actionindex, Action method)
        {
            FsmState state = fsm.GetState(statename);
            state.Actions = state.Actions.Insert(actionindex, new CustomCallMethod(method) { Fsm = fsm.Fsm }).ToArray();
        }
        public static void InsertMethod(this FsmState state, int actionindex, Action method)
        {
            state.Actions = state.Actions.Insert(actionindex, new CustomCallMethod(method) { Fsm = state.Fsm }).ToArray();
        }
        public static void AddMethod(this PMFSM fsm, string statename, Action<FsmStateAction> method)
        {
            FsmState state = fsm.GetState(statename);
            state.Actions = state.Actions.Append(new CustomCallMethod(method) { Fsm = fsm.Fsm }).ToArray();
        }
        public static void AddMethod(this FsmState state, Action<FsmStateAction> method)
        {
            state.Actions = state.Actions.Append(new CustomCallMethod(method) { Fsm = state.Fsm }).ToArray();
        }
        public static void InsertMethod(this PMFSM fsm, int stateindex, int actionindex, Action<FsmStateAction> method)
        {
            FsmState state = fsm.GetState(stateindex);
            state.Actions = state.Actions.Insert(actionindex, new CustomCallMethod(method) { Fsm = fsm.Fsm }).ToArray();
        }
        public static void InsertMethod(this PMFSM fsm, string statename, int actionindex, Action<FsmStateAction> method)
        {
            FsmState state = fsm.GetState(statename);
            state.Actions = state.Actions.Insert(actionindex, new CustomCallMethod(method) { Fsm = fsm.Fsm }).ToArray();
        }
        public static void InsertMethod(this FsmState state, int actionindex, Action<FsmStateAction> method)
        {
            state.Actions = state.Actions.Insert(actionindex, new CustomCallMethod(method) { Fsm = state.Fsm }).ToArray();
        }
        public static void AddCoroutine(this PMFSM fsm, string statename, Func<IEnumerator> coroutine)
        {
            FsmState state = fsm.GetState(statename);
            state.Actions = state.Actions.Append(new CustomCallCoroutine(coroutine) { Fsm = fsm.Fsm }).ToArray();
        }
        public static void AddCoroutine(this FsmState state, Func<IEnumerator> coroutine)
        {
            state.Actions = state.Actions.Append(new CustomCallCoroutine(coroutine) { Fsm = state.Fsm }).ToArray();
        }
        public static void InsertCoroutine(this PMFSM fsm, int stateindex, int actionindex, Func<IEnumerator> coroutine)
        {
            FsmState state = fsm.GetState(stateindex);
            state.Actions = state.Actions.Insert(actionindex, new CustomCallCoroutine(coroutine) { Fsm = fsm.Fsm }).ToArray();
        }
        public static void InsertCoroutine(this PMFSM fsm, string statename, int actionindex, Func<IEnumerator> coroutine)
        {
            FsmState state = fsm.GetState(statename);
            state.Actions = state.Actions.Insert(actionindex, new CustomCallCoroutine(coroutine) { Fsm = fsm.Fsm }).ToArray();
        }
        public static void InsertCoroutine(this FsmState state, int actionindex, Func<IEnumerator> coroutine)
        {
            state.Actions = state.Actions.Insert(actionindex, new CustomCallCoroutine(coroutine) { Fsm = state.Fsm }).ToArray();
        }
        public static void AddCoroutine(this PMFSM fsm, string statename, Func<FsmStateAction, IEnumerator> coroutine)
        {
            FsmState state = fsm.GetState(statename);
            state.Actions = state.Actions.Append(new CustomCallCoroutine(coroutine) { Fsm = fsm.Fsm }).ToArray();
        }
        public static void AddCoroutine(this FsmState state, Func<FsmStateAction, IEnumerator> coroutine)
        {
            state.Actions = state.Actions.Append(new CustomCallCoroutine(coroutine) { Fsm = state.Fsm }).ToArray();
        }
        public static void InsertCoroutine(this PMFSM fsm, int stateindex, int actionindex, Func<FsmStateAction, IEnumerator> coroutine)
        {
            FsmState state = fsm.GetState(stateindex);
            state.Actions = state.Actions.Insert(actionindex, new CustomCallCoroutine(coroutine) { Fsm = fsm.Fsm }).ToArray();
        }
        public static void InsertCoroutine(this PMFSM fsm, string statename, int actionindex, Func<FsmStateAction, IEnumerator> coroutine)
        {
            FsmState state = fsm.GetState(statename);
            state.Actions = state.Actions.Insert(actionindex, new CustomCallCoroutine(coroutine) { Fsm = fsm.Fsm }).ToArray();
        }
        public static void InsertCoroutine(this FsmState state, int actionindex, Func<FsmStateAction, IEnumerator> coroutine)
        {
            state.Actions = state.Actions.Insert(actionindex, new CustomCallCoroutine(coroutine) { Fsm = state.Fsm }).ToArray();
        }
        #endregion

        #region FsmTransitions
        public static void AddTransition(this FsmState state, FsmEvent fsmEvent, FsmState toState)
        {
            FsmTransition transition = new FsmTransition() { FsmEvent = fsmEvent, ToFsmState = toState };
            state.Transitions = state.Transitions.Append(transition).ToArray();
        }
        public static void AddTransition(this FsmState state, FsmEvent fsmEvent, string toState)
        {
            AddTransition(state, fsmEvent, state.Fsm.GetState(toState));
        }
        public static void AddTransition(this FsmState state, string fsmEvent, FsmState toState)
        {
            AddTransition(state, FsmEvent.FindEvent(fsmEvent), toState);
        }
        public static void AddTransition(this FsmState state, string fsmEvent, string toState)
        {
            AddTransition(state, FsmEvent.FindEvent(fsmEvent), state.Fsm.GetState(toState));
        }
        public static void AddTransition(this PMFSM fsm, string stateName, FsmEvent fsmEvent, string toState)
        {
            AddTransition(fsm.GetState(stateName), fsmEvent, fsm.GetState(toState));
        }
        public static void AddTransition(this PMFSM fsm, string stateName, string fsmEvent, string toState)
        {
            AddTransition(fsm.GetState(stateName), FsmEvent.FindEvent(fsmEvent), fsm.GetState(toState));
        }
        public static void ChangeTransition(this FsmState state, FsmEvent fsmEvent, FsmState toState)
        {
            state.Transitions.First(t => t.FsmEvent == fsmEvent || t.FsmEvent.Name == fsmEvent.Name).ToFsmState = toState;
        }
        public static void ChangeTransition(this FsmState state, FsmEvent fsmEvent, string toState)
        {
            ChangeTransition(state, fsmEvent, state.Fsm.GetState(toState));
        }
        public static void ChangeTransition(this FsmState state, string fsmEvent, FsmState toState)
        {
            state.Transitions.First(t => t.FsmEvent.Name == fsmEvent).ToFsmState = toState;
        }
        public static void ChangeTransition(this FsmState state, string fsmEvent, string toState)
        {
            ChangeTransition(state, fsmEvent, state.Fsm.GetState(toState));
        }
        public static void ChangeTransition(this PMFSM fsm, string stateName, FsmEvent fsmEvent, string toState)
        {
            ChangeTransition(fsm.GetState(stateName), fsmEvent, fsm.GetState(toState));
        }
        public static void ChangeTransition(this PMFSM fsm, string stateName, string fsmEvent, string toState)
        {
            ChangeTransition(fsm.GetState(stateName), fsmEvent, fsm.GetState(toState));
        }
        public static FsmTransition Copy(this FsmTransition transition)
        {
            FsmTransition newtransition = new FsmTransition(transition);
            newtransition.ToFsmState = transition.ToFsmState;
            return newtransition;
        }
        #endregion

        #region fsmVars
        public static FsmObject GetFsmObject(this PMFSM fsm, string varname)
        {
            return fsm.FsmVariables.ObjectVariables.FirstOrDefault(var => var.Name == varname);
        }
        public static FsmArray GetFsmArray(this PMFSM fsm, string varname)
        {
            return fsm.FsmVariables.ArrayVariables.FirstOrDefault(var => var.Name == varname);
        }
        public static FsmBool GetFsmBool(this PMFSM fsm, string varname)
        {
            return fsm.FsmVariables.BoolVariables.FirstOrDefault(var => var.Name == varname);
        }
        public static FsmColor GetFsmColor(this PMFSM fsm, string varname)
        {
            return fsm.FsmVariables.ColorVariables.FirstOrDefault(var => var.Name == varname);
        }
        public static FsmEnum GetFsmEnum(this PMFSM fsm, string varname)
        {
            return fsm.FsmVariables.EnumVariables.FirstOrDefault(var => var.Name == varname);
        }
        public static FsmFloat GetFsmFloat(this PMFSM fsm, string varname)
        {
            return fsm.FsmVariables.FloatVariables.FirstOrDefault(var => var.Name == varname);
        }
        public static FsmGameObject GetFsmGameObject(this PMFSM fsm, string varname)
        {
            return fsm.FsmVariables.GameObjectVariables.FirstOrDefault(var => var.Name == varname);
        }
        public static FsmInt GetFsmInt(this PMFSM fsm, string varname)
        {
            return fsm.FsmVariables.IntVariables.FirstOrDefault(var => var.Name == varname);
        }
        public static FsmMaterial GetFsmMaterial(this PMFSM fsm, string varname)
        {
            return fsm.FsmVariables.MaterialVariables.FirstOrDefault(var => var.Name == varname);
        }
        public static FsmQuaternion GetFsmQuaternion(this PMFSM fsm, string varname)
        {
            return fsm.FsmVariables.QuaternionVariables.FirstOrDefault(var => var.Name == varname);
        }
        public static FsmRect GetFsmRect(this PMFSM fsm, string varname)
        {
            return fsm.FsmVariables.RectVariables.FirstOrDefault(var => var.Name == varname);
        }
        public static FsmString GetFsmString(this PMFSM fsm, string varname)
        {
            return fsm.FsmVariables.StringVariables.FirstOrDefault(var => var.Name == varname);
        }
        public static FsmTexture GetFsmTexture(this PMFSM fsm, string varname)
        {
            return fsm.FsmVariables.TextureVariables.FirstOrDefault(var => var.Name == varname);
        }
        public static FsmVector2 GetFsmVector2(this PMFSM fsm, string varname)
        {
            return fsm.FsmVariables.Vector2Variables.FirstOrDefault(var => var.Name == varname);
        }
        public static FsmVector3 GetFsmVector3(this PMFSM fsm, string varname)
        {
            return fsm.FsmVariables.Vector3Variables.FirstOrDefault(var => var.Name == varname);
        }
        public static FsmObject GetFsmObject(this PMFSM fsm, int varindex)
        {
            return fsm.FsmVariables.ObjectVariables[varindex];
        }
        public static FsmArray GetFsmArray(this PMFSM fsm, int varindex)
        {
            return fsm.FsmVariables.ArrayVariables[varindex];
        }
        public static FsmBool GetFsmBool(this PMFSM fsm, int varindex)
        {
            return fsm.FsmVariables.BoolVariables[varindex];
        }
        public static FsmColor GetFsmColor(this PMFSM fsm, int varindex)
        {
            return fsm.FsmVariables.ColorVariables[varindex];
        }
        public static FsmEnum GetFsmEnum(this PMFSM fsm, int varindex)
        {
            return fsm.FsmVariables.EnumVariables[varindex];
        }
        public static FsmFloat GetFsmFloat(this PMFSM fsm, int varindex)
        {
            return fsm.FsmVariables.FloatVariables[varindex];
        }
        public static FsmGameObject GetFsmGameObject(this PMFSM fsm, int varindex)
        {
            return fsm.FsmVariables.GameObjectVariables[varindex];
        }
        public static FsmInt GetFsmInt(this PMFSM fsm, int varindex)
        {
            return fsm.FsmVariables.IntVariables[varindex];
        }
        public static FsmMaterial GetFsmMaterial(this PMFSM fsm, int varindex)
        {
            return fsm.FsmVariables.MaterialVariables[varindex];
        }
        public static FsmQuaternion GetFsmQuaternion(this PMFSM fsm, int varindex)
        {
            return fsm.FsmVariables.QuaternionVariables[varindex];
        }
        public static FsmRect GetFsmRect(this PMFSM fsm, int varindex)
        {
            return fsm.FsmVariables.RectVariables[varindex];
        }
        public static FsmString GetFsmString(this PMFSM fsm, int varindex)
        {
            return fsm.FsmVariables.StringVariables[varindex];
        }
        public static FsmTexture GetFsmTexture(this PMFSM fsm, int varindex)
        {
            return fsm.FsmVariables.TextureVariables[varindex];
        }
        public static FsmVector2 GetFsmVector2(this PMFSM fsm, int varindex)
        {
            return fsm.FsmVariables.Vector2Variables[varindex];
        }
        public static FsmVector3 GetFsmVector3(this PMFSM fsm, int varindex)
        {
            return fsm.FsmVariables.Vector3Variables[varindex];
        }
        #endregion
    }
}
