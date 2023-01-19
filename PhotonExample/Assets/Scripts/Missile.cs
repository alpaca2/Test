using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Missile : MonoBehaviourPun
{
    private GameObject owner = null;
    private Vector3 direction = Vector3.zero;
    private float speed = 10.0f;
    private float duration = 5.0f;
    private bool isShoot = false;
    


    private void Update()
    {
        if (isShoot)
        {
            this.transform.Translate(direction * speed * Time.deltaTime);
        }
    }

    public void Shoot(GameObject _owner, Vector3 _dir)
    {
        owner = _owner;
        direction = _dir;
        isShoot = true;

        if (photonView.IsMine)
            Invoke("SelfDestroy", duration);
    }

    private void SelfDestroy()
    {
        PhotonNetwork.Destroy(this.gameObject);
    }

    private void OnTriggerEnter(Collider _other)
    {
        if (!photonView.IsMine) return;

        if (owner != _other.gameObject && _other.CompareTag("Player"))
        {
            _other.GetComponent<PlayerControl>().OnDamage(1);
            SelfDestroy();
        }
    }
}
