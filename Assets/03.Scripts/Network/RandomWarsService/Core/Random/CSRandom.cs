using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RandomWarsService.Core.Random
{
    public class CSRandom : IRWRandom
    {
        System.Random _rand;


        public CSRandom(int seed)
        {
            _rand = new System.Random(seed);
        }

        public int Next()
        {
            return _rand.Next();
        }

        public int Next(int min, int max)
        {
            int temp = Next();
            return (temp % max) + min;
        }

    }
}
