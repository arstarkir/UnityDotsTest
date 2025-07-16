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
        if (!IsServer || !IsSpawned)
            return;

        if (target == null)
            CheckForTarget();

        if (target == null)
            return;

        float sq = (target.transform.position - transform.position).sqrMagnitude;
        float dist = Vector3.Distance(transform.position, target.transform.position);
        if (sq > attackR * attackR && !mover.IsMoveing.Value)
        {
            Vector3 toEnemy = (target.transform.position - transform.position).normalized;
            Vector3 approach = target.transform.position - toEnemy * attackR * 0.9f;
            mover.RequestMoveDirectServerRpc(approach);
        }
        else
        if (dist < attackR)
        {
            myAttack.TryUse(target);
        }
        else
            target = null;
    }

    void CheckForTarget()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, spotR, targetMask);
        foreach (Collider c in hits)
        {
            if (c.TryGetComponent(out Health h) && h.curHealth.Value > 0 && h.OwnerClientId != OwnerClientId || c.gameObject.name.Contains("PunchingBag"))
            {
                target = h;
                break;
            }
        }
    }
}
