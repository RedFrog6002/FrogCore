﻿using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using InControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using System.Collections;
using System.IO;
using FrogCore.Ext;
using Logger = Modding.Logger;
using FrogCore.Fsm;

namespace FrogCore
{
    /// <summary>
    /// Allows for easily adding a npc.
    /// </summary>
    public class DialogueNPC : MonoBehaviour
    {
        internal static GameObject zotePrefab;

        public static DialogueNPC CreateInstance()
        {
            GameObject instance = GameObject.Instantiate(zotePrefab);
            instance.SetActive(true);
            return instance.AddComponent<DialogueNPC>();
        }

        DialogueCallbackOptions lastResponse = new DialogueCallbackOptions() { Type = DialogueType.Normal, Key = "", Sheet = "", Cost = 0, Response = DialogueResponse.None };

        GameObject DialogueManager;
        GameObject NormalBox;
        GameObject YNBox;

        DialogueBox NormalDialogueBox;
        DialogueBox YNDialogueBox;
        PlayMakerFSM YNFsm;
        PlayMakerFSM ManagerNormalFsm;
        PlayMakerFSM ManagerYNFsm;

        private void TryGetReferences()
        {
            try
            {
                NormalBox ??= gameObject.LocateMyFSM("Conversation Control").GetState("Repeat").GetAction<CallMethodProper>(0).gameObject.GameObject.Value;
                DialogueManager ??= NormalBox.transform.parent.gameObject;
                YNBox = DialogueManager.transform.Find("Text YN").gameObject;
                NormalDialogueBox ??= NormalBox.GetComponent<DialogueBox>();
                YNDialogueBox ??= YNBox.GetComponent<DialogueBox>();
                YNFsm ??= YNBox.LocateMyFSM("Dialogue Page Control");
                ManagerNormalFsm ??= DialogueManager.LocateMyFSM("Box Open");
                ManagerYNFsm ??= DialogueManager.LocateMyFSM("Box Open YN");
            }
            catch { }
        }

        public void SetTitle(string key)
        {
            gameObject.LocateMyFSM("Conversation Control").GetState("Convo Choice").GetAction<SetFsmString>(4).setValue = key;
        }
        public void SetDreamKey(string key)
        {
            transform.Find("Dream Dialogue").gameObject.LocateMyFSM("npc_dream_dialogue").FsmVariables.FindFsmString("Convo Name").Value = key;
        }

        public Func<DialogueCallbackOptions, DialogueOptions> DialogueSelector;

        public void SetUp()
        {
            TryGetReferences();
            gameObject.GetComponent<AudioSource>().Stop();
            gameObject.GetComponent<AudioSource>().loop = false;
            gameObject.LocateMyFSM("Conversation Control").GetState("Convo Choice").RemoveAction(6);
            FsmState state = gameObject.LocateMyFSM("Conversation Control").GetState("Precept");
            state.Actions = new FsmStateAction[] { new CustomCallMethod(() => StartCoroutine(SelectDialogue())) };
            FsmState more = gameObject.LocateMyFSM("Conversation Control").CreateState("More", () => StartCoroutine(SelectDialogue()));
            FsmState yes = gameObject.LocateMyFSM("Conversation Control").CreateState("MoreYes", () => lastResponse.Response = DialogueResponse.Yes);
            FsmState no = gameObject.LocateMyFSM("Conversation Control").CreateState("MoreNo", () => lastResponse.Response = DialogueResponse.No);
            state.ChangeTransition("CONVO_FINISH", more.Name);
            state.AddTransition("YES", yes.Name);
            state.AddTransition("NO", no.Name);
            more.AddTransition("CONVO_FINISH", more.Name);
            more.AddTransition("YES", yes.Name);
            more.AddTransition("NO", no.Name);
            yes.AddTransition("FINISHED", more.Name);
            no.AddTransition("FINISHED", more.Name);
        }

        private IEnumerator SelectDialogue()
        {
            TryGetReferences();
            DialogueOptions options = DialogueSelector.Invoke(lastResponse);
            if (options.Wait != null)
            {
                if (lastResponse.Continue)
                {
                    lastResponse.Continue = false;
                    yield return Down(lastResponse.Type);
                }
                yield return options.Wait;
            }
            if (!options.Continue)
            {
                if (lastResponse.Continue)
                    yield return Down(lastResponse.Type);
                lastResponse = new DialogueCallbackOptions(options);
                gameObject.LocateMyFSM("Conversation Control").SetState("Talk Finish");
                yield break;
            }
            if (!lastResponse.Continue)
                yield return Up(options.Type);
            else if (options.Type != lastResponse.Type)
            {
                yield return Down(lastResponse.Type);
                yield return Up(options.Type);
            }
            lastResponse = new DialogueCallbackOptions(options);
            if (options.Type == DialogueType.Normal)
                NormalDialogueBox.StartConversation(options.Key, options.Sheet);
            else
            {
                YNFsm.GetFsmInt("Toll Cost").Value = options.Cost;
                YNFsm.GetFsmGameObject("Requester").Value = gameObject;
                YNDialogueBox.StartConversation(options.Key, options.Sheet);
            }
        }

        private IEnumerator Up(DialogueType type)
        {
            if (type == DialogueType.Normal)
                ManagerNormalFsm.SendEvent("BOX UP");
            else
            {
                ManagerNormalFsm.SendEvent("BOX UP YN");
                ManagerYNFsm.SendEvent("BOX UP YN");
            }
            yield return new WaitForSeconds(0.3f);
        }
        private IEnumerator Down(DialogueType type)
        {
            if (type == DialogueType.Normal)
                ManagerNormalFsm.SendEvent("BOX DOWN");
            else
            {
                ManagerNormalFsm.SendEvent("BOX DOWN YN");
                ManagerYNFsm.SendEvent("BOX DOWN YN");
            }
            yield return new WaitForSeconds(0.3f);
        }
    }
    public struct DialogueOptions
    {
        public DialogueType Type;
        public string Key;
        public string Sheet;
        public int Cost;
        public bool Continue;
        public IEnumerator Wait;
    }
    public struct DialogueCallbackOptions
    {
        internal DialogueCallbackOptions(DialogueOptions options) { Type = options.Type; Key = options.Key; Sheet = options.Sheet; Cost = options.Cost; Continue = options.Continue; Response = DialogueResponse.None; }
        public DialogueType Type;
        public string Key;
        public string Sheet;
        public int Cost;
        public bool Continue;
        public DialogueResponse Response;
    }
    public enum DialogueType
    {
        Normal,
        YesNo
    }
    public enum DialogueResponse
    {
        None,
        Yes,
        No
    }
}