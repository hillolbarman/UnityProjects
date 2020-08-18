using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoBehaviour
{
    public Transform weaponHold;
    Gun equipedGun;
    public Gun[] allGuns;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Aim(Vector3 aimPoint)
    {
        if(equipedGun!=null)
        {
            equipedGun.Aim(aimPoint);
        } 
    }

    public void EquipGun(Gun gunToEquip)
    {
        if (equipedGun != null)
        {
            Destroy(equipedGun.gameObject);
        }
        equipedGun = Instantiate(gunToEquip, weaponHold.position, weaponHold.rotation) as Gun;
        equipedGun.transform.parent = weaponHold;
    }

    public void EquipGun(int weaponIndex)
    {
        EquipGun(allGuns[weaponIndex]);
    }

    public void OnTriggerHold()
    {
        if(equipedGun!=null)
        {
            equipedGun.OnTriggerHold();
        }
    }

    public void OnTriggerRelease()
    {
        if (equipedGun != null)
        {
            equipedGun.OnTriggerRelease();
        }
    }

    public float GunHeight
    {
        get { 
        return weaponHold.position.y;
        }
    }

    public void Reload()
    {
        if (equipedGun != null)
            equipedGun.Reload();
    }
}
