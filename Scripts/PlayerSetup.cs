using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
[RequireComponent(typeof(Player))]
[RequireComponent(typeof(PlayerController))]
public class PlayerSetup : NetworkBehaviour
{
    [SerializeField] Behaviour[] componentToDisable;
    [SerializeField] string remotePlayerName = "RemotePlayer";
    [SerializeField] GameObject PlayerGraphics;
    [SerializeField] string DontDrawLayerName = "DontDraw";
    [SerializeField] GameObject playerUIPrefab;
    [HideInInspector]
    public GameObject playerUIinstance;

    void Start()
    {
        if (!isLocalPlayer)
        {
            disableComponents();
            assignRemoteLayer();
        }
        else
        {
           
            SetLayerRecursively(PlayerGraphics, LayerMask.NameToLayer(DontDrawLayerName));


            GetComponent<Player>().setUp();

            //create player UI
            playerUIinstance = Instantiate(playerUIPrefab);
            playerUIinstance.transform.name = playerUIPrefab.name;
            //configure playerUI
            PlayerUI ui = playerUIinstance.GetComponent<PlayerUI>();
            if (ui == null)
            {
                Debug.Log("NO UI COMPO");
            }
            ui.setController(GetComponent<PlayerController>());
        }
    }

    void SetLayerRecursively(GameObject obj,int newLayer)
    {
        obj.layer = newLayer;

        foreach(Transform child in obj.transform)
        {
            SetLayerRecursively(child.gameObject, newLayer);
        }     

    }
   

    public override void OnStartClient()
    {
        base.OnStartClient();

        string playerID = GetComponent<NetworkIdentity>().netId.ToString();
        Player player = GetComponent<Player>();
        Game_Manager.registerPlayer(playerID,player);
    }


    void assignRemoteLayer()
    {
        gameObject.layer = LayerMask.NameToLayer(remotePlayerName);
    }

    void disableComponents()
    {
        for (int i = 0; i < componentToDisable.Length; i++)
        {
            componentToDisable[i].enabled = false;
        }
    }

    private void OnDisable()
    {
        Destroy(playerUIinstance);
        if (isLocalPlayer)
        {
            Game_Manager.instance.SetSceneCameraActive(true);
        }
        Game_Manager.UnRegisterPlayer(transform.name);
         
    }


}
