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

    public void DisplayDamagePopUp(int amount, Transform PopUpParent)
    {
        GameObject GO = Instantiate(damagePopUpPrefab, PopUpParent.transform.position, Quaternion.identity, PopUpParent);
        GO.GetComponent<DamagePopup>().SetUp(amount);
    }
}
