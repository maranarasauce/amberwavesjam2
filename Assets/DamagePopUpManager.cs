using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamagePopUpManager : MonoBehaviour
{
    public static DamagePopUpManager instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
        {
            Destroy(gameObject);
        }
    }


    [SerializeField]
    private GameObject damagePopUpPrefab;
    public GameObject PopUpParent;
    public GameObject Damageable;
    private IDamageable dam;


    public void Start()
    {
        dam = Damageable.GetComponent<IDamageable>();
        Damageable.GetComponent<IDamageable>().OnDamage += DisplayDamagePopUp;
    }

    public void DisplayDamagePopUp()
    {
        GameObject GO = Instantiate(damagePopUpPrefab, PopUpParent.transform.position, Quaternion.identity, PopUpParent.transform);
        GO.GetComponent<DamagePopup>().SetUp(dam.LastDamageValue);
    }
}
