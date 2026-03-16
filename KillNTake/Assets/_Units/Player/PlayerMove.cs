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

    public Vector2 CurrentMousePos => _currentMousePos;
    private Vector2 _currentMousePos;

    private Vector2 _startMousePos;
    [SerializeField] private RectTransform _selectionBoxVisual;

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
        _selectAction.performed += OnSelectStarted;
        _selectAction.canceled  += OnSelectCanceled;

            _moveAction.Enable();
        _mousePosAction.Enable();
          _selectAction.Enable();
    }

    private void Update()
    {
        _currentMousePos = _mousePosAction.ReadValue<Vector2>();

        if (_selectionBoxVisual.gameObject.activeInHierarchy)
        {
            UpdateSelectionBoxVisual();
        }
    }

    private void OnDisable()
    {
          _moveAction.performed -= OnMovePerformed;
        _selectAction.performed -= OnSelectStarted;
        _selectAction.canceled  -= OnSelectCanceled;

            _moveAction.Disable();
        _mousePosAction.Disable();
          _selectAction.Disable();
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

    private void OnSelectStarted(InputAction.CallbackContext ctx)
    {
        _startMousePos = _currentMousePos;
        _selectionBoxVisual.gameObject.SetActive(true);
    }

    private void OnSelectCanceled(InputAction.CallbackContext ctx)
    {
        _selectionBoxVisual.gameObject.SetActive(false);

        if (Vector2.Distance(_startMousePos, _currentMousePos) < 5f)
        {
            SingleSelect();
        }
        else
        {
            BoxSelect();
        }
    }

    private void SingleSelect()
    {
        Ray ray = Camera.main.ScreenPointToRay(_currentMousePos);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, _teamLayer))
        {
            UnitMove unit = hit.collider.GetComponentInParent<UnitMove>();
            if (unit != null)
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
        else
        {
            ClearSelection();
        }
    }

    private void BoxSelect()
    {
        ClearSelection();

        foreach (var unit in UnitMove.AllUnits)
        {
            Vector3 screenPos = Camera.main.WorldToScreenPoint(unit.transform.position);

            if (IsInsideSelectionBox(screenPos))
            {
                selectedUnits.Add(unit);
                unit.SetSelected(true);
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

    private void UpdateSelectionBoxVisual()
    {
        float width = _currentMousePos.x - _startMousePos.x;
        float height = _currentMousePos.y - _startMousePos.y;

        _selectionBoxVisual.sizeDelta = new Vector2(Mathf.Abs(width), Mathf.Abs(height));

        float minX = Mathf.Min(_startMousePos.x, _currentMousePos.x);
        float minY = Mathf.Min(_startMousePos.y, _currentMousePos.y);

        _selectionBoxVisual.anchoredPosition = new Vector2(minX, minY);
    }

    private bool IsInsideSelectionBox(Vector3 screenPos)
    {
        Rect rect = new Rect(
            Mathf.Min(_startMousePos.x, _currentMousePos.x),
            Mathf.Min(_startMousePos.y, _currentMousePos.y),
            Mathf.Abs(_startMousePos.x - _currentMousePos.x),
            Mathf.Abs(_startMousePos.y - _currentMousePos.y)
        );

        return rect.Contains(screenPos);
    }
}