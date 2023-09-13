using System.Collections.Generic;
using UnityEngine;
using Utility;

namespace Enemy
{

    public class EnemyBase : Combatant
    {
        [Header("Base Enemy Variables")]
        [Rename("Lock Enemy Position")] public bool b_lockEnemyPosition;
        [Rename("Aim Range")] public float f_aimRange = 6;
        [Rename("Fire Range")] public float f_fireRange = 4;
        [Rename("Melee Damage")] public float f_meleeDamage = 8;
        [Rename("Melee Knockback")] public float f_meleeKnockback = 3;

        //runtime variables
        private PlayerController C_player;

        private void Awake()
        {
            base.Start();
            C_player = FindObjectOfType<PlayerController>();
            SetRotationDirection(new Vector2(transform.forward.x, transform.forward.z));
        }

        protected override void Update()
        {
            base.Update();
        }

        public void LookAtPlayer()
        {
            if (C_player != null)
            {
                Vector3 fromToPlayer = C_player.transform.position - transform.position;
                SetRotationDirection(new Vector2(fromToPlayer.x, fromToPlayer.z));
            }
        }
        public float f_distanceToPlayer
        {
            get
            {
                return (C_player.transform.position - transform.position).magnitude;
            }
        }
        public Vector2 DirectionOfPlayer()
        {
            Vector3 fromToPlayer = C_player.transform.position - transform.position;
            return new Vector2(fromToPlayer.x, fromToPlayer.z);
        }

        public void ReflectMovementDirection(Vector2 normal)
        {
            ChangeMovementDirection(Vector2.Reflect(S_movementVec2Direction, normal).normalized);
        }

        protected override void Move()
        {
            base.Move();
        }


        protected virtual void MeleeDamage()
        {
            Collider[] collisions = Physics.OverlapSphere(transform.position, f_size * 1.95f);
            PlayerController player = null;
            for (int i = 0; i < collisions.Length; i++)
            {
                if (collisions[i].transform.GetComponent<PlayerController>() != null)
                {
                    player = collisions[i].transform.GetComponent<PlayerController>();
                }
            }

            if (player != null)
            {
                if (player.e_combatState != CombatState.Invincible && player.e_combatState != CombatState.Dodge)
                {
                    player.Damage(f_meleeDamage);
                    player.AddVelocity(new Vector3(DirectionOfPlayer().x, 0, DirectionOfPlayer().y) * f_meleeKnockback);
                    return;
                }
            }
        }
    }
}