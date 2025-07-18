using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(FlowFollower))]
public class UnitCombat : NetworkBehaviour
{
    [HideInInspector] public Health target;
    [SerializeField] LayerMask targetMask;
    [SerializeField] CoreAttack attack;

    [HideInInspector] public FlowFollower mover;
    float spotR = 0;
    
    private void Awake()
    {
        mover = GetComponent<FlowFollower>();
        spotR = attack.spotR;
    }

    void FixedUpdate()
    {
        if (!IsServer || !IsSpawned)
            return;

        if (target == null)
            CheckForTarget();

        if (target == null)
            return;

        if(!attack.Pursuit(this,target.transform))
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
