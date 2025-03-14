using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Utility.Scripts;

public class GroundAgentComponentState : AgentComponentState, IAgent
{
    private SectorReference _sectorReference;
    private NavMeshAgent _agent;
    private StatsManager _stats;
    private float _speed => _stats.GetModifiedStat(EUnitFloatStats.MovementSpeed);

    private bool _activationReady;
    private bool _enabled;
    private HashSet<object> _disablers = new();
    private bool _pathSet;
    private Vector3 _destination;
    
    public event Action OnPathSet = delegate { };
    public event Action OnPathFinished = delegate { };
    public event Action OnWarped = delegate { };

    protected override void OnSpawnSuccess()
    {
        _sectorReference = Container.GetComponent<SectorReference>();
        _stats = Container.GetComponent<StatsManager>();
        _agent = Container.GetComponent<NavMeshAgent>();
    }

    protected override void OnEnter()
    {
        _enabled = true;
        EnableAgent(this);
        _activationReady = false;
    }

    protected override void Tick()
    {
        if (!_agent.enabled) return;
        if (_agent.pathPending) return;
        if (_agent.hasPath) _pathSet = true;
        if (!_pathSet) return;
        if (_agent.remainingDistance >= .1f) return;
        FinishPath();
    }

    public override void OnExit()
    {
        _enabled = false;
        _activationReady = false;
    }

    public void Activate() => _activationReady = true;

    public bool IsActive() => _enabled;
    
    public bool CalculateTeleportPath(Vector3 destination, out List<TeleportPoint> points)
    {
        points = null;
        var targetSector = SectorFinder.FindSector(destination, 3, LayerManager.Instance.GroundLayer);

        return SectorManager.Instance.PathBetween(Container.transform.position, _agent.speed, _sectorReference.Sector, targetSector, out points) > -1;
    }

    public void SetDestination(Vector3 destination)
    {
        var fromSector = _sectorReference.Sector;
        var targetSector = SectorFinder.FindSector(destination, 3, LayerManager.Instance.GroundLayer);
        if (fromSector != targetSector) return;
        _agent.SetDestination(new Vector3(destination.x, Container.transform.position.y, destination.z));
        _pathSet = true;
        OnPathSet.Invoke();
    }

    private void FinishPath()
    {
        _agent.ResetPath();
        _pathSet = false;
        OnPathFinished.Invoke();
    }

    public void ClearDestination()
    {
        _pathSet = false;
        _agent.ResetPath();
    }

    public float GetSpeed() => _pathSet ? _speed : 0;

    public void EnableAgent(object caller)
    {
        _disablers.Remove(caller);
        if (_disablers.Count <= 0) _agent.enabled = true;
    }

    public void DisableAgent(object caller)
    {
        _disablers.Add(caller);
        _agent.enabled = false;
    }

    public void Warp(Vector3 desiredPos)
    {
        _agent.Warp(desiredPos);
        OnWarped.Invoke();
    }
}
