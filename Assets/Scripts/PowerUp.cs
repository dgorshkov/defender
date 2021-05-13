using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour
{

    [SerializeField] private int healthEffect = 30;
    [SerializeField] private int secondaryAttackEffect = 0;


    public int GetHealthEffect()
    {
        return healthEffect;
    }

    public int GetSecondaryAttackEffect()
    {
        return secondaryAttackEffect;
    }

    public void Remove()
    {
        Destroy(gameObject);
    }


}
