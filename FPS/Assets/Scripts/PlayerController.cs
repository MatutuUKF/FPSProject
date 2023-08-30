using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using Photon.Realtime;
using UnityEngine.UI;

public class PlayerController : MonoBehaviourPunCallbacks, IDamagable
{
    [SerializeField] Image healthBarImage;
    [SerializeField] GameObject ui;
    [SerializeField] GameObject cameraHolder;
    [SerializeField] CharacterController characterController;
    [SerializeField] float mouseSensitivity, sprintSpeed, walkSpeed, jumpForce, smoothTime, gravityStrength;
    [SerializeField] Item[] items;

    int itemIndex;
    int previousItemIndex = -1;

    float verticalLookRotation;
    [SerializeField] bool grounded;
    private Vector3 moveDumpVelocity, currentMoveVelocity, currentForceVelocity;



    Rigidbody rb;

    PhotonView pv;

    const float maxHealth = 100f;
    float currentHealth = maxHealth;
 
    PlayerManager playerManager;
    
    private void Awake()
    {

        characterController = GetComponent<CharacterController>();
        pv = GetComponent<PhotonView>();

        playerManager = PhotonView.Find((int)pv.InstantiationData[0]).GetComponent<PlayerManager>();
    }

    private void Start()
    {
        if (pv.IsMine)
        {

            EquipItem(0);

        }
        else
        {
            Destroy(GetComponentInChildren<Camera>().gameObject);
            Destroy(rb);
            Destroy(ui);
        }
    }

    private void Update()
    {
        if (!pv.IsMine)
            return;
        Look();
        Move();
        ItemSwap();

        if (Input.GetMouseButtonDown(0)) {

            items[itemIndex].Use();
        
        }

        if (transform.position.y < -10f) {

            Die();

        }

    }


    void ItemSwap() {

        for (int i = 0; i < items.Length; i++) {

            if (Input.GetKeyDown((i + 1).ToString()))
            {
                EquipItem(i);
                break;
            }
        
        }

        

        if (Input.GetAxisRaw("Mouse ScrollWheel") > 0f)
        {

            if (itemIndex >= items.Length - 1)
            {
                EquipItem(0);
            }
            else
            {
                EquipItem(itemIndex + 1);
            }

        }
        else if (Input.GetAxisRaw("Mouse ScrollWheel") < 0f) {

            if (itemIndex <= 0)
            {
                EquipItem(items.Length - 1);

            }
            else
            {
                EquipItem(itemIndex - 1);
            }
        
        }

        
    
    }
    void Look() {

        transform.Rotate(Vector3.up, Input.GetAxis("Mouse X"));

        verticalLookRotation += Input.GetAxisRaw("Mouse Y") + mouseSensitivity;
        verticalLookRotation = Mathf.Clamp(verticalLookRotation, -90f, 90f);

        cameraHolder.transform.localEulerAngles = Vector3.left * verticalLookRotation;

    }

    void Move()
    {

        if (!pv.IsMine)
            return;

        Vector3 playerInput = new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical"));

        if (playerInput.magnitude > 1f)
        {
            playerInput.Normalize();
        }

        Vector3 moveVector = transform.TransformDirection(playerInput);
        float currentSpeed = Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : walkSpeed;

        currentMoveVelocity = Vector3.SmoothDamp(
            currentMoveVelocity,
            moveVector * currentSpeed,
            ref moveDumpVelocity,
            smoothTime
         );

        // Jumping
        if (characterController.isGrounded)
        {
            currentForceVelocity.y = -gravityStrength; // Reset vertical velocity when grounded

            if (Input.GetButtonDown("Jump"))
            {
                currentForceVelocity.y = jumpForce;
            }
        }
        else
        {
            // Apply gravity
            currentForceVelocity.y -= gravityStrength * Time.deltaTime;
        }

        // Apply movement and force velocities
        characterController.Move((currentMoveVelocity + currentForceVelocity) * Time.deltaTime);
    }

    public void SetGroundedState(bool _grounded) {

        grounded = _grounded;
    }

    void EquipItem(int _index) {

        if (_index == previousItemIndex)
            return;
        itemIndex = _index;

        items[itemIndex].itemGameObject.SetActive(true);

        if (previousItemIndex != -1)
        {
            items[previousItemIndex].itemGameObject.SetActive(false);

        }

        previousItemIndex = itemIndex;


        if (pv.IsMine) {

            Hashtable hash = new Hashtable();
            hash.Add("itemIndex", itemIndex);
            PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
        }
    }
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if (changedProps.ContainsKey("itemIndex") && !pv.IsMine && targetPlayer == pv.Owner) {

            EquipItem((int)changedProps["itemIndex"]);

        }
    }

    public void TakeDamage(float damage) {

        pv.RPC(nameof(RPC_TakeDamage), RpcTarget.All, damage);
    }

    [PunRPC]
    void RPC_TakeDamage(float damage, PhotonMessageInfo info) {

        if (!pv.IsMine)
            return;

        currentHealth -= damage;

        healthBarImage.fillAmount = currentHealth / maxHealth;

        if (currentHealth <= 0)
        {
            Die();
            PlayerManager.Find(info.Sender).GetKill();
        }
    }

    void Die() {

        playerManager.Die();
    
    }
}
