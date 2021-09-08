using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Modding;

namespace FrogCore
{
    /// <summary>
    /// not sure if this works, prob will be removed soon either way
    /// </summary>
    public static class OldHooks
    {
        static OldHooks()
        {
            ModHooks.LanguageGetHook += LanguageGet;
            ModHooks.GetPlayerIntHook += GetPlayerInt;
            ModHooks.SetPlayerIntHook += SetPlayerInt;
            ModHooks.GetPlayerBoolHook += GetPlayerBool;
            ModHooks.SetPlayerBoolHook += SetPlayerBool;
        }

        /// <summary>
        ///     Called whenever localization specific strings are requested
        /// </summary>
        /// <param name="key"></param>
        /// <param name="sheetTitle"></param>
        /// <returns>Localized Value</returns>
        public delegate string LanguageGetHandler(string key, string sheet);
        /// <summary>
        ///     Called whenever localization specific strings are requested
        /// </summary>
        public static event LanguageGetHandler LanguageGetHook;
        private static string LanguageGet(string key, string sheetTitle, string orig)
        {
            string text = orig;
            bool gotText = false;

            if (LanguageGetHook == null)
                return orig;

            Delegate[] invocationList = LanguageGetHook.GetInvocationList();

            foreach (LanguageGetHandler toInvoke in invocationList)
            {
                try
                {
                    string res = toInvoke(key, sheetTitle);
                    
                    if (res == orig || gotText)
                        continue;

                    text = res;
                    gotText = true;
                }
                catch (Exception ex)
                {
                    Log(ex);
                }
            }

            return text;
        }

        /// <summary>
        ///     Called when anything in the game tries to get an int
        /// </summary>
        /// <param name="intName">Value's Name</param>
        /// <returns>The bool value</returns>
        public delegate int GetIntProxy(string intName);
        /// <summary>
        ///     Called when anything in the game tries to get an int from player data
        /// </summary>
        /// <remarks>PlayerData.GetInt</remarks>
        public static event GetIntProxy GetPlayerIntHook;
        private static int GetPlayerInt(string name, int orig)
        {
            int result = orig;
            bool gotValue = false;

            if (GetPlayerIntHook == null)
                return orig;

            Delegate[] invocationList = GetPlayerIntHook.GetInvocationList();

            foreach (GetIntProxy toInvoke in invocationList)
            {
                try
                {
                    int num = toInvoke.Invoke(name);
                    if (num == orig || gotValue)
                        continue;

                    result = num;
                    gotValue = true;
                }
                catch (Exception ex)
                {
                    Log(ex);
                }
            }

            return result;
        }

        /// <summary>
        ///     Called when anything in the game tries to set an int
        /// </summary>
        /// <param name="intName">Name of the Int</param>
        /// <param name="value">Value to be used</param>
        public delegate void SetIntProxy(string intName, int value);
        /// <summary>
        ///     Called when anything in the game tries to set an int in player data
        /// </summary>
        /// <remarks>PlayerData.SetInt</remarks>
        public static event SetIntProxy SetPlayerIntHook;
        private static int SetPlayerInt(string name, int orig)
        {
            if (SetPlayerIntHook != null)
            {
                Delegate[] invocationList = SetPlayerIntHook.GetInvocationList();

                foreach (SetIntProxy toInvoke in invocationList)
                {
                    try
                    {
                        toInvoke.Invoke(name, orig);
                    }
                    catch (Exception ex)
                    {
                        Log(ex);
                    }
                }
            }

            return orig;
        }

        /// <summary>
        ///     Called when anything in the game tries to get a bool
        /// </summary>
        /// <param name="originalSet">Value's Name</param>
        /// <returns>The bool value</returns>
        public delegate bool GetBoolProxy(string originalSet);
        /// <summary>
        ///     Called when anything in the game tries to get a bool from player data
        /// </summary>
        /// <remarks>PlayerData.GetBool</remarks>
        public static event GetBoolProxy GetPlayerBoolHook;
        private static bool GetPlayerBool(string name, bool orig)
        {
            bool result = orig;
            bool gotValue = false;
            if (GetPlayerBoolHook == null)
                return orig;

            Delegate[] invocationList = GetPlayerBoolHook.GetInvocationList();

            foreach (GetBoolProxy toInvoke in invocationList)
            {
                try
                {
                    bool flag2 = toInvoke.Invoke(name);

                    if (flag2 == orig || gotValue)
                        continue;

                    result = flag2;
                    gotValue = true;
                }
                catch (Exception ex)
                {
                    Log(ex);
                }
            }

            return result;
        }

        /// <summary>
        ///     Called when anything in the game tries to set a bool
        /// </summary>
        /// <param name="originalSet">Name of the Bool</param>
        /// <param name="value">Value to be used</param>
        public delegate void SetBoolProxy(string originalSet, bool value);
        /// <summary>
        ///     Called when anything in the game tries to set a bool in player data
        /// </summary>
        /// <remarks>PlayerData.SetBool</remarks>
        /// <see cref="SetBoolProxy" />
        public static event SetBoolProxy SetPlayerBoolHook;
        private static bool SetPlayerBool(string name, bool orig)
        {
            if (SetPlayerBoolHook != null)
            {
                Delegate[] invocationList = SetPlayerBoolHook.GetInvocationList();

                foreach (SetBoolProxy toInvoke in invocationList)
                {
                    try
                    {
                        toInvoke.Invoke(name, orig);
                    }
                    catch (Exception ex)
                    {
                        Log(ex);
                    }
                }
            }

            return orig;
        }

        private static void Log(object o) => Ext.Extensions.Log(new string[] { "OldHooks", new StackTrace().GetFrame(1).GetMethod().Name }, o);
    }
}
