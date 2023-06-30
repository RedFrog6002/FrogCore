using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Modding;
using System.Reflection;
using UnityEngine;

namespace FrogCore
{
    public class FrogCore : Mod
    {
        public static FrogCore instance;
        public override string GetVersion() => Assembly.GetExecutingAssembly().GetName().Version.ToString();

        public FrogCore() : base("FrogCore") { }

        public override List<(string, string)> GetPreloadNames()
        {
            return new() { 
                ("Town", "_NPCs/Zote Final Scene/Zote Final"),
                ("Crossroads_01", "Uninfected Parent/Zombie Runner 1") };
        }

        public override void Initialize(Dictionary<string, Dictionary<string, GameObject>> preloadedObjects)
        {
            instance = this;
            DialogueNPC.zotePrefab = preloadedObjects["Town"]["_NPCs/Zote Final Scene/Zote Final"];
            JournalHelper.notificationPrefab = ReflectionHelper.GetField<EnemyDeathEffects, GameObject>(preloadedObjects["Crossroads_01"]["Uninfected Parent/Zombie Runner 1"].GetComponent<EnemyDeathEffects>(), "journalUpdateMessagePrefab");
        }
    }
}
