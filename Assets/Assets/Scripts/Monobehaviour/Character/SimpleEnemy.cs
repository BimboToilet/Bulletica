using UnityEngine;
using EnemyState;

[RequireComponent(typeof(BaseSleep))]
public class SimpleEnemy : Gunman, IObservable, IStatable
{
    public EnemyProfile profile;

    //EnemyState
    public ActionState actionState;
    public MotionState motionState;

    [SerializeField] private LayerMask masks;

    public BasePursue pursue;
    public BaseSleep sleep;

    protected override void Awake()
    {
        base.Awake();

        sleep = GetComponent<BaseSleep>();

        target = GetComponent<BaseTargeting>();
        target.SetTarget();

        pursue = GetComponent<BasePursue>();
    }

    void OnEnable()
    {
        Vector2 startDir = (target.target.position - transform.position).normalized;
        shooting.RotateGunInstantly(startDir);
    }

    protected override void FixedUpdate()
    {
        Vector2 dir = target.target.position - transform.position;
        switch (motionState)
        {
            case MotionState.MoveToTarget:
                move.Move(ref rb, dir);
                break;
            case MotionState.Stay:
                move.StopMovement(ref rb);
                break;
            case MotionState.Regroup:
                move.Move(ref rb, dir);
                break;
            case MotionState.Pursue:
                Vector2 pursueDir = pursue.lastSeenPos - (Vector2)transform.position;
                move.Move(ref rb, pursueDir);
                break;
            case MotionState.Sleep:
                move.StopMovement(ref rb);
                break;
            default:
                move.StopMovement(ref rb);
                break;
        }
    }

    protected override void Update()
    {
        Vector2 dir = (target.target.position - transform.position).normalized;

        if (health.healthPoints == 0)
        {
            death.Die(gameObject);
        }

        Observe();
        UpdateActionState();
        UpdateMovingState();

        switch (actionState)
        {
            case ActionState.Shoot:
                ShootingHandler();
                shooting.RotateGun(dir);
                break;
            case ActionState.Reload:
                ReloadHandler();
                shooting.RotateGun(dir);
                break;
            case ActionState.Idle:
                break;
            case ActionState.Pursue:
                break;
            case ActionState.Sleep:
                break;
            default:
                break;
        }
    }
    public void UpdateActionState()
    {
        if (sleep.onSleep)
        {
            actionState = ActionState.Sleep;
        }
        else if (move.onPush)
        {
            actionState = ActionState.Stun;
        }
        else if (shooting.IsMagazineEmpty())
        {
            actionState = ActionState.Reload;
        }
        else if (inShootingRange)
        {
            actionState = ActionState.Shoot;
        }
        else
        {
            actionState = ActionState.Idle;
        }
    }

    public void UpdateMovingState()
    {
        if (sleep.onSleep)
        {
            motionState = MotionState.Sleep;
            return;
        }
        else if (move.onPush)
        {
            motionState = MotionState.Stay;
            return;
        }
        else if (actionState == ActionState.Reload)
        {
            if (target.targetSeen && !targetApproached)
            {
                motionState = MotionState.MoveToTarget;
            }
            else
            {
                motionState = MotionState.Stay;
            }
        }
        else if (actionState == ActionState.Shoot || shooting.onCooldown)
        {
            if (targetApproached)
            {
                motionState = MotionState.Stay;
            }
            else
            {
                if (profile.shootingOnMove)
                {
                    motionState = MotionState.MoveToTarget;
                }
                else
                {
                    motionState = MotionState.Stay;
                }    
            }
        }
        else if (target.targetSeen)
        {
            motionState = MotionState.MoveToTarget;
        }
        else
        {
            motionState = MotionState.Stay;
        }
    }

/*    public void UpdateState()
    {
        if (sleep.onSleep)
        {
            actionState = ActionState.Sleep;
            motionState = MotionState.Sleep;
            return;
        }

        if (move.onPush)
        {
            actionState = ActionState.Stun;
            motionState = MotionState.Stay;
            return;
        }

*//*        if (!target.targetSeen && pursue.canPursue)
        {
            actionState = ActionState.Pursue;
            motionState = MotionState.Pursue;
            return;
        }*//*

        if (shooting.IsMagazineEmpty())
        {
            actionState = ActionState.Reload;

            if (target.targetSeen)
            {
                motionState = MotionState.MoveToTarget;
            }
            else
            {
                motionState = MotionState.Stay;
            }
        }

        if (!shooting.IsMagazineEmpty() && !shooting.onCooldown)
        {
            actionState = GetShootingBehaviourState();
            motionState = GetShootingMoveState();
        }
        else
        {

        }
    }

    MotionState GetShootingMoveState()
    {
        if (!target.targetSeen || targetApproached)
        {
            return MotionState.Stay;
        }
        else if (inShootingRange || !shooting.onAttack)
        {
            if (profile.shootingOnMove)
            {
                return MotionState.MoveToTarget;
            }
            else
            {
                return MotionState.Stay;
            }
        }
        else
        {
            return MotionState.MoveToTarget;
        }
    }

    ActionState GetShootingBehaviourState()
    {
        if (inShootingRange)
        {
            return ActionState.Shoot;
        }
        else
        {
            return ActionState.Idle;
        }
    }*/

    void ShootingHandler()
    {
        if (!shooting.onCooldown && !shooting.onReload && !shooting.onAttack)
        {
            shooting.ShootingManager();
        }
    }

    void ReloadHandler()
    {
        if (!shooting.onReload)
        {
            shooting.ReloadManager();
        }
    }

    public void Observe()
    {
        Vector2 dir = target.target.position - transform.position;

        LookToPoint(dir, profile.sight, masks, ref target.targetSeen);
        LookToPoint(dir, profile.shootingRange, masks, ref inShootingRange);
        LookToPoint(dir, profile.approachedDistance, masks, ref targetApproached);
    }

    public void LookToPoint(in Vector2 dir, in float length, in LayerMask masks, ref bool boolFlag)
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, dir.normalized, length, masks);

        if (hit)
        {
            boolFlag = hit.transform.CompareTag(target.target.tag);
        }
        else
        {
            boolFlag = false;
        }
    }

    public void OnDrawGizmosSelected()
    {
        Vector2 shootingDirection = Vector2.zero;
        Vector2 aimingDirection = Vector2.zero;
        if (target != null)
        {
            shootingDirection = (target.target.position - transform.position).normalized;
            aimingDirection = (target.target.position - transform.position).normalized;
        }

        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(transform.position, aimingDirection * profile.sight);

        Gizmos.color = Color.green;
        Gizmos.DrawRay(transform.position, shootingDirection * profile.shootingRange);

        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, shootingDirection * profile.approachedDistance);
    }
}
