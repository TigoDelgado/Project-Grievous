using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine.Events;
using UnityEngine;

[RequireComponent(typeof(Turret))]
public class TurretAI : MonoBehaviour
{

    [Header("Parameters")]
    [Tooltip("The Y height at which the enemy will be automatically killed (if it falls off of the level)")]
    public float SelfDestructYHeight = -20f;

    [Tooltip("The distance at which the enemy considers that it has reached its current path destination point")]
    public float PathReachingRadius = 2f;

    [Tooltip("The speed at which the enemy rotates")]
    public float OrientationSpeed = 10f;

    [Tooltip("Delay after death where the GameObject is destroyed (to allow for animation)")]
    public float DeathDuration = 0f;

    [Tooltip("Time delay between attacks")]
    public float DelayBetweenPositionUpdates = 100f;

    [Tooltip("Time delay between attacks")]
    public float DelayBetweenShots = 3f;

    [Header("Eye color")]
    [Tooltip("Material for the eye color")]
    public Material EyeColorMaterial;

    [Tooltip("The default color of the bot's eye")]
    [ColorUsageAttribute(true, true)]
    public Color DefaultEyeColor;

    [Tooltip("The attack color of the bot's eye")]
    [ColorUsageAttribute(true, true)]
    public Color AttackEyeColor;

    [Header("Flash on hit")]
    [Tooltip("The material used for the body of the hoverbot")]
    public Material BodyMaterial;

    [Tooltip("The gradient representing the color of the flash on hit")]
    [GradientUsageAttribute(true)]
    public Gradient OnHitBodyGradient;

    [Tooltip("The duration of the flash on hit")]
    public float FlashOnHitDuration = 0.5f;

    [Header("Sounds")]
    [Tooltip("Sound played when recieving damages")]
    public AudioClip DamageTick;

    [Header("VFX")]
    [Tooltip("The VFX prefab spawned when the enemy dies")]
    public GameObject DeathVfx;

    [Tooltip("The point at which the death VFX is spawned")]
    public Transform DeathVfxSpawnPoint;

    [Header("Debug Display")]
    [Tooltip("Color of the sphere gizmo representing the path reaching range")]
    public Color PathReachingRangeColor = Color.yellow;

    [Tooltip("Color of the sphere gizmo representing the attack range")]
    public Color AttackRangeColor = Color.red;

    [Tooltip("Color of the sphere gizmo representing the detection range")]
    public Color DetectionRangeColor = Color.blue;

    [Header("Projectile")]
    [Tooltip("Projectile prefab")]
    public Transform pfProjectile;

    [SerializeField] Transform center;

    public UnityAction onAttack;
    public UnityAction onDetectedTarget;
    public UnityAction onLostTarget;
    public UnityAction onDamaged;

    private float lastFireTime = 0.0f;
    private int currentPositionUpdate = 0;
    private Vector3 lastKnownPosition;

    public DetectionModule detectionModule { get; private set; }

    GameObject player;
    Collider[] m_SelfColliders;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player");
        m_SelfColliders = GetComponentsInChildren<Collider>();


        // Initialize detection module
        detectionModule = GetComponent<DetectionModule>();
        detectionModule.onDetectedTarget += OnDetectedTarget;
        detectionModule.onLostTarget += OnLostTarget;
        onAttack += detectionModule.OnAttack;
    }

    // Update is called once per frame
    void Update()
    {
        detectionModule.HandleTargetDetection(this, m_SelfColliders);

        if (detectionModule.IsSeeingTarget && detectionModule.IsTargetInAttackRange)
        {
            if (++currentPositionUpdate % DelayBetweenPositionUpdates == 0)
            {
                lastKnownPosition = detectionModule.KnownDetectedTarget.transform.position;
                currentPositionUpdate = 0;
            } 
            OrientTowards(lastKnownPosition);
            TryAtack(lastKnownPosition);
        }
        //Color currentColor = OnHitBodyGradient.Evaluate((Time.time - m_LastTimeDamaged) / FlashOnHitDuration);
        //m_BodyFlashMaterialPropertyBlock.SetColor("_EmissionColor", currentColor);
        //foreach (var data in m_BodyRenderers)
        //{
        //    data.Renderer.SetPropertyBlock(m_BodyFlashMaterialPropertyBlock, data.MaterialIndex);
        //}

        //m_WasDamagedThisFrame = false;
    }

    void OnLostTarget()
    {
        Debug.Log("target lost");
    }

    void OnDetectedTarget()
    {
        //onDetectedTarget.Invoke();
        OrientTowards(detectionModule.KnownDetectedTarget.transform.position);
        //// Set the eye default color and property block if the eye renderer is set
        //if (m_EyeRendererData.Renderer != null)
        //{
        //    m_EyeColorMaterialPropertyBlock.SetColor("_EmissionColor", AttackEyeColor);
        //    m_EyeRendererData.Renderer.SetPropertyBlock(m_EyeColorMaterialPropertyBlock,
        //        m_EyeRendererData.MaterialIndex);
        //}
    }

    public void OrientTowards(Vector3 lookPosition)
    {
        Vector3 lookDirection = Vector3.ProjectOnPlane(lookPosition - transform.position, Vector3.up).normalized;
        if (lookDirection.sqrMagnitude != 0f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
            transform.rotation =
                Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * OrientationSpeed);
        }
    }

    void OnDamaged(float damage, GameObject damageSource)
    {
        // TODO
    }

    void Die()
    {
        OnDie();
    }

    void OnDie()
    {
        Destroy(gameObject, DeathDuration);
    }

    void OnDrawGizmosSelected()
    {
        //// Path reaching range
        //Gizmos.color = PathReachingRangeColor;
        //Gizmos.DrawWireSphere(transform.position, PathReachingRadius);

        //if (DetectionModule != null)
        //{
        //    // Detection range
        //    Gizmos.color = DetectionRangeColor;
        //    Gizmos.DrawWireSphere(transform.position, DetectionModule.DetectionRange);

        //    // Attack range
        //    Gizmos.color = AttackRangeColor;
        //    Gizmos.DrawWireSphere(transform.position, DetectionModule.AttackRange);
        //}
    }

    public bool TryAtack(Vector3 enemyPosition)
    {
        if ((lastFireTime + DelayBetweenShots) < Time.time)
        {
            Vector3 initPosition = center.position;
            Transform projectileTransform = Instantiate(pfProjectile, initPosition, Quaternion.identity);
            Vector3 shootDirection = enemyPosition - initPosition;
            projectileTransform.GetComponent<Projectile>().Setup(shootDirection);
            lastFireTime = Time.time;
            return true;
        }
        return false;
    }
}
