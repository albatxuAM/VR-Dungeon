using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class Pickup : MonoBehaviour
{
    [Tooltip("Frequency at which the item will move up and down")]
    public float VerticalBobFrequency = 1f;

    [Tooltip("Distance the item will move up and down")]
    public float BobbingAmount = 1f;

    [Tooltip("Rotation angle per second")]
    public float RotatingSpeed = 360f;

    [Tooltip("Sound played on pickup")]
    public AudioClip PickupSfx;

    [Tooltip("VFX spawned on pickup")]
    public GameObject PickupVfxPrefab;

    public Rigidbody PickupRigidbody { get; private set; }

    Collider m_Collider;
    Vector3 m_StartPosition;
    bool m_HasPlayedFeedback;

    protected virtual void Start()
    {
        PickupRigidbody = GetComponent<Rigidbody>();
        m_Collider = GetComponent<Collider>();

        // ensure the physics setup is a kinematic rigidbody trigger
        PickupRigidbody.isKinematic = true;
        m_Collider.isTrigger = true;

        // Remember start position for animation
        m_StartPosition = transform.position;
    }

    void Update()
    {
        // Handle bobbing
        float bobbingAnimationPhase = ((Mathf.Sin(Time.time * VerticalBobFrequency) * 0.5f) + 0.5f) * BobbingAmount;
        transform.position = m_StartPosition + Vector3.up * bobbingAnimationPhase;

        // Handle rotating
        transform.Rotate(Vector3.up, RotatingSpeed * Time.deltaTime, Space.Self);
    }

    void OnTriggerEnter(Collider other)
    {
        InventoryManager pickingPlayer = other.GetComponent<InventoryManager>();

        if (pickingPlayer != null)
        {
            OnPicked(pickingPlayer);
        }
    }

    protected virtual void OnPicked(InventoryManager playerController)
    {
        PlayPickupFeedback();
    }

    public void PlayPickupFeedback()
    {
        if (m_HasPlayedFeedback)
            return;

        if (PickupSfx)
        {
            // Play the pickup sound at the position of the pickup
            AudioSource.PlayClipAtPoint(PickupSfx, transform.position);
        }

        if (PickupVfxPrefab)
        {
            // Instantiate the VFX prefab
            Instantiate(PickupVfxPrefab, transform.position, Quaternion.identity);
        }

        m_HasPlayedFeedback = true;
    }
}
