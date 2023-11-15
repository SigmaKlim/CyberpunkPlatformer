using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using Unity.VisualScripting;

public class GameManager : MonoBehaviourPunCallbacks
{
    public Text LogTextOutput;
    public GameObject PlayerPrefab;
    public GameObject PlayerOrigin;
    //public GameObject DeathScreen;
    //public GameObject VictoryScreen;

    // Start is called before the first frame update
    void Start()
    {
        if (PhotonNetwork.IsConnected == false)
            SceneManager.LoadScene("Lobby");
        Vector2 playerOrigin = PlayerOrigin.transform.position;
        Vector2 playerPosition = playerOrigin + new Vector2(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f));
        GameObject newPlayerObject = PhotonNetwork.Instantiate(PlayerPrefab.name, playerPosition, Quaternion.identity);

        string message = "You were spawned at " + playerPosition;
        //Log(message);
        //DeathScreen.SetActive(false);
        //VictoryScreen.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public override void OnLeftRoom() //called when local client left the room
    {
        SceneManager.LoadScene("Lobby");
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        string message = "Player " + newPlayer.NickName + " entered the room. ";
        Log(message);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        string message = "Player " + otherPlayer.NickName + " left the room. ";
        Log(message);
    }

    public void Leave()
    {
        PhotonNetwork.LeaveRoom();
    }

    public void Log(string message)
    {
        Debug.Log(message);
        LogTextOutput.text = "\n" + message;
    }

    //public void ProcessPlayerDeath()
    //{
    //    GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
    //    foreach (GameObject player in players)
    //    {
    //        if (player.GetComponent<PhotonView>().IsMine)
    //        {
    //            if (player.GetComponent<PlayerController>().Health == 0)
    //            {
    //                DeathScreen.SetActive(true);
    //            }
    //            else
    //                VictoryScreen.SetActive(true);
    //        }
    //    }
    //}
}
