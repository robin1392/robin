using System;
using System.Collections.Generic;
using System.Text;
using Service.Net;
using Service.Core;

namespace Template.Shop.GameBaseShop.Common
{
	public class GameBaseShopProtocol : ProtocolBase
	{
		public GameBaseShopProtocol(ISender sender = null, string serverAddr = "")
			: base(sender, serverAddr)
		{
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
			request.accessToken = _sender.GetAccessToken();
			return _sender.SendHttpPost(ShopInfoRequest.ProtocolId,
				_serverAddr + "shopinfo-v1.0.0",
				request.JsonSerialize());
		}

		public delegate ShopInfoResponse ShopInfoReqCallback(ShopInfoRequest request);
		public ShopInfoReqCallback OnShopInfoReqCallback;
		public ISerializer ShopInfoReqController(byte[] msg, int length)
		{
			string json = Encoding.Default.GetString(msg, 0, length);
			ShopInfoRequest request = new ShopInfoRequest();
			request.JsonDeserialize(json);
			return OnShopInfoReqCallback(request) as ISerializer;
		}

		public delegate bool ShopInfoResCallback(ShopInfoResponse response);
		public ShopInfoResCallback OnShopInfoResCallback;
		public ISerializer ShopInfoResController(byte[] msg, int length)
		{
			string json = Encoding.Default.GetString(msg, 0, length);
			ShopInfoResponse response = new ShopInfoResponse();
			response.JsonDeserialize(json);
			OnShopInfoResCallback(response);
			return null;
		}
		#endregion

		#region ShopBuy ------------------------------------------
		public bool ShopBuyReq(ShopBuyRequest request, ShopBuyResCallback callback)
		{
			OnShopBuyResCallback = callback;
			request.accessToken = _sender.GetAccessToken();
			return _sender.SendHttpPost(ShopBuyRequest.ProtocolId,
				_serverAddr + "shopbuy-v1.0.0",
				request.JsonSerialize());
		}

		public delegate ShopBuyResponse ShopBuyReqCallback(ShopBuyRequest request);
		public ShopBuyReqCallback OnShopBuyReqCallback;
		public ISerializer ShopBuyReqController(byte[] msg, int length)
		{
			string json = Encoding.Default.GetString(msg, 0, length);
			ShopBuyRequest request = new ShopBuyRequest();
			request.JsonDeserialize(json);
			return OnShopBuyReqCallback(request) as ISerializer;
		}

		public delegate bool ShopBuyResCallback(ShopBuyResponse response);
		public ShopBuyResCallback OnShopBuyResCallback;
		public ISerializer ShopBuyResController(byte[] msg, int length)
		{
			string json = Encoding.Default.GetString(msg, 0, length);
			ShopBuyResponse response = new ShopBuyResponse();
			response.JsonDeserialize(json);
			OnShopBuyResCallback(response);
			return null;
		}
		#endregion

		#region ShopPurchase ------------------------------------------
		public bool ShopPurchaseReq(ShopPurchaseRequest request, ShopPurchaseResCallback callback)
		{
			OnShopPurchaseResCallback = callback;
			request.accessToken = _sender.GetAccessToken();
			return _sender.SendHttpPost(ShopPurchaseRequest.ProtocolId,
				_serverAddr + "shoppurchase-v1.0.0",
				request.JsonSerialize());
		}

		public delegate ShopPurchaseResponse ShopPurchaseReqCallback(ShopPurchaseRequest request);
		public ShopPurchaseReqCallback OnShopPurchaseReqCallback;
		public ISerializer ShopPurchaseReqController(byte[] msg, int length)
		{
			string json = Encoding.Default.GetString(msg, 0, length);
			ShopPurchaseRequest request = new ShopPurchaseRequest();
			request.JsonDeserialize(json);
			return OnShopPurchaseReqCallback(request) as ISerializer;
		}

		public delegate bool ShopPurchaseResCallback(ShopPurchaseResponse response);
		public ShopPurchaseResCallback OnShopPurchaseResCallback;
		public ISerializer ShopPurchaseResController(byte[] msg, int length)
		{
			string json = Encoding.Default.GetString(msg, 0, length);
			ShopPurchaseResponse response = new ShopPurchaseResponse();
			response.JsonDeserialize(json);
			OnShopPurchaseResCallback(response);
			return null;
		}
		#endregion

		#region ShopReset ------------------------------------------
		public bool ShopResetReq(ShopResetRequest request, ShopResetResCallback callback)
		{
			OnShopResetResCallback = callback;
			request.accessToken = _sender.GetAccessToken();
			return _sender.SendHttpPost(ShopResetRequest.ProtocolId,
				_serverAddr + "shopreset-v1.0.0",
				request.JsonSerialize());
		}

		public delegate ShopResetResponse ShopResetReqCallback(ShopResetRequest request);
		public ShopResetReqCallback OnShopResetReqCallback;
		public ISerializer ShopResetReqController(byte[] msg, int length)
		{
			string json = Encoding.Default.GetString(msg, 0, length);
			ShopResetRequest request = new ShopResetRequest();
			request.JsonDeserialize(json);
			return OnShopResetReqCallback(request) as ISerializer;
		}

		public delegate bool ShopResetResCallback(ShopResetResponse response);
		public ShopResetResCallback OnShopResetResCallback;
		public ISerializer ShopResetResController(byte[] msg, int length)
		{
			string json = Encoding.Default.GetString(msg, 0, length);
			ShopResetResponse response = new ShopResetResponse();
			response.JsonDeserialize(json);
			OnShopResetResCallback(response);
			return null;
		}
		#endregion

	}
}
