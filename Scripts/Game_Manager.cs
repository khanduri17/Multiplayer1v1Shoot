using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game_Manager : MonoBehaviour
{

    public static Game_Manager instance;
    public MatchSettings matchSettings;
    [SerializeField] GameObject sceneCamera;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.Log("MORE GAMEMANAGER INSTANCE");
        }
        else
        {
            instance = this;
        }
    }
    public void SetSceneCameraActive(bool isactive)
    {
        if (sceneCamera == null) { return; }

        sceneCamera.SetActive(isactive);

    }

    #region Player Tracking
    public const string PLAYER_PREFIX = "Player";
    public static Dictionary<string,Player> players = new Dictionary<string , Player>();

    public static void registerPlayer(string netID , Player _player )        
    {
        string _playerID = PLAYER_PREFIX + netID;
        players.Add(_playerID, _player);
        _player.transform.name = _playerID;
    }
    public static void UnRegisterPlayer(string _playerID)
    {
        players.Remove(_playerID);
       
    }

    public static Player getPlayer(string _playerID)
    {
        return players[_playerID];
    }

    private void OnGUI()
    {
        GUILayout.BeginArea(new Rect(200, 200, 200, 500));
        GUILayout.BeginVertical();
        foreach (string _playerID in players.Keys)
        {
            GUILayout.Label(_playerID + "-"+ players[_playerID].transform.name); 
        }
       
        GUILayout.EndVertical();
        GUILayout.EndArea();

    }

    #endregion

}
