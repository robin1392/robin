using System;
using System.Collections.Generic;
using System.Text;
using Service.Net;
using Service.Core;

namespace Template.Shop.GameBaseShop.Common
{
	public class GameBaseShopProtocol
	{
		public readonly ClientSession Session;
		public readonly string ServerAddr;
		public Dictionary<int, ControllerDelegate> MessageControllers { get;  private set; }

		public GameBaseShopProtocol()
		{
			Session = null;
			ServerAddr = string.Empty;

			Init();
		}

		public GameBaseShopProtocol(ClientSession session, string serverAddr)
		{
			Session = session;
			ServerAddr = serverAddr;

			Init();
		}

		void Init()
		{
			MessageControllers = new Dictionary<int, ControllerDelegate>();
			MessageControllers.Add(ShopInfoRequest.ProtocolId, ShopInfoReqController);
			MessageControllers.Add(ShopInfoResponse.ProtocolId, ShopInfoResController);
			MessageControllers.Add(ShopBuyRequest.ProtocolId, ShopBuyReqController);
			MessageControllers.Add(ShopBuyResponse.ProtocolId, ShopBuyResController);
			MessageControllers.Add(ShopPurchaseRequest.ProtocolId, ShopPurchaseReqController);
			MessageControllers.Add(ShopPurchaseResponse.ProtocolId, ShopPurchaseResController);
			MessageControllers.Add(ShopResetRequest.ProtocolId, ShopResetReqController);
			MessageControllers.Add(ShopResetResponse.ProtocolId, ShopResetResController);
		}

		#region ShopInfo ------------------------------------------
		public bool ShopInfoReq(ShopInfoRequest request, ShopInfoResCallback callback)
		{
			OnShopInfoResCallback = callback;

			request.accessToken = Session.SessionKey;
			return Session.Send(ShopInfoRequest.ProtocolId,
				ServerAddr + "shopinfo-v1.0.0",
				request.JsonSerialize());
		}

		public delegate ShopInfoResponse ShopInfoReqCallback(ClientSession session, ShopInfoRequest request);
		public ShopInfoReqCallback OnShopInfoReqCallback;
		public ISerializer ShopInfoReqController(ClientSession session, byte[] msg, int length)
		{
			string json = Encoding.Default.GetString(msg, 0, length);
			ShopInfoRequest request = ShopInfoRequest.JsonDeserialize(json);
			return OnShopInfoReqCallback(session, request) as ISerializer;
		}

		public delegate bool ShopInfoResCallback(ShopInfoResponse response);
		public ShopInfoResCallback OnShopInfoResCallback;
		public ISerializer ShopInfoResController(ClientSession session, byte[] msg, int length)
		{
			string json = Encoding.Default.GetString(msg, 0, length);
			ShopInfoResponse response = ShopInfoResponse.JsonDeserialize(json);
			OnShopInfoResCallback(response);
			return null;
		}
		#endregion

		#region ShopBuy ------------------------------------------
		public bool ShopBuyReq(ShopBuyRequest request, ShopBuyResCallback callback)
		{
			OnShopBuyResCallback = callback;

			request.accessToken = Session.SessionKey;
			return Session.Send(ShopBuyRequest.ProtocolId,
				ServerAddr + "shopbuy-v1.0.0",
				request.JsonSerialize());
		}

		public delegate ShopBuyResponse ShopBuyReqCallback(ClientSession session, ShopBuyRequest request);
		public ShopBuyReqCallback OnShopBuyReqCallback;
		public ISerializer ShopBuyReqController(ClientSession session, byte[] msg, int length)
		{
			string json = Encoding.Default.GetString(msg, 0, length);
			ShopBuyRequest request = ShopBuyRequest.JsonDeserialize(json);
			return OnShopBuyReqCallback(session, request) as ISerializer;
		}

		public delegate bool ShopBuyResCallback(ShopBuyResponse response);
		public ShopBuyResCallback OnShopBuyResCallback;
		public ISerializer ShopBuyResController(ClientSession session, byte[] msg, int length)
		{
			string json = Encoding.Default.GetString(msg, 0, length);
			ShopBuyResponse response = ShopBuyResponse.JsonDeserialize(json);
			OnShopBuyResCallback(response);
			return null;
		}
		#endregion

		#region ShopPurchase ------------------------------------------
		public bool ShopPurchaseReq(ShopPurchaseRequest request, ShopPurchaseResCallback callback)
		{
			OnShopPurchaseResCallback = callback;

			request.accessToken = Session.SessionKey;
			return Session.Send(ShopPurchaseRequest.ProtocolId,
				ServerAddr + "shoppurchase-v1.0.0",
				request.JsonSerialize());
		}

		public delegate ShopPurchaseResponse ShopPurchaseReqCallback(ClientSession session, ShopPurchaseRequest request);
		public ShopPurchaseReqCallback OnShopPurchaseReqCallback;
		public ISerializer ShopPurchaseReqController(ClientSession session, byte[] msg, int length)
		{
			string json = Encoding.Default.GetString(msg, 0, length);
			ShopPurchaseRequest request = ShopPurchaseRequest.JsonDeserialize(json);
			return OnShopPurchaseReqCallback(session, request) as ISerializer;
		}

		public delegate bool ShopPurchaseResCallback(ShopPurchaseResponse response);
		public ShopPurchaseResCallback OnShopPurchaseResCallback;
		public ISerializer ShopPurchaseResController(ClientSession session, byte[] msg, int length)
		{
			string json = Encoding.Default.GetString(msg, 0, length);
			ShopPurchaseResponse response = ShopPurchaseResponse.JsonDeserialize(json);
			OnShopPurchaseResCallback(response);
			return null;
		}
		#endregion

		#region ShopReset ------------------------------------------
		public bool ShopResetReq(ShopResetRequest request, ShopResetResCallback callback)
		{
			OnShopResetResCallback = callback;

			request.accessToken = Session.SessionKey;
			return Session.Send(ShopResetRequest.ProtocolId,
				ServerAddr + "shopreset-v1.0.0",
				request.JsonSerialize());
		}

		public delegate ShopResetResponse ShopResetReqCallback(ClientSession session, ShopResetRequest request);
		public ShopResetReqCallback OnShopResetReqCallback;
		public ISerializer ShopResetReqController(ClientSession session, byte[] msg, int length)
		{
			string json = Encoding.Default.GetString(msg, 0, length);
			ShopResetRequest request = ShopResetRequest.JsonDeserialize(json);
			return OnShopResetReqCallback(session, request) as ISerializer;
		}

		public delegate bool ShopResetResCallback(ShopResetResponse response);
		public ShopResetResCallback OnShopResetResCallback;
		public ISerializer ShopResetResController(ClientSession session, byte[] msg, int length)
		{
			string json = Encoding.Default.GetString(msg, 0, length);
			ShopResetResponse response = ShopResetResponse.JsonDeserialize(json);
			OnShopResetResCallback(response);
			return null;
		}
		#endregion

	}
}
