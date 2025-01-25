using System;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.InputSystem.XR;

public class PlayerController : MonoBehaviour
{
    public Action OnPlayerDeath;

    [Header("Movement variables:")]
    public float movementSpeed;
    [Range(1f, 2f)]
    public float sprintSpeedMultiplier;
    [Range(0f, 1f)]
    public float crouchSpeedMultiplier;
    public float gravityForce;
    Vector3 _playerVelocity;

    [Header("Crouching variables:")]
    public float crouchHeightMultiplier;
    public float crouchingSpeed;
    public float uncrouchingSpeed;
    Vector3 _startScale;

    [Header("View variables:")]
    [SerializeField] GameObject _playerCamera;
    public float viewSensitivity;
    Vector3 _viewRotation;

    [Header("Shooting:")]
    [SerializeField] Transform _shootingOrigin;
    [SerializeField] GameObject _projectilePrefab;

    CharacterController _cc;

    public static PlayerController activePlayer;

    private void Awake()
    {
        if (activePlayer != null && activePlayer != this)
        {
            Destroy(gameObject);
        }
        activePlayer = this;
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;

        _cc = GetComponent<CharacterController>();
        _playerVelocity = Vector3.zero;
        _startScale = transform.localScale;
        _viewRotation = _playerCamera.transform.localRotation.eulerAngles;
    }

    void Update()
    {
        //whole movement
        Movement();
        Crouching();
        View();
        Gravity();
        _cc.Move(_playerVelocity * Time.deltaTime);

        //actions
        Shooting();
    }

    public void HitPlayer()
    {
        Debug.Log("Player was hit!");
    }

    void Movement()
    {
        //walk direction
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 direction = transform.right * horizontal + transform.forward * vertical;

        //walking
        if(direction.magnitude > 0)
        {
            _playerVelocity.x = direction.normalized.x * movementSpeed;
            _playerVelocity.z = direction.normalized.z * movementSpeed;
        }
        else
        {
            _playerVelocity.x = 0f;
            _playerVelocity.z = 0f;
        }

        //sprinting
        if(Input.GetKey(KeyCode.LeftControl))
        {
            _playerVelocity.x *= crouchSpeedMultiplier;
            _playerVelocity.z *= crouchSpeedMultiplier;
        }
        else 
        {
            if(Input.GetKey(KeyCode.LeftShift))
            {
                _playerVelocity.x *= sprintSpeedMultiplier;
                _playerVelocity.z *= sprintSpeedMultiplier;
            }
        }
    }

    void Crouching()
    {
        if (Input.GetKey(KeyCode.LeftControl))
        {
            transform.localScale = Vector3.MoveTowards(
                transform.localScale, 
                new Vector3(_startScale.x, _startScale.y * crouchHeightMultiplier, _startScale.z), 
                crouchingSpeed * Time.deltaTime);
        }
        else if(transform.localScale != _startScale)
        {
            Vector3 playerTop = transform.TransformPoint(_cc.center) + transform.up * (_cc.height * transform.lossyScale.y / 2f);
            RaycastHit hit;

            if (Physics.Raycast(playerTop,transform.up,out hit,0.5f,LayerMask.GetMask("Obstacle")))
            {
                float possibleStep = Vector3.Distance(playerTop, hit.point);
                Debug.Log("roof, posible step is: " + possibleStep);
                if(possibleStep < 0.1f) { possibleStep = 0f; }
                transform.localScale = Vector3.MoveTowards(
                    transform.localScale,
                    new Vector3(_startScale.x, _startScale.y, _startScale.z),
                    Mathf.Min(uncrouchingSpeed * Time.deltaTime, possibleStep));
            }
            else
            {
                transform.localScale = Vector3.MoveTowards(
                    transform.localScale,
                    new Vector3(_startScale.x, _startScale.y, _startScale.z),
                    uncrouchingSpeed * Time.deltaTime);
            }
        }
    }

    void View()
    {
        float mouseX = Input.GetAxis("Mouse X") * viewSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * viewSensitivity * Time.deltaTime;

        _viewRotation.x -= mouseY;
        _viewRotation.x = Mathf.Clamp(_viewRotation.x, -90f, 90f);

        _playerCamera.transform.localRotation = Quaternion.Euler(_viewRotation);

        transform.Rotate(transform.up * mouseX);
    }   
    
    void Gravity()
    {
        if(_cc.isGrounded && _playerVelocity.y < -2f)
        {
            _playerVelocity.y = -2f;
        }

        _playerVelocity.y += gravityForce * Time.deltaTime;
    }

    void Shooting()
    {
        if(Input.GetKeyDown(KeyCode.Mouse0))
        {
            GameObject bubble = Instantiate(_projectilePrefab, _shootingOrigin.transform.position, Quaternion.identity);
            bubble.GetComponent<BubbleProjectileController>().ShootInDirection(_playerCamera.transform.forward);
        }
    }
}