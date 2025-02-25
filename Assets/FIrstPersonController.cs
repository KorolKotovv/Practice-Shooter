using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class FIrstPersonController : MonoBehaviour
{
    [Header("Movement Speeds")]
    [SerializeField] private float walkSpeed = 3.0f;
    [SerializeField] private float sprintMultiplier = 2.0f;
    
    [Header("Jump Parameters")]
    [SerializeField] private float jumpForce = 5.0f;
    [SerializeField] private float gravity = 9.81f;
    
    [Header("Sensitivity")]
    [SerializeField] private float mouseSensitivity = 2.0f;
    [SerializeField] private float upDownRange = 80.0f;
    
    [Header("Input Actions")]
    [SerializeField] private InputActionAsset playerControlls;
    
    private InputAction _moveAction;
    private InputAction _jumpAction;
    private InputAction _sprintAction;
    private InputAction _lookAction;
    private Vector2 _moveInput;
    private Vector2 _lookInput;
    
    private Camera _mainCamera;
    private float _verticalRotation;
    private Vector3 _currentMovement = Vector3.zero;
    private CharacterController _characterController;

    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
        _mainCamera = Camera.main;
         
        _moveAction = playerControlls.FindActionMap("Player").FindAction("Move");
        _jumpAction = playerControlls.FindActionMap("Player").FindAction("Jump");
        _sprintAction = playerControlls.FindActionMap("Player").FindAction("Sprint");
        _lookAction = playerControlls.FindActionMap("Player").FindAction("Look");
        
        _moveAction.performed += context => _moveInput = context.ReadValue<Vector2>();
        _moveAction.canceled += context => _moveInput = Vector2.zero;
        
        _lookAction.performed += context => _lookInput = context.ReadValue<Vector2>();
        _lookAction.canceled += context => _lookInput = Vector2.zero;
    }

    private void OnEnable()
    {
        _moveAction.Enable();
        _jumpAction.Enable();
        _sprintAction.Enable();
        _lookAction.Enable();
    }

    private void OnDisable()
    {
        _moveAction.Disable();
        _jumpAction.Disable();
        _sprintAction.Disable();
        _lookAction.Disable();
    }

    private void Update()
    {
        HandleMovement();
        HandleRotation();
    }

    void HandleRotation()
    {
        Debug.Log($"Look Input Y: {_lookInput.y}");
        float mouseXRotation = _lookInput.x * mouseSensitivity;
        transform.Rotate(0, mouseXRotation, 0);
        
        _verticalRotation -= _lookInput.y * mouseSensitivity;;
        _verticalRotation = Mathf.Clamp(_verticalRotation, -upDownRange, upDownRange);
        _mainCamera.transform.localRotation = Quaternion.Euler(_verticalRotation, 0, 0);
    }
    
    void HandleMovement()
    {
        float speedMultiplier = _sprintAction.ReadValue<float>() > 0 ? sprintMultiplier : 1f;
        float verticalSpeed = _moveInput.y * walkSpeed * speedMultiplier;
        float horizontalSpeed = _moveInput.x * walkSpeed * speedMultiplier;
        
        
        Vector3 horizontalMovement = new Vector3(horizontalSpeed, 0, verticalSpeed);
        horizontalMovement = transform.rotation * horizontalMovement;
        
        HandleJumpAndGravity();
        
        _currentMovement.x = horizontalMovement.x; 
        _currentMovement.z = horizontalMovement.z;
        
        _characterController.Move(_currentMovement * Time.deltaTime);
    } 
    
    void HandleJumpAndGravity()
    {
        if (_characterController.isGrounded)
        {
            _currentMovement.y = -0.5f;
            if (_jumpAction.triggered)
            {
                _currentMovement.y = Mathf.Sqrt(2 * jumpForce * gravity);
            }
        }
        _currentMovement.y -= gravity * Time.deltaTime;
    }
}
