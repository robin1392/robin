using System;
using Cysharp.Threading.Tasks;
using MirageTest.Scripts.Entities;

namespace MirageTest.Scripts
{
    public class AIPlayer
    {
        private PlayerState _playerState;

        public AIPlayer(PlayerState playerState)
        {
            _playerState = playerState;
        }

        public async UniTask UpdataAI()
        {
            await UniTask.Delay(TimeSpan.FromSeconds(5));
            
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
                    foreach (var deck in _playerState.Deck)
                    {
                        if (_playerState.GetUpgradeIngameCost(deck.inGameLevel) <= _playerState.sp)
                        {
                            _playerState.UpgradeIngameLevel(deck.diceId);
                            break;
                        }
                    }
                }
            }
        }
    }
}