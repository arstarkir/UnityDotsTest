using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(FlowFollower))]
public class UnitCombat : NetworkBehaviour
{
    [SerializeField] float spotR = 6f;
    [SerializeField] float attackR = 1.4f;
    [SerializeField] LayerMask targetMask;
    [SerializeField] CoreAttack myAttack;

    FlowFollower mover;
    Health target;

    void Awake()
    {
        mover = GetComponent<FlowFollower>();
    }

    void FixedUpdate()
    {
        if (!IsOwner) 
            return;

        if (!IsSpawned)
            return;

        if (target == null) 
            CheckForTarget();

        if (target == null) 
            return;

        float sq = (target.transform.position - transform.position).sqrMagnitude;

        if (sq > attackR * attackR && !mover.IsMoveing.Value)
        {
            Vector3 toEnemy = (target.transform.position - transform.position).normalized;
            Vector3 approach = target.transform.position - toEnemy * attackR * 0.9f;
            mover.MoveDirectRpc(approach);
        }
        else
        {
            myAttack.TryUse(target);
        }

    }

    void CheckForTarget()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, spotR, targetMask);
        foreach (Collider c in hits)
        {
            if (c.TryGetComponent(out Health h) && h.curHealth.Value > 0 && h.OwnerClientId != OwnerClientId)
            {
                target = h;
                break;
            }
        }
    }
}
