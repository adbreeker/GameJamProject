using System;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
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
    [SerializeField] Camera _playerCamera;
    public float viewSensitivity;
    Vector3 _viewRotation;

    [Header("Shooting:")]
    [SerializeField] Transform _shootingOrigin;
    [SerializeField] GameObject _projectilePrefab;

    [Header("Interactions:")]
    [SerializeField] float interactionRange;
    [SerializeField] float interactionRadius;
    GameObject _pointedInteraction;
    public Action<ItemType> OnPlayerPickUpItem;
    public Action<ItemType> OnPlayerUseItem;
    ItemType _currentItem = ItemType.NONE;

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
        CheckInteractions(interactionRange, interactionRadius);
        Interaction();
        UseItem();
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
                if(possibleStep < 0.1f) { possibleStep = 0f; }
                transform.localScale = Vector3.MoveTowards(
                    transform.localScale,
                    new Vector3(_startScale.x, _startScale.y, _startScale.z),
                    Mathf.Min(uncrouchingSpeed * Time.deltaTime, possibleStep * 0.1f));
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

    void CheckInteractions(float distance, float radius)
    {
        Ray ray = _playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f, 0));

        Vector3 startPoint = ray.origin;
        Vector3 endPoint = ray.origin + ray.direction * distance;

        Collider[] hitColliders = Physics.OverlapCapsule(startPoint, endPoint, radius, LayerMask.GetMask("Interaction"), QueryTriggerInteraction.Collide);
#if UNITY_EDITOR
        DrawDebugCylinder(startPoint, endPoint, radius);
#endif
        if(hitColliders.Length > 0)
        {
            _pointedInteraction = hitColliders[0].gameObject;
        }
        else
        {
            _pointedInteraction = null;
        }
    }

    void Interaction()
    {
        if (Input.GetKeyDown(KeyCode.E) && _pointedInteraction != null)
        {
            //getting item from spawner
            if(_pointedInteraction.GetComponent<ItemSpawner>() != null)
            {
                ItemType itemPickedUp = _pointedInteraction.GetComponent<ItemSpawner>().GetItem();

                switch(itemPickedUp) 
                {
                    case ItemType.NONE:
                        return;
                    case ItemType.BUBBLE_TEA:
                        OnPlayerPickUpItem?.Invoke(itemPickedUp);
                        OnPlayerUseItem?.Invoke(itemPickedUp);
                        PlayerBehavior.activePlayer.HealPlayer(4);
                        return;
                    case ItemType.GUM_GRENADE:
                        OnPlayerPickUpItem?.Invoke(itemPickedUp);
                        _currentItem = itemPickedUp;
                        return;
                    case ItemType.GUM_SHIELD:
                        OnPlayerPickUpItem?.Invoke(itemPickedUp);
                        _currentItem = itemPickedUp;
                        return;
                }
            }
        }
    }
    
    void UseItem()
    {
        if (Input.GetKeyDown(KeyCode.Mouse1) && _currentItem != ItemType.NONE)
        {
            switch(_currentItem)
            {
                case ItemType.GUM_GRENADE:
                    Debug.Log("GRENADE!");
                    break;
                case ItemType.GUM_SHIELD:
                    Debug.Log("SHIELD!");
                    break;
            }

            OnPlayerUseItem?.Invoke(_currentItem);
            _currentItem = ItemType.NONE;
        }
    }







    //Debug ----------------------------------------------------------------------------------------------------------------- Debug
    void DrawDebugCylinder(Vector3 startPoint, Vector3 endPoint, float radius, int segments = 20)
    {
        // Vector representing the axis of the cylinder
        Vector3 axis = endPoint - startPoint;
        Vector3 axisNormalized = axis.normalized;

        // Find any vector that is not parallel to the cylinder's axis
        Vector3 perpendicularVector = Vector3.Cross(axisNormalized, Vector3.up);
        if (perpendicularVector == Vector3.zero) // If the vector happened to be parallel to "up", choose a different vector
        {
            perpendicularVector = Vector3.Cross(axisNormalized, Vector3.right);
        }
        perpendicularVector.Normalize();

        // Draw the center axis of the cylinder
        Debug.DrawLine(startPoint, endPoint, Color.cyan);

        // Draw the cross-section of the cylinder
        for (int i = 0; i < segments; i++)
        {
            float angle = i * Mathf.PI * 2 / segments;
            float nextAngle = (i + 1) * Mathf.PI * 2 / segments;

            // Calculate points on the circle around startPoint and endPoint
            Vector3 point1 = startPoint + Quaternion.AngleAxis(angle * Mathf.Rad2Deg, axisNormalized) * perpendicularVector * radius;
            Vector3 point2 = startPoint + Quaternion.AngleAxis(nextAngle * Mathf.Rad2Deg, axisNormalized) * perpendicularVector * radius;

            Vector3 point3 = endPoint + Quaternion.AngleAxis(angle * Mathf.Rad2Deg, axisNormalized) * perpendicularVector * radius;
            Vector3 point4 = endPoint + Quaternion.AngleAxis(nextAngle * Mathf.Rad2Deg, axisNormalized) * perpendicularVector * radius;

            // Draw the cross-sections (base and top of the cylinder)
            Debug.DrawLine(point1, point2, Color.blue);
            Debug.DrawLine(point3, point4, Color.blue);

            // Draw lines connecting the two bases
            Debug.DrawLine(point1, point3, Color.blue);
        }
    }
}