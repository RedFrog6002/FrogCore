using System;
using UnityEngine;

namespace FrogCore
{
    public enum CustomItemType
    {
        None,
        HeartPiece,
        Charm,
        SoulPiece,
        Relic1,
        Relic2,
        Relic3,
        Relic4,
        Notch,
        Map,
        SimpleKey,
        RancidEgg,
        RepairGlassHP,
        RepairGlassGeo,
        RepairGlassAttack,
        SalubrasBlessing,
        MapPin,
        MapMarker,
        Custom
    }
    public struct CustomItemStats
    {
        public static readonly Color defaultActiveColor = new Color(1f, 1f, 1f, 1f);
        public static readonly Color defaultInactiveColor = new Color(0.551f, 0.551f, 0.551f, 1f);

        #region constructor methods
        public static CustomItemStats CreateNormalItem(Sprite sprite, string playerDataBoolName, string nameConvo, string descConvo, int cost, int charmsRequired = 0, string requiredPlayerDataBool = "", string removalPlayerDataBool = "", bool dungDiscount = false, bool canBuy = true)
        {
            CustomItemStats item = new CustomItemStats();
            item.playerDataBoolName = playerDataBoolName;
            item.nameConvo = nameConvo;
            item.descConvo = descConvo;
            item.requiredPlayerDataBool = requiredPlayerDataBool;
            item.removalPlayerDataBool = removalPlayerDataBool;
            item.specialType = CustomItemType.None;
            item.relicNumber = 0;
            item.charmsRequired = charmsRequired;
            item.activeColour = defaultActiveColor;
            item.inactiveColour = defaultInactiveColor;
            item.dungDiscount = dungDiscount;
            item.relic = false;
            item.relicPDInt = "";
            item.notchCostInt = "";
            item.cost = cost;
            item.canBuy = canBuy;
            item.sprite = sprite;

            return item;
        }
        public static CustomItemStats CreateNormalItem(Sprite sprite, string playerDataBoolName, string nameConvo, string descConvo, int cost, Color activeColor, Color inactiveColor, int charmsRequired = 0, string requiredPlayerDataBool = "", string removalPlayerDataBool = "", bool dungDiscount = false, bool canBuy = true)
        {
            CustomItemStats item = CreateNormalItem(sprite, playerDataBoolName, nameConvo, descConvo, cost, charmsRequired, requiredPlayerDataBool, removalPlayerDataBool, dungDiscount, canBuy);
            item.activeColour = activeColor;
            item.inactiveColour = inactiveColor;

            return item;
        }

        public static CustomItemStats CreateHeartPieceItem(Sprite sprite, string playerDataBoolName, string nameConvo, string descConvo, int cost, int charmsRequired = 0, string requiredPlayerDataBool = "", string removalPlayerDataBool = "", bool dungDiscount = false, bool canBuy = true)
        {
            CustomItemStats item = new CustomItemStats();
            item.playerDataBoolName = playerDataBoolName;
            item.nameConvo = nameConvo;
            item.descConvo = descConvo;
            item.requiredPlayerDataBool = requiredPlayerDataBool;
            item.removalPlayerDataBool = removalPlayerDataBool;
            item.specialType = CustomItemType.HeartPiece;
            item.relicNumber = 0;
            item.charmsRequired = charmsRequired;
            item.activeColour = defaultActiveColor;
            item.inactiveColour = defaultInactiveColor;
            item.dungDiscount = dungDiscount;
            item.relic = false;
            item.relicPDInt = "";
            item.notchCostInt = "";
            item.cost = cost;
            item.canBuy = canBuy;
            item.sprite = sprite;

            return item;
        }
        public static CustomItemStats CreateHeartPieceItem(Sprite sprite, string playerDataBoolName, string nameConvo, string descConvo, int cost, Color activeColor, Color inactiveColor, int charmsRequired = 0, string requiredPlayerDataBool = "", string removalPlayerDataBool = "", bool dungDiscount = false, bool canBuy = true)
        {
            CustomItemStats item = CreateHeartPieceItem(sprite, playerDataBoolName, nameConvo, descConvo, cost, charmsRequired, requiredPlayerDataBool, removalPlayerDataBool, dungDiscount, canBuy);
            item.activeColour = activeColor;
            item.inactiveColour = inactiveColor;

            return item;
        }

        public static CustomItemStats CreateCharmItem(Sprite sprite, string playerDataBoolName, string nameConvo, string descConvo, int cost, string notchCostInt, int charmsRequired = 0, string requiredPlayerDataBool = "", string removalPlayerDataBool = "", bool dungDiscount = false, bool canBuy = true)
        {
            CustomItemStats item = new CustomItemStats();
            item.playerDataBoolName = playerDataBoolName;
            item.nameConvo = nameConvo;
            item.descConvo = descConvo;
            item.requiredPlayerDataBool = requiredPlayerDataBool;
            item.removalPlayerDataBool = removalPlayerDataBool;
            item.specialType = CustomItemType.Charm;
            item.relicNumber = 0;
            item.charmsRequired = charmsRequired;
            item.activeColour = defaultActiveColor;
            item.inactiveColour = defaultInactiveColor;
            item.dungDiscount = dungDiscount;
            item.relic = false;
            item.relicPDInt = "";
            item.notchCostInt = notchCostInt;
            item.cost = cost;
            item.canBuy = canBuy;
            item.sprite = sprite;

            return item;
        }
        public static CustomItemStats CreateCharmItem(Sprite sprite, string playerDataBoolName, string nameConvo, string descConvo, int cost, string notchCostInt, Color activeColor, Color inactiveColor, int charmsRequired = 0, string requiredPlayerDataBool = "", string removalPlayerDataBool = "", bool dungDiscount = false, bool canBuy = true)
        {
            CustomItemStats item = CreateCharmItem(sprite, playerDataBoolName, nameConvo, descConvo, cost, notchCostInt, charmsRequired, requiredPlayerDataBool, removalPlayerDataBool, dungDiscount, canBuy);
            item.activeColour = activeColor;
            item.inactiveColour = inactiveColor;

            return item;
        }

        public static CustomItemStats CreateSoulPieceItem(Sprite sprite, string playerDataBoolName, string nameConvo, string descConvo, int cost, int charmsRequired = 0, string requiredPlayerDataBool = "", string removalPlayerDataBool = "", bool dungDiscount = false, bool canBuy = true)
        {
            CustomItemStats item = new CustomItemStats();
            item.playerDataBoolName = playerDataBoolName;
            item.nameConvo = nameConvo;
            item.descConvo = descConvo;
            item.requiredPlayerDataBool = requiredPlayerDataBool;
            item.removalPlayerDataBool = removalPlayerDataBool;
            item.specialType = CustomItemType.SoulPiece;
            item.relicNumber = 0;
            item.charmsRequired = charmsRequired;
            item.activeColour = defaultActiveColor;
            item.inactiveColour = defaultInactiveColor;
            item.dungDiscount = dungDiscount;
            item.relic = false;
            item.relicPDInt = "";
            item.notchCostInt = "";
            item.cost = cost;
            item.canBuy = canBuy;
            item.sprite = sprite;

            return item;
        }
        public static CustomItemStats CreateSoulPieceItem(Sprite sprite, string playerDataBoolName, string nameConvo, string descConvo, int cost, Color activeColor, Color inactiveColor, int charmsRequired = 0, string requiredPlayerDataBool = "", string removalPlayerDataBool = "", bool dungDiscount = false, bool canBuy = true)
        {
            CustomItemStats item = CreateSoulPieceItem(sprite, playerDataBoolName, nameConvo, descConvo, cost, charmsRequired, requiredPlayerDataBool, removalPlayerDataBool, dungDiscount, canBuy);
            item.activeColour = activeColor;
            item.inactiveColour = inactiveColor;

            return item;
        }

        public static CustomItemStats CreateRelic1Item(Sprite sprite, string playerDataBoolName, string nameConvo, string descConvo, int cost, string relicPDInt, int charmsRequired = 0, string requiredPlayerDataBool = "", string removalPlayerDataBool = "", bool dungDiscount = false, bool canBuy = true)
        {
            CustomItemStats item = new CustomItemStats();
            item.playerDataBoolName = playerDataBoolName;
            item.nameConvo = nameConvo;
            item.descConvo = descConvo;
            item.requiredPlayerDataBool = requiredPlayerDataBool;
            item.removalPlayerDataBool = removalPlayerDataBool;
            item.specialType = CustomItemType.Relic1;
            item.relicNumber = 1;
            item.charmsRequired = charmsRequired;
            item.activeColour = defaultActiveColor;
            item.inactiveColour = defaultInactiveColor;
            item.dungDiscount = dungDiscount;
            item.relic = true;
            item.relicPDInt = relicPDInt;
            item.notchCostInt = "";
            item.cost = cost;
            item.canBuy = canBuy;
            item.sprite = sprite;

            return item;
        }
        public static CustomItemStats CreateRelic1Item(Sprite sprite, string playerDataBoolName, string nameConvo, string descConvo, int cost, string relicPDInt, Color activeColor, Color inactiveColor, int charmsRequired = 0, string requiredPlayerDataBool = "", string removalPlayerDataBool = "", bool dungDiscount = false, bool canBuy = true)
        {
            CustomItemStats item = CreateRelic1Item(sprite, playerDataBoolName, nameConvo, descConvo, cost, relicPDInt, charmsRequired, requiredPlayerDataBool, removalPlayerDataBool, dungDiscount, canBuy);
            item.activeColour = activeColor;
            item.inactiveColour = inactiveColor;

            return item;
        }

        public static CustomItemStats CreateRelic2Item(Sprite sprite, string playerDataBoolName, string nameConvo, string descConvo, int cost, string relicPDInt, int charmsRequired = 0, string requiredPlayerDataBool = "", string removalPlayerDataBool = "", bool dungDiscount = false, bool canBuy = true)
        {
            CustomItemStats item = new CustomItemStats();
            item.playerDataBoolName = playerDataBoolName;
            item.nameConvo = nameConvo;
            item.descConvo = descConvo;
            item.requiredPlayerDataBool = requiredPlayerDataBool;
            item.removalPlayerDataBool = removalPlayerDataBool;
            item.specialType = CustomItemType.Relic2;
            item.relicNumber = 2;
            item.charmsRequired = charmsRequired;
            item.activeColour = defaultActiveColor;
            item.inactiveColour = defaultInactiveColor;
            item.dungDiscount = dungDiscount;
            item.relic = true;
            item.relicPDInt = relicPDInt;
            item.notchCostInt = "";
            item.cost = cost;
            item.canBuy = canBuy;
            item.sprite = sprite;

            return item;
        }
        public static CustomItemStats CreateRelic2Item(Sprite sprite, string playerDataBoolName, string nameConvo, string descConvo, int cost, string relicPDInt, Color activeColor, Color inactiveColor, int charmsRequired = 0, string requiredPlayerDataBool = "", string removalPlayerDataBool = "", bool dungDiscount = false, bool canBuy = true)
        {
            CustomItemStats item = CreateRelic2Item(sprite, playerDataBoolName, nameConvo, descConvo, cost, relicPDInt, charmsRequired, requiredPlayerDataBool, removalPlayerDataBool, dungDiscount, canBuy);
            item.activeColour = activeColor;
            item.inactiveColour = inactiveColor;

            return item;
        }

        public static CustomItemStats CreateRelic3Item(Sprite sprite, string playerDataBoolName, string nameConvo, string descConvo, int cost, string relicPDInt, int charmsRequired = 0, string requiredPlayerDataBool = "", string removalPlayerDataBool = "", bool dungDiscount = false, bool canBuy = true)
        {
            CustomItemStats item = new CustomItemStats();
            item.playerDataBoolName = playerDataBoolName;
            item.nameConvo = nameConvo;
            item.descConvo = descConvo;
            item.requiredPlayerDataBool = requiredPlayerDataBool;
            item.removalPlayerDataBool = removalPlayerDataBool;
            item.specialType = CustomItemType.Relic3;
            item.relicNumber = 3;
            item.charmsRequired = charmsRequired;
            item.activeColour = defaultActiveColor;
            item.inactiveColour = defaultInactiveColor;
            item.dungDiscount = dungDiscount;
            item.relic = true;
            item.relicPDInt = relicPDInt;
            item.notchCostInt = "";
            item.cost = cost;
            item.canBuy = canBuy;
            item.sprite = sprite;

            return item;
        }
        public static CustomItemStats CreateRelic3Item(Sprite sprite, string playerDataBoolName, string nameConvo, string descConvo, int cost, string relicPDInt, Color activeColor, Color inactiveColor, int charmsRequired = 0, string requiredPlayerDataBool = "", string removalPlayerDataBool = "", bool dungDiscount = false, bool canBuy = true)
        {
            CustomItemStats item = CreateRelic3Item(sprite, playerDataBoolName, nameConvo, descConvo, cost, relicPDInt, charmsRequired, requiredPlayerDataBool, removalPlayerDataBool, dungDiscount, canBuy);
            item.activeColour = activeColor;
            item.inactiveColour = inactiveColor;

            return item;
        }

        public static CustomItemStats CreateRelic4Item(Sprite sprite, string playerDataBoolName, string nameConvo, string descConvo, int cost, string relicPDInt, int charmsRequired = 0, string requiredPlayerDataBool = "", string removalPlayerDataBool = "", bool dungDiscount = false, bool canBuy = true)
        {
            CustomItemStats item = new CustomItemStats();
            item.playerDataBoolName = playerDataBoolName;
            item.nameConvo = nameConvo;
            item.descConvo = descConvo;
            item.requiredPlayerDataBool = requiredPlayerDataBool;
            item.removalPlayerDataBool = removalPlayerDataBool;
            item.specialType = CustomItemType.Relic4;
            item.relicNumber = 4;
            item.charmsRequired = charmsRequired;
            item.activeColour = defaultActiveColor;
            item.inactiveColour = defaultInactiveColor;
            item.dungDiscount = dungDiscount;
            item.relic = true;
            item.relicPDInt = relicPDInt;
            item.notchCostInt = "";
            item.cost = cost;
            item.canBuy = canBuy;
            item.sprite = sprite;

            return item;
        }
        public static CustomItemStats CreateRelic4Item(Sprite sprite, string playerDataBoolName, string nameConvo, string descConvo, int cost, string relicPDInt, Color activeColor, Color inactiveColor, int charmsRequired = 0, string requiredPlayerDataBool = "", string removalPlayerDataBool = "", bool dungDiscount = false, bool canBuy = true)
        {
            CustomItemStats item = CreateRelic4Item(sprite, playerDataBoolName, nameConvo, descConvo, cost, relicPDInt, charmsRequired, requiredPlayerDataBool, removalPlayerDataBool, dungDiscount, canBuy);
            item.activeColour = activeColor;
            item.inactiveColour = inactiveColor;

            return item;
        }

        public static CustomItemStats CreateNotchItem(Sprite sprite, string playerDataBoolName, string nameConvo, string descConvo, int cost, int charmsRequired = 0, string requiredPlayerDataBool = "", string removalPlayerDataBool = "", bool dungDiscount = false, bool canBuy = true)
        {
            CustomItemStats item = new CustomItemStats();
            item.playerDataBoolName = playerDataBoolName;
            item.nameConvo = nameConvo;
            item.descConvo = descConvo;
            item.requiredPlayerDataBool = requiredPlayerDataBool;
            item.removalPlayerDataBool = removalPlayerDataBool;
            item.specialType = CustomItemType.Notch;
            item.relicNumber = 0;
            item.charmsRequired = charmsRequired;
            item.activeColour = defaultActiveColor;
            item.inactiveColour = defaultInactiveColor;
            item.dungDiscount = dungDiscount;
            item.relic = false;
            item.relicPDInt = "";
            item.notchCostInt = "";
            item.cost = cost;
            item.canBuy = canBuy;
            item.sprite = sprite;

            return item;
        }
        public static CustomItemStats CreateNotchItem(Sprite sprite, string playerDataBoolName, string nameConvo, string descConvo, int cost, Color activeColor, Color inactiveColor, int charmsRequired = 0, string requiredPlayerDataBool = "", string removalPlayerDataBool = "", bool dungDiscount = false, bool canBuy = true)
        {
            CustomItemStats item = CreateNotchItem(sprite, playerDataBoolName, nameConvo, descConvo, cost, charmsRequired, requiredPlayerDataBool, removalPlayerDataBool, dungDiscount, canBuy);
            item.activeColour = activeColor;
            item.inactiveColour = inactiveColor;

            return item;
        }

        public static CustomItemStats CreateMapItem(Sprite sprite, string playerDataBoolName, string nameConvo, string descConvo, int cost, int charmsRequired = 0, string requiredPlayerDataBool = "", string removalPlayerDataBool = "", bool dungDiscount = false, bool canBuy = true)
        {
            CustomItemStats item = new CustomItemStats();
            item.playerDataBoolName = playerDataBoolName;
            item.nameConvo = nameConvo;
            item.descConvo = descConvo;
            item.requiredPlayerDataBool = requiredPlayerDataBool;
            item.removalPlayerDataBool = removalPlayerDataBool;
            item.specialType = CustomItemType.Map;
            item.relicNumber = 0;
            item.charmsRequired = charmsRequired;
            item.activeColour = defaultActiveColor;
            item.inactiveColour = defaultInactiveColor;
            item.dungDiscount = dungDiscount;
            item.relic = false;
            item.relicPDInt = "";
            item.notchCostInt = "";
            item.cost = cost;
            item.canBuy = canBuy;
            item.sprite = sprite;

            return item;
        }
        public static CustomItemStats CreateMapItem(Sprite sprite, string playerDataBoolName, string nameConvo, string descConvo, int cost, Color activeColor, Color inactiveColor, int charmsRequired = 0, string requiredPlayerDataBool = "", string removalPlayerDataBool = "", bool dungDiscount = false, bool canBuy = true)
        {
            CustomItemStats item = CreateMapItem(sprite, playerDataBoolName, nameConvo, descConvo, cost, charmsRequired, requiredPlayerDataBool, removalPlayerDataBool, dungDiscount, canBuy);
            item.activeColour = activeColor;
            item.inactiveColour = inactiveColor;

            return item;
        }
        
        public static CustomItemStats CreateSimpleKeyItem(Sprite sprite, string playerDataBoolName, string nameConvo, string descConvo, int cost, int charmsRequired = 0, string requiredPlayerDataBool = "", string removalPlayerDataBool = "", bool dungDiscount = false, bool canBuy = true)
        {
            CustomItemStats item = new CustomItemStats();
            item.playerDataBoolName = playerDataBoolName;
            item.nameConvo = nameConvo;
            item.descConvo = descConvo;
            item.requiredPlayerDataBool = requiredPlayerDataBool;
            item.removalPlayerDataBool = removalPlayerDataBool;
            item.specialType = CustomItemType.SimpleKey;
            item.relicNumber = 0;
            item.charmsRequired = charmsRequired;
            item.activeColour = defaultActiveColor;
            item.inactiveColour = defaultInactiveColor;
            item.dungDiscount = dungDiscount;
            item.relic = false;
            item.relicPDInt = "";
            item.notchCostInt = "";
            item.cost = cost;
            item.canBuy = canBuy;
            item.sprite = sprite;

            return item;
        }
        public static CustomItemStats CreateSimpleKeyItem(Sprite sprite, string playerDataBoolName, string nameConvo, string descConvo, int cost, Color activeColor, Color inactiveColor, int charmsRequired = 0, string requiredPlayerDataBool = "", string removalPlayerDataBool = "", bool dungDiscount = false, bool canBuy = true)
        {
            CustomItemStats item = CreateSimpleKeyItem(sprite, playerDataBoolName, nameConvo, descConvo, cost, charmsRequired, requiredPlayerDataBool, removalPlayerDataBool, dungDiscount, canBuy);
            item.activeColour = activeColor;
            item.inactiveColour = inactiveColor;

            return item;
        }
        
        public static CustomItemStats CreateRancidEggItem(Sprite sprite, string playerDataBoolName, string nameConvo, string descConvo, int cost, int charmsRequired = 0, string requiredPlayerDataBool = "", string removalPlayerDataBool = "", bool dungDiscount = false, bool canBuy = true)
        {
            CustomItemStats item = new CustomItemStats();
            item.playerDataBoolName = playerDataBoolName;
            item.nameConvo = nameConvo;
            item.descConvo = descConvo;
            item.requiredPlayerDataBool = requiredPlayerDataBool;
            item.removalPlayerDataBool = removalPlayerDataBool;
            item.specialType = CustomItemType.RancidEgg;
            item.relicNumber = 0;
            item.charmsRequired = charmsRequired;
            item.activeColour = defaultActiveColor;
            item.inactiveColour = defaultInactiveColor;
            item.dungDiscount = dungDiscount;
            item.relic = false;
            item.relicPDInt = "";
            item.notchCostInt = "";
            item.cost = cost;
            item.canBuy = canBuy;
            item.sprite = sprite;

            return item;
        }
        public static CustomItemStats CreateRancidEggItem(Sprite sprite, string playerDataBoolName, string nameConvo, string descConvo, int cost, Color activeColor, Color inactiveColor, int charmsRequired = 0, string requiredPlayerDataBool = "", string removalPlayerDataBool = "", bool dungDiscount = false, bool canBuy = true)
        {
            CustomItemStats item = CreateRancidEggItem(sprite, playerDataBoolName, nameConvo, descConvo, cost, charmsRequired, requiredPlayerDataBool, removalPlayerDataBool, dungDiscount, canBuy);
            item.activeColour = activeColor;
            item.inactiveColour = inactiveColor;

            return item;
        }
        
        public static CustomItemStats CreateRepairGlassHPItem(Sprite sprite, string playerDataBoolName, string nameConvo, string descConvo, int cost, int charmsRequired = 0, string requiredPlayerDataBool = "", string removalPlayerDataBool = "", bool dungDiscount = false, bool canBuy = true)
        {
            CustomItemStats item = new CustomItemStats();
            item.playerDataBoolName = playerDataBoolName;
            item.nameConvo = nameConvo;
            item.descConvo = descConvo;
            item.requiredPlayerDataBool = requiredPlayerDataBool;
            item.removalPlayerDataBool = removalPlayerDataBool;
            item.specialType = CustomItemType.RepairGlassHP;
            item.relicNumber = 0;
            item.charmsRequired = charmsRequired;
            item.activeColour = defaultActiveColor;
            item.inactiveColour = defaultInactiveColor;
            item.dungDiscount = dungDiscount;
            item.relic = false;
            item.relicPDInt = "";
            item.notchCostInt = "";
            item.cost = cost;
            item.canBuy = canBuy;
            item.sprite = sprite;

            return item;
        }
        public static CustomItemStats CreateRepairGlassHPItem(Sprite sprite, string playerDataBoolName, string nameConvo, string descConvo, int cost, Color activeColor, Color inactiveColor, int charmsRequired = 0, string requiredPlayerDataBool = "", string removalPlayerDataBool = "", bool dungDiscount = false, bool canBuy = true)
        {
            CustomItemStats item = CreateRepairGlassHPItem(sprite, playerDataBoolName, nameConvo, descConvo, cost, charmsRequired, requiredPlayerDataBool, removalPlayerDataBool, dungDiscount, canBuy);
            item.activeColour = activeColor;
            item.inactiveColour = inactiveColor;

            return item;
        }
        
        public static CustomItemStats CreateRepairGlassGeoItem(Sprite sprite, string playerDataBoolName, string nameConvo, string descConvo, int cost, int charmsRequired = 0, string requiredPlayerDataBool = "", string removalPlayerDataBool = "", bool dungDiscount = false, bool canBuy = true)
        {
            CustomItemStats item = new CustomItemStats();
            item.playerDataBoolName = playerDataBoolName;
            item.nameConvo = nameConvo;
            item.descConvo = descConvo;
            item.requiredPlayerDataBool = requiredPlayerDataBool;
            item.removalPlayerDataBool = removalPlayerDataBool;
            item.specialType = CustomItemType.RepairGlassGeo;
            item.relicNumber = 0;
            item.charmsRequired = charmsRequired;
            item.activeColour = defaultActiveColor;
            item.inactiveColour = defaultInactiveColor;
            item.dungDiscount = dungDiscount;
            item.relic = false;
            item.relicPDInt = "";
            item.notchCostInt = "";
            item.cost = cost;
            item.canBuy = canBuy;
            item.sprite = sprite;

            return item;
        }
        public static CustomItemStats CreateRepairGlassGeoItem(Sprite sprite, string playerDataBoolName, string nameConvo, string descConvo, int cost, Color activeColor, Color inactiveColor, int charmsRequired = 0, string requiredPlayerDataBool = "", string removalPlayerDataBool = "", bool dungDiscount = false, bool canBuy = true)
        {
            CustomItemStats item = CreateRepairGlassGeoItem(sprite, playerDataBoolName, nameConvo, descConvo, cost, charmsRequired, requiredPlayerDataBool, removalPlayerDataBool, dungDiscount, canBuy);
            item.activeColour = activeColor;
            item.inactiveColour = inactiveColor;

            return item;
        }
        
        public static CustomItemStats CreateRepairGlassAttackItem(Sprite sprite, string playerDataBoolName, string nameConvo, string descConvo, int cost, int charmsRequired = 0, string requiredPlayerDataBool = "", string removalPlayerDataBool = "", bool dungDiscount = false, bool canBuy = true)
        {
            CustomItemStats item = new CustomItemStats();
            item.playerDataBoolName = playerDataBoolName;
            item.nameConvo = nameConvo;
            item.descConvo = descConvo;
            item.requiredPlayerDataBool = requiredPlayerDataBool;
            item.removalPlayerDataBool = removalPlayerDataBool;
            item.specialType = CustomItemType.RepairGlassAttack;
            item.relicNumber = 0;
            item.charmsRequired = charmsRequired;
            item.activeColour = defaultActiveColor;
            item.inactiveColour = defaultInactiveColor;
            item.dungDiscount = dungDiscount;
            item.relic = false;
            item.relicPDInt = "";
            item.notchCostInt = "";
            item.cost = cost;
            item.canBuy = canBuy;
            item.sprite = sprite;

            return item;
        }
        public static CustomItemStats CreateRepairGlassAttackItem(Sprite sprite, string playerDataBoolName, string nameConvo, string descConvo, int cost, Color activeColor, Color inactiveColor, int charmsRequired = 0, string requiredPlayerDataBool = "", string removalPlayerDataBool = "", bool dungDiscount = false, bool canBuy = true)
        {
            CustomItemStats item = CreateRepairGlassAttackItem(sprite, playerDataBoolName, nameConvo, descConvo, cost, charmsRequired, requiredPlayerDataBool, removalPlayerDataBool, dungDiscount, canBuy);
            item.activeColour = activeColor;
            item.inactiveColour = inactiveColor;

            return item;
        }
        
        public static CustomItemStats CreateSalubrasBlessingItem(Sprite sprite, string playerDataBoolName, string nameConvo, string descConvo, int cost, int charmsRequired = 0, string requiredPlayerDataBool = "", string removalPlayerDataBool = "", bool dungDiscount = false, bool canBuy = true)
        {
            CustomItemStats item = new CustomItemStats();
            item.playerDataBoolName = playerDataBoolName;
            item.nameConvo = nameConvo;
            item.descConvo = descConvo;
            item.requiredPlayerDataBool = requiredPlayerDataBool;
            item.removalPlayerDataBool = removalPlayerDataBool;
            item.specialType = CustomItemType.SalubrasBlessing;
            item.relicNumber = 0;
            item.charmsRequired = charmsRequired;
            item.activeColour = defaultActiveColor;
            item.inactiveColour = defaultInactiveColor;
            item.dungDiscount = dungDiscount;
            item.relic = false;
            item.relicPDInt = "";
            item.notchCostInt = "";
            item.cost = cost;
            item.canBuy = canBuy;
            item.sprite = sprite;

            return item;
        }
        public static CustomItemStats CreateSalubrasBlessingItem(Sprite sprite, string playerDataBoolName, string nameConvo, string descConvo, int cost, Color activeColor, Color inactiveColor, int charmsRequired = 0, string requiredPlayerDataBool = "", string removalPlayerDataBool = "", bool dungDiscount = false, bool canBuy = true)
        {
            CustomItemStats item = CreateSalubrasBlessingItem(sprite, playerDataBoolName, nameConvo, descConvo, cost, charmsRequired, requiredPlayerDataBool, removalPlayerDataBool, dungDiscount, canBuy);
            item.activeColour = activeColor;
            item.inactiveColour = inactiveColor;

            return item;
        }
        
        public static CustomItemStats CreateMapPinItem(Sprite sprite, string playerDataBoolName, string nameConvo, string descConvo, int cost, int charmsRequired = 0, string requiredPlayerDataBool = "", string removalPlayerDataBool = "", bool dungDiscount = false, bool canBuy = true)
        {
            CustomItemStats item = new CustomItemStats();
            item.playerDataBoolName = playerDataBoolName;
            item.nameConvo = nameConvo;
            item.descConvo = descConvo;
            item.requiredPlayerDataBool = requiredPlayerDataBool;
            item.removalPlayerDataBool = removalPlayerDataBool;
            item.specialType = CustomItemType.MapPin;
            item.relicNumber = 0;
            item.charmsRequired = charmsRequired;
            item.activeColour = defaultActiveColor;
            item.inactiveColour = defaultInactiveColor;
            item.dungDiscount = dungDiscount;
            item.relic = false;
            item.relicPDInt = "";
            item.notchCostInt = "";
            item.cost = cost;
            item.canBuy = canBuy;
            item.sprite = sprite;

            return item;
        }
        public static CustomItemStats CreateMapPinItem(Sprite sprite, string playerDataBoolName, string nameConvo, string descConvo, int cost, Color activeColor, Color inactiveColor, int charmsRequired = 0, string requiredPlayerDataBool = "", string removalPlayerDataBool = "", bool dungDiscount = false, bool canBuy = true)
        {
            CustomItemStats item = CreateMapPinItem(sprite, playerDataBoolName, nameConvo, descConvo, cost, charmsRequired, requiredPlayerDataBool, removalPlayerDataBool, dungDiscount, canBuy);
            item.activeColour = activeColor;
            item.inactiveColour = inactiveColor;

            return item;
        }
        
        public static CustomItemStats CreateMapMarkerItem(Sprite sprite, string playerDataBoolName, string nameConvo, string descConvo, int cost, int charmsRequired = 0, string requiredPlayerDataBool = "", string removalPlayerDataBool = "", bool dungDiscount = false, bool canBuy = true)
        {
            CustomItemStats item = new CustomItemStats();
            item.playerDataBoolName = playerDataBoolName;
            item.nameConvo = nameConvo;
            item.descConvo = descConvo;
            item.requiredPlayerDataBool = requiredPlayerDataBool;
            item.removalPlayerDataBool = removalPlayerDataBool;
            item.specialType = CustomItemType.MapMarker;
            item.relicNumber = 0;
            item.charmsRequired = charmsRequired;
            item.activeColour = defaultActiveColor;
            item.inactiveColour = defaultInactiveColor;
            item.dungDiscount = dungDiscount;
            item.relic = false;
            item.relicPDInt = "";
            item.notchCostInt = "";
            item.cost = cost;
            item.canBuy = canBuy;
            item.sprite = sprite;

            return item;
        }
        public static CustomItemStats CreateMapMarkerItem(Sprite sprite, string playerDataBoolName, string nameConvo, string descConvo, int cost, Color activeColor, Color inactiveColor, int charmsRequired = 0, string requiredPlayerDataBool = "", string removalPlayerDataBool = "", bool dungDiscount = false, bool canBuy = true)
        {
            CustomItemStats item = CreateMapMarkerItem(sprite, playerDataBoolName, nameConvo, descConvo, cost, charmsRequired, requiredPlayerDataBool, removalPlayerDataBool, dungDiscount, canBuy);
            item.activeColour = activeColor;
            item.inactiveColour = inactiveColor;

            return item;
        }
        #endregion

        #region fields
        public string playerDataBoolName;

        public string nameConvo;

        public string descConvo;

        public string requiredPlayerDataBool;

        public string removalPlayerDataBool;

        public CustomItemType specialType;

        public int relicNumber;

        public int charmsRequired;

        public Color activeColour;

        public Color inactiveColour;

        public bool dungDiscount;

        public bool relic;

        public string relicPDInt;
        public string notchCostInt;
        public int cost;
        public bool canBuy;
        public Sprite sprite;
        #endregion
    }
}