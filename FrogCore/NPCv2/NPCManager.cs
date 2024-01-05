using System;
using UnityEngine;
using FrogCore.NPCv2.Prompts;
using FrogCore.Fsm;
using HutongGames.PlayMaker.Actions;

namespace FrogCore.NPCv2
{
    internal static class NPCManager
    {
        public static GameObject zotePrefab { get; internal set; }
        public static GameObject shopPrefab { get; internal set; }

        public static GameObject ShopUI { get { if (!_ShopUI) SetUpShopUI(); return _ShopUI; } }
        private static GameObject _ShopUI;
        public static GameObject ShopConfirmUI { get { if (!_ShopConfirmUI) SetUpShopConfirmUI(); return _ShopConfirmUI; } }
        private static GameObject _ShopConfirmUI;

        public static GameObject DialogueManager { get { if (!_DialogueManager) SetUpDialogueManager(); return _DialogueManager; } }
        private static GameObject _DialogueManager;
        public static GameObject NormalBox { get { if (!_NormalBox) SetUpNormalBox(); return _NormalBox; } }
        private static GameObject _NormalBox;
        public static GameObject YNBox { get { if (!_YNBox) SetUpYNBox(); return _YNBox; } }
        private static GameObject _YNBox;

        public static DialogueBox NormalDialogueBox { get { if (!_NormalDialogueBox) SetUpNormalDialogueBox(); return _NormalDialogueBox; } }
        private static DialogueBox _NormalDialogueBox;
        public static DialogueBox YNDialogueBox { get { if (!_YNDialogueBox) SetUpYNDialogueBox(); return _YNDialogueBox; } }
        private static DialogueBox _YNDialogueBox;
        public static PlayMakerFSM YNFsm { get { if (!_YNFsm) SetUpYNFsm(); return _YNFsm; } }
        private static PlayMakerFSM _YNFsm;
        public static PlayMakerFSM ManagerNormalFsm { get { if (!_ManagerNormalFsm) SetUpManagerNormalFsm(); return _ManagerNormalFsm; } }
        private static PlayMakerFSM _ManagerNormalFsm;
        public static PlayMakerFSM ManagerYNFsm { get { if (!_ManagerYNFsm) SetUpManagerYNFsm(); return _ManagerYNFsm; } }
        private static PlayMakerFSM _ManagerYNFsm;

        public static SetTextMeshProGameText YesSetText { get { if (!_YesSetText) SetUpYesSetText(); return _YesSetText; } }
        public static SetTextMeshProGameText _YesSetText;
        public static SetTextMeshProGameText NoSetText { get { if (!_NoSetText) SetUpNoSetText(); return _NoSetText; } }
        public static SetTextMeshProGameText _NoSetText;

        public static void SetUpPrefabs(GameObject zotePrefab, GameObject shopPrefab)
        {
            NPCManager.zotePrefab = zotePrefab;
            NPCManager.shopPrefab = shopPrefab;
        }

        private static void SetUpShopUI()
        {
            _ShopUI = Shop.SetUpPrefab();
        }

        private static void SetUpShopConfirmUI()
        {
            _ShopConfirmUI = ShopConfirmUI.SetUpPrefab();
        }

        private static void SetUpDialogueManager() => _DialogueManager ??= NormalBox?.transform.parent.gameObject;

        private static void SetUpNormalBox() => _NormalBox ??= zotePrefab.LocateMyFSM("Conversation Control").GetState("Repeat").GetAction<CallMethodProper>(0).gameObject.GameObject.Value;

        private static void SetUpYNBox() => _YNBox ??= DialogueManager?.transform.Find("Text YN").gameObject;

        private static void SetUpNormalDialogueBox() => _NormalDialogueBox ??= NormalBox?.GetComponent<DialogueBox>();

        private static void SetUpYNDialogueBox() => _YNDialogueBox ??= YNBox?.GetComponent<DialogueBox>();

        private static void SetUpYNFsm() => _YNFsm ??= YNBox?.LocateMyFSM("Dialogue Page Control");

        private static void SetUpManagerNormalFsm() => _ManagerNormalFsm ??= DialogueManager?.LocateMyFSM("Box Open");

        private static void SetUpManagerYNFsm() => _ManagerYNFsm ??= DialogueManager?.LocateMyFSM("Box Open YN");

        private static void SetUpYesSetText() => _YesSetText ??= YNBox?.transform.Find("UI List/Yes").GetComponent<SetTextMeshProGameText>();

        private static void SetUpNoSetText() => _NoSetText ??= YNBox?.transform.Find("UI List/No").GetComponent<SetTextMeshProGameText>();
    }
}