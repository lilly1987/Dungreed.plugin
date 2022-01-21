using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Dungreed.plugin
{
    public class DungreedPlugin : BaseUnityPlugin
    {
        Harmony harmony;
        static ManualLogSource logger;

        private ConfigEntry<bool> isGUIOn;
        private ConfigEntry<bool> isOpen;
        public ConfigEntry<BepInEx.Configuration.KeyboardShortcut> ShowCounter;

        public string windowName = "";
        public string FullName = "DungreedPlugin";
        public string ShortName = "DP";

        private int windowId;
        public Rect WindowRect { get; private set; }

        private Vector2 scrollPosition;

        public void Awake()
        {
            logger = Logger;
            Logger.LogInfo("This is information");
            Logger.LogWarning("This is a warning");
            Logger.LogError("This is an error");

            isGUIOn = Config.Bind("GUI", "isGUIOn", true);
            isOpen = Config.Bind("GUI", "isOpen", true);
            ShowCounter = Config.Bind("GUI", "isGUIOnKey", new KeyboardShortcut(KeyCode.Keypad0));// 이건 단축키
        }

        public void OnEnable()
        {
            harmony.PatchAll(typeof(DungreedPlugin));
        }

        public void Update()
        {
            if (ShowCounter.Value.IsUp())// 단축키가 일치할때
            {
                isGUIOn.Value = !isGUIOn.Value;// 보이거나 안보이게. 이런 배열이였네 지웠음
                //MyLog.LogMessage("IsUp", ShowCounter.Value.MainKey);
            }
        }

        public void OnGUI()
        {
            if (!isGUIOn.Value)
                return;

            WindowRect = GUILayout.Window(windowId, WindowRect, WindowFunction, "DungreedPlugin");
        }

        public virtual void WindowFunction(int id)
        {
            GUI.enabled = true; // 기능 클릭 가능

            GUILayout.BeginHorizontal();// 가로 정렬
            // 라벨 추가
            GUILayout.Label(windowName, GUILayout.Height(20));
            // 안쓰는 공간이 생기더라도 다른 기능으로 꽉 채우지 않고 빈공간 만들기
            GUILayout.FlexibleSpace();

            if (GUILayout.Button("-", GUILayout.Width(20), GUILayout.Height(20))) { isOpen.Value = !isOpen.Value; }
            if (GUILayout.Button("x", GUILayout.Width(20), GUILayout.Height(20))) { isGUIOn.Value = false; }
            GUI.changed = false;

            GUILayout.EndHorizontal();// 가로 정렬 끝

            if (!isOpen.Value)
            {

            }
            else
            {
                scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, true);

                if (GUILayout.Button("Money+10000")) { }

                GUILayout.EndScrollView();
            }
            GUI.enabled = true;
            GUI.DragWindow(); // 창 드레그 가능하게 해줌. 마지막에만 넣어야함
        }

        public void OnDisable()
        {
            harmony.UnpatchSelf();
        }

        public static Player player;

        [HarmonyPrefix, HarmonyPatch(typeof(Player_Accessory), "SetPlayer")]
        public static bool SetPlayer(Player player)
        {
            logger.LogInfo("SetPlayer");
            DungreedPlugin.player = player;
            return true;
        }








    }
}
