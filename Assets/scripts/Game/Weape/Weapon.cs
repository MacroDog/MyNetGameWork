using UnityEngine;
using System.Collections;

public class Weapon : MonoBehaviour {

    public float Damage;
    public float damage
    {
        get
        {
            return Damage;
        }
        protected set
        {
            Damage = value;
        }
    }


    /// <summary>
    /// 武器类攻击
    /// </summary>
    protected virtual void Attack()
    {

    }
     
}
