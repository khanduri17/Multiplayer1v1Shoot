using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
[RequireComponent(typeof(PlayerSetup))]
public class Player : NetworkBehaviour
{

    private bool FirstSetup = true;
    [SyncVar] private bool _isDead = false;
    public bool isDead
    {
        get { return _isDead; }
        protected set { _isDead = value; }
    }

    [SerializeField] GameObject deathEffects;
    [SerializeField] GameObject spawnEffects;
    [SerializeField] private int maxDamage = 100;
    [SyncVar] private int currentHealth;
    [SerializeField] Behaviour[] disableOnDeath;
    [SerializeField] GameObject[] disableGameObjectsOnDeath; 
    private bool[] wasEnabled;
   

    public void setUp()
    {
        if (isLocalPlayer)
        {
            Game_Manager.instance.SetSceneCameraActive(false);
            if (GetComponent<PlayerSetup>().playerUIinstance == null) { }
            else
            {
                GetComponent<PlayerSetup>().playerUIinstance.SetActive(true);
            }
        }
        CmdBroadcastNewPlayerSetup();
    }
    [Command]
    public void CmdBroadcastNewPlayerSetup()
    {
        RpcSetupPlayerOnAllClient();
    }
    [ClientRpc]
    public void RpcSetupPlayerOnAllClient()
    {
        if (FirstSetup)
        {
            wasEnabled = new bool[disableOnDeath.Length];

            for (int i = 0; i < wasEnabled.Length; i++)
            {
                wasEnabled[i] = disableOnDeath[i].enabled;
            }
            FirstSetup = false;
        }

        setDefault();
    }


    private void setDefault()
    {

        isDead = false;
        currentHealth = maxDamage;
        for (int i = 0; i < disableOnDeath.Length; i++)
        {
            disableOnDeath[i].enabled = wasEnabled[i];
        }
        //enable GameObjects
        for (int i = 0; i < disableGameObjectsOnDeath.Length; i++)
        {
            disableGameObjectsOnDeath[i].SetActive(true);
        }

        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            col.enabled = true;
        }
       
        GameObject _spawnfx = Instantiate(spawnEffects,transform.position,Quaternion.identity);
        Destroy(_spawnfx, 3f);

    }
    private void Update()
    {

        if (!isLocalPlayer) { return; };
        if (Input.GetKeyDown(KeyCode.K))
        {
            RpcTakeDamage(99999);
        }
    }

    [ClientRpc]
    public void RpcTakeDamage(int amount)
    {
        if (isDead) { return; }
        currentHealth -= amount;
        Debug.Log(transform.name + "has now" + currentHealth);
        if (currentHealth <= 0)
        {
            Die();
        }

    }

    private void Die()
    {
        isDead = true;
        //disable components
        for (int i = 0; i < disableOnDeath.Length; i++)
        {
            disableOnDeath[i].enabled = false;
        }
        //disable GameObjects
        for (int i = 0; i < disableGameObjectsOnDeath.Length; i++)
        {
            disableGameObjectsOnDeath[i].SetActive(false);
        }
        //disable collider
        Collider col = GetComponent<Collider>();        
        if (col != null)
        {
            col.enabled = false;
        }
        //spawn deatheffects
        GameObject _deathgfxIns = Instantiate(deathEffects, transform.position, Quaternion.identity);
        Destroy(_deathgfxIns, 3f);

        if (isLocalPlayer)
        {
            Game_Manager.instance.SetSceneCameraActive(true);
            GetComponent<PlayerSetup>().playerUIinstance.SetActive(false);
        }


        Debug.Log(transform.name+"isdead");
        StartCoroutine(reSpawn());

    }
    IEnumerator reSpawn()
    {
        yield return new WaitForSeconds(Game_Manager.instance.matchSettings.respawnTime);
       
        Transform spawnPoint = NetworkManager.singleton.GetStartPosition();
        transform.position = spawnPoint.position;
        transform.rotation = spawnPoint.rotation;

        /* Game_Manager.instance.SetSceneCameraActive(false);
         if (GetComponent<PlayerSetup>().playerUIinstance == null) { }
         else
         {
             GetComponent<PlayerSetup>().playerUIinstance.SetActive(true);
         }*/

       yield return new WaitForSeconds(0.1f);

       setUp();


    }


}
