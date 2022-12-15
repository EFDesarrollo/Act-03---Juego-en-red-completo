using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUD_Controller : MonoBehaviour
{
    public Image HealthBarFront;
    public HUD_Controller instance;
    public GameObject PlayerListPanel, team1content, tem2content, linePrefab;

    public List<GameObject> playersInList = new List<GameObject>();
    public List<string> gamename = new List<string>();
    public List<float> gamehealth = new List<float>();

    private void Start()
    {
        instance = this;
    }
    void Update()
    {
        PrintPlayerList();
    }
    public void UpdateHealth(float lifePoints)
    {
        HealthBarFront.fillAmount = lifePoints/100;
    }
    void PrintPlayerList()
    {
        if (Input.GetKey(KeyCode.Tab))
        {
            PlayerListPanel.SetActive(true);
            AddPlayerToList();
        }
        else
            PlayerListPanel.SetActive(false);
    }

    void AddPlayerToList()
    {
        playersInList.ForEach(x => Destroy(x));
        playersInList.Clear();
        gamename.Clear();
        gamehealth.Clear();
        Debug.Log("AddPlayerToList: " + PhotonNetwork.PlayerList.Length);
        foreach (Player jugador in PhotonNetwork.PlayerList)
        {
            // int.Parse(jugador.CustomProperties["health"].ToString())
            object health_instance = jugador.CustomProperties["health"];
            int equip = (int)jugador.CustomProperties["equipo"];
            GameObject obj = Instantiate(linePrefab, equip == 1 ? team1content.transform : tem2content.transform);
            obj.GetComponentInChildren<TextMeshProUGUI>().text = jugador.NickName + " -----> " + (int)health_instance;
            playersInList.Add(obj);

            
        }
    }
}
