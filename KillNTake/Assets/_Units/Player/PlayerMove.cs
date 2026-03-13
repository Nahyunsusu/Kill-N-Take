using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class PlayerMove : MonoBehaviour
{
    // InputActions
    private PlayerInput _playerInput;

    private InputAction _moveAction;
    private InputAction _mousePosAction;
    private InputAction _selectAction;

    private Vector2 _currentMousePos;

    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private LayerMask _teamLayer;

    // Select
    public List<UnitMove> selectedUnits = new List<UnitMove>();

    private void Awake()
    {
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
        Ray ray = Camera.main.ScreenPointToRay(_currentMousePos);
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
        Debug.Log("클릭 실행");
        Ray ray = Camera.main.ScreenPointToRay(_currentMousePos);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, _teamLayer))
        {
            Debug.Log("레이캐스트");
            UnitMove unit = hit.collider.GetComponent<UnitMove>();
            if(unit != null)
            {
                selectedUnits.Clear();
                selectedUnits.Add(unit);
                Debug.Log("선택됨!");
            }
            else
            {
                selectedUnits.Clear();
                Debug.Log("선택된 유닛 없음");
            }
        }
    }
}