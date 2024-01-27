using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;

public class FirstPersonMovement : MonoBehaviourPun, IPunObservable
{

    public float speed = 5;
    [SerializeField] private Camera m_Camera;
    [Header("Running")]
    public bool canRun = true;
    public bool IsRunning { get; private set; }
    public float runSpeed = 9;
    public KeyCode runningKey = KeyCode.LeftShift;

    Rigidbody rigidbody;
    public event System.Action Jumped;
    private GroundCheck _groundCheck;

 
    public List<System.Func<float>> speedOverrides = new List<System.Func<float>>();


    [SerializeField, Tooltip("Prevents jumping when the transform is in mid-air.")]
    GroundCheck groundCheck;


    void Reset()
    {

        groundCheck = GetComponentInChildren<GroundCheck>();
    }


    void Start()
    {

        if (!this.photonView.IsMine)
        {
            m_Camera.gameObject.SetActive(false);

            return;

        }
        rigidbody = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        if (!this.photonView.IsMine) return;


        IsRunning = canRun && Input.GetKey(runningKey);

        // Get targetMovingSpeed.
        float targetMovingSpeed = IsRunning ? runSpeed : speed;
        if (speedOverrides.Count > 0)
        {
            targetMovingSpeed = speedOverrides[speedOverrides.Count - 1]();
        }

        // Get targetVelocity from input.
        Vector2 targetVelocity =new Vector2( Input.GetAxis("Horizontal") * targetMovingSpeed, Input.GetAxis("Vertical") * targetMovingSpeed);

        // Apply movement.
        rigidbody.velocity = transform.rotation * new Vector3(targetVelocity.x, rigidbody.velocity.y, targetVelocity.y);
    }

    void LateUpdate()
    {

        if (!this.photonView.IsMine) return;

        if (Input.GetButtonDown("Jump") && (!groundCheck || groundCheck.isGrounded))
        {
            rigidbody.AddForce(Vector3.up * 100 * 1);
            Jumped?.Invoke();
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        
    }
}