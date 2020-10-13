using System.Reflection;
using HarmonyLib;
using Rocket.API;
using Rocket.Core.Logging;
using Rocket.Core.Plugins;
using SDG.Unturned;
using Steamworks;

namespace PlayerIPPatch
{
    public class PlayerIPPatch : RocketPlugin
    {
        public Harmony HarmonyInstance;

        public override void LoadPlugin()
        {
            base.LoadPlugin();
            HarmonyInstance = new Harmony("PlayerIPPatch");
            Logger.Log("Player IP Patch by ShimmyMySherbet (great job nelson!)");
            Logger.Log("Patching IP Methods...");
            MethodInfo PatchBase = typeof(SteamGameServerNetworking).GetMethod("GetP2PSessionState");
            PatchProcessor Processor = HarmonyInstance.CreateProcessor(PatchBase);
            Processor.AddPostfix(new HarmonyMethod(typeof(PlayerIPPatch).GetMethod("PostFixObj")));
            Processor.Patch();
            Logger.Log("Patched!");
        }

        public static void PostFixObj(CSteamID steamIDRemote, ref P2PSessionState_t pConnectionState)
        {
            SteamPlayer pl = PlayerTool.getSteamPlayer(steamIDRemote);
            if (pl.transportConnection.TryGetIPv4Address(out uint IP))
            {
                pConnectionState.m_nRemoteIP = IP;
            }
        }

        public override void UnloadPlugin(PluginState state = PluginState.Unloaded)
        {
            Logger.Log("Removing all patches...");
            HarmonyInstance?.UnpatchAll("PlayerIPPatch");
            Logger.Log("IP Methods unpatched!");
            base.UnloadPlugin(state);
        }
    }
}