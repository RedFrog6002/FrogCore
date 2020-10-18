using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Vasi;
using Modding;
using ModCommon;
using System.Collections;
using HutongGames.PlayMaker.Actions;

namespace FrogCore
{
    /// <summary>
    /// Allows for easily adding journal entries. Just use the static method or create a new JournalHelper to add one
    /// </summary>
    public class JournalHelper
    {
        #region addentries
        public static JournalHelper AddJournalEntry(Sprite portrait, Sprite picture, JournalPlayerData jpd, JournalNameStrings names, EntryType entryType = EntryType.Normal, Sprite customentrysprite = null, bool addtracker = true, bool addhooks = true)
        {
            return new JournalHelper(portrait, picture, jpd, names, entryType, customentrysprite, addtracker, addhooks);
        }
        public JournalHelper(Sprite portrait, Sprite picture, JournalPlayerData jpd, JournalNameStrings names, EntryType entryType = EntryType.Normal, Sprite customentrysprite = null, bool addtracker = true, bool addhooks = true)
        {
            portraitsprite = portrait;
            picturesprite = picture;
            addingtracker = addtracker;
            CustomEntries++;
            EType = entryType;
            On.JournalList.BuildEnemyList += JournalList_BuildEnemyList;
            if (addhooks)
            {
                playerData = jpd;
                nameStrings = names;
                ModHooks.Instance.LanguageGetHook += Instance_LanguageGetHook;
                ModHooks.Instance.GetPlayerIntHook += Instance_GetPlayerIntHook;
                ModHooks.Instance.SetPlayerIntHook += Instance_SetPlayerIntHook;
                ModHooks.Instance.GetPlayerBoolHook += Instance_GetPlayerBoolHook;
                ModHooks.Instance.SetPlayerBoolHook += Instance_SetPlayerBoolHook;
                On.PlayerData.CountJournalEntries += PlayerData_CountJournalEntries;
            }
            if (entryType == EntryType.Custom && customentrysprite != null)
            {
                CustomSprite = customentrysprite;
            }
        }

        public EntryType EType;
        public bool addingtracker;
        public JournalNameStrings nameStrings;
        public JournalPlayerData playerData;
        public JournalTracker tracker;
        public Sprite portraitsprite;
        public Sprite picturesprite;
        public Sprite CustomSprite;
        public JournalList ListInstance { get; private set; }
        public int entrynumber { get; private set; } = 0;
        public static int CustomEntries { get; private set; } = 0;
        public static List<JournalTracker> trackers { get; private set; } = new List<JournalTracker>();
        #endregion
        #region hooks
        #region other hooks
        private void PlayerData_CountJournalEntries(On.PlayerData.orig_CountJournalEntries orig, PlayerData self)
        {
            orig(self);
            if (playerData.haskilled || !playerData.Hidden)
            {
                self.SetInt("journalEntriesTotal", self.GetInt("journalEntriesTotal") + 1);
            }
            if (playerData.haskilled)
            {
                self.SetInt("journalEntriesCompleted", self.GetInt("journalEntriesCompleted") + 1);
            }
            if (playerData.killsremaining == 0)
            {
                self.SetInt("journalNotesCompleted", self.GetInt("journalNotesCompleted") + 1);
            }
        }
        private void JournalList_BuildEnemyList(On.JournalList.orig_BuildEnemyList orig, JournalList self)
        {
            #region modifyFSM
            ListInstance = self;
            var fsm = self.gameObject.LocateMyFSM("Item List Control");
            var skip = false;
            foreach(var state in fsm.FsmStates)
            {
                if (state.Name == "Custom Check" || state.Name == "Custom")
                {
                    skip = true;
                }
            }
            if (!skip)
            {
                var custom = fsm.CopyState("Normal", "Custom");
                var state = fsm.CreateState("Custom Check");
                void Check()
                {
                    var list = ReflectionHelper.GetAttr<JournalList, GameObject[]>(self, "currentList");
                    var currentitem = list[fsm.FsmVariables.GetFsmInt("Current Item").Value];
                    if (currentitem.GetComponent<JournalEntryStats>().grimmEntry && currentitem.GetComponent<JournalEntryStats>().warriorGhost)
                    {
                        if (currentitem.GetComponent<JournalTracker>() != null)
                            if (currentitem.GetComponent<JournalTracker>().CustomEntrySprite != null)
                            {
                                custom.GetAction<SetSpriteRendererSprite>(0).sprite = currentitem.GetComponent<JournalTracker>().CustomEntrySprite;
                                fsm.SetState("Custom");
                                return;
                            }
                    }
                    else
                    {
                        fsm.SetState("Normal");
                    }
                    fsm.SetState("Type");
                }
                state.AddMethod(Check);
                fsm.ChangeTransition("Display Kills", "FINISHED", "Custom Check");
                fsm.ChangeTransition("Get Notes", "FINISHED", "Custom Check");
            }
            #endregion
            var alreadyadded = false;
            foreach (var v in self.list)
            {
                if (entrynumber != 0)
                {
                    if (v.GetComponent<JournalEntryStats>().convoName == "CustomJournal" + entrynumber)
                        alreadyadded = true;
                }
            }
            if (!alreadyadded)
            {
                entrynumber = self.list.Length + 1;
                var go = GameObject.Instantiate(self.list[0]);
                var listitem = go.GetComponent<JournalEntryStats>();
                listitem.convoName = "CustomJournal" + entrynumber;
                listitem.sprite = picturesprite;
                go.transform.Find("Portrait").GetComponent<SpriteRenderer>().sprite = portraitsprite;
                go.transform.Find("Name").GetComponent<SetTextMeshProGameText>().convName = "CustomJournal" + entrynumber;
                listitem.playerDataName = "CustomJournal" + entrynumber;
                switch (EType)
                {
                    case EntryType.Normal:
                        listitem.warriorGhost = false;
                        listitem.grimmEntry = false;
                        break;
                    case EntryType.Dream:
                        listitem.warriorGhost = true;
                        listitem.grimmEntry = false;
                        break;
                    case EntryType.Grimm:
                        listitem.warriorGhost = false;
                        listitem.grimmEntry = true;
                        break;
                    case EntryType.Custom:
                        listitem.warriorGhost = true;
                        listitem.grimmEntry = true;
                        break;
                }
                if (addingtracker)
                {
                    tracker = go.AddComponent<JournalTracker>();
                    trackers.Add(tracker);
                    go.GetComponent<JournalTracker>().entrynumber = entrynumber;
                    if (CustomSprite != null && EType == EntryType.Custom)
                        go.GetComponent<JournalTracker>().CustomEntrySprite = CustomSprite;
                }
                self.list = self.list.Append(go).ToArray();
            }
            orig(self);
            PlayerData.instance.CountJournalEntries();
        }
        private string Instance_LanguageGetHook(string key, string sheetTitle)
        {
            if (key == "NAME_" + "CustomJournal" + entrynumber)
            {
                return nameStrings.name;
            }
            if (key == "DESC_" + "CustomJournal" + entrynumber)
            {
                return nameStrings.desc;
            }
            if (key == "NOTE_" + "CustomJournal" + entrynumber)
            {
                return nameStrings.note;
            }
            if (key == "CustomJournal" + entrynumber)
            {
                return nameStrings.shortname;
            }
            return Language.Language.GetInternal(key, sheetTitle);
        }
        #endregion
        #region bool hooks
        private void Instance_SetPlayerBoolHook(string originalSet, bool value)
        {
            if (originalSet == "killed" + "CustomJournal" + entrynumber)
                playerData.haskilled = value;
            if (originalSet == "newData" + "CustomJournal" + entrynumber)
                playerData.newentry = value;
            PlayerData.instance.SetBoolInternal(originalSet, value);
        }
        private bool Instance_GetPlayerBoolHook(string originalSet)
        {
            if (originalSet == "killed" + "CustomJournal" + entrynumber)
                return playerData.haskilled;
            if (originalSet == "newData" + "CustomJournal" + entrynumber)
                return playerData.newentry;
            return PlayerData.instance.GetBoolInternal(originalSet);
        }
        #endregion
        #region int hooks
        private void Instance_SetPlayerIntHook(string intName, int value)
        {
            if (intName == "kills" + "CustomJournal" + entrynumber)
                playerData.killsremaining = value;
            PlayerData.instance.SetIntInternal(intName, value);
        }
        private int Instance_GetPlayerIntHook(string intName)
        {
            if (intName == "kills" + "CustomJournal" + entrynumber)
                return playerData.killsremaining;
            return PlayerData.instance.GetIntInternal(intName);
        }
        #endregion
        #endregion
        #region CustomData
        public class JournalPlayerData
        {
            public int killsremaining = 0;
            public bool haskilled = true;
            public bool newentry = false;
            public bool Hidden = true;
        }
        public class JournalNameStrings
        {
            public string name = "My Name";
            public string desc = "My description";
            public string note = "My notes";
            public string shortname = "Name";
        }
        public class JournalTracker : MonoBehaviour
        {
            public int entrynumber;
            public Sprite CustomEntrySprite;
        }
        /// <summary>
        /// Changes the icon between the note/kills left and description. If you are using custom, you MUST set addtracker to true or else the default one will show.
        /// </summary>
        public enum EntryType
        {
            Normal,
            Dream,
            Grimm,
            Custom
        }
        #endregion
    }
}
