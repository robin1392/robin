using System;
using System.Collections.Generic;
using System.Text;
using Service.Net;
using Service.Core;

namespace Template.Character.RandomwarsDice.Common
{
	public class RandomwarsDiceProtocol
	{
		public readonly ClientSession Session;
		public readonly string ServerAddr;
		public Dictionary<int, ControllerDelegate> MessageControllers { get;  private set; }

		public RandomwarsDiceProtocol()
		{
			Session = null;
			ServerAddr = string.Empty;

			Init();
		}

		public RandomwarsDiceProtocol(ClientSession session, string serverAddr)
		{
			Session = session;
			ServerAddr = serverAddr;

			Init();
		}

		void Init()
		{
			MessageControllers = new Dictionary<int, ControllerDelegate>();
			MessageControllers.Add(DiceUpgradeRequest.ProtocolId, DiceUpgradeReqController);
			MessageControllers.Add(DiceUpgradeResponse.ProtocolId, DiceUpgradeResController);
			MessageControllers.Add(DiceChangeDeckRequest.ProtocolId, DiceChangeDeckReqController);
			MessageControllers.Add(DiceChangeDeckResponse.ProtocolId, DiceChangeDeckResController);
		}

		#region DiceUpgrade ------------------------------------------
		public bool DiceUpgradeReq(DiceUpgradeRequest request, DiceUpgradeResCallback callback)
		{
			OnDiceUpgradeResCallback = callback;

			request.accessToken = Session.SessionKey;
			return Session.Send(DiceUpgradeRequest.ProtocolId,
				ServerAddr + "diceupgrade-v1.0.0",
				request.JsonSerialize());
		}

		public delegate DiceUpgradeResponse DiceUpgradeReqCallback(ClientSession session, DiceUpgradeRequest request);
		public DiceUpgradeReqCallback OnDiceUpgradeReqCallback;
		public ISerializer DiceUpgradeReqController(ClientSession session, byte[] msg, int length)
		{
			string json = Encoding.Default.GetString(msg, 0, length);
			DiceUpgradeRequest request = DiceUpgradeRequest.JsonDeserialize(json);
			return OnDiceUpgradeReqCallback(session, request) as ISerializer;
		}

		public delegate bool DiceUpgradeResCallback(DiceUpgradeResponse response);
		public DiceUpgradeResCallback OnDiceUpgradeResCallback;
		public ISerializer DiceUpgradeResController(ClientSession session, byte[] msg, int length)
		{
			string json = Encoding.Default.GetString(msg, 0, length);
			DiceUpgradeResponse response = DiceUpgradeResponse.JsonDeserialize(json);
			OnDiceUpgradeResCallback(response);
			return null;
		}
		#endregion

		#region DiceChangeDeck ------------------------------------------
		public bool DiceChangeDeckReq(DiceChangeDeckRequest request, DiceChangeDeckResCallback callback)
		{
			OnDiceChangeDeckResCallback = callback;

			request.accessToken = Session.SessionKey;
			return Session.Send(DiceChangeDeckRequest.ProtocolId,
				ServerAddr + "dicechangedeck-v1.0.0",
				request.JsonSerialize());
		}

		public delegate DiceChangeDeckResponse DiceChangeDeckReqCallback(ClientSession session, DiceChangeDeckRequest request);
		public DiceChangeDeckReqCallback OnDiceChangeDeckReqCallback;
		public ISerializer DiceChangeDeckReqController(ClientSession session, byte[] msg, int length)
		{
			string json = Encoding.Default.GetString(msg, 0, length);
			DiceChangeDeckRequest request = DiceChangeDeckRequest.JsonDeserialize(json);
			return OnDiceChangeDeckReqCallback(session, request) as ISerializer;
		}

		public delegate bool DiceChangeDeckResCallback(DiceChangeDeckResponse response);
		public DiceChangeDeckResCallback OnDiceChangeDeckResCallback;
		public ISerializer DiceChangeDeckResController(ClientSession session, byte[] msg, int length)
		{
			string json = Encoding.Default.GetString(msg, 0, length);
			DiceChangeDeckResponse response = DiceChangeDeckResponse.JsonDeserialize(json);
			OnDiceChangeDeckResCallback(response);
			return null;
		}
		#endregion

	}
}
