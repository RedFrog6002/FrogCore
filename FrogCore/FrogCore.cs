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
        public override string GetVersion() => Assembly.GetExecutingAssembly().GetName().Version.ToString();

        public FrogCore() : base("FrogCore") { }

        public override List<(string, string)> GetPreloadNames()
        {
            return new() { ("Town", "_NPCs/Zote Final Scene/Zote Final") };
        }

        public override void Initialize(Dictionary<string, Dictionary<string, GameObject>> preloadedObjects)
        {
            DialogueNPC.zotePrefab = preloadedObjects["Town"]["_NPCs/Zote Final Scene/Zote Final"];
        }
    }
}
