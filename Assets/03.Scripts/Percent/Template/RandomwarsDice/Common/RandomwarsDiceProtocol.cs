using System;
using System.Collections.Generic;
using System.Text;
using Service.Net;
using Service.Core;

namespace Template.Character.RandomwarsDice.Common
{
	public class RandomwarsDiceProtocol : ProtocolBase
	{
		public RandomwarsDiceProtocol(ISender sender = null, string serverAddr = "")
			: base(sender, serverAddr)
		{
			MessageControllers.Add(DiceUpgradeRequest.ProtocolId, DiceUpgradeReqController);
			MessageControllers.Add(DiceUpgradeResponse.ProtocolId, DiceUpgradeResController);
			MessageControllers.Add(DiceChangeDeckRequest.ProtocolId, DiceChangeDeckReqController);
			MessageControllers.Add(DiceChangeDeckResponse.ProtocolId, DiceChangeDeckResController);
		}

		#region DiceUpgrade ------------------------------------------
		public bool DiceUpgradeReq(DiceUpgradeRequest request, DiceUpgradeResCallback callback)
		{
			OnDiceUpgradeResCallback = callback;
			request.accessToken = _sender.GetAccessToken();
			return _sender.SendHttpPost(DiceUpgradeRequest.ProtocolId,
				_serverAddr + "diceupgrade-v1.0.0",
				request.JsonSerialize());
		}

		public delegate DiceUpgradeResponse DiceUpgradeReqCallback(DiceUpgradeRequest request);
		public DiceUpgradeReqCallback OnDiceUpgradeReqCallback;
		public ISerializer DiceUpgradeReqController(byte[] msg, int length)
		{
			string json = Encoding.Default.GetString(msg, 0, length);
			DiceUpgradeRequest request = new DiceUpgradeRequest();
			request.JsonDeserialize(json);
			return OnDiceUpgradeReqCallback(request) as ISerializer;
		}

		public delegate bool DiceUpgradeResCallback(DiceUpgradeResponse response);
		public DiceUpgradeResCallback OnDiceUpgradeResCallback;
		public ISerializer DiceUpgradeResController(byte[] msg, int length)
		{
			string json = Encoding.Default.GetString(msg, 0, length);
			DiceUpgradeResponse response = new DiceUpgradeResponse();
			response.JsonDeserialize(json);
			OnDiceUpgradeResCallback(response);
			return null;
		}
		#endregion

		#region DiceChangeDeck ------------------------------------------
		public bool DiceChangeDeckReq(DiceChangeDeckRequest request, DiceChangeDeckResCallback callback)
		{
			OnDiceChangeDeckResCallback = callback;
			request.accessToken = _sender.GetAccessToken();
			return _sender.SendHttpPost(DiceChangeDeckRequest.ProtocolId,
				_serverAddr + "dicechangedeck-v1.0.0",
				request.JsonSerialize());
		}

		public delegate DiceChangeDeckResponse DiceChangeDeckReqCallback(DiceChangeDeckRequest request);
		public DiceChangeDeckReqCallback OnDiceChangeDeckReqCallback;
		public ISerializer DiceChangeDeckReqController(byte[] msg, int length)
		{
			string json = Encoding.Default.GetString(msg, 0, length);
			DiceChangeDeckRequest request = new DiceChangeDeckRequest();
			request.JsonDeserialize(json);
			return OnDiceChangeDeckReqCallback(request) as ISerializer;
		}

		public delegate bool DiceChangeDeckResCallback(DiceChangeDeckResponse response);
		public DiceChangeDeckResCallback OnDiceChangeDeckResCallback;
		public ISerializer DiceChangeDeckResController(byte[] msg, int length)
		{
			string json = Encoding.Default.GetString(msg, 0, length);
			DiceChangeDeckResponse response = new DiceChangeDeckResponse();
			response.JsonDeserialize(json);
			OnDiceChangeDeckResCallback(response);
			return null;
		}
		#endregion

	}
}
