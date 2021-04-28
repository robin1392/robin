using UnityEngine;

namespace ED
{
    public static class AnimationHash
    {
        public static readonly int MoveSpeed = Animator.StringToHash("MoveSpeed");
        public static readonly int Shoot = Animator.StringToHash("Shoot");
        public static readonly int Set = Animator.StringToHash("Set");
        public static readonly int Break = Animator.StringToHash("Break");
        public static readonly int Idle = Animator.StringToHash("Idle");
        public static readonly int Incubation = Animator.StringToHash("Incubation");
        public static readonly int AttackReady = Animator.StringToHash("AttackReady");
        public static readonly int Skill = Animator.StringToHash("Skill");
        public static readonly int Attack = Animator.StringToHash("Attack");
        public static readonly int Attack1 = Animator.StringToHash("Attack1");
        public static readonly int Attack2 = Animator.StringToHash("Attack2");
        public static readonly int SkillLoop = Animator.StringToHash("SkillLoop");
    }
}