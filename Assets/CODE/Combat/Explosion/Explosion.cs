using Enemy;
using Gun;
using System.Collections.Generic;
using UnityEngine;
using Utility;

namespace Explosion
{

    public class Explosion : MonoBehaviour
    {
        public GameObject C_prefab;
        public GunModule C_gunModuleCreator;
        public float f_explosionSize;
        public float f_explosionDamage;
        public float f_explosionKnockbackStrength;
        public float f_explosionLifeTime;
        private float f_lifeTime;
        List<Transform> lC_alreadyCollided = new List<Transform>();

        public void InitialiseExplosion()
        {
            f_lifeTime = 0;
        }

        private void Update()
        {
            if (LifeTimeCheck())
            {
                Destroy(gameObject);
            }
            CheckCollisions();
            transform.localScale = Vector3.Lerp(Vector3.zero, new Vector3(f_explosionSize, f_explosionSize, f_explosionSize), f_lifeTime / f_explosionLifeTime);
            f_lifeTime += Time.deltaTime;
        }

        private void CheckCollisions()
        {
            Collider[] collisions = Physics.OverlapSphere(transform.position, f_explosionSize);
            if (collisions.Length == 0)
            {
                return;
            }
            ResolveCollisions(collisions);
        }

        private void ResolveCollisions(Collider[] collisions)
        {
            for (int i = 0; i < collisions.Length; i++)
            {
                if (lC_alreadyCollided.Contains(collisions[i].transform))
                {
                    continue;
                }

                lC_alreadyCollided.Add(collisions[i].transform);
                Combatant combatant = collisions[i].transform.GetComponent<Combatant>();


                Vector3 hitDirection = collisions[i].transform.position - transform.position;
                hitDirection = new Vector3(hitDirection.x, 0, hitDirection.z);
                float notCollisionDepth = Vector3.ClampMagnitude(hitDirection, 1.0f).magnitude;
                notCollisionDepth = 1 - notCollisionDepth;

                if (combatant == null && collisions[i].gameObject.layer != LayerMask.GetMask("Explosive"))
                {
                    continue;
                }
                else if (combatant != null)
                {
                    combatant.Damage(f_explosionDamage);
                    combatant.AddVelocity(hitDirection * (f_explosionKnockbackStrength * notCollisionDepth));
                    continue;
                }
            }
        }

        private bool LifeTimeCheck()
        {
            if (f_lifeTime < f_explosionLifeTime)
            {
                return false;
            }
            return true;
        }

        private void ApplyElementDamage()
        {

        }

    }

}