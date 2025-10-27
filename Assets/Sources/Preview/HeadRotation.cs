using System;
using UnityEngine;

public class HeadRotation : MonoBehaviour
{
    [Header("Настройки мыши")]
    [SerializeField] private float mouseSensitivity = 100f;
    [SerializeField] private float smoothTime = 0.1f;
    
    [Header("Ограничения поворота")]
    [SerializeField] private float minVerticalAngle = -80f;
    [SerializeField] private float maxVerticalAngle = 80f;
    
    [Header("Ссылки")]
    [SerializeField] private Transform playerBody;
    [SerializeField] private Transform cameraTransform;

    [SerializeField] private CharacterComponent _component;
    
    private float _xRotation = 0f;
    private float _yRotation = 0f;
    private float _xRotationVelocity;
    private float _yRotationVelocity;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        HandleSmoothMouseLook();
    }
    
    private void HandleSmoothMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        // Плавный вертикальный поворот
        _xRotation -= mouseY;
        _xRotation = Mathf.Clamp(_xRotation, minVerticalAngle, maxVerticalAngle);
        float smoothedXRotation = Mathf.SmoothDampAngle(
            cameraTransform.localEulerAngles.x, 
            _xRotation, 
            ref _xRotationVelocity, 
            smoothTime
        );
        
        // Плавный горизонтальный поворот
        _yRotation += mouseX;
        float smoothedYRotation = Mathf.SmoothDampAngle(
            playerBody.eulerAngles.y, 
            _yRotation, 
            ref _yRotationVelocity, 
            smoothTime
        );
        
        playerBody.rotation = Quaternion.Euler(smoothedXRotation, smoothedYRotation, 0f);
        _component.UpdateAngle(smoothedYRotation);
    }
}