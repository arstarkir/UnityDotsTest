using Unity.Netcode;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public abstract class CoreAttack : NetworkBehaviour
{
    public float spotR = 6f;
    public float attackR = 1.4f;

    [SerializeField] float cooldown = 1f;
    float timer;

    public bool Ready => timer <= 0f;

    public void TryUse(Transform target)
    {
        if (!IsServer || !Ready || target == null) 
            return;

        timer = cooldown;
        Attack(target);
    }

    public abstract void Attack(Transform t);

    public abstract bool Pursuit(UnitCombat unitCombat, Transform target);

    void Update() 
    { 
        timer -= Time.deltaTime;
    }
}
