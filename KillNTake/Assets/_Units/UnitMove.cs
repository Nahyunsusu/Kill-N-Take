using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class UnitMove : MonoBehaviour
{
    public static List<UnitMove> AllUnits = new List<UnitMove>();

    protected NavMeshAgent _agent;

    protected Animator _animator;

    private Outline _outline;

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();

        _outline = GetComponent<Outline>();
        if (_outline != null)
        {
            _outline.enabled = false;
            _outline.OutlineWidth = 5f;
            _outline.OutlineColor = Color.green;
        }
    }

    private void OnEnable()
    {
        if (!AllUnits.Contains(this))
        {
            AllUnits.Add(this);
        }
    }

    protected virtual void Update()
    {
        
    }

    private void OnDisable()
    {
        AllUnits.Remove(this);
    }

    public void MoveTo(Vector3 targetPos)
    {
        if (_agent != null && _agent.isOnNavMesh)
        {
            _agent.isStopped = false;
            _agent.velocity  = Vector3.zero;

            Vector3 direction = (targetPos - transform.position).normalized;
            if (direction != Vector3.zero)
            {
                direction.y = 0;
                transform.rotation = Quaternion.LookRotation(direction);
            }

            _agent.SetDestination(targetPos);
        }
    }

    protected void Stop()
    {
        if (_agent != null && _agent.isOnNavMesh)
        {
            _agent.isStopped = true;
            _agent.ResetPath();     
        }
    }

    public void SetSelected(bool isSelected)
    {
        if (_outline != null)
        {
            _outline.enabled = isSelected;
        }
    }
}