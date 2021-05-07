using System;
using Photon.Deterministic;
using Quantum.Util;
using RandomWarsResource.Data;

namespace Quantum.Actors
{
    public static partial class ActorFactory
    {
        public static unsafe void CreateDiceActor(Frame f, ActorCreationSpec spec, EntityPrototype prototype)
        {
            var diceInfos = f.Context.TableData.DiceInfo;
            if (diceInfos.GetData(spec.DataId, out var diceInfo) == false)
            {
                Log.Error(
                    $"다이스정보 {spec.DataId}가 없습니다. {spec.Owner} ");
                return;
            }

            var diceScale = spec.DiceScale;
            var spawnCount = diceInfo.spawnMultiply;
            if (diceInfo.castType == (int) DiceType.Minion)
            {
                spawnCount *= diceScale + 1;
            }

            Stat stat = new Stat();
            if ((DiceType) diceInfo.castType == DiceType.Minion)
            {
                stat = CalcMinionStat(diceInfo, spec.IngameLevel, spec.OutgameLevel);
            }
            else if ((DiceType) diceInfo.castType == DiceType.Hero)
            {
                stat = CalcHeroStat(diceInfo, spec.IngameLevel, spec.OutgameLevel, diceScale);
            }
            else if ((DiceType) diceInfo.castType == DiceType.Installation ||
                     (DiceType) diceInfo.castType == DiceType.Magic)
            {
                stat = CalcMagicOrInstallationStat(diceInfo, spec.IngameLevel, spec.OutgameLevel,
                    diceScale);
            }

            for (int i = 0; i < spawnCount; ++i)
            {
                var spawnPosition = spec.Position;
                //스폰 카운트에 따라 균일한 포지션을 같는 방법을 고려해본다. 예) 1일때는 스폰트랜스폼 포지션, 2개일때는 스폰트랜스폼 포지션을 기준으로 좌우 10센티미터
                if ((DiceType) diceInfo.castType == DiceType.Minion ||
                    (DiceType) diceInfo.castType == DiceType.Hero)
                {
                    spawnPosition.X += f.RNG->Next(-FP._0_20, FP._0_20);
                    spawnPosition.Y += f.RNG->Next(-FP._0_20, FP._0_20);
                }

                //TODO: 데이터 시트에 스폰포지션 컬럼을 추가한다.
                //지뢰 
                else if (diceInfo.id == 1005 || diceInfo.id == 1013)
                {
                    spawnPosition = GetRandomPlayerFieldPosition(f);
                }

                var rotation = GetSpawnRotation(spec.Team);
                
                var entity = f.Create(prototype);
                
                var dice = f.Unsafe.GetPointer<Dice>(entity);
                dice->DiceInfoId = spec.DataId;
                dice->DiceScale = spec.DiceScale;
                dice->IngameUpgradeLevel = spec.IngameLevel;
                dice->OutgameUpgradeLevel = spec.OutgameLevel;
                dice->FieldIndex = spec.FieldIndex;

                var actor = f.Unsafe.GetPointer<Actor>(entity);
                actor->Owner = spec.Owner;
                actor->Team = spec.Team;
                actor->Power = stat.power;
                actor->MaxHealth = stat.maxHealth;
                actor->Health = stat.maxHealth;
                actor->Effect = stat.effect;
                actor->EffectDurationTime = stat.effectDurationTime;
                actor->AttackSpeed = diceInfo.attackSpeed / f.Global->SuddenDeathAttackSpeedFactor;
                actor->MoveSpeed = diceInfo.moveSpeed * f.Global->SuddenDeathMoveSpeedFactor;

                var transform = f.Unsafe.GetPointer<Transform2D>(entity);
                transform->Position = spawnPosition;
                transform->Rotation = rotation;
                
                f.Events.ActionChanged(entity, ActionStateType.Idle);

                BTHelper.SetupBT(f, entity, diceInfo.btAssetName);
                var agent = f.Unsafe.GetPointer<NavMeshSteeringAgent>(entity);
                agent->Acceleration = actor->MoveSpeed;
                agent->MaxSpeed = actor->MoveSpeed;
            }
        }
        
        public static FP GetSpawnRotation(Int32 team)
        {
            if (team == GameConstants.BottomCamp)
            {
                return FP._0;
            }

            return FP.Rad_180 * FP.Rad2Deg;
        }

        private static unsafe FPVector2 GetRandomPlayerFieldPosition(Frame f)
        {
            var x = f.RNG->Next(-FP._3, FP._3);
            var z = f.RNG->Next(-FP._2, FP._2);
            return new FPVector2(x, z);
        }

        public static Stat CalcMinionStat(TDataDiceInfo diceInfo, Int32 inGameLevel, Int32 outGameLevel)
        {
            var power = diceInfo.power
                        + (diceInfo.powerUpgrade * outGameLevel)
                        + (diceInfo.powerInGameUp * inGameLevel);
            var maxHealth = diceInfo.maxHealth + (diceInfo.maxHpUpgrade * outGameLevel) +
                            (diceInfo.maxHpInGameUp * inGameLevel);
            var effect = diceInfo.effect + (diceInfo.effectUpgrade * outGameLevel) +
                         (diceInfo.effectInGameUp * inGameLevel);
            var effectDurationTime = diceInfo.effectDurationTime + (diceInfo.effectDurationTimeUpgrade * outGameLevel) +
                                     (diceInfo.effectDurationTimeIngameUp * inGameLevel);

            return new Stat()
            {
                power = power,
                maxHealth = maxHealth,
                effect = effect,
                effectDurationTime = effectDurationTime,
            };
        }

        public static Stat CalcHeroStat(TDataDiceInfo diceInfo, Int32 inGameLevel, Int32 outGameLevel, Int32 diceScale)
        {
            var stat = CalcMinionStat(diceInfo, inGameLevel, outGameLevel);
            return new Stat()
            {
                power = stat.power * (diceScale + 1),
                maxHealth = stat.maxHealth * (diceScale + 1),
                effect = stat.effect * (diceScale + 1),
                effectDurationTime = stat.effectDurationTime
            };
        }

        public static Stat CalcMagicOrInstallationStat(TDataDiceInfo diceInfo, Int32 inGameLevel, Int32 outGameLevel,
            Int32 diceScale)
        {
            var stat = CalcMinionStat(diceInfo, inGameLevel, outGameLevel);

            return new Stat()
            {
                power = stat.power * MathUtil.Pow(FP._1_50, diceScale - 1),
                maxHealth = stat.maxHealth * MathUtil.Pow(FP._2, diceScale - 1),
                effect = stat.effect * MathUtil.Pow(FP._1_50, diceScale - 1),
                effectDurationTime = stat.effectDurationTime,
            };
        }

        public struct Stat
        {
            public FP power;
            public FP maxHealth;
            public FP effect;
            public FP effectDurationTime;
        }
    }
}