using Photon.Deterministic;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Quantum
{
    [Serializable]
    public unsafe partial class MergeFieldDiceRandom : BTLeaf
    {
        protected override BTStatus OnUpdate(BTParams p)
        {
            var e = p.Entity;
            var f = p.Frame;

            var rwPlayer = f.GetRWPlayer(e);

            var deck = f.Get<Deck>(e);
            var field = f.Get<Field>(e);

            var dic = new Dictionary<KeyValuePair<int, int>, List<int>>();
            for (var i = 0; i < field.Dices.Length; ++i)
            {
                var fieldDice = field.Dices[i];
                if (fieldDice.IsEmpty)
                {
                    continue;
                }
                
                var deckDice = deck.Dices[fieldDice.DeckIndex];

                var key = new KeyValuePair<int, int>(fieldDice.DiceScale, deckDice.DiceId);
                if (dic.TryGetValue(key, out var list) == false)
                {
                    list = new List<int>();
                    dic.Add(key, list);
                }
                
                list.Add(i);
            }

            foreach (var same in dic.Where(l => l.Value.Count > 1))
            {
                var ordered = same.Value.OrderBy(_ => f.RNG->Next());
                MergeDiceCommandSystem.MergeFieldDice(f, ordered.ElementAt(0), ordered.ElementAt(1), rwPlayer);
                return BTStatus.Success;
            }

            return BTStatus.Failure;
        }
    }
}