using System;
using System.Collections;
using FrogCore.Fsm;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using UnityEngine;

namespace FrogCore
{
    public class CustomShop
    {
        public static GameObject shopPrefab { get; internal set; }

        public ShopMenuStock shop;
        public PlayMakerFSM fsm;

        public CustomShop(CustomItemStats[] items)
        {
            shop = GameObject.Instantiate(shopPrefab).GetComponent<ShopMenuStock>();
            fsm = shop.gameObject.LocateMyFSM("shop_control");
            shop.tag = "Untagged";

            fsm.ChangeTransition("Idle", "SHOP UP", "Open Window");
            shop.altPlayerDataBool = "";
            shop.altPlayerDataBoolAlt = "";
            shop.masterList = null;
            GameObject prefab = shop.stock[0];
            shop.stock = new GameObject[items.Length];
            for (int i = 0; i < items.Length; i++)
            {
                GameObject itemGo = GameObject.Instantiate(prefab);
                ShopItemStats item = itemGo.GetComponent<ShopItemStats>();
                CustomItemStats custom = items[i];

                item.playerDataBoolName = custom.playerDataBoolName;
                item.nameConvo = custom.nameConvo;
                item.descConvo = custom.descConvo;
                item.priceConvo = "";
                item.requiredPlayerDataBool = custom.requiredPlayerDataBool;
                item.removalPlayerDataBool = custom.removalPlayerDataBool;
                item.specialType = (int)custom.specialType;
                item.relicNumber = custom.relicNumber;
                item.charmsRequired = custom.charmsRequired;
                item.activeColour = custom.activeColour;
                item.inactiveColour = custom.inactiveColour;
                item.dungDiscount = custom.dungDiscount;
                item.relic = custom.relic;
                item.relicPDInt = custom.relicPDInt;
                item.notchCostBool = custom.notchCostInt;
                item.cost = custom.cost;
                item.itemNumber = i;
                item.canBuy = custom.canBuy;
                itemGo.transform.Find("Item Sprite").GetComponent<SpriteRenderer>().sprite = custom.sprite;

                shop.stock[i] = itemGo;
            }

            shop.gameObject.SetActive(true);
        }

        public bool ShopUp(bool pauseHero = true, bool pauseHeroAnim = true, bool isInvincible = false)
        {
            if (shop.StockLeft())
            {
                fsm.SendEvent("SHOP UP");
                if (pauseHero)
                    HeroController.instance.RelinquishControl();
                if (pauseHeroAnim)
                    HeroController.instance.StopAnimationControl();
                PlayerData.instance.isInvincible = isInvincible;
                return true;
            }
            return false;
        }

        public void ShopDown(bool unPauseHero = true, bool unPauseHeroAnim = true, bool isInvincible = false)
        {
            fsm.SendEvent("CLOSE SHOP WINDOW");
            if (unPauseHero)
                HeroController.instance.RegainControl();
            if (unPauseHeroAnim)
                HeroController.instance.StartAnimationControl();
            PlayerData.instance.isInvincible = isInvincible;
        }

        public void SetConfirmText(string key, string sheet)
        {
            GetLanguageString lang = fsm.GetAction<GetLanguageString>("Check Relics", 0);
            lang.convName = key;
            lang.sheetName = sheet;
        }

        public void SetCancelText(string key, string sheet)
        {
            GetLanguageString lang = fsm.GetAction<GetLanguageString>("Check Relics", 1);
            lang.convName = key;
            lang.sheetName = sheet;
        }
    }
}