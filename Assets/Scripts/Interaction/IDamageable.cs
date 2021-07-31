using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Events;

public interface IDamageable
{
    float Health { get; }

    event Action OnDamage;
    event Action OnKill;

    void DoDamage(float damage);
    void Kill();
}
