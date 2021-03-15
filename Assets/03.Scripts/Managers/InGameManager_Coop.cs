using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using CodeStage.AntiCheat.ObscuredTypes;
using MirageTest.Scripts;
using RandomWarsProtocol;

namespace ED
{
    public class InGameManager_Coop : InGameManager //, IPunObservable
    {
        [Header("Coop")]
        public GameObject pref_PlayerEmpty;
        public GameObject pref_Coop_AI;

        private bool isMaster;

        protected override void Start()
        {
            // 개발 버전이라..중간에서 실행햇을시에..
            if (DataPatchManager.Get().isDataLoadComplete == false)
                DataPatchManager.Get().JsonDownLoad();

            // 전체 주사위 정보
            data_DiceInfo = TableManager.Get().DiceInfo;

            // 위치를 옮김.. 차후 데이터 로딩후 풀링을 해야되서....
            PoolManager.Get().MakePool();

            StartManager();

            SoundManager.instance.PlayBGM(Global.E_SOUND.BGM_INGAME_COOP);
        }

        public override void StartManager()
        {
            if (IsNetwork == false) return;
            
            UI_InGamePopup.Get().SetViewWaiting(true);

            // player controller create...my and other
            Vector3 towerPos = FieldManager.Get().GetPlayerPos(true);
            Vector3 AIPos = FieldManager.Get().GetPlayerPos(false);

            GameObject myTObj = null;
            GameObject otherTObj = null;
            GameObject coopTObj = null;
            isMaster = NetworkManager.Get().IsMaster;
            if (isMaster)
            {
                myTObj = UnityUtil.Instantiate("Tower/" + pref_Player.name);
                otherTObj = UnityUtil.Instantiate("Tower/" + pref_PlayerEmpty.name);
            }
            else
            {
                myTObj = UnityUtil.Instantiate("Tower/" + pref_PlayerEmpty.name);
                otherTObj = UnityUtil.Instantiate("Tower/" + pref_Player.name);
            }
            coopTObj = UnityUtil.Instantiate("Tower/" + pref_Coop_AI.name);

            myTObj.transform.parent = FieldManager.Get().GetPlayerTrs(NetworkManager.Get().GetNetInfo().playerInfo.IsBottomPlayer);
            myTObj.transform.position = towerPos;
            playerController = myTObj.GetComponent<PlayerController>();
            playerController.isMine = true;
            playerController.ChangeLayer(true, true);
            
            otherTObj.transform.parent = FieldManager.Get().GetPlayerTrs(NetworkManager.Get().GetNetInfo().otherInfo.IsBottomPlayer);
            otherTObj.transform.position = towerPos;
            playerController.targetPlayer = otherTObj.GetComponent<PlayerController>();
            playerController.targetPlayer.isMine = false;
            // playerController.targetPlayer.isBottomCamp = true;
            playerController.targetPlayer.targetPlayer = playerController;
            
            playerController.targetPlayer.ChangeLayer(true, true);
            
            FindObjectOfType<UI_CoopSpawnTurn>().Set(NetworkManager.Get().GetNetInfo().playerInfo.IsMaster);
            
            // AI
            coopTObj.transform.parent = FieldManager.Get().GetPlayerTrs(false);
            coopTObj.transform.position = AIPos;
            var AI = coopTObj.GetComponent<Coop_AI>();
            playerController.coopPlayer = AI;
            playerController.targetPlayer.coopPlayer = AI;
            AI.isMine = isMaster;
            AI.targetPlayer = isMaster ? playerController : playerController.targetPlayer;
            AI.coopPlayer = AI.targetPlayer;
            AI.ChangeLayer(false, true);

            //
            UI_InGame.Get().SetNickName(NetworkManager.Get().GetNetInfo().playerInfo.Name , NetworkManager.Get().GetNetInfo().otherInfo.Name);
            
            
            UI_InGame.Get().ViewTargetDice(true);
            
            event_SP_Edit.AddListener(RefreshSP);
            event_SP_Edit.AddListener(SetSPUpgradeButton);
            
            //
            if (NetworkManager.Get().isReconnect)
            {
                UI_InGamePopup.Get().ViewGameIndicator(true);
                
                SendInGameManager(GameProtocol.READY_SYNC_GAME_REQ);
                return;
            }
            
            // deck setting
            if (IsNetwork == true)
            {
                // my
                for(int i = 0 ; i < NetworkManager.Get().GetNetInfo().playerInfo.DiceIdArray.Length; i++)
                    print(NetworkManager.Get().GetNetInfo().playerInfo.DiceIdArray[i]);
                
                playerController.SetDeck(NetworkManager.Get().GetNetInfo().playerInfo.DiceIdArray);
                //other
                playerController.targetPlayer.SetDeck(NetworkManager.Get().GetNetInfo().otherInfo.DiceIdArray);
            }
            else
            {
                // 네트워크 안쓰니까...개발용으로
                //var deck = ObscuredPrefs.GetString("Deck", "1000/1001/1002/1003/1004");
                if (UserInfoManager.Get() != null)
                {
                    var deck = UserInfoManager.Get().GetActiveDeck();
                    playerController.SetDeck(deck);
                }

            }
            
            if (IsNetwork == true)
            {
                WorldUIManager.Get().SetWave(wave);
                SendInGameManager(GameProtocol.READY_GAME_REQ);
            }
            else
            {
                StartGame();
                RefreshTimeUI(true);
            }
        }
    }
}
