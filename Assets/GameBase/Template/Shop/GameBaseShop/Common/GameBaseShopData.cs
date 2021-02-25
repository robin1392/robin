using System;

namespace Template.Shop.GameBaseShop.Common
{
    // 상점 정보
    [Serializable]
    public class ShopInfo
    {
        // ShopInfo 테이블 아이디
        public int shopId;
        // 이벤트 남은 시간(초단위)
        public int eventRemainTime;
        // 갱신 남은 시간(초단위)
        public int resetRemainTime;
        // 상품 목록
        public ShopProductInfo[] arrayProductInfo;
        // 재화 갱신 횟수
        public byte pointResetCount;
        // 광고 갱신 횟수
        public byte adResetCount;
    }


    // 상점 상품 정보
    [Serializable]
    public class ShopProductInfo
    {
        // 상품 아이디
        public int shopProductId;
        // 구매 횟수
        public byte buyCount;
    }


    // 상점 취급 아이템 정보(재화, 아이템 등등)
    [Serializable]
    public class ShopItemInfo
    {
        // ItemList에 정의된 각종 재화, 아이템 아이디
        public int itemId;
        // 재화, 아이템 변동값
        public int value;
    }


    // 상점 초기화 정보
    [Serializable]
    public class ShopResetInfo
    {
    }
}