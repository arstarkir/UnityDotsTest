using UnityEngine;

public class JumpscareAttack : CoreAttack
{
    [SerializeField] float damage = 15f;
    [SerializeField] float jumpSpeed = 15;
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
            Vector3 approach = target.transform.position + toEnemy * Vector3.Distance(target.transform.position, transform.position);
            unitCombat.mover.RequestJumpServerRpc(approach, jumpSpeed);
        }
        else
        if (dist < attackR)
        {
            Debug.Log("A");
            TryUse(target);
        }
        else
            return false;
        return true;
    }
}
