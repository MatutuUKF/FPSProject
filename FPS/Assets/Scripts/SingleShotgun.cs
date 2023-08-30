using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleShotgun : Gun
{
    [SerializeField] Camera cam;

    public PhotonView pv;

    private void Awake()
    {
        pv = GetComponent<PhotonView>();
    }
    public override void Use()
    {
        Shoot();
    }

    public void Shoot() {

        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f));
        ray.origin = cam.transform.position;
        if (Physics.Raycast(ray, out RaycastHit hit))
        {

            hit.collider.gameObject.GetComponent<IDamagable>()?.TakeDamage(((GunInfo)itemInfo).damage);
            pv.RPC("RPC_Shoot", RpcTarget.All, hit.point,hit.normal);
        }
    
    }


    [PunRPC]
    void RPC_Shoot(Vector3 hitPosition, Vector3 hitNomral) {

        Collider[] colliders = Physics.OverlapSphere(hitPosition,0.3f);
        if(colliders.Length != 0)
        {

            GameObject bulletImpactObj = Instantiate(bulletImpactPrefab, hitPosition + hitNomral * 0.001f, Quaternion.LookRotation(hitNomral, Vector3.up) * bulletImpactPrefab.transform.rotation);
            Destroy(bulletImpactObj, 10);
            bulletImpactObj.transform.SetParent(colliders[0].transform);

        }
    
    }
}
