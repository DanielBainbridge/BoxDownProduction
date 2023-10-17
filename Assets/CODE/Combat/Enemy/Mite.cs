﻿using Utility;
using UnityEngine;
using Managers;
using System.Collections;

namespace Enemy
{
    public class Mite : EnemyBase
    {
        [Header("Mite Specific Variables:")]
        [Rename("Chase Behaviour")] public bool b_chasePlayer = false;
        [Rename("Look At Player")] public bool b_lookAtPlayer = false;
        [Rename("Chase Distance")] public float f_chaseDistance = 3.5f;
        [Rename("Stop Chase Distance")] public float f_stopChaseDistance = 4.5f;

        private void Start()
        {
            base.Start();
        }

        protected override IEnumerator SpawnRoutine()
        {
            b_spawning = true;
            StartCoroutine(ChangeStateForSeconds(CombatState.Invincible, 2.5f));
            if (C_spawnEffects != null)
            {
                StartCoroutine(PlaySpawnEffect(f_spawnEffectDelay));
            }
            yield return new WaitForSeconds(2.5f);
            b_spawning = false;
            if (!b_chasePlayer)
            {
                ChangeMovementDirection(new Vector2(transform.forward.x, transform.forward.z));
            }
        }

        private void Update()
        {
            base.Update();
            if (b_spawning || !PlayerLineOfSightCheck())
            {
                CancelGun();
                return;
            }
            MeleeDamage();
            if (f_distanceToPlayer < f_aimRange && b_lookAtPlayer)
            {
                LookAtPlayer();
            }
            if (!b_chasePlayer)
            {
                if (Physics.SphereCast(transform.position, f_size, transform.forward, out RaycastHit hit, f_size * 2, i_bulletLayerMask))
                {
                    if (hit.transform.GetComponent<Combatant>() != null)
                    {
                        return;
                    }
                    ReflectMovementDirection(new Vector2(hit.normal.x, hit.normal.z));
                    SetRotationDirection(Vector2.ClampMagnitude(S_movementVec2Direction,0.1f));
                }
                return;
            }
            if (f_distanceToPlayer < f_chaseDistance)
            {
                LookAtPlayer();
                ChangeMovementDirection(DirectionOfPlayer());
            }
            if (f_distanceToPlayer > f_stopChaseDistance)
            {
                ChangeMovementDirection(Vector2.zero);
            }
        }
        protected override void CheckCollisions()
        {
            if (b_chasePlayer)
            {
                base.CheckCollisions();
                return;
            }
            RaycastHit hit;
            if (Physics.SphereCast(transform.localPosition, f_size, Vector3.right, out hit, f_size, i_bulletLayerMask) && S_velocity.x > 0)
            {
                if (hit.transform.GetComponent<Combatant>() == null && !hit.collider.isTrigger)
                {
                    S_velocity.x = -S_velocity.x * f_collisionBounciness;
                }
            }
            else if (Physics.SphereCast(transform.localPosition, f_size, -Vector3.right, out hit, f_size, i_bulletLayerMask) && S_velocity.x < 0)
            {
                if (hit.transform.GetComponent<Combatant>() == null && !hit.collider.isTrigger)
                {
                    S_velocity.x = -S_velocity.x * f_collisionBounciness;
                }
            }
            if (Physics.SphereCast(transform.localPosition, f_size, Vector3.forward, out hit, f_size, i_bulletLayerMask) && S_velocity.z > 0)
            {
                if (hit.transform.GetComponent<Combatant>() == null && !hit.collider.isTrigger)
                {
                    S_velocity.z = -S_velocity.z * f_collisionBounciness;
                }
            }
            else if (Physics.SphereCast(transform.localPosition, f_size, -Vector3.forward, out hit, f_size, i_bulletLayerMask) && S_velocity.z < 0)
            {
                if (hit.transform.GetComponent<Combatant>() == null && !hit.collider.isTrigger)
                {
                    S_velocity.z = -S_velocity.z * f_collisionBounciness;
                }
            }
        }
        public override void Die()
        {
            base.Die();
            GameManager.IncrementMiteKill();
        }
    }
}
