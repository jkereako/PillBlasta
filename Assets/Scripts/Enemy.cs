using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Enemy: LiveEntity {
  enum State {
    Idle,
    Chasing,
    Attacking}

  ;

  public ParticleSystem deathEffect;

  State state = State.Idle;
  NavMeshAgent pathFinder;
  Transform target;
  LiveEntity targetEntity;
  const float attackDistanceThreshold = 0.5f;
  const float attackWaitValue = 1.0f;
  float attackDamage = 1.0f;
  float nextAttackTime;
  float collisionRadius;
  float targetCollisionRadius;
  bool hasTarget;

  void Awake() {
    if (GameObject.FindGameObjectWithTag("Player") == null) {
      hasTarget = false;
      return;
    }
      
    target = GameObject.FindGameObjectWithTag("Player").transform;
    pathFinder = GetComponent<NavMeshAgent>();
    targetEntity = target.GetComponent<LiveEntity>();
    collisionRadius = GetComponent<CapsuleCollider>().radius;
    targetCollisionRadius = target.GetComponent<CapsuleCollider>().radius;
    hasTarget = true;
    nextAttackTime = Time.time;
    targetEntity.OnDeath += OnTargetDeath;
  }

  protected override void Start() {
    base.Start();

    if (hasTarget) {
      state = State.Chasing;
      StartCoroutine(UpdatePath());
    }
  }

  public override void TakeHit(float damage, RaycastHit hit) {
    if (damage >= health) {
      GameObject particles;

      particles = Instantiate(
        deathEffect.gameObject,
        hit.point, 
        Quaternion.FromToRotation(hit.point, Vector3.forward)
      );

      Destroy(particles, 2);
    }

    base.TakeHit(damage, hit);
  }

  void Update() {
    if (!hasTarget || Time.time < nextAttackTime) {
      return;
    }

    float squareDistanceToTarget = (target.position - transform.position).sqrMagnitude;
    float dist = attackDistanceThreshold + collisionRadius + targetCollisionRadius;
    if (squareDistanceToTarget < Mathf.Pow(dist, 2)) {
      nextAttackTime = Time.time + attackWaitValue;
      StartCoroutine(Attack());
    }
  }

  public void SetTrait(EntityTrait trait) {
    GetComponent<Renderer>().material.color = trait.color;
    pathFinder.speed = trait.locomotiveSpeed;
    health = trait.health;
    attackDamage = trait.attackDamage;
  }

  void OnTargetDeath() {
    hasTarget = false;
    state = State.Idle;
  }

  IEnumerator Attack() {
    Material material = GetComponent<Renderer>().material;
    Color originalColor = material.color;
    state = State.Attacking;
    pathFinder.enabled = false;

    Vector3 originalPosition = transform.position;
    Vector3 directionToTarget = (target.position - transform.position).normalized;
    Vector3 attackPosition = target.position - directionToTarget * collisionRadius;
    
    const float attackSpeed = 3.0f;
    float percentCompleted = 0.0f;
    material.color = Color.red;
    bool hasAppliedDamage = false;

    while (percentCompleted <= 1) {
      if (percentCompleted > 0.5f && !hasAppliedDamage) {
        hasAppliedDamage = true;
        targetEntity.TakeDamage(attackDamage);
      }

      percentCompleted += Time.deltaTime * attackSpeed;
      // `interpolation` is a point on a porabola. Since the enemy will lunge forward and then 
      // receed, we need to increase a value gradually from zero and then decrease it back to zero.
      // This is expressable as the equation `y = 4(-x^2 + x)`.
      float interpolation = (-Mathf.Pow(percentCompleted, 2) + percentCompleted) * 4;
      transform.position = Vector3.Lerp(originalPosition, attackPosition, interpolation);

      yield return null;
    }

    material.color = originalColor;
    state = State.Chasing;
    pathFinder.enabled = true;
  }

  IEnumerator UpdatePath() {
    const float delay = 0.25f;

    while (hasTarget) {
      if (state == State.Chasing) {
        Vector3 directionToTarget = (target.position - transform.position).normalized;
        Vector3 targetPosition = target.position - directionToTarget *
                                 (collisionRadius + targetCollisionRadius + attackDistanceThreshold / 2);

        if (isAlive) {
          pathFinder.SetDestination(targetPosition); 
        } 
      }

      yield return new WaitForSeconds(delay);
    }
  }
}
