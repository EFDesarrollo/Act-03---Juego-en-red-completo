using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviourPunCallbacks
{
    Rigidbody rig;
    Animator anim;
    public float speed; //Velocidad máxima del movimiento
    public float rotationSpeed; //Velocidad de giro del ratón
    public GameObject bala; //Bala del arma
    public GameObject posCanyon; //Posicion del cañon del arma
    private HUD_Controller _Hud_Controller;

    public int health = 100;
    private ExitGames.Client.Photon.Hashtable playerProperties;

    // Start is called before the first frame update
    void Start()
    {
        rig = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        _Hud_Controller = gameObject.GetComponentInChildren<HUD_Controller>();
    }

    // Update is called once per frame
    void Update()
    {
        if (photonView.IsMine)
        {
            //Si se pulsa teclas Horizontal o Vertical, movemos el personaje
            Move();

            if (Input.GetButtonDown("Fire1"))
            {
                Debug.Log("Disparo");
                photonView.RPC("Shoot", RpcTarget.All);
                //Shoot();
            }
        }
        if (_Hud_Controller != null)
            _Hud_Controller.instance.UpdateHealth(health);
        if (PhotonNetwork.CurrentRoom.PlayerCount <= 1)
        {
            SceneManager.LoadScene(3);
        }
    }

    [PunRPC]
    private void Shoot()
    {
        GameObject miBala=  Instantiate(bala, posCanyon.transform.position, Quaternion.identity);
        miBala.transform.forward = posCanyon.transform.forward;
            

    }

    private void Move()
    {
        float movZ = Input.GetAxis("Vertical");
        float movX = Input.GetAxis("Horizontal");

        Vector3 velocidad = transform.forward * movZ * speed 
            + transform.right * movX * speed 
            + transform.up * rig.velocity.y;

        rig.velocity = velocidad;

 

        transform.Rotate(transform.up * Input.GetAxis("Mouse X") 
            * rotationSpeed);

        velocidad = new Vector3(velocidad.x, 0, velocidad.z);

        anim.SetFloat("velocity", velocidad.magnitude);




    }
    [PunRPC]
    public void TakeDamage(int damage)
    {
        
        health += damage;
        if (health <= 0)
        {
            health = 0;
            PhotonNetwork.AutomaticallySyncScene=false;
            Destroy(gameObject);
            if (photonView.IsMine)
            {
                PhotonNetwork.LeaveRoom();
                SceneManager.LoadScene(2);
            }
        }
        if (health >= 100)
        {
            health = 100;
        }
        if (photonView.IsMine)
        {
            playerProperties = PhotonNetwork.LocalPlayer.CustomProperties;
            playerProperties["health"] = health;
            PhotonNetwork.LocalPlayer.SetCustomProperties(playerProperties);
        }
        Debug.Log("TakeDamage" + gameObject.name);
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Bullet")
        {
            TakeDamage(-other.gameObject.GetComponent<BulletController>().damage);
        }
    }

}
