using UnityEngine;
using UnityEngine.AI;

public enum NPCState
{
    Idle,
    Wandering,
    MovingToFood,
    Eating,
    MovingToBed,
    Sleeping
}

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Npcneeds))]

public class Npcbehaviour : MonoBehaviour
{
    
    public NPCState currentState = NPCState.Idle;

    public float hungerThresholdToSeekFood = 70f;
    public float hungerThresholdSatisfied = 20f;
    public float eatingSpeed = 15f;
    
    public float sleepThresholdToSeekBed = 70f;
    public float sleepThresholdSatisfied = 10f;
    public float sleepingSpeed = 10f;
    public float energyRegenPerSecondWhileSleeping = 8f;

    public Transform bedTarget;
    public Transform foodTarget;

    public float wanderRadius = 8f;
    public float minWaitBeforeWander = 2f;
    public float maxWaitBeforeWander = 5f;

    private NavMeshAgent navMeshAgent;
    private Npcneeds npcneeds;
    private float idleTimer;

    void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        npcneeds = GetComponent<Npcneeds>();
        idleTimer = Random.Range(minWaitBeforeWander, maxWaitBeforeWander);
    }

    
    
    void Update()
    {

        switch (currentState)
        {
            case NPCState.Idle:
                TickIdle();
                break;
            case NPCState.Wandering:
                TickWandering();
                break;
            case NPCState.MovingToFood:
                TickMovingToFood();
                break;
            case NPCState.Eating:
                TickEating();
                break;
            case NPCState.MovingToBed:
                TickMovingToBed();
                break;
            case NPCState.Sleeping:
                TickSleeping();
                break;
                
        }
        
    }
    
    void TickIdle()
    {
        if (npcneeds.sleep >= sleepThresholdToSeekBed && bedTarget != null)
        {
            navMeshAgent.SetDestination(bedTarget.position);
            currentState = NPCState.MovingToBed;
            return;
        }
        
        if (npcneeds.hunger >= hungerThresholdToSeekFood && foodTarget != null)
        {
            navMeshAgent.SetDestination(foodTarget.position);
            currentState = NPCState.MovingToFood;
            return;
        }

        idleTimer -= Time.deltaTime;

        if (idleTimer <= 0f)
        {
            TryStartWandering();
        }
    }

    void TryStartWandering()
    {
        Vector3 randomPoint = transform.position + Random.insideUnitSphere * wanderRadius;
        NavMeshHit hit;

        if (NavMesh.SamplePosition(randomPoint, out hit, wanderRadius, NavMesh.AllAreas))
        {
            navMeshAgent.SetDestination(hit.position);
            currentState = NPCState.Wandering;
        }
        else
        {
            idleTimer = Random.Range(minWaitBeforeWander, maxWaitBeforeWander);
        }
    }

    void TickWandering()
    {
        if (npcneeds.sleep >= sleepThresholdToSeekBed && bedTarget != null)
        {
            navMeshAgent.SetDestination(bedTarget.position);
            currentState = NPCState.MovingToBed;
            return;
        }

        if (npcneeds.hunger >= hungerThresholdToSeekFood && foodTarget != null)
        {
            navMeshAgent.SetDestination(foodTarget.position);
            currentState = NPCState.MovingToFood;
            return;
        }

        if (!navMeshAgent.pathPending && navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
        {
            idleTimer = Random.Range(minWaitBeforeWander, maxWaitBeforeWander);
            currentState = NPCState.Idle;
        }
    }

    void TickMovingToFood()
    {
        if (!navMeshAgent.pathPending && navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
        {
            currentState = NPCState.Eating;
            
        }
    }

    void TickEating()
    {
        npcneeds.EatFood(eatingSpeed * Time.deltaTime);

        if (npcneeds.hunger <= hungerThresholdSatisfied)
        {
            currentState = NPCState.Idle;
        }
    }
    void TickMovingToBed()
    {
        if (!navMeshAgent.pathPending && navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
        {
            currentState = NPCState.Sleeping;
        }
    }

    void TickSleeping()
    {
        npcneeds.SleepOff(sleepingSpeed * Time.deltaTime);
        npcneeds.RegenEnergy(energyRegenPerSecondWhileSleeping * Time.deltaTime);

        if (npcneeds.sleep <= sleepThresholdSatisfied)
        {
            currentState = NPCState.Idle;
        }
    }
    
}