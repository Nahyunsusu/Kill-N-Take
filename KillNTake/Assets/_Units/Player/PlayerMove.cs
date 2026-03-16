using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class PlayerMove : MonoBehaviour
{
    // Instance
    public static PlayerMove Instance { get; private set; }

    // InputActions
    private PlayerInput _playerInput;

    private InputAction _moveAction;
    private InputAction _mousePosAction;
    private InputAction _selectAction;

    // Camera
    [SerializeField] private Camera _mainCamera;

    private Vector2 _currentMousePos;
    public Vector2 CurrentMousePos => _currentMousePos;

    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private LayerMask _teamLayer;

    // Select
    public List<UnitMove> selectedUnits = new List<UnitMove>();

    //

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        _playerInput = GetComponent<PlayerInput>();

        if (_playerInput != null)
        {
                _moveAction = _playerInput.actions.FindAction("Move");
            _mousePosAction = _playerInput.actions.FindAction("MousePosition");
              _selectAction = _playerInput.actions.FindAction("Select");
        }
    }

    private void OnEnable()
    {
          _moveAction.performed += OnMovePerformed;
        _selectAction.performed += OnSelectPerformed;

            _moveAction.Enable();
        _mousePosAction.Enable();
          _selectAction.Enable();
    }

    private void Update()
    {
        _currentMousePos = _mousePosAction.ReadValue<Vector2>();
    }

    private void OnDisable()
    {
          _moveAction.performed -= OnMovePerformed;
        _selectAction.performed -= OnSelectPerformed;

            _moveAction.Disable();
        _mousePosAction.Disable();
          _selectAction.Enable();
    }

    private void OnMovePerformed(InputAction.CallbackContext ctx)
    {
        if (_mainCamera == null) return;

        Ray ray = _mainCamera.ScreenPointToRay(_currentMousePos);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, _groundLayer))
        {
            foreach (var unit in selectedUnits)
            {
                unit.MoveTo(hit.point);
            }
        }
    }

    private void OnSelectPerformed(InputAction.CallbackContext ctx)
    {
        Ray ray = Camera.main.ScreenPointToRay(_currentMousePos);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, _teamLayer))
        {
            UnitMove unit = hit.collider.GetComponentInParent<UnitMove>();
            if(unit != null)
            {
                ClearSelection();
                selectedUnits.Add(unit);
                unit.SetSelected(true);
            }
            else
            {
                selectedUnits.Clear();
            }
        }
    }

    private void ClearSelection()
    {
        foreach (var unit in selectedUnits)
        {
            unit.SetSelected(false); // 외곽선 끄기
        }
        selectedUnits.Clear();
    }
}