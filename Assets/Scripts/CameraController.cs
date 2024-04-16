using System.Collections;
using System.Collections.Generic;
using Cinemachine;

//using System.Diagnostics;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    private const float minFollowYOffset = 2f;
    private const float maxFollowYOffset = 12f;
    [SerializeField] private CinemachineVirtualCamera cinemachineVirtualCamera;

    private CinemachineTransposer cinemachineTransposer;
    private Vector3 targetFollowOffset;

    private void Start() 
    {
        cinemachineTransposer = cinemachineVirtualCamera.GetCinemachineComponent<CinemachineTransposer>();
        targetFollowOffset = cinemachineTransposer.m_FollowOffset;
        
    }
    private void Update() 
    {
        Movement();
        Rotation();
        Zoom();
    }

    private void Movement()
    {
        Vector2 inputMoveDirection = InputManager.Instance.GetCameraMove();
        

        float moveSpeed = 7f; 

        Vector3 moveVector = transform.forward * inputMoveDirection.y + transform.right * inputMoveDirection.x;
        transform.position += moveVector * moveSpeed * Time.deltaTime;

        
    }
    private void Rotation()
    {
        Vector3 rotationVector = new Vector3(0,0,0);

        rotationVector.y = InputManager.Instance.GetCameraRotation();
        float rotationSpeed = 100f;
        transform.eulerAngles += rotationVector * rotationSpeed * Time.deltaTime;

        
    }
    private void Zoom()
    {
        
        targetFollowOffset.y += InputManager.Instance.GetCameraZoom();
        targetFollowOffset.y = Mathf.Clamp(targetFollowOffset.y, minFollowYOffset, maxFollowYOffset);
        float zoomSpeed = 5f;
        cinemachineTransposer.m_FollowOffset = Vector3.Lerp(cinemachineTransposer.m_FollowOffset, targetFollowOffset, Time.deltaTime * zoomSpeed);
    }
}
