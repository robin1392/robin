using System;
using System.Collections.Generic;
using Service.Net;
using System.IO;

namespace Template.Account.RandomWarsAccount.Common
{
    public enum ERandomWarsAccountProtocol
    {
        BEGIN = 1000000,
        
        LOGIN_ACCOUNT_REQ,
        LOGIN_ACCOUNT_ACK,
        LOGIN_ACCOUNT_NOTIFY,

        END
    }


    public class RandomWarsAccountProtocol : MessageControllerBase
    {
        public RandomWarsAccountProtocol()
        {
            MessageControllers = new Dictionary<int, ControllerDelegate>
            {
                {(int)ERandomWarsAccountProtocol.LOGIN_ACCOUNT_REQ, ReceiveLoginAccountReq},
                {(int)ERandomWarsAccountProtocol.LOGIN_ACCOUNT_ACK, ReceiveLoginAccountAck},
            };
        }


        // -------------------------------------------------------------------
        // Http Controller 구현부
        // -------------------------------------------------------------------
#region Http Controller 구현부        
        public bool SendLoginAccountReq(ISender sender, string platformId, EPlatformType platformType)
        {
            using (var ms = new MemoryStream())
            {
                BinaryWriter bw = new BinaryWriter(ms);
                bw.Write(platformId);
                bw.Write((int)platformType);
                sender.SendMessage((int)ERandomWarsAccountProtocol.LOGIN_ACCOUNT_REQ, "accountlogin", ms.ToArray());
            }
            return true;
        }


        public delegate (ERandomWarsAccountErrorCode errorCode, MsgAccount accountInfo) ReceiveLoginAccountReqDelegate(string platformId, EPlatformType platformType);
        public ReceiveLoginAccountReqDelegate ReceiveLoginAccountReqCallback;
        public bool ReceiveLoginAccountReq(ISender sender, byte[] msg)
        {
            using (var ms = new MemoryStream(msg))
            {
                BinaryReader br = new BinaryReader(ms);
                var res = ReceiveLoginAccountReqCallback( 
                    br.ReadString(), 
                    (EPlatformType)br.ReadInt32());

                return SendLoginAccountAck(sender, res.errorCode, res.accountInfo);
            }
        }


        public bool SendLoginAccountAck(ISender sender, ERandomWarsAccountErrorCode errorCode, MsgAccount accountInfo)
        {
            using (var ms = new MemoryStream())
            {
                BinaryWriter bw = new BinaryWriter(ms);
                bw.Write((int)errorCode);
                accountInfo.Write(bw);
                return sender.SendMessage((int)ERandomWarsAccountProtocol.LOGIN_ACCOUNT_ACK, ms.ToArray());
            }
        }


        public delegate bool ReceiveLoginAccountAckDelegate(ERandomWarsAccountErrorCode errorCode, MsgAccount accountInfo);
        public ReceiveLoginAccountAckDelegate ReceiveLoginAccountAckCallback;
        public bool ReceiveLoginAccountAck(ISender sender, byte[] msg)
        {
            using (var ms = new MemoryStream(msg))
            {
                BinaryReader br = new BinaryReader(ms);
                return ReceiveLoginAccountAckCallback( 
                    (ERandomWarsAccountErrorCode)br.ReadInt32(), 
                    MsgAccount.Read(br));
            }
        }
#endregion

    }
}