using Photon.Deterministic;

namespace Quantum.Util
{
    public static class ParseUtil
    {
        public static FP ToFP(string value)
        {
            return ToFP(value, FP._0);
        }

        public static FP ToFP(string value, FP defatulValue)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return defatulValue;
            }
            
            var values  = value.Split('.');
            var digit = int.Parse(values[0]);
            if (values.Length == 1)
            {
                return digit;
            }

            if (values.Length == 2)
            {
                var dec = int.Parse(values[1]);
                return digit + (dec / FP._100);    
            }
            
            Log.Error($"{value} FP로 변환 할 수 없습니다.");
            return defatulValue;
        }
    }
}