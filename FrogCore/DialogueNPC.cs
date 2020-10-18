using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using InControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using Vasi;
using ModCommon;
using System.Collections;
using System.IO;

namespace FrogCore
{
    /// <summary>
    /// Allows for easily adding a npc. Add this monobehavior to a clone of the   "_NPCs\Zote Final Scene\Zote Final"   object in the   "Town"   scene and fill out all of the variables to get it working
    /// </summary>
    class DialogueNPC : MonoBehaviour
    {
        public string NextKey;

        GameObject BoxObject;

        DialogueBox DBox;

        public string NPC_TITLE = "MyNpc";

        public string NPC_DREAM_KEY = "MyNpc_Dreamnail";

        public static void Log(object o)
        {
            Modding.Logger.Log(string.Format("[{0}]: ", Assembly.GetExecutingAssembly().GetName().Name) + " DialogueNPC: " + o);
        }
        public void Start()
        {
            BoxObject = gameObject.LocateMyFSM("Conversation Control").GetState("Repeat").GetAction<CallMethodProper>(0).gameObject.GameObject.Value;
            gameObject.LocateMyFSM("Conversation Control").GetState("Convo Choice").RemoveAction(6);
            gameObject.LocateMyFSM("Conversation Control").GetState("Convo Choice").GetAction<SetFsmString>().setValue = NPC_TITLE;
            FsmState state = gameObject.LocateMyFSM("Conversation Control").GetState("Precept");
            transform.Find("Dream Dialogue").gameObject.LocateMyFSM("npc_dream_dialogue").FsmVariables.FindFsmString("Convo Name").Value = NPC_DREAM_KEY;
            gameObject.GetComponent<AudioSource>().Stop();
            gameObject.GetComponent<AudioSource>().loop = false;
            state.Actions = new FsmStateAction[]
            {
                new Vasi.InvokeMethod(SelectDialogue)
            };
            FsmState state2 = gameObject.LocateMyFSM("Conversation Control").CreateState("More");
            state2.Actions = new FsmStateAction[]
            {
                new Vasi.InvokeCoroutine(ContinueConvo, false)
            };
            Log("Editing fsm8");
            state.ChangeTransition("CONVO_FINISH", "More");
            state2.Transitions = state2.Transitions.Append(new FsmTransition
            {
                FsmEvent = FsmEvent.FindEvent("CONVO_FINISH"),
                ToState = "More"
            }).ToArray<FsmTransition>();
        }
        private IEnumerator ContinueConvo()
        {
            yield return null;
            gameObject.GetComponent<AudioSource>().Stop();
            int convonum = int.Parse(NextKey.Split('_')[2]);
            Log(NextKey);
            Log(NextKey.Split('_')[0] + "_" + NextKey.Split('_')[1] + "_" + (convonum + 1).ToString());
            if (Dialogue.Contains(NextKey.Split('_')[0] + "_" + NextKey.Split('_')[1] + "_" + (convonum + 1).ToString()))
            {
                NextKey = NextKey.Split('_')[0] + "_" + NextKey.Split('_')[1] + "_" + (convonum + 1).ToString();
                Log(NextKey);
                yield return new WaitForSeconds(0.1f);
                DecideSound();
                yield return new WaitForSeconds(0.1f);
                DBox.StartConversation(NextKey, "");
            }
            else
            {
                Log(NextKey);
                gameObject.LocateMyFSM("Conversation Control").SetState("Talk Finish");
            }
            yield return null;
        }
        private void DecideSound()
        {
            if (SingleClips.ContainsKey(NextKey))
                SingleSound();
            else if (MultiClips.ContainsKey(NextKey))
                MultiSound();
            else
                MissingSound();
        }
        private void SingleSound()
        {
            Log("Key found in single clips: " + NextKey);
            gameObject.GetComponent<AudioSource>().clip = SingleClips[NextKey];
            gameObject.GetComponent<AudioSource>().Play();
        }
        private void MultiSound()
        {
            Log("Key found in multi clips: " + NextKey);
            gameObject.GetComponent<AudioSource>().clip = MultiClips[NextKey][UnityEngine.Random.Range(0, MultiClips[NextKey].Length)];
            gameObject.GetComponent<AudioSource>().Play();
        }
        private void MissingSound()
        {
            Log("Key not found in clips: " + NextKey + "    This is not an error");
        }
        public void SelectDialogue()
        {
            if (!BoxObject)
                BoxObject = gameObject.LocateMyFSM("Conversation Control").GetState("Repeat").GetAction<CallMethodProper>(0).gameObject.GameObject.Value;
            DBox = BoxObject.GetOrAddComponent<DialogueBox>();
            NextKey = DialogueSelector.Invoke();
            DecideSound();
            DBox.StartConversation(NextKey, "");
        }
        public Dictionary<string, AudioClip> SingleClips = new Dictionary<string, AudioClip>() {

            { "PLACEHOLDER_1", new AudioClip() },
        };
        public Dictionary<string, AudioClip[]> MultiClips = new Dictionary<string, AudioClip[]>() {

            { "MULTIHOLDER_1", new AudioClip[]
            { new AudioClip()} },
        };
        public string[] Dialogue = new string[]
        {
            "HOLDER_1",
            "PLACEHOLDER_1",
            "MULTIHOLDER_1",
            "HOLDER_2"
        };
        public Func<string> DialogueSelector;
    }
}
