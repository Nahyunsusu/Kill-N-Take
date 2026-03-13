using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class PlayerMove : UnitMove
{
    // InputActions
    private PlayerInput _playerInput;

    private InputAction _moveAction;
    private InputAction _mousePosAction;

    private Vector2 _currentMousePos;

    [SerializeField] private LayerMask _groundLayer;

    protected override void Awake()
    {
        base.Awake();

        _playerInput = GetComponent<PlayerInput>();

        if (_playerInput != null)
        {
            _moveAction = _playerInput.actions.FindAction("Move");
            _mousePosAction = _playerInput.actions.FindAction("MousePosition");
        }
    }

    private void OnEnable()
    {
        _moveAction.performed += OnMovePerformed;

        _moveAction.Enable();
        _mousePosAction.Enable();
    }

    protected override void Update()
    {
        _currentMousePos = _mousePosAction.ReadValue<Vector2>();
    }

    private void OnDisable()
    {
        _moveAction.performed -= OnMovePerformed;

        _moveAction.Disable();
        _mousePosAction.Disable();
    }

    private void OnMovePerformed(InputAction.CallbackContext ctx)
    {
        Ray ray = Camera.main.ScreenPointToRay(_currentMousePos);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, _groundLayer))
        {
            MoveTo(hit.point);
            Debug.Log("이동합니다");
        }
    }
}