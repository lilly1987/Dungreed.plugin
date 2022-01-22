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


        ConfigEntry<bool> hpMaxSet;
        ConfigEntry<bool> dashMaxSet;
        static ConfigEntry<bool> ReloadZero;
        private static ConfigEntry<bool> skilluse;

        public bool HpMaxSet { get => hpMaxSet.Value; set => hpMaxSet.Value = value; }
        public bool DashMaxSet { get => dashMaxSet.Value; set => dashMaxSet.Value = value; }
        public static bool ReloadZero1 { get => ReloadZero.Value; set => ReloadZero.Value = value; }
        public static bool Skilluse { get => skilluse.Value; set => skilluse.Value = value; }


        public string windowName = "";
        public string FullName = "DungreedPlugin";
        public string ShortName = "DP";

        public int windowId = 984;
        public Rect WindowRect;

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
            if (isOpen.Value)            
                WindowRect = new Rect(Screen.width - 65, 0, 200, 800);
            else
                WindowRect = new Rect(Screen.width - 200, 0, 200, 800);

            IsOpen_SettingChanged(null, null);

            hpMaxSet = Config.Bind("GUI", "hpMaxSet", true);
            dashMaxSet = Config.Bind("GUI", "dashMaxSet", true);
            ReloadZero = Config.Bind("GUI", "ReloadZero", true);
            skilluse = Config.Bind("GUI", "Skilluse", true);

            ;

            ShowCounter = Config.Bind("GUI", "isGUIOnKey", new KeyboardShortcut(KeyCode.Keypad0));// 이건 단축키
        }

        private void IsOpen_SettingChanged(object sender, EventArgs e)
        {
            logger.LogInfo($"IsOpen_SettingChanged {isOpen.Value} , {isGUIOn.Value},{WindowRect.x} ");
            if (isOpen.Value)
            {
                h = GUILayout.Height(800);
                w = GUILayout.Width(200);
                windowName = FullName;
                WindowRect.x-=135;
            }
            else
            {
                h = GUILayout.Height(40);
                w = GUILayout.Width(60);
                windowName = ShortName;
                WindowRect.x += 135;
            }
        }

        public void OnEnable()
        {
            Logger.LogWarning("OnEnable");
            Debug.LogWarning("OnEnable");
            harmony = Harmony.CreateAndPatchAll(typeof(DungreedPlugin));
        }

        public static MyItemData[] myItemDatas;
        public static string[] myItemDataNames;
        public static int myItemDataCnt;
        public static int[] myItemDataRaritys;

        //public void Start()
        //{
        //    myItemDatas = MyItemManager.Instance.GetAllAvailable();
        //    myItemDataNames=myItemDatas.Select(x => x.aName).ToArray();
        //}



        public void Update()
        {
            if (ShowCounter.Value.IsUp())// 단축키가 일치할때
            {
                isGUIOn.Value = !isGUIOn.Value;// 보이거나 안보이게. 이런 배열이였네 지웠음
                //MyLog.LogMessage("IsUp", ShowCounter.Value.MainKey);
            }
            if (player != null)
            {
                if (HpMaxSet)
                {
                    status.hp = status.maxHP;
                }
                if (DashMaxSet)
                {
                    controller2D.ramainingDashCount = controller2D.maxDashCount;
                }
            }
        }

        public void OnGUI()
        {
            if (!isGUIOn.Value)
                return;

            WindowRect = GUILayout.Window(windowId, WindowRect, WindowFunction, windowName, w, h);
        }

        public virtual void WindowFunction(int id)
        {
            GUI.enabled = true; // 기능 클릭 가능

            GUILayout.BeginHorizontal();// 가로 정렬
            // 라벨 추가
            //GUILayout.Label(windowName, GUILayout.Height(20));
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
                    if (GUILayout.Button($"Auto hp Set Max {HpMaxSet}")) { HpMaxSet = !HpMaxSet; }
                    if (GUILayout.Button($"hp {status.hp}={status.maxHP}")) { status.hp = status.maxHP; }
                    if (GUILayout.Button($"lockHP { status.lockHP}")) { status.lockHP = !status.lockHP; }
                    if (GUILayout.Button($"Auto Dash Set Max {DashMaxSet}")) { DashMaxSet = !DashMaxSet; }
                    if (GUILayout.Button($"Reload zero {ReloadZero1}")) { ReloadZero1 = !ReloadZero1; }
                    if (GUILayout.Button($"skill use clear {Skilluse}")) { Skilluse = !Skilluse; }
                    GUI.enabled = false;
                    if (GUILayout.Button($"ramainingDashCount {controller2D.ramainingDashCount}")) { }
                    if (GUILayout.Button($"maxDashCount {controller2D.maxDashCount}")) { HpMaxSet = !HpMaxSet; }
                    if (GUILayout.Button($"currentJumpCount {controller2D.currentJumpCount}")) { HpMaxSet = !HpMaxSet; }
                    if (GUILayout.Button($"jumpCount {controller2D.jumpCount}")) { HpMaxSet = !HpMaxSet; }
                    GUI.enabled = true;
                    if (GUILayout.Button($"abilityPoint { ability.abilityPoint}+100")) { ability.abilityPoint += 100; }
                    if (GUILayout.Button($"GetItem 0" )) { GetItemL(0); }
                    if (GUILayout.Button($"GetItem 1" )) { GetItemL(1); }
                    if (GUILayout.Button($"GetItem 2" )) { GetItemL(2); }
                    if (GUILayout.Button($"GetItem 3" )) { GetItemL(3); }

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

                    for (int i = 0; i < myItemDataCnt; i++)
                    {
                        if (GUILayout.Button($"{myItemDataRaritys[i]},{myItemDataNames[i]}")) { GetItem(myItemDatas[i]); }
                    }
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

        //public void Reload(bool beforeReload = false)


        [HarmonyPrefix, HarmonyPatch(typeof(Character_Hand), "Reload", typeof(bool))]
        // public void Reload(bool beforeReload = false)
        public static void ReloadPre(Character_Hand __instance, float ___reloadTimer, float ___chargeTimer, float ___chargeTime,ref int ___remainingShots)
        {
            logger.LogInfo($"ReloadPre , {___reloadTimer} , {___chargeTimer} , {___chargeTime }");
            //return true;
            if (ReloadZero1)
            {
                ___remainingShots = __instance.GetMaxShots();
            }



        }

        [HarmonyPostfix, HarmonyPatch(typeof(Character_Hand), "Reload", typeof(bool))]
        // public void Reload(bool beforeReload = false)
        public static void ReloadPost(Character_Hand __instance, float ___reloadTimer, float ___chargeTimer, float ___chargeTime)
        {
            logger.LogInfo($"ReloadPost , {___reloadTimer} , {___chargeTimer} , {___chargeTime }");
            //return true;
        }


        [HarmonyPrefix, HarmonyPatch(typeof(Character_Hand), "CompleteReload", typeof(bool))]
        // public void Reload(bool beforeReload = false)
        public static void CompleteReloadPre(Character_Hand __instance, float ___reloadTimer, float ___chargeTimer, float ___chargeTime)
        {
            logger.LogInfo($"CompleteReloadPre , {___reloadTimer} , {___chargeTimer} , {___chargeTime }");
            //return true;
        }

        [HarmonyPostfix, HarmonyPatch(typeof(Character_Hand), "CompleteReload", typeof(bool))]
        // public void Reload(bool beforeReload = false)
        public static void CompleteReloadPost(Character_Hand __instance, float ___reloadTimer, float ___chargeTimer, float ___chargeTime)
        {
            logger.LogInfo($"CompleteReloadPost , {___reloadTimer} , {___chargeTimer} , {___chargeTime }");
            //return true;
        }

        /// <summary>
        /// 하켄 대화
        /// </summary>
        [HarmonyPostfix, HarmonyPatch(typeof(NPC_Blacksmith), "Interactive")]
        // public void Reload(bool beforeReload = false)
        public static void Interactive()
        {
            logger.LogInfo($"NPC_Blacksmith Interactive Post ");
            //return true;
        }

        /// <summary>
        /// 대화후 아이템 떨굼
        /// </summary>
        [HarmonyPostfix, HarmonyPatch(typeof(NPC_Blacksmith), "SpawnItem")]
        // public void Reload(bool beforeReload = false)
        public static void SpawnItem(NPC_Blacksmith __instance)
        {
            logger.LogInfo($"NPC_Blacksmith SpawnItem Post ");
            //return true;

            //for (int i = 1; i <= 8; i++)
            //{
            //    MyItemData myItemData = __instance.SearchRandomItem();
            //    var id = myItemData.id;
            //    Item component = UnityEngine.Object.Instantiate<GameObject>(Resources.Load<GameObject>("Objects/Item"), player.transform.position + new Vector3(0f + i, 1f), Quaternion.identity).GetComponent<Item>();
            //    component.SetItem(id);
            //    component.ownType = ItemOwnInfo.OwnType.GIVENFROMTOWN;
            //    component._characterController2D.velocity.y = 10f;
            //    component.gameObject.AddComponent<AttributionToRoom>();
            //}
        }

        public void GetItemL(int l)
        {
            float a = 0f, b = 0f, c = 0f;

            switch (l)
            {
                case 0:
                    break;
                case 1:
                    a = 1f;
                    break;
                case 2:
                    b = 1f;
                    break;
                case 3:
                    c = 1f;
                    break;
                default:
                    break;
            }

            for (int i = 1; i <= 8; i++)
            {
                Item component = UnityEngine.Object.Instantiate<GameObject>(Resources.Load<GameObject>("Objects/Item"), player.transform.position + new Vector3(0f + i, 1f), Quaternion.identity).GetComponent<Item>();
                var t=MyItemManager.Instance.GetRandomItemAvailableWithCriteria(a, b, c, ItemUniverse.NONE);
                logger.LogInfo($"{t.id},{t.aName},{t.aDescription}");
                component.SetItem(t.id);
                component.ownType = ItemOwnInfo.OwnType.GIVENFROMTOWN;
                component._characterController2D.velocity.y = 10f;
                component.gameObject.AddComponent<AttributionToRoom>();
            }
        }

        public void GetItem(int l)
        {
            Item component = UnityEngine.Object.Instantiate<GameObject>(Resources.Load<GameObject>("Objects/Item"), player.transform.position + new Vector3(1f, 1f), Quaternion.identity).GetComponent<Item>();
            MyItemData t = MyItemManager.Instance.GetItem(l);
            GetItem(t);
        }
        
        public void GetItem(MyItemData t)
        {
            Item component = UnityEngine.Object.Instantiate<GameObject>(Resources.Load<GameObject>("Objects/Item"), player.transform.position + new Vector3(1f, 1f), Quaternion.identity).GetComponent<Item>();            
            logger.LogInfo($"{t.id},{t.aName},{t.aDescription}");
            component.SetItem(t.id);
            component.ownType = ItemOwnInfo.OwnType.GIVENFROMTOWN;
            component._characterController2D.velocity.y = 10f;
            component.gameObject.AddComponent<AttributionToRoom>();
        }

        public MyItemData SearchRandomItem()
        {
            MyItemData randomItemAvailableWithCriteria;
            do
            {
                randomItemAvailableWithCriteria = MyItemManager.Instance.GetRandomItemAvailableWithCriteria(0.3f, 0.01f, 0f, ItemUniverse.NONE);
            }
            while (!randomItemAvailableWithCriteria.sellAtShop || !randomItemAvailableWithCriteria.appearInTownNPC);
            return randomItemAvailableWithCriteria;
        }

        [HarmonyPostfix, HarmonyPatch(typeof(NPC_PistolMan), "Interactive")]
        // public void Reload(bool beforeReload = false)
        public static void Interactive2()
        {
            logger.LogInfo($"NPC_PistolMan Interactive Post ");
            //return true;
        }


        [HarmonyPostfix, HarmonyPatch(typeof(NPC_PistolMan), "SpawnItem")]
        // public void Reload(bool beforeReload = false)
        public static void SpawnItem2()
        {
            logger.LogInfo($"NPC_PistolMan SpawnItem Post ");
            //return true;
        }
        

        [HarmonyPostfix, HarmonyPatch(typeof(MyItemManager), "Initialize")]
        // public void Reload(bool beforeReload = false)
        public static void Initialize()
        {
            logger.LogInfo($"MyItemManager Initialize Post ");
            myItemDatas = MyItemManager.Instance.GetAllAvailable();
            myItemDataNames = myItemDatas.Select(x => I._(x.aName) ).ToArray();
            myItemDataRaritys = myItemDatas.Select(x => ((int)x.rarity)).ToArray();
            myItemDataCnt=myItemDataNames.Length;
        }


        /// <summary>
        /// ActiveSkill 자체는 추상 클래스라서 패치 불가?
        /// </summary>
        /// <param name="__instance"></param>
        /// <param name="use"></param>
        //[HarmonyPostfix, HarmonyPatch(typeof(ActiveSkill), "Use" )]
        // public void Reload(bool beforeReload = false)
        public static void Use(ActiveSkill __instance, ref bool use)
        {
            logger.LogInfo($"ActiveSkill Use Post {__instance.use}");
            if (Skilluse)
            {
                __instance.use = false;
            }
        }

              

        [HarmonyPrefix, HarmonyPatch(typeof(Character_Hand), "UseSkill")]
        // public void Reload(bool beforeReload = false)
        public static bool UseSkill(Character_Hand __instance, int num, Vector3 castingPosition)
        {
            logger.LogInfo($"Character_Hand UseSkill Post {num}");
            
            if (__instance.HasSkill(num) && Skilluse)
            {
                __instance.connectedSkills[num].use = false;
                __instance.connectedSkills[num].Use(castingPosition, null);
            }
            return false;
            //if(this.HasSkill(num) && this.connectedSkills[num].CanUse())
            //{
            //    this.connectedSkills[num].Use(castingPosition, null);
            //}
        }




    }
}
