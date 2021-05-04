using System;
using Photon.Deterministic;

namespace Quantum.Util
{
    public class MathUtil
    {
        public static FP Pow(FP baseNumber, Int32 expNumber)
        {
            FP result = 1;
            
            bool sing = true;
            if (expNumber < 0)
            {
                sing = false;
                expNumber = expNumber * -1;
            }
            
            for (Int32 i = 1; i <= expNumber; i++)
            {
                if (sing)
                    result = result * baseNumber;
                else
                    result /= baseNumber;
            }

            return result;
        }
    }
}