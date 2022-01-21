using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Dungreed.plugin
{
    class MyAttribute
    {
        public const string PLAGIN_NAME = "Dungreed";
        public const string PLAGIN_VERSION = "22.01";
        public const string PLAGIN_FULL_NAME = "Lilly.Dungreed.Plugin";
    }

    [BepInPlugin(MyAttribute.PLAGIN_FULL_NAME, MyAttribute.PLAGIN_NAME, MyAttribute.PLAGIN_VERSION)]// 버전 규칙 잇음. 반드시 2~4개의 숫자구성으로 해야함. 미준수시 못읽어들임
    [BepInProcess("Dungreed.exe")]
    public class DungreedPlugin : BaseUnityPlugin
    {
        public Harmony harmony = null;
        public static ManualLogSource logger;

        private ConfigEntry<bool> isGUIOn;
        private ConfigEntry<bool> isOpen;
        public ConfigEntry<BepInEx.Configuration.KeyboardShortcut> ShowCounter;

        public string windowName = "";
        public string FullName = "DungreedPlugin";
        public string ShortName = "DP";

        public int windowId = 984;
        public Rect WindowRect { get; private set; }
        GUILayoutOption h;
        GUILayoutOption w;
        public Vector2 scrollPosition;

        public void Awake()
        {
            logger = Logger;
            Logger.LogInfo("This is information");
            Logger.LogWarning("This is a warning");
            Logger.LogError("This is an error");

            isGUIOn = Config.Bind("GUI", "isGUIOn", true);
            isOpen = Config.Bind("GUI", "isOpen", true);
            isOpen.SettingChanged += IsOpen_SettingChanged;
            IsOpen_SettingChanged(null, null);
            ;

            ShowCounter = Config.Bind("GUI", "isGUIOnKey", new KeyboardShortcut(KeyCode.Keypad0));// 이건 단축키
        }

        private void IsOpen_SettingChanged(object sender, EventArgs e)
        {
            logger.LogInfo($"IsOpen_SettingChanged {isOpen.Value} , {isGUIOn.Value} ");
            if (isOpen.Value)
            {
                h = GUILayout.Height(800);
                w = GUILayout.Width(200);
                windowName = FullName;
            }
            else
            {
                h = GUILayout.Height(40);
                w = GUILayout.Height(60);
                windowName = ShortName;
            }
        }

        public void OnEnable()
        {
            Logger.LogWarning("OnEnable");
            Debug.LogWarning("OnEnable");
            harmony = Harmony.CreateAndPatchAll(typeof(DungreedPlugin));
        }

        bool hpMaxSet;
        bool dashMaxSet;

        public void Update()
        {
            if (ShowCounter.Value.IsUp())// 단축키가 일치할때
            {
                isGUIOn.Value = !isGUIOn.Value;// 보이거나 안보이게. 이런 배열이였네 지웠음
                //MyLog.LogMessage("IsUp", ShowCounter.Value.MainKey);
            }
            if (player != null)
            {
                if (hpMaxSet)
                {
                    status.hp = status.maxHP;
                }
                            
                if (dashMaxSet)
                {
                    controller2D.ramainingDashCount = controller2D.maxDashCount;
                }
            }
        }

        public void OnGUI()
        {
            if (!isGUIOn.Value)
                return;

            WindowRect = GUILayout.Window(windowId, WindowRect, WindowFunction, "DungreedPlugin", w, h);
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
                if (player != null)
                {
                    if (GUILayout.Button($"EXP {player.EXP}+10")) { player.AddEXP(10); }
                    if (GUILayout.Button($"Money {player.Money}+10000")) { player.AddMoney(10000, true); }
                    if (GUILayout.Button($"Auto hp Set Max {hpMaxSet}")) { hpMaxSet=!hpMaxSet; }
                    if (GUILayout.Button($"hp {status.hp}={status.maxHP}")) { status.hp = status.maxHP; }
                    if (GUILayout.Button($"lockHP { status.lockHP}")) { status.lockHP = !status.lockHP; }
                    if (GUILayout.Button($"Auto Dash Set Max {dashMaxSet}")) { dashMaxSet = !dashMaxSet;  }
                    GUI.enabled = false;
                    if (GUILayout.Button($"ramainingDashCount {controller2D.ramainingDashCount}")) {  }
                    if (GUILayout.Button($"maxDashCount {controller2D.maxDashCount}")) { hpMaxSet=!hpMaxSet; }
                    if (GUILayout.Button($"currentJumpCount {controller2D.currentJumpCount}")) { hpMaxSet=!hpMaxSet; }
                    if (GUILayout.Button($"jumpCount {controller2D.jumpCount}")) { hpMaxSet=!hpMaxSet; }
                    GUI.enabled = true;
                    if (GUILayout.Button($"abilityPoint { ability.abilityPoint}+100")) { ability.abilityPoint+=100; }

                    GUILayout.Label($"coinBonus : {player.coinBonus}");
                    GUILayout.Label($"killEnemyCount : {player.killEnemyCount}");
                    GUILayout.Label($"killExp : {player.killExp}");
                    GUILayout.Label($"life : {player.life}");
                    GUILayout.Label($"minotaurLife : {player.minotaurLife}");
                    GUILayout.Label($"satiety : {player.satiety}");
                    GUILayout.Label($"satietyDiscount : {player.satietyDiscount}");
                    GUILayout.Label($"Soul : {player.Soul}");                                       
                    GUILayout.Label($"bonusHPRatio : {status.bonusHPRatio}");                    
                    GUILayout.Label($"shield : {status.shield}");
                    GUILayout.Label($"maxShield : {status.maxShield}");
                    GUILayout.Label($"power : {status.power}");
                    GUILayout.Label($"regeneration : {status.regeneration}");
                    GUILayout.Label($"critical : {status.critical}");
                    GUILayout.Label($"offense : {status.offense}");
                    GUILayout.Label($"maxOffense : {status.maxOffense}");
                    GUILayout.Label($"defense : {status.defense}");
                    GUILayout.Label($"superShield : {status.superShield}");
                    GUILayout.Label($"tempSuperShield : {status.tempSuperShield}");
                    GUILayout.Label($"vitality : {status.vitality}");
                }
                else
                {
                    GUILayout.Label($"No player");
                }

                GUILayout.EndScrollView();
            }
            GUI.enabled = true;
            GUI.DragWindow(); // 창 드레그 가능하게 해줌. 마지막에만 넣어야함
        }

        public void OnDisable()
        {
            Logger.LogWarning("OnDisable");
            Debug.LogWarning("SetPlayer");
            harmony?.UnpatchSelf();
        }

        public static Player player = null;
        public static Status status = null;
        public static CharacterController2D controller2D = null;
        public static PlayerAbility ability = null;

        // 무의미
        //[HarmonyPrefix, HarmonyPatch(typeof(Player_Accessory), "SetPlayer")]
        //public static bool SetPlayer(Player player)
        //{
        //    logger.LogInfo("SetPlayer");
        //    Debug.Log("SetPlayer");
        //    DungreedPlugin.player = player;
        //    return true;
        //}

        [HarmonyPostfix, HarmonyPatch(typeof(GameManager), "CreatePlayer")]
        // public void CreatePlayer(bool with2P = true)
        public static void CreatePlayer(bool with2P, Player ___currentPlayer)
        {
            logger.LogInfo($"CreatePlayer , {with2P}");
            Debug.Log("CreatePlayer");
            DungreedPlugin.player = ___currentPlayer;
            status = player._creature.status;
            controller2D = player._creature._controller2D;
            ability = player._ability;
            //return true;
        }








    }
}
