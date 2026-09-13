using UnityEngine;
using UnityEngine.AI;

public enum NPCState
{
    Idle,
    Wandering,
    MovingToTrader,
    Buying,
    Eating,
    MovingToBed,
    Sleeping
}

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Npcneeds))]
[RequireComponent(typeof(Wallet))]
[RequireComponent(typeof(Inventory))]
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
    public Transform traderTarget;
    public ItemData foodItem;

    public float wanderRadius = 8f;
    public float minWaitBeforeWander = 2f;
    public float maxWaitBeforeWander = 5f;

    private NavMeshAgent navMeshAgent;
    private Npcneeds npcneeds;
    private Wallet wallet;
    private Inventory inventory;
    private Trader targetTrader;
    private float idleTimer;

    void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        npcneeds = GetComponent<Npcneeds>();
        wallet = GetComponent<Wallet>();
        inventory = GetComponent<Inventory>();
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
            case NPCState.MovingToTrader:
                TickMovingToTrader();
                break;
            case NPCState.Buying:
                TickBuying();
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

        if (npcneeds.hunger >= hungerThresholdToSeekFood && TryHandleHunger())
        {
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

        if (npcneeds.hunger >= hungerThresholdToSeekFood && TryHandleHunger())
        {
            return;
        }

        if (!navMeshAgent.pathPending && navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
        {
            idleTimer = Random.Range(minWaitBeforeWander, maxWaitBeforeWander);
            currentState = NPCState.Idle;
        }
    }

    bool TryHandleHunger()
    {
        if (inventory.HasItem(foodItem))
        {
            EnterEatingState();
            return true;
        }

        if (traderTarget != null)
        {
            Trader trader = traderTarget.GetComponent<Trader>();

            if (trader != null && wallet.coins >= trader.itemForSale.price)
            {
                targetTrader = trader;
                navMeshAgent.SetDestination(traderTarget.position);
                currentState = NPCState.MovingToTrader;
                return true;
            }
        }

        return false;
    }

    void EnterEatingState()
    {
        inventory.RemoveItem(foodItem);
        currentState = NPCState.Eating;
    }

    void TickMovingToTrader()
    {
        if (!navMeshAgent.pathPending && navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
        {
            currentState = NPCState.Buying;
        }
    }

    void TickBuying()
    {
        if (targetTrader.TryBuy(wallet, inventory))
        {
            EnterEatingState();
        }
        else
        {
            currentState = NPCState.Idle;
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