using UnityEngine;
using UnityEngine.UI;

public class SimpleDynamicCrosshair : MonoBehaviour
{
    private RectTransform reticle; // The RectTransform of the reticle UI element.
    public CharacterController characterController; // Reference to the CharacterController.
    
    public float restingSize;
    public float maxSize;
    public float speed;
    private float currentSize;

    private void Start()
    {
        reticle = GetComponent<RectTransform>();
    }

    private void Update()
    {
        // Check if the player is currently moving and lerp currentSize to the appropriate value.
        if (IsMoving())
        {
            currentSize = Mathf.Lerp(currentSize, maxSize, Time.deltaTime * speed);
        }
        else
        {
            currentSize = Mathf.Lerp(currentSize, restingSize, Time.deltaTime * speed);
        }

        // Set the reticle's size to the currentSize value.
        reticle.sizeDelta = new Vector2(currentSize, currentSize);
    }

    bool IsMoving()
    {
        // Check the CharacterController's velocity magnitude to determine movement.
        return characterController.velocity.sqrMagnitude > 0.01f;
    }
}