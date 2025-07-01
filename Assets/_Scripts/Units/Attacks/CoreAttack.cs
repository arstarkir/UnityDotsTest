using Unity.Netcode;
using UnityEngine;

public abstract class CoreAttack : NetworkBehaviour
{
    [SerializeField] float cooldown = 1f;
    float timer;

    public bool Ready => timer <= 0f;

    public void TryUse(Health target)
    {
        if (!IsServer || !Ready || target == null) 
            return;

        timer = cooldown;
        Attack(target);
    }

    public abstract void Attack(Health t);

    void Update() 
    { 
        timer -= Time.deltaTime;
    }
}
