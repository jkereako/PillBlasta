using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using System;

[RequireComponent(typeof(NavMeshAgent))]
public class Enemy: LiveEntity {
  enum State {
    Idle,
    Chasing,
    Attacking}

  ;

  State state;
  NavMeshAgent pathFinder;
  Transform target;
  float attackDistanceThreshold = 1.5f;
  float attackWaitValue = 1.0f;
  float nextAttackTime;

  protected override void Start() {
    base.Start();
    nextAttackTime = Time.time;
    state = State.Chasing;
    pathFinder = GetComponent<NavMeshAgent>();
    target = GameObject.FindGameObjectWithTag("Player").transform;

    StartCoroutine(UpdatePath());
  }

  void Update() {
    if (Time.time < nextAttackTime) {
      return;
    }

    float squareDistanceToTarget = (target.position - transform.position).sqrMagnitude;

    if (squareDistanceToTarget < Mathf.Pow(attackDistanceThreshold, 2)) {
      nextAttackTime = Time.time + attackWaitValue;
      StartCoroutine(Attack());
    }
  }

  IEnumerator Attack() {
    state = State.Attacking;
    pathFinder.enabled = false;

    Vector3 originalPosition = transform.position;
    Vector3 attackPosition = target.position;
    float attackSpeed = 3.0f;
    float percentCompleted = 0.0f;

    while (percentCompleted <= 1) {
      percentCompleted += Time.deltaTime * attackSpeed;
      // `interpolation` is a point on a porabola. Since the enemy will lunge forward and then 
      // receed, we need to increase a value gradually from zero and then decrease it back to zero.
      // This is expressable as the equation `y = 4(-x^2 + x)`.
      float interpolation = (-Mathf.Pow(percentCompleted, 2) + percentCompleted) * 4;
      transform.position = Vector3.Lerp(originalPosition, attackPosition, interpolation);

      yield return null;
    }

    state = State.Chasing;
    pathFinder.enabled = true;
  }

  IEnumerator UpdatePath() {
    const float refreshRate = 0.25f;

    while (target != null) {
      if (state == State.Chasing) {
        Vector3 targetPosition = new Vector3(target.position.x, 0, target.position.z);

        if (isAlive) {
          pathFinder.SetDestination(targetPosition); 
        } 
      }

      yield return new WaitForSeconds(refreshRate);
    }
  }
}
