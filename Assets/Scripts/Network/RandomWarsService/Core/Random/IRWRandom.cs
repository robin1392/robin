using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RandomWarsService.Core.Random
{
    public interface IRWRandom
    {
        int Next();

        int Next(int min, int max);
    }
}
