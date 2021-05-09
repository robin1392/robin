
using System;
using Photon.Deterministic;
using Quantum.Util;
using RandomWarsResource.Data;

namespace Quantum.Actors
{
    public static partial class ActorFactory
    {
        public static unsafe void CreateTowerActor(Frame f, ActorCreationSpec spec, EntityPrototype prototype)
        {
            var rotation = GetSpawnRotation(spec.Team);
                
            var entity = f.Create(prototype);
            f.Add<Tower>(entity);

            //TODO: 주사위 획득 시 TowerHp를 증가시킨다. 게임 시작과 동시에 서버에 시작 요청을 보내고 합산된 TowerHp를 받으면 될 것 같다.
            var hp = f.Context.TableData.VsMode.KeyValues[(int) EVsmodeKey.TowerHp].value;
            var actor = f.Unsafe.GetPointer<Actor>(entity);
            
            f.Add<Hittable>(entity);
            var hittable = f.Unsafe.GetPointer<Hittable>(entity);
            actor->Owner = spec.Owner;
            actor->Team = spec.Team;
            hittable->MaxHealth = hp;
            hittable->Health = hp;

            var transform = f.Unsafe.GetPointer<Transform2D>(entity);
            transform->Position = spec.Position;
            transform->Rotation = rotation;

            f.Add<NoCC>(entity);
        }
    }
}