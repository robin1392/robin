namespace Service.Template.Common
{
    public enum ITEM_GRADE : byte
    {
        NORMAL = 0,
        MAGIC = 1,
        EPIC = 2,
        LEGEND = 3,
    }


    public enum REWARD_TYPE : byte
    {
        NONE = 0,
        TROPHY,
        GOLD,
        DIAMOND,
        KEY,
        BOX,
        ITEM,
        ITEM_NORMAL,
        ITEM_MAGIC,
        ITEM_EPIC,
        ITEM_LEGEND,
    }


    public enum BOX_TYPE : byte
    {
        NONE,
        NORMAL_BOX,
        COOP_BOX,
    }    
}