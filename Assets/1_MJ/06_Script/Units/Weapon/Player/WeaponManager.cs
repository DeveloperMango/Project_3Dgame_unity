using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CharacterController;

public class WeaponManager
{

    public BaseWeapon Weapon { get; private set; }
    public Action<GameObject> unRegisterWeapon { get; set; }
    private Transform handPosition;
    [SerializeField]
    private GameObject weaponObject;
    private List<GameObject> weapons = new List<GameObject>();

    public WeaponManager(Transform hand)
    {
        handPosition = hand;
    }

    // 무기 등록
    public void RegisterWeapon(GameObject weapon)
    {
        if (!weapons.Contains(weapon))
        {
            BaseWeapon weaponInfo = weapon.GetComponent<BaseWeapon>();
            weapon.transform.SetParent(handPosition);
            weapon.transform.localPosition = weaponInfo.HandleData.localPosition;
            weapon.transform.localEulerAngles = weaponInfo.HandleData.localRotation;
            weapon.transform.localScale = weaponInfo.HandleData.localScale;
            weapons.Add(weapon);
            weapon.SetActive(false);
        }
    }

    // 무기 삭제
    public void UnRegisterWeapon(GameObject weapon)
    {
        if (weapons.Contains(weapon))
        {
            weapons.Remove(weapon);
            unRegisterWeapon.Invoke(weapon);
        }
    }

    // 무기 변경
    public void SetWeapon(GameObject weapon)
    {
        BaseWeapon weaponInfo = weapon.GetComponent<BaseWeapon>();

        if (!Player.Instance._PlayerAnimationEvents.myWeaponEffects.ContainsKey(weaponInfo.Name))
        {
            Player.Instance._PlayerAnimationEvents.myWeaponEffects.Add(weaponInfo.Name, weapon.GetComponent<IEffect>());
        }

        if (Weapon == null)
        {
            weaponObject = weapon;
            Weapon = weapon.GetComponent<BaseWeapon>();
            weaponObject.SetActive(true);
            Player.Instance.animator.runtimeAnimatorController = Weapon.WeaponAnimator;
            return;
        }

        for (int i = 0; i < weapons.Count; i++)
        {
            if (weapons[i].Equals(Weapon))
            {
                weaponObject = weapon;
                weaponObject.SetActive(true);
                Weapon = weapon.GetComponent<BaseWeapon>();
                Player.Instance.animator.runtimeAnimatorController = Weapon.WeaponAnimator;
                continue;
            }
            weapons[i].SetActive(false);
        }
    }

}