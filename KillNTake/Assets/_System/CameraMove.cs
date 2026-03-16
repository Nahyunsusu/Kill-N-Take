using Unity.VisualScripting;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    private Vector2 mousePos;

    [SerializeField] private float _camSpeed = 25f;
    [SerializeField] private float _edgeSize = 20f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Confined;
    }

    private void Update()
    {
        if (PlayerMove.Instance == null) return;

        HandleCameraLock();
        MoveCamera();
    }

    private void HandleCameraLock()
    {
        if (Application.isFocused == false)
        {
            Cursor.lockState = CursorLockMode.None;
        }

        if (Application.isFocused && Cursor.lockState == CursorLockMode.None)
        {
            Cursor.lockState = CursorLockMode.Confined;
        }
    }

    private void MoveCamera()
    {
        mousePos = PlayerMove.Instance.CurrentMousePos;
        Vector3 moveDir = Vector3.zero;

        if (mousePos.x >= Screen.width - _edgeSize) moveDir.x = +1;
        else if (mousePos.x <= _edgeSize) moveDir.x = -1;

        if (mousePos.y >= Screen.height - _edgeSize) moveDir.z = +1;
        else if (mousePos.y <= _edgeSize) moveDir.z = -1;

        if (moveDir != Vector3.zero)
            transform.Translate(moveDir.normalized * _camSpeed * Time.deltaTime, Space.World);

    }
}
