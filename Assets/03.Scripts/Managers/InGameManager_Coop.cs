using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using CodeStage.AntiCheat.ObscuredTypes;
using MirageTest.Scripts;

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

            // 위치를 옮김.. 차후 데이터 로딩후 풀링을 해야되서....
            PoolManager.Get().MakePool();

            StartManager();

            SoundManager.instance.PlayBGM(Global.E_SOUND.BGM_INGAME_COOP);
        }

        public override void StartManager()
        {
            // UI_InGame.Get().SetMyNickName(NetworkManager.Get().GetNetInfo().playerInfo.Name , NetworkManager.Get().GetNetInfo().otherInfo.Name);
            
            
            UI_InGame.Get().ViewTargetDice(true);
            
            //KZSee:
            // event_SP_Edit.AddListener(RefreshSP);
            // event_SP_Edit.AddListener(SetSPUpgradeButton);
            
            //
            if (NetworkManager.Get().isReconnect)
            {
                UI_InGamePopup.Get().ViewGameIndicator(true);
                
                return;
            }
            
            // deck setting
            if (IsNetwork == true)
            {
                // my
                for(int i = 0 ; i < NetworkManager.Get().GetNetInfo().playerInfo.DiceIdArray.Length; i++)
                    print(NetworkManager.Get().GetNetInfo().playerInfo.DiceIdArray[i]);
                
                //서버에서 플레이어 스테이트를 통해 덱정보가 내려옴
                // playerController.SetDeck(NetworkManager.Get().GetNetInfo().playerInfo.DiceIdArray);
                //other
                // playerController.targetPlayer.SetDeck(NetworkManager.Get().GetNetInfo().otherInfo.DiceIdArray);
            }
            else
            {
                // 네트워크 안쓰니까...개발용으로
                //var deck = ObscuredPrefs.GetString("Deck", "1000/1001/1002/1003/1004");
                if (UserInfoManager.Get() != null)
                {
                    var deck = UserInfoManager.Get().GetActiveDeck();
                    //KZSee:
                    // playerController.SetDeck(deck);
                }

            }
        }
    }
}
