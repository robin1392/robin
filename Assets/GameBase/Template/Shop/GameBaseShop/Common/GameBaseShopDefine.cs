namespace Template.Shop.GameBaseShop.Common
{
    public enum GameBaseShopErrorCode
    {
        Success = 0,
        Fatal = 40000,
        ErrorNotFoundShopInfo = 40001,                  
        ErrorNotFoundEventShopInfo = 40002,          
        ErrorNotFoundDailyShopInfo = 40003,          
        ErrorShopResetTime = 40004,
        ErrorShopResetProduct = 40005,
        ErrorShopNotFoundProduct = 40006,
        ErrorShopBuyItemLack = 40007,
        ErrorShopConfirmBilling = 40008,
        ErrorShopResetNotAllow = 40009,


        PurchaseSuccessed = 40010, // 구매 성공 
        PurchaseNotFound = 40011,  // 구매항목 찾지 못함
        PurchaseInitError = 40012, // 초기화 실패, 인터넷 또는 스토어 로그인 확인 
        PurchaseException = 40013, // 로직부분 충돌 
        PurchaseNotSupportOS = 40014, // 지원하지 않는 OS
        PurchaseFailed = 40015, // 결제 로직 중 실패
        PurchaseAlreadyBuyProcess = 40016 // 다른 결제 처리가 이미 진행중에 있음         
    }

}