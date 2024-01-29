using Photon.Pun;
using PlayFab.EconomyModels;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FirstPersonMovement : MonoBehaviourPunCallbacks, IPunObservable
{

    public float speed = 5;
    [SerializeField] private Camera m_Camera;
    [SerializeField] private List<GameObject> _hidenObjects;
    [Header("Running")]
    public bool canRun = true;
    public bool IsRunning { get; private set; }
    public float runSpeed = 9;
    public KeyCode runningKey = KeyCode.LeftShift;

    Rigidbody rigidbody;
    public event System.Action Jumped;
    private GroundCheck _groundCheck;


    [SerializeField] private Canvas _canvasHealth;

    public List<System.Func<float>> speedOverrides = new List<System.Func<float>>();


    [SerializeField]
    GroundCheck groundCheck;

    GameObject instance;

    private int _health;
    [SerializeField] private TMP_Text _healthText;



    void Reset()
    {

        groundCheck = GetComponentInChildren<GroundCheck>();
    }


    public void UpdatePlayersCameraUI()
    {

        var players = GameObject.FindGameObjectsWithTag("Player");

        foreach (var player in players)
        {
            if (player == this.gameObject) continue;

            player.GetComponent<FirstPersonMovement>().SetWorldCameraUI(m_Camera);
        }
    }


    public void SetWorldCameraUI(Camera camera)
    {
       
            _canvasHealth.worldCamera = camera;
        
    }

    public override void OnEnable()
    {
        base.OnEnable();
    }
  
    public override void OnDisable()
    {
        // Always call the base to remove callbacks
        base.OnDisable();

 
    }


    public void Awake()
    {
         
        if (photonView.IsMine)
        {
            _health = 100;//?
            _healthText.text = _health.ToString();
            instance = gameObject;
        }
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
      
        if (!photonView.IsMine)
        {

            m_Camera.gameObject.SetActive(false);
            _hidenObjects.ForEach(obj => obj.SetActive(false));
            m_Camera.GetComponent<AudioListener>().enabled = false;
            return;
        }
        else rigidbody = GetComponent<Rigidbody>();
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

        if (stream.IsWriting)
        {
            stream.SendNext(_health);
            _healthText.text = $" <color=green>{_health}</color>";
        }
        else
        {
           _health = (int) stream.ReceiveNext();
            _healthText.text = $" <color=green>{_health}</color>";
        }
        return;
    }
}