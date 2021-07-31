using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public interface IDamageable
{
    float Health { get; }

    void DoDamage(float damage);
    void Kill();
}
