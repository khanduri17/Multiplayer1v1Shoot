using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


[RequireComponent(typeof(WeaponManager))]
public class PlayerShoot : NetworkBehaviour
{
    private PlayerWeapon currentWeapon;
    private WeaponManager weaponManager;
   
    private string Player_Tag = "Player";


    [SerializeField] private Camera cam;
    [SerializeField] private LayerMask mask;
   
    


    private void Start()
    {
        if (cam == null)
        {
            Debug.Log("Player shoot : No camera reference");
            this.enabled = false;
        }
        weaponManager = GetComponent<WeaponManager>();
    }

    private void Update()
    {
        currentWeapon = weaponManager.GetCurrentWeapon();
        if (currentWeapon.fireRate <= 0)
        {
            if (Input.GetButtonDown("Fire1"))
            {
                Shoot();
            }
        }
        else 
        {
            if (Input.GetButtonDown("Fire1"))
            {
                InvokeRepeating("Shoot", 0f, 1f / currentWeapon.fireRate);
            }
            else if (Input.GetButtonUp("Fire1"))
            {
                CancelInvoke("Shoot");
            }
        }
        
    }

    [Command]
    void CmdOnShoot()
    {
        RpcDoShootingEffects();
    }

    [ClientRpc]
    void RpcDoShootingEffects()
    {
        weaponManager.GetCurrentGraphics().muzzleFlashl.Play();
    }

    [Command]
    void CmdOnHit(Vector3 _pos,Vector3 _normal)
    {
        RpcDoHitEffects(_pos, _normal);
    }
    [ClientRpc]
    void RpcDoHitEffects(Vector3 _pos, Vector3 _normal)
    {
        GameObject _hitEffect =Instantiate(weaponManager.GetCurrentGraphics().hiteffectPrefab,_pos,Quaternion.LookRotation(_normal));
        Destroy(_hitEffect, 2f);
    }

    [Client]
    public void Shoot()
    {
        if (!isLocalPlayer)
        {
            return;
        }
        CmdOnShoot();

        RaycastHit _hit;
        Debug.Log("SHOOT");
        if (Physics.Raycast(cam.transform.position,cam.transform.forward,out _hit, currentWeapon.range,mask))
        {
            if (_hit.collider.tag == Player_Tag)
            {
                CmdPlayerShot(_hit.collider.gameObject.name, currentWeapon.damage);
            }
            CmdOnHit(_hit.point, _hit.normal);
        }

    }
    [Command]
    void CmdPlayerShot(string _playerID,int _damage)
    {
        
        Debug.Log(_playerID + " has been shot");
        Player _player = Game_Manager.getPlayer(_playerID);
        _player.RpcTakeDamage(_damage);

    }


}
