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

  State state;
  NavMeshAgent pathFinder;
  Material material;
  Color originalColor;
  Transform target;
  float attackDistanceThreshold = 0.5f;
  float attackWaitValue = 1.0f;
  float nextAttackTime;
  float collisionRadius;
  float targetCollisionRadius;

  protected override void Start() {
    base.Start();
    nextAttackTime = Time.time;
    state = State.Chasing;
    pathFinder = GetComponent<NavMeshAgent>();
    material = GetComponent<Renderer>().material;
    originalColor = material.color;
    target = GameObject.FindGameObjectWithTag("Player").transform;
    collisionRadius = GetComponent<CapsuleCollider>().radius;
    targetCollisionRadius = target.GetComponent<CapsuleCollider>().radius;
    StartCoroutine(UpdatePath());
  }

  void Update() {
    if (Time.time < nextAttackTime) {
      return;
    }

    float squareDistanceToTarget = (target.position - transform.position).sqrMagnitude;
    float dist = attackDistanceThreshold + collisionRadius + targetCollisionRadius;
    if (squareDistanceToTarget < Mathf.Pow(dist, 2)) {
      nextAttackTime = Time.time + attackWaitValue;
      StartCoroutine(Attack());
    }
  }

  IEnumerator Attack() {
    state = State.Attacking;
    pathFinder.enabled = false;

    Vector3 originalPosition = transform.position;
    Vector3 directionToTarget = (target.position - transform.position).normalized;
    Vector3 attackPosition = target.position - directionToTarget * collisionRadius;
    
    float attackSpeed = 3.0f;
    float percentCompleted = 0.0f;
    material.color = Color.red;

    while (percentCompleted <= 1) {
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
    const float refreshRate = 0.25f;

    while (target != null) {
      if (state == State.Chasing) {
        Vector3 directionToTarget = (target.position - transform.position).normalized;
        Vector3 targetPosition = target.position - directionToTarget *
                                 (collisionRadius + targetCollisionRadius + attackDistanceThreshold / 2);

        if (isAlive) {
          pathFinder.SetDestination(targetPosition); 
        } 
      }

      yield return new WaitForSeconds(refreshRate);
    }
  }
}
