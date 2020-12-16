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
        DICE_NORMAL,
        DICE_MAGIC,
        DICE_EPIC,
        DICE_LEGEND,
    }


    public enum BOX_TYPE : byte
    {
        NONE,
        NORMAL_BOX,
        COOP_BOX,
    }    
}