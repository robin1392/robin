namespace ED
{
    public partial class BaseStat
    {
        public virtual void StartAI()
        {
        }
        
        public virtual void StopAI()
        {
            if (_syncActionCoroutine != null)
            {
                StopCoroutine(_syncActionCoroutine);
                _syncActionCoroutine = null;
            }
        }
    }
}