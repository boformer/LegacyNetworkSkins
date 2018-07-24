using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using ColossalFramework.Math;
using NetworkSkins.Data;
using NetworkSkins.Net;
using NetworkSkins.Props;
using UnityEngine;

namespace NetworkSkins.Detour
{
    /// <summary>
    /// RenderManager is detoured to provide a special loading hook
    /// that is executed before LODs and other render information is cached.
    /// We use it to initialize NetworkSkins, instead of onLevelLoaded
    /// </summary>
    public class RenderManagerDetour : RenderManager
    {
        private static bool deployed;

        private static RedirectCallsState _RenderManager_UpdateData_state;
        private static MethodInfo _RenderManager_UpdateData_original;
        private static MethodInfo _RenderManager_UpdateData_detour;

        public static event UpdateDataEventListener EventUpdateDataPre;
        public static event UpdateDataEventListener EventUpdateData;
        public static event UpdateDataEventListener EventUpdateDataPost;
        public delegate void UpdateDataEventListener(SimulationManager.UpdateMode mode);

        public static void Deploy()
        {
            if (deployed) return;

            _RenderManager_UpdateData_original = typeof(RenderManager).GetMethod("UpdateData", BindingFlags.Instance | BindingFlags.Public);
            _RenderManager_UpdateData_detour = typeof(RenderManagerDetour).GetMethod("UpdateData", BindingFlags.Instance | BindingFlags.Public);
            _RenderManager_UpdateData_state = RedirectionHelper.RedirectCalls(_RenderManager_UpdateData_original, _RenderManager_UpdateData_detour);

            deployed = true;
        }

        public static void Revert()
        {
            if (!deployed) return;

            RedirectionHelper.RevertRedirect(_RenderManager_UpdateData_original, _RenderManager_UpdateData_state);
            _RenderManager_UpdateData_original = null;
            _RenderManager_UpdateData_detour = null;

            deployed = false;
        }


        public override void UpdateData(SimulationManager.UpdateMode mode)
        {
            // Call 
            EventUpdateDataPre?.Invoke(mode);
            EventUpdateData?.Invoke(mode);
            EventUpdateDataPost?.Invoke(mode);

            // Execute original method
            RedirectionHelper.RevertRedirect(_RenderManager_UpdateData_original, _RenderManager_UpdateData_state);
            RenderManager.instance.UpdateData(mode);
            RedirectionHelper.RedirectCalls(_RenderManager_UpdateData_original, _RenderManager_UpdateData_detour);
        }
    }
}