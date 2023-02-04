using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Modding;
using System.Collections;
using HutongGames.PlayMaker.Actions;
using HutongGames.PlayMaker;
using FrogCore.Ext;
using FrogCore.Fsm;
using System.Reflection;

namespace FrogCore
{
    /// <summary>
    /// Allows for easily adding journal entries. Just use the static method or create a new JournalHelper to add one
    /// </summary>
    public class JournalHelper
    {
        #region Extentions
        public void RecordJournalEntry()
        {
            string playerDataName = "CustomJournal" + entrynumber;
            PlayerData playerData = GameManager.instance.playerData;
            string killedName = "killed" + playerDataName;
            string killsName = "kills" + playerDataName;
            string newName = "newData" + playerDataName;
            bool firstKill = false;
            if (!playerData.GetBool(killedName))
            {
                firstKill = true;
                playerData.SetBool(killedName, true);
                playerData.SetBool(newName, true);
            }
            bool lastKill = false;
            int killsLeft = playerData.GetInt(killsName);
            if (killsLeft > 0)
            {
                killsLeft--;
                playerData.SetInt(killsName, killsLeft);
                if (killsLeft <= 0)
                    lastKill = true;
            }
            if (playerData.GetBool("hasJournal"))
            {
                if (lastKill)
                    playerData.SetInt("journalEntriesCompleted", playerData.GetInt("journalEntriesCompleted") + 1);
                else if (firstKill)
                    playerData.SetInt("journalNotesCompleted", playerData.GetInt("journalNotesCompleted") + 1);
                if (lastKill || firstKill)
                {
                    GameObject journalUpdateMessageSpawned = ReflectionHelper.GetField<EnemyDeathEffects, GameObject>("journalUpdateMessageSpawned");
                    if (!journalUpdateMessageSpawned && !notificationPrefab)
                    {
                        foreach (EnemyDeathEffects ede in Resources.FindObjectsOfTypeAll<EnemyDeathEffects>())
                        {
                            GameObject journalUpdateMessagePrefab = ReflectionHelper.GetField<EnemyDeathEffects, GameObject>(ede, "journalUpdateMessagePrefab");
                            if (journalUpdateMessagePrefab)
                            {
                                notificationPrefab = GameObject.Instantiate(journalUpdateMessagePrefab);
                                notificationPrefab.name = journalUpdateMessagePrefab.name;
                                notificationPrefab.SetActive(false);
                                GameObject.DontDestroyOnLoad(notificationPrefab);
                                journalUpdateMessageSpawned = UnityEngine.Object.Instantiate<GameObject>(journalUpdateMessagePrefab);
                                journalUpdateMessageSpawned.SetActive(false);
                                ReflectionHelper.SetField<EnemyDeathEffects, GameObject>("journalUpdateMessageSpawned", journalUpdateMessageSpawned);
                                break;
                            }
                        }
                    }
                    else if (!journalUpdateMessageSpawned && notificationPrefab)
                    {
                        journalUpdateMessageSpawned = UnityEngine.Object.Instantiate<GameObject>(notificationPrefab);
                        journalUpdateMessageSpawned.SetActive(false);
                    }
                    if (journalUpdateMessageSpawned)
                    {
                        if (journalUpdateMessageSpawned.activeSelf)
                            journalUpdateMessageSpawned.SetActive(false);
                        journalUpdateMessageSpawned.SetActive(true);
                        PlayMakerFSM playMakerFSM = journalUpdateMessageSpawned.LocateMyFSM("Journal Msg");
                        playMakerFSM.FsmVariables.FindFsmBool("Full").Value = lastKill;
                        playMakerFSM.FsmVariables.FindFsmBool("Should Recycle").Value = true;
                    }
                }
            }
        }
        public void UnHook()
        {
            On.JournalList.BuildEnemyList -= JournalList_BuildEnemyList;
            //ExtraHooks.OnFsmAwake["Item List Control"] -= ItemListControlFSMAwake;
            ModHooks.LanguageGetHook -= Instance_LanguageGetHook;
            ModHooks.GetPlayerIntHook -= Instance_GetPlayerIntHook;
            ModHooks.SetPlayerIntHook -= Instance_SetPlayerIntHook;
            ModHooks.GetPlayerBoolHook -= Instance_GetPlayerBoolHook;
            ModHooks.SetPlayerBoolHook -= Instance_SetPlayerBoolHook;
            On.PlayerData.CountJournalEntries -= PlayerData_CountJournalEntries;
        }
        public void Hook()
        {
            On.JournalList.BuildEnemyList += JournalList_BuildEnemyList;
            //ExtraHooks.OnFsmAwake["Item List Control"] += ItemListControlFSMAwake;
            ModHooks.LanguageGetHook += Instance_LanguageGetHook;
            ModHooks.GetPlayerIntHook += Instance_GetPlayerIntHook;
            ModHooks.SetPlayerIntHook += Instance_SetPlayerIntHook;
            ModHooks.GetPlayerBoolHook += Instance_GetPlayerBoolHook;
            ModHooks.SetPlayerBoolHook += Instance_SetPlayerBoolHook;
            On.PlayerData.CountJournalEntries += PlayerData_CountJournalEntries;
        }
        private void ItemListControlFSMAwake(PlayMakerFSM fsm)
        {
            Ext.Extensions.Log("item list control fsm awake");
        }
        public override string ToString() => GetEntryName();
        public string GetEntryName() => "CustomJournal" + entrynumber;
        #endregion
        #region Add Entries
        static JournalHelper()
        {
            On.JournalList.BuildEnemyList += JournalList_BuildEnemyList_Static;
        }
        public static JournalHelper AddJournalEntry(Sprite portrait, Sprite picture, JournalPlayerData jpd, JournalNameStrings names, string insertafter = null, EntryType entryType = EntryType.Normal, Sprite customentrysprite = null, bool addtracker = true, bool addhooks = true)
        {
            return new JournalHelper(portrait, picture, jpd, names, insertafter, entryType, customentrysprite, addtracker, addhooks);
        }
        public JournalHelper(Sprite portrait, Sprite picture, JournalPlayerData jpd, JournalNameStrings names, string insertafter = null, EntryType entryType = EntryType.Normal, Sprite customentrysprite = null, bool addtracker = true, bool extrahooks = true)
        {
            bool hook = true;
            portraitsprite = portrait;
            picturesprite = picture;
            addingtracker = addtracker;
            CustomEntries++;
            entrynumber = CustomEntries;
            InsertAfter = insertafter;
            EType = entryType;
            if (hook)
            {
                On.JournalList.BuildEnemyList += JournalList_BuildEnemyList;
                //ExtraHooks.OnFsmAwake["Item List Control"] += ItemListControlFSMAwake;
                if (extrahooks)
                {
                    playerData = jpd;
                    nameStrings = names;
                    ModHooks.LanguageGetHook += Instance_LanguageGetHook;
                    ModHooks.GetPlayerIntHook += Instance_GetPlayerIntHook;
                    ModHooks.SetPlayerIntHook += Instance_SetPlayerIntHook;
                    ModHooks.GetPlayerBoolHook += Instance_GetPlayerBoolHook;
                    ModHooks.SetPlayerBoolHook += Instance_SetPlayerBoolHook;
                    On.PlayerData.CountJournalEntries += PlayerData_CountJournalEntries;
                }
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
        public string InsertAfter;
        public JournalList ListInstance { get; private set; }
        public int entrynumber { get; private set; } = 0;
        public static int CustomEntries { get; private set; } = 0;
        public static List<JournalTracker> trackers { get; private set; } = new List<JournalTracker>();
        private static GameObject notificationPrefab;
        #endregion
        #region Hooks
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
        private static void JournalList_BuildEnemyList_Static(On.JournalList.orig_BuildEnemyList orig, JournalList self)
        {
            orig(self);
            try
            {
                var current = ReflectionHelper.GetField<JournalList, GameObject[]>(self, "currentList");
                if (current == null)
                {
                    self.UpdateEnemyList();
                    current = ReflectionHelper.GetField<JournalList, GameObject[]>(self, "currentList");
                }
                PlayerData.instance.lastJournalItem = Mathf.Clamp(PlayerData.instance.lastJournalItem, 0, current.Length - 1);
            }
            catch (Exception e)
            {
                Ext.Extensions.Log("Journal Helper", e);
            }
            PlayerData.instance.CountJournalEntries();
        }
        private void JournalList_BuildEnemyList(On.JournalList.orig_BuildEnemyList orig, JournalList self)
        {
            #region modifyFSM
            ListInstance = self;
            var fsm = self.gameObject.LocateMyFSM("Item List Control");
            var skip = false;
            foreach (var state in fsm.FsmStates)
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
                    var list = ReflectionHelper.GetField<JournalList, GameObject[]>(self, "currentList");
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
                state.AddAction(new CustomCallMethod(Check));
                fsm.ChangeTransition("Display Kills", "FINISHED", "Custom Check");
                fsm.ChangeTransition("Get Notes", "FINISHED", "Custom Check");
            }
            #endregion
            GameObject tmpGo = null;
            try
            {
                tmpGo = self.list.First(x => x.GetComponent<JournalEntryStats>().convoName == GetEntryName());
            }
            catch { }
            if (tmpGo == null)
            {
                //entrynumber = self.list.Length + 1;
                var go = GameObject.Instantiate(self.list[0]);
                var listitem = go.GetComponent<JournalEntryStats>();
                listitem.convoName = GetEntryName();
                listitem.sprite = picturesprite;
                go.transform.Find("Portrait").GetComponent<SpriteRenderer>().sprite = portraitsprite;
                go.transform.Find("Name").GetComponent<SetTextMeshProGameText>().convName = GetEntryName();
                listitem.playerDataName = GetEntryName();
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
                if (string.IsNullOrEmpty(InsertAfter))
                {
                    Ext.Extensions.Log("Journal Helper", "NO Insert After present(not a error), defaulting to the end of the list on custom entry " + entrynumber);
                    self.list = self.list.Append(go).ToArray();
                }
                else
                {
                    bool containsbool = false;
                    tmpGo = null;
                    try
                    {
                        tmpGo = self.list.First(x => x.GetComponent<JournalEntryStats>().playerDataName == InsertAfter);
                        containsbool = tmpGo != null;
                    }
                    catch { }
                    int index = containsbool ? Array.IndexOf(self.list, tmpGo) : 0;
                    if (containsbool)
                    {
                        Ext.Extensions.Log("Journal Helper", "Insert After found, adding custom entry " + entrynumber);
                        //var tmpList = self.list.Insert(index + 1, go).ToList();
                        //if ((tmpList[tmpList.Count - 1] = go) && (tmpList.IndexOf(go) != tmpList.Count - 1))
                        //tmpList.RemoveAt(tmpList.Count - 1); //for when vasi was broken
                        //self.list = tmpList.ToArray();
                        self.list = self.list.Insert(index + 1, go).ToArray();
                    }
                    else
                    {
                        Ext.Extensions.Log("Journal Helper", "Insert After present, but NOT found, defaulting to the end of the list on custom entry " + entrynumber);
                        self.list = self.list.Append(go).ToArray();
                    }
                }
            }
            orig(self);
        }
        private string Instance_LanguageGetHook(string key, string sheetTitle, string orig)
        {
            if (key == "NAME_" + GetEntryName())
            {
                return nameStrings.name;
            }
            if (key == "DESC_" + GetEntryName())
            {
                return nameStrings.desc;
            }
            if (key == "NOTE_" + GetEntryName())
            {
                return nameStrings.note;
            }
            if (key == GetEntryName())
            {
                return nameStrings.shortname;
            }
            //if (key == "PANE_" + panelindex)
                //return nameStrings.name;
            return orig;
        }
        #endregion
        #region bool hooks
        private bool Instance_SetPlayerBoolHook(string originalSet, bool orig)
        {
            if (originalSet == "killed" + GetEntryName())
                playerData.haskilled = orig;
            if (originalSet == "newData" + GetEntryName())
                playerData.newentry = orig;
            return orig;
        }
        private bool Instance_GetPlayerBoolHook(string originalSet, bool orig)
        {
            if (originalSet == "killed" + GetEntryName())
                return playerData.haskilled;
            if (originalSet == "newData" + GetEntryName())
                return playerData.newentry;
            //if (originalSet == "hasPane" + panelindex)
                //return true;
            return orig;
        }
        #endregion
        #region int hooks
        private int Instance_SetPlayerIntHook(string intName, int orig)
        {
            if (intName == "kills" + GetEntryName())
                playerData.killsremaining = orig;
            return orig;
        }
        private int Instance_GetPlayerIntHook(string intName, int orig)
        {
            if (intName == "kills" + GetEntryName())
                return playerData.killsremaining;
            return orig;
        }
        #endregion
        #endregion
        #region Custom Data
        [Serializable]
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
