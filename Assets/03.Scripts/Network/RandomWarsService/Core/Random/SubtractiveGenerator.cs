using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RandomWarsService.Core.Random
{
    public class SubtractiveGenerator : IRWRandom
    {
        public static int MAX = 1000000000;
        private int[] state;
        private int pos;

        private int Mod(int n)
        {
            return ((n % MAX) + MAX) % MAX;
        }

        public SubtractiveGenerator(int seed)
        {
            state = new int[55];

            int[] temp = new int[55];
            temp[0] = Mod(seed);
            temp[1] = 1;
            for (int i = 2; i < 55; ++i)
                temp[i] = Mod(temp[i - 2] - temp[i - 1]);

            for (int i = 0; i < 55; ++i)
                state[i] = temp[(34 * (i + 1)) % 55];

            pos = 54;
            for (int i = 55; i < 220; ++i)
                Next();
        }

        public int Next()
        {
            int temp = Mod(state[(pos + 1) % 55] - state[(pos + 32) % 55]);
            pos = (pos + 1) % 55;
            state[pos] = temp;
            return temp;
        }

        public int Next(int min, int max)
        {
            int temp = Next();
            return (temp % max) + min;
        }

    }
}
