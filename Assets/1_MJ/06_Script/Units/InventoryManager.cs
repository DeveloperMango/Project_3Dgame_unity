using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }
    private static InventoryManager instance;
    public GameObject bigAxe; 

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            return;
        }
        DestroyImmediate(gameObject);
    }

    void Start()
    {
        Init();
    }


    private void Init()   /// 게임 시작 후 유저의 사용 가능한 무기를 읽어오는 초기화 메소드
    {
        GameObject weapon = Instantiate(bigAxe);
        Player.Instance.weaponManager.RegisterWeapon(weapon);
        Player.Instance.weaponManager.SetWeapon(weapon);
    }
}