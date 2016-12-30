using UnityEngine;
using UnityEngine.AI;
using System.Collections;

[RequireComponent(typeof(NavMeshAgent))]
public class Enemy: LiveEntity {

  NavMeshAgent pathFinder;
  Transform target;

  protected override void Start() {
    base.Start();
    pathFinder = GetComponent<NavMeshAgent>();
    target = GameObject.FindGameObjectWithTag("Player").transform;

    StartCoroutine(UpdatePath());
  }

  IEnumerator UpdatePath() {
    const float refreshRate = 0.25f;

    while (target != null) {
      Vector3 targetPosition = new Vector3(target.position.x, 0, target.position.z);

      if (isAlive) {
        pathFinder.SetDestination(targetPosition); 
      }
        
      yield return new WaitForSeconds(refreshRate);
    }
  }
}
