using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class DetectionModule : MonoBehaviour
{

    [Tooltip("The max distance at which the enemy can see targets")]
    public float DetectionRange = 100f;

    [Tooltip("The max distance at which the enemy can attack its target")]
    public float AttackRange = 10f;

    [Tooltip("Time before an enemy abandons a known target that it can't see anymore")]
    public float KnownTargetTimeout = 4f;

    [Tooltip("Optional animator for OnShoot animations")]
    public Animator Animator;

    public UnityAction onDetectedTarget;
    public UnityAction onLostTarget;

    public GameObject KnownDetectedTarget { get; private set; }
    public bool IsTargetInAttackRange { get; private set; }
    public bool IsSeeingTarget { get; private set; }
    public bool HadKnownTarget { get; private set; }

    protected float TimeLastSeenTarget = Mathf.NegativeInfinity;

    GameObject player;

    const string k_AnimAttackParameter = "Attack";
    const string k_AnimOnDamagedParameter = "OnDamaged";

    protected virtual void Start()
    {
        player = GameObject.FindWithTag("Player");
    }

    public virtual void HandleTargetDetection(TurretAI turret, Collider[] selfColliders)
    {
        // Handle known target detection timeout
        if (KnownDetectedTarget && !IsSeeingTarget && (Time.time - TimeLastSeenTarget) > KnownTargetTimeout)
        {
            KnownDetectedTarget = null;
        }

        // Find the closest visible hostile actor
        float sqrDetectionRange = DetectionRange * DetectionRange;
        IsSeeingTarget = false;

        Vector3 turretPosition = turret.transform.GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(1).position;
        float playerDistance = (turretPosition - player.transform.position).magnitude;

        if (playerDistance < DetectionRange)
        {

            KnownDetectedTarget = player;
            bool foundValidHit = false;
            
            // Check for obstructions
            RaycastHit[] hits = Physics.RaycastAll(turretPosition,
               (player.transform.position - turretPosition).normalized, playerDistance,
               -1, QueryTriggerInteraction.Ignore);
            foreach (var hit in hits)
            {
                if (!selfColliders.Contains(hit.collider) 
                    && hit.collider.gameObject.name != player.name 
                    && hit.distance < playerDistance)
                {
                    foundValidHit = true;
                }
            }

            if (foundValidHit)
            {
                IsSeeingTarget = false;
            }
            else
            {
                KnownDetectedTarget = player;
                IsTargetInAttackRange = true;
                IsSeeingTarget = true;

             }
        }

        // Detection events
        if (!HadKnownTarget &&
            KnownDetectedTarget != null)
        {
            OnDetect();
        }

        if (HadKnownTarget &&
            KnownDetectedTarget == null)
        {
            OnLostTarget();
        }

        // Remember if we already knew a target (for next frame)
        HadKnownTarget = KnownDetectedTarget != null;
    }

    public virtual void OnLostTarget() => onLostTarget?.Invoke();

    public virtual void OnDetect() => onDetectedTarget?.Invoke();

    public virtual void OnDamaged(GameObject damageSource)
    {
        
    }

    public virtual void OnAttack()
    {

    }
}

