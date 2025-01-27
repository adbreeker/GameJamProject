using System;
using System.Collections;
using FMODUnity;
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
    [SerializeField] float _shootingCooldown;
    bool _isAbleToShoot = true;

    [Header("Interactions:")]
    [SerializeField] float _interactionRange;
    [SerializeField] float _interactionRadius;
    [SerializeField] GameObject _gumGrenadePrefab;
    [SerializeField] GameObject _gumShieldPrefab;

    [Header("Sounds:")]
    [SerializeField] EventReference _drinkSound;
    [SerializeField] EventReference _shootSound;
    [SerializeField] EventReference _greenGumGranadeSound;
    [SerializeField] EventReference _yellowGumShieldSound;

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
        CheckInteractions(_interactionRange, _interactionRadius);
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
        if(Input.GetKey(KeyCode.Space))
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
        if (Input.GetKey(KeyCode.Space))
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
        if(Input.GetKeyDown(KeyCode.Mouse0) && _isAbleToShoot)
        {
            _isAbleToShoot = false;
            StartCoroutine(ShootingCooldown());

            RuntimeManager.PlayOneShot(_shootSound);
            GameObject bubble = Instantiate(_projectilePrefab, _shootingOrigin.transform.position, Quaternion.identity);
            RaycastHit target;
            Ray ray = new(_playerCamera.transform.position, _playerCamera.transform.forward);
            Vector3 direction = Physics.Raycast(ray,out target, 50f, LayerMask.GetMask("Obstacle", "Enemy"), QueryTriggerInteraction.Ignore)? 
                (target.point - _shootingOrigin.transform.position).normalized : (ray.GetPoint(30f) - _shootingOrigin.transform.position).normalized;
            bubble.GetComponent<BubbleProjectileController>().ShootInDirection(direction);
        }
    }

    IEnumerator ShootingCooldown()
    {
        yield return new WaitForSeconds(_shootingCooldown);
        _isAbleToShoot = true;
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
                        RuntimeManager.PlayOneShot(_drinkSound);
                        PlayerBehavior.activePlayer.HealPlayer(3);
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
                    RuntimeManager.PlayOneShot(_greenGumGranadeSound);
                    StartCoroutine(ThrowGumGrenadeDeleyed(0.5f));
                    break;
                case ItemType.GUM_SHIELD:
                    RuntimeManager.PlayOneShot(_yellowGumShieldSound);
                    StartCoroutine(UseGumShieldDeleyed(0.5f));
                    break;
            }

            OnPlayerUseItem?.Invoke(_currentItem);
            _currentItem = ItemType.NONE;
        }
    }

    IEnumerator ThrowGumGrenadeDeleyed(float deley)
    {
        yield return new WaitForSeconds(deley);
        Vector3 mouthPos = (_playerCamera.transform.position - new Vector3(0f, 0.2f, 0f)) + _playerCamera.transform.forward;
        Instantiate(_gumGrenadePrefab,mouthPos, Quaternion.identity)
                        .GetComponent<GumGrenadeController>().ThrowInDirection(_playerCamera.transform.forward, 10f, 3f);
    }

    IEnumerator UseGumShieldDeleyed(float deley)
    {
        yield return new WaitForSeconds(deley);
        Vector3 mouthPos = (_playerCamera.transform.position - new Vector3(0f, 0.2f, 0f)) + _playerCamera.transform.forward;
        Instantiate(_gumShieldPrefab, _playerCamera.transform);
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