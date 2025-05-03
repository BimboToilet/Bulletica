using UnityEngine;
using EnemyState;
using static UnityEngine.UI.Image;
using Unity.VisualScripting;


[RequireComponent(typeof(SmartMovement))]
public class SmartEnemy : Gunman, IObservable, IStatable
{
    //public EnemyProfile profile;

    //EnemyState
    public ActionState actionState;
    public MotionState motionState;
    public SmartMovement smartMove;

    [SerializeField] private LayerMask masks;
    [SerializeField] private LayerMask sightMask;

    public BasePursue pursue;

    protected override void Awake()
    {
        base.Awake();

        pursue = GetComponent<BasePursue>();
        smartMove = GetComponent<SmartMovement>();
    }

    void OnEnable()
    {
        if (shooting)
        {
            Vector2 startDir = (target.target.position - transform.position).normalized;
            rotator.RotateInstantly(startDir);
        }
    }

    protected override void FixedUpdate()
    {
        Vector2 dir = target.target.position - transform.position;
        switch (motionState)
        {
            case MotionState.MoveToTarget:

                break;
            case MotionState.Stay:
                smartMove.Stop();
                break;
            case MotionState.Regroup:
                //move.Move(ref rb, dir, profile.moveSpeed);
                break;
            case MotionState.Pursue:
                smartMove.Outflank(target.target.position);
                break;
            case MotionState.Sleep:
                smartMove.Stop();
                break;
            default:
                smartMove.Stop();
                break;
        }
    }

    protected override void Update()
    {
        Vector2 dir = (target.target.position - transform.position).normalized;

        if (health.health == 0)
        {   
            death.Die(gameObject);
        }

        if (targetApproached)
        {
            move.Buffering();
        }

        Observe();
        UpdateActionState();
        UpdateMovingState();

        switch (actionState)
        {
            case ActionState.Shoot:
                ShootingHandler();
                rotator.Rotate(dir, shooting.GetRotationSpeed());
                move.Buffering();
                break;
            case ActionState.Reload:
                ReloadHandler();
                rotator.Rotate(dir, shooting.GetRotationSpeed());
                break;
            case ActionState.Idle:
                break;
            case ActionState.Pursue:
                rotator.Rotate(smartMove.GetMoveDir(), shooting.GetRotationSpeed());
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
        else if (target.targetSeen)
        {
            actionState = ActionState.Pursue;
        }
        else
        {
            actionState = ActionState.Idle;
        }
    }

    public void UpdateMovingState()
    {
        motionState = MotionState.Pursue;

        if (sleep.onSleep)
        {
            motionState = MotionState.Sleep;
            return;
        }
        else if (move.onPush || !move.CanMove())
        {
            motionState = MotionState.Stay;
            return;
        }
        else if (actionState == ActionState.Shoot || shooting.OnCooldown())
        {
            if (((ShootingProfile)profile).shootingOnMove && target.targetSeen)
            {
                motionState = MotionState.Pursue;
            }
            else
            {
                motionState = MotionState.Stay;
            }
        }
        else if (target.targetSeen && !smartMove.OnPosition())
        {
            motionState = MotionState.Pursue;
        }
        else
        {
            //������� ������� roaming
            motionState = MotionState.Stay;
        }
    }

    void ShootingHandler()
    {
        if (!shooting.OnCooldown() && !shooting.OnReload() && !shooting.OnAttack())
        {
            shooting.ShootingManager();
        }
    }

    void ReloadHandler()
    {
        if (!shooting.OnReload())
        {
            shooting.ReloadManager();
        }
    }

    public void Observe()
    {
        Vector3 origin = shooting.gunController.muzzle.position;
        Vector2 dir = origin - shooting.gunController.bulletSpawn.position;
        Vector2 sigtDir = target.target.position - transform.position;

        LookToPoint(transform.position, sigtDir, ((ShootingProfile)profile).sight, sightMask, ref target.targetSeen);

        WideProjectileCheck(dir, ((ShootingProfile)profile).shootingRange, masks, ref inShootingRange);
    }

    public void LookToPoint(in Vector3 origin, in Vector2 dir, in float length, in LayerMask masks, ref bool boolFlag)
    {
        RaycastHit2D hit = Physics2D.Raycast(origin, dir.normalized, length, masks);

        if (hit)
        {
            boolFlag = hit.transform.CompareTag(target.target.tag);
        }
        else
        {
            boolFlag = false;
        }
    }
    
    public void WideProjectileCheck(in Vector2 dir, in float length, in LayerMask masks, ref bool boolFlag)
    {
        Vector3 origin = shooting.gunController.muzzle.position;
        RaycastHit2D hit = Physics2D.CircleCast(origin, ((ShootingProfile)profile).bulletRadius, dir.normalized, length, masks);

        if (hit)
        {
            boolFlag = hit.transform.CompareTag(target.target.tag);
        }
        else
        {
            boolFlag = false;
        }
    }

    public virtual void OnDrawGizmosSelected()
    {
        return;

        Vector3 origin = shooting.gunController.muzzle.position;

        Vector2 shootingDirection = Vector2.zero;
        Vector2 aimingDirection = Vector2.zero;
        if (target != null)
        {
            shootingDirection = (origin - shooting.gunController.bulletSpawn.position).normalized;
            aimingDirection = (target.target.position - origin).normalized;
        }

        Vector2 sigtDir = (target.target.position - transform.position).normalized;

        //Gizmos.color = Color.yellow;
        //Gizmos.DrawRay(transform.position, aimingDirection * profile.sight);

        //Gizmos.color = Color.green;
        //Gizmos.DrawRay(transform.position, shootingDirection * ((ShootingProfile)profile).shootingRange);

        if (targetApproached)
        {
            Gizmos.color = Color.red;
        }
        else
        {
            Gizmos.color = Color.green;
        }

        Gizmos.DrawRay(origin, shootingDirection * ((ShootingProfile)profile).approachedDistance);

        if (target.targetSeen)
        {
            Gizmos.color = Color.blue;
        }
        else
        {
            Gizmos.color = Color.magenta;
        }

        //Gizmos.DrawRay(transform.position, sigtDir * ((ShootingProfile)profile).sight);

        if (inShootingRange)
        {
            Gizmos.color = Color.green;    // ������������� ���������
        }
        else 
        {
            Gizmos.color = Color.red; // � ��������� �� ������� - �����
        }
        return;

        // --- ������ ��� CircleCast ---


        Vector3 direction = (Vector3)shootingDirection;
        Vector3 endPoint = origin + direction * ((ShootingProfile)profile).shootingRange;

        // 1. ������ ��������� ����
        // Gizmos.DrawWireSphere(origin, circleRadius); // Sphere �������� ��� ���� � 2D
        DrawWireDisk(origin, ((ShootingProfile)profile).bulletRadius, Gizmos.color); // ����� "�������" ���

        // 2. ������ �������� ���� (� ����� ������������ ��� �� ����. ���������)
        // Gizmos.DrawWireSphere(endPoint, circleRadius);
        DrawWireDisk(endPoint, ((ShootingProfile)profile).bulletRadius, Gizmos.color);

        // 3. ������ �����, ����������� ���� ������
        // ��������� ������, ���������������� ����������� (� 2D)
        Vector3 perpendicular = (Vector3)(new Vector2(-direction.y, direction.x).normalized) * ((ShootingProfile)profile).bulletRadius;

        // ������ ��� ����� �� �����
        Gizmos.DrawLine(origin + perpendicular, endPoint + perpendicular);
        Gizmos.DrawLine(origin - perpendicular, endPoint - perpendicular);

        // 4. (�����������) ������ ����������� ����� �����
        //Gizmos.color = Color.gray; // ������� �� ����� ��� �������
        //Gizmos.DrawLine(origin, endPoint);
    }


    void DrawWireDisk(Vector3 position, float radius, Color color, int segments = 20)
    {
        Color previousColor = Gizmos.color;
        Gizmos.color = color;
        Vector3 startPoint = position + Vector3.right * radius; // �������� ������
        Vector3 lastPoint = startPoint;

        float angleStep = 360f / segments;
        for (int i = 1; i <= segments; i++)
        {
            float angle = i * angleStep * Mathf.Deg2Rad;
            Vector3 nextPoint = position + new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * radius;
            Gizmos.DrawLine(lastPoint, nextPoint);
            lastPoint = nextPoint;
        }
        Gizmos.color = previousColor; // ���������� �������� ���� Gizmos
    }

}
