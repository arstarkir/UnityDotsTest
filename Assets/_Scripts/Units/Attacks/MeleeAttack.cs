using UnityEngine;

public class MeleeAttack : CoreAttack
{
    [SerializeField] float damage = 15f;
    public override void Attack(Health h) => h.RequestDealDmgServerRpc(damage);
}
