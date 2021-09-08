using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using FrogCore.Fsm;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;

namespace FrogCore
{
    /// <summary>
    /// UNFINISHED! DO NOT USE
    /// </summary>
    public static class InventoryHelper
    {
        private static void Init()
        {
            initiated = true;
            GameManager.instance.inventoryFSM.Preprocess();
            Panels = GameManager.instance.inventoryFSM.GetAction<IntSwitch>("Check Current Pane", 11).compareTo.Last().Value + 1;
            GameManager.instance.inventoryFSM.AddMethod("Open", () => OpenMethod = "Open");
            GameManager.instance.inventoryFSM.AddMethod("Set L Increment", () => OpenMethod = "Left");
            GameManager.instance.inventoryFSM.AddMethod("Set R Increment", () => OpenMethod = "Right");
            GameManager.instance.inventoryFSM.AddMethod("Opened", CallPanel);
        }
        private static void CallPanel()
        {
            int currentPanel = GameManager.instance.inventoryFSM.FsmVariables.FindFsmInt("Current Pane Num").Value;
            if (CustomPanelReceivers.TryGetValue(currentPanel, out InventoryPanel panel))
                switch (OpenMethod)
                {
                    case "Open":
                        panel.SetSelected(panel.FromOpen());
                        break;
                    case "Right":
                        panel.SetSelected(panel.FromLeft());
                        break;
                    case "Left":
                        panel.SetSelected(panel.FromRight());
                        break;
                }
        }
        /// <summary>
        /// adds a invintory panel
        /// </summary>
        /// <param name="PanelIndex">the panel index, used in (language) PANE_# and (bool) hasPane#</param>
        /// <returns>the inventory panel</returns>
        public static GameObject AddInventoryPanel(out int PanelIndex)
        {
            if (!initiated)
                Init();
            GameObject go = new GameObject("Inventory Panel " + Panels);
            PanelIndex = AddInventoryPanel(go);
            return go;
        }
        /// <summary>
        /// adds a invintory panel
        /// </summary>
        /// <param name="go">the gameobject used for the panel</param>
        /// <returns>the panel index, used in (language) PANE_# and (bool) hasPane#</returns>
        public static int AddInventoryPanel(GameObject go)
        {
            if (!initiated)
                Init();

            FsmEvent panelEvent = new FsmEvent("Panel " + Panels);
            FsmGameObject fsmPanel = new FsmGameObject() { Name = Panels + " Pane", Value = go };

            GameManager.instance.inventoryFSM.FsmVariables.GameObjectVariables = 
                GameManager.instance.inventoryFSM.FsmVariables.GameObjectVariables.Append(fsmPanel).ToArray();

            IntSwitch loopThrough = GameManager.instance.inventoryFSM.GetAction<IntSwitch>("Loop Through", 3);

            FsmEvent[] newevents = loopThrough.sendEvent.Append(panelEvent).ToArray();
            FsmInt[] newpanels = loopThrough.compareTo.Append(Panels).ToArray();
            newpanels[5] = Panels + 1;

            loopThrough.compareTo = newpanels;
            loopThrough.sendEvent = newevents;

            IntSwitch checkLPane = GameManager.instance.inventoryFSM.GetAction<IntSwitch>("Check L Pane", 1);
            checkLPane.compareTo = newpanels;
            checkLPane.sendEvent = newevents;

            IntSwitch checkRPane = GameManager.instance.inventoryFSM.GetAction<IntSwitch>("Check R Pane", 1);
            checkRPane.compareTo = newpanels;
            checkRPane.sendEvent = newevents;

            GameManager.instance.inventoryFSM.GetAction<IntSwitch>("Check Current Pane", 11).compareTo =
                GameManager.instance.inventoryFSM.GetAction<IntSwitch>("Check Current Pane", 11).compareTo.Append(Panels).ToArray();
            GameManager.instance.inventoryFSM.GetAction<IntSwitch>("Check Current Pane", 11).sendEvent =
                GameManager.instance.inventoryFSM.GetAction<IntSwitch>("Check Current Pane", 11).sendEvent.Append(panelEvent).ToArray();

            GameManager.instance.inventoryFSM.GetAction<SetIntValue>("Under", 0).intValue = Panels + 1;
            GameManager.instance.inventoryFSM.GetAction<SetIntValue>("Under 2", 0).intValue = Panels + 1;
            GameManager.instance.inventoryFSM.GetAction<SetIntValue>("Under 3", 0).intValue = Panels + 1;

            FsmState test = GameManager.instance.inventoryFSM.CopyState("Next Charms", "Next Panel " + Panels);
            FsmState testR = GameManager.instance.inventoryFSM.CopyState("Next Charms 2", "Next Panel " + Panels + " 2");
            FsmState testL = GameManager.instance.inventoryFSM.CopyState("Next Charms 3", "Next Panel " + Panels + " 3");
            FsmState open = GameManager.instance.inventoryFSM.CopyState("Open charms", "Open Panel " + Panels);

            test.GetAction<PlayerDataBoolTest>(0).boolName = "hasPane" + Panels;
            test.GetAction<SetGameObject>(2).gameObject = fsmPanel;
            test.GetAction<GetLanguageString>(3).convName = "PANE_" + Panels;

            testR.GetAction<PlayerDataBoolTest>(0).boolName = "hasPane" + Panels;
            testR.GetAction<GetLanguageString>(1).convName = "PANE_" + Panels;

            testL.GetAction<PlayerDataBoolTest>(0).boolName = "hasPane" + Panels;
            testL.GetAction<GetLanguageString>(1).convName = "PANE_" + Panels;

            open.GetAction<GetLanguageString>(0).convName = "PANE_" + Panels;
            open.GetAction<SetGameObject>(2).gameObject = fsmPanel;
            open.GetAction<SetIntValue>(3).intValue = Panels;
            open.GetAction<SetIntValue>(4).intValue = Panels;

            GameManager.instance.inventoryFSM.AddTransition("Loop Through", panelEvent, test.Name);
            GameManager.instance.inventoryFSM.AddTransition("Check R Pane", panelEvent, testR.Name);
            GameManager.instance.inventoryFSM.AddTransition("Check L Pane", panelEvent, testL.Name);
            GameManager.instance.inventoryFSM.AddTransition("Check Current Pane", panelEvent, open.Name);

            Panels++;
            return Panels - 1;
        }
        internal static void AddInventoryPanel(InventoryPanel type)
        {
            if (!initiated)
                Init();
            GameObject go = new GameObject("Inventory Panel " + Panels + type.GetType().Name);
            AddInventoryPanel(go);
            GameObject cursor = GameObject.Instantiate(GameManager.instance.inventoryFSM.transform.Find("Inv").Find("Cursor").gameObject);
            PlayMakerFSM cursorFSM = cursor.LocateMyFSM("Cursor Movement");
            MethodBehaviour methods = go.AddComponent<MethodBehaviour>();
            methods.OnEnableMethod = _ => type.OnEnable();
            methods.OnDisableMethod = _ => type.OnDisable();
            methods.UpdateMethod = _ => type.OnUpdate();
            cursorFSM.InsertMethod("Move", 18, type.SetCursorOffsets);
            type.cursorFSM = cursorFSM;
            type.go = go;
            go.SetActive(false);
            CustomPanelReceivers.Add(Panels - 1, type);
        }
        private static int Panels = 0;
        private static bool initiated = false;
        private static string OpenMethod = "Open";
        private static Dictionary<int, InventoryPanel> CustomPanelReceivers = new Dictionary<int, InventoryPanel>();
    }
}
