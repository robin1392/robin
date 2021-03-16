using System;
using System.Collections.Generic;
using UnityEngine;

namespace MirageTest.Scripts
{
    public class Actor : MonoBehaviour
    {
        public Animator animator;
        public int Team;
        // public bool IsAlive => Stat.HP > 0;

        private void Awake()
        {
            // Stat = new Stat() {HP = 10, Attack = 1};
            animator = GetComponentInChildren<Animator>();
        }

        public bool IsEnemyOf(Actor actor)
        {
            return Team != actor.Team;
        }

        public float GetDistanceSqrMagnitudeWith(Actor actor)
        {
            return (transform.position - actor.transform.position).sqrMagnitude;
        }

        public Actor GetNearestEnemy(IEnumerable<Actor> others)
        { 
            Actor nearestEnemy = null;
            var nearestDistanceSqr = float.MaxValue;
            
            foreach (var other in others)
            {
                if (!IsEnemyOf(other))
                {
                    continue;
                }
            
                var distanceSqrMagnitude = GetDistanceSqrMagnitudeWith(other);
                if (distanceSqrMagnitude < nearestDistanceSqr)
                {
                    nearestDistanceSqr = distanceSqrMagnitude;
                    nearestEnemy = other;
                }
            }
            
            return nearestEnemy;
        }
    }
}