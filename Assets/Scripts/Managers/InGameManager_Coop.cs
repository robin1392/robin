using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using Photon.Pun;
using Photon.Realtime;
using CodeStage.AntiCheat.ObscuredTypes;

namespace ED
{
    public class InGameManager_Coop : InGameManager, IPunObservable
    {
        public GameObject pref_Enemy_AI;

        protected override void Start()
        {
            arrUpgradeLevel = new int[6];
            event_SP_Edit = new IntEvent();

            if (PhotonNetwork.IsConnected)
            {
                if (PlayerController.Get() == null)
                {
                    Debug.LogFormat("We are Instantiating LocalPlayer from {0}", Application.identifier);

                    Vector3 startPos = FieldManager.Get().GetPlayerPos(PhotonNetwork.IsMasterClient);
                    GameObject obj = PhotonNetwork.Instantiate("Tower/"+pref_Player.name, startPos, Quaternion.identity, 0);
                    obj.transform.parent = FieldManager.Get().GetPlayerTrs(true);
                    playerController = obj.GetComponent<PlayerController>();
                    //playerController.photonView.RPC("ChangeLayer", RpcTarget.All, true);
                    playerController.SendPlayer(RpcTarget.All, E_PTDefine.PT_CHANGELAYER , true);

                    if (PhotonNetwork.IsMasterClient)
                    {
                        obj = PhotonNetwork.Instantiate(pref_Enemy_AI.name, FieldManager.Get().GetPlayerPos(false), Quaternion.identity, 0);
                        obj.transform.parent = FieldManager.Get().GetPlayerTrs(false);;
                    }
                }
                else
                {
                    Debug.LogFormat("Ignoring scene load for {0}", SceneManagerHelper.ActiveSceneName);
                }
            }
            else
            {
                GameObject obj = Instantiate(pref_Player, FieldManager.Get().GetPlayerPos(true), Quaternion.identity);
                obj.transform.parent = FieldManager.Get().GetPlayerTrs(true);
                obj.layer = LayerMask.NameToLayer("BottomPlayer");
                playerController = obj.GetComponent<PlayerController>();
                playerController.isBottomPlayer = true;
                playerController.isMine = true;

                obj = Instantiate(pref_Player, FieldManager.Get().GetPlayerPos(true), Quaternion.identity);
                obj.transform.parent = FieldManager.Get().GetPlayerTrs(true);
                obj.layer = LayerMask.NameToLayer("BottomPlayer");

                // 적 생성
                obj = Instantiate(pref_Enemy_AI, FieldManager.Get().GetPlayerPos(false), Quaternion.identity);
                obj.transform.parent = FieldManager.Get().GetPlayerTrs(false);
            }

            string deck = ObscuredPrefs.GetString("Deck", "0/1/2/3/4/5");
            if (PhotonNetwork.IsConnected)
            {
                //playerController.photonView.RPC("SetDeck", RpcTarget.All, deck);
                playerController.SendPlayer(RpcTarget.All , E_PTDefine.PT_SETDECK , deck);
            }
            else
            {
                playerController.SetDeck(deck);
            }

            // Upgrade buttons
            UI_InGame.Get().SetArrayDeck(playerController.arrDiceDeck , arrUpgradeLevel);

            StartGame();
            RefreshTimeUI(true);
        }

        public override void OnPlayerEnteredRoom(Player newPlayer)
        {

        }

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {

        }

        public override void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                stream.SendNext(time);
                stream.SendNext(wave);
            }
            else
            {
                time = (float)stream.ReceiveNext();
                wave = (int)stream.ReceiveNext();
            }
        }

        public void ShowTeamField(bool isShow)
        {
            if (isShow)
            {
                playerController.uiDiceField.SetField(playerController.targetPlayer.arrDice);
                playerController.uiDiceField.RefreshField(0.5f);
            }
            else
            {
                playerController.uiDiceField.SetField(playerController.arrDice);
                playerController.uiDiceField.RefreshField();
            }
        }
    }
}
