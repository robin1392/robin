using System;
using System.Linq;
using Cysharp.Threading.Tasks;
using MirageTest.Scripts.Entities;

namespace MirageTest.Scripts
{
    public class AIPlayer
    {
        private PlayerState _playerState;
        private Random _random;

        public AIPlayer(PlayerState playerState)
        {
            _playerState = playerState;
            _random = new Random();
        }

        public async UniTask UpdataAI()
        {
            await UniTask.Delay(TimeSpan.FromSeconds(1));
            
            while (true)
            {
                await UniTask.Delay(TimeSpan.FromSeconds(1));

                if (_playerState.GetEmptySlotCount() > 0)
                {
                    if (_playerState.GetDiceCost() <= _playerState.sp)
                    {
                        _playerState.GetDice();
                    }
                }
                else
                {
                    var groups = _playerState.Field.GroupBy(f => (f.diceId, f.diceScale));
                    foreach (var group in groups)
                    {
                        if (group.Key.diceId == 0)
                        {
                            continue;
                        }

                        if (group.Count() >= 2)
                        {
                            var shuffled = group.OrderBy(x => _random.Next());
                            _playerState.MergeDice(shuffled.ElementAt(0).index, shuffled.ElementAt(1).index);
                        }
                    }
                }
            }
        }
    }
}