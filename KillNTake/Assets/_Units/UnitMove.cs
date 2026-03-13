using UnityEngine;
using UnityEngine.AI;

public class UnitMove : MonoBehaviour
{
    protected NavMeshAgent _agent;

    protected Animator _animator;

    protected virtual void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
    }

    protected virtual void Update()
    {
        
    }

    protected void MoveTo(Vector3 targetPos)
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
}