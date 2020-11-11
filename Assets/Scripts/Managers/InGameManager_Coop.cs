using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using CodeStage.AntiCheat.ObscuredTypes;
using RandomWarsProtocol;

namespace ED
{
    public class InGameManager_Coop : InGameManager //, IPunObservable
    {
        [Header("Coop")]
        public GameObject pref_PlayerEmpty;
        public GameObject pref_Coop_AI;

        private bool isMaster;

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
            playerController.isBottomPlayer = NetworkManager.Get().GetNetInfo().playerInfo.IsBottomPlayer;
            playerController.ChangeLayer(NetworkManager.Get().GetNetInfo().playerInfo.IsBottomPlayer);
            
            otherTObj.transform.parent = FieldManager.Get().GetPlayerTrs(NetworkManager.Get().GetNetInfo().otherInfo.IsBottomPlayer);
            otherTObj.transform.position = towerPos;
            playerController.targetPlayer = otherTObj.GetComponent<PlayerController>();
            playerController.targetPlayer.isMine = false;
            playerController.targetPlayer.isBottomPlayer = NetworkManager.Get().GetNetInfo().otherInfo.IsBottomPlayer;
            playerController.targetPlayer.targetPlayer = playerController;
            
            playerController.targetPlayer.ChangeLayer(NetworkManager.Get().GetNetInfo().otherInfo.IsBottomPlayer);
            
            // AI
            coopTObj.transform.parent = FieldManager.Get().GetPlayerTrs(false);
            coopTObj.transform.position = AIPos;
            var AI = coopTObj.GetComponent<Coop_AI>();
            playerController.coopPlayer = AI;
            playerController.targetPlayer.coopPlayer = AI;
            AI.isMine = isMaster;
            AI.isBottomPlayer = false;
            AI.ChangeLayer(false);

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
            
            // Upgrade buttons
            // ui 셋팅
            UI_InGame.Get().SetArrayDeck(playerController.arrDiceDeck, arrUpgradeLevel);

            if (IsNetwork == true)
            {
                if (NetworkManager.Get().IsMaster == false)
                {
                    ts_StadiumTop.localRotation = Quaternion.Euler(180f, 0, 180f);
                    ts_NexusHealthBar.localRotation = Quaternion.Euler(0, 0, 180f);
                    ts_Lights.localRotation = Quaternion.Euler(0, 340f, 0);
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
