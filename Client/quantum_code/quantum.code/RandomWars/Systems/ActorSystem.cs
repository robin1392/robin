using System;
using Photon.Deterministic;

namespace Quantum
{
    public unsafe struct ActorFilter
    {
        public EntityRef EntityRef;
        public Actor* Actor;
        public BTAgent* BtAgent;
        public AIBlackboardComponent* BlackboardComponent;
    }
    public unsafe class ActorSystem : SystemMainThreadFilter<ActorFilter>, ISignalOnComponentRemoved<AIBlackboardComponent>
    {
        public override void Update(Frame f, ref ActorFilter filter)
        {
            filter.BtAgent->Update(f, filter.BtAgent, filter.BlackboardComponent, filter.EntityRef);
        }
        
        public void OnRemoved(Frame frame, EntityRef entity, AIBlackboardComponent* component)
        {
            component->FreeBlackboardComponent(frame);
        }

        public void OnActorInit(Frame f, EntityRef entity)
        {
            string btAssetName = String.Empty;
            if (f.Has<Dice>(entity))
            {
                var dice = f.Get<Dice>(entity);
                if (f.Context.TableData.DiceInfo.GetData(dice.DiceInfoId, out var diceInfo))
                {
                    btAssetName = diceInfo.botData;
                }
                else
                {
                    Log.Error($"DiceInfo가 없습니다. {dice.DiceInfoId}");
                }
            }
            else if (f.Has<Guardian>(entity))
            {
                var guardian = f.Get<Guardian>(entity);
                if (f.Context.TableData.GuardianInfo.GetData(guardian.GuardianInfoId, out var guardianInfo))
                {
                    btAssetName = guardianInfo.btAssetName;
                }
                else
                {
                    Log.Error($"GuardianInfo가 없습니다. {guardian.GuardianInfoId}");
                }
            }
            else if (f.Has<Boss>(entity))
            {
                var boss = f.Get<Boss>(entity);
                if (f.Context.TableData.GuardianInfo.GetData(boss.BossInfoId, out var bossInfo))
                {
                    btAssetName = bossInfo.btAssetName;
                }
                else
                {
                    Log.Error($"BossInfo가 없습니다. {boss.BossInfoId}");
                }
            }
            else if(f.Has<Tower>(entity))
            {
                
            }

            if (string.IsNullOrWhiteSpace(btAssetName))
            {
                return;
            }
            
            BTHelper.SetupBT(f, entity, btAssetName);
        }
    }
}