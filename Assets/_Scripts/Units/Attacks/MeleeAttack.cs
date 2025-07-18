using UnityEngine;

public class MeleeAttack : CoreAttack
{
    [SerializeField] float damage = 15f;

    public override void Attack(Transform target)
    {
        target.GetComponent<Health>().RequestChangeHealthServerRpc(-damage);
    }

    public override bool Pursuit(UnitCombat unitCombat, Transform target)
    {
        float sq = (target.transform.position - transform.position).sqrMagnitude;
        float dist = Vector3.Distance(transform.position, target.transform.position);
        if (sq > attackR * attackR && !unitCombat.mover.IsMoveing.Value)
        {
            Vector3 toEnemy = (target.transform.position - transform.position).normalized;
            Vector3 approach = target.transform.position - toEnemy * attackR * 0.9f;
            unitCombat.mover.RequestMoveDirectServerRpc(approach);
        }
        else
        if (dist < attackR)
        {
            TryUse(target);
        }
        else
            return false;
        return true;
    }
}
