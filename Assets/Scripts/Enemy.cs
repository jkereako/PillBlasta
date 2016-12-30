using UnityEngine;
using UnityEngine.AI;
using System.Collections;

[RequireComponent(typeof(NavMeshAgent))]
public class Enemy: MonoBehaviour {

  NavMeshAgent pathFinder;
  Transform target;

  void Start() {
    pathFinder = GetComponent<NavMeshAgent>();
    target = GameObject.FindGameObjectWithTag("Player").transform;

    StartCoroutine(UpdatePath());
  }

  void Update() {
//    pathFinder.SetDestination(target.position);
  }

  IEnumerator UpdatePath() {
    const float refreshRate = 0.25f;

    while (target != null) {
      Vector3 targetPosition = new Vector3(target.position.x, 0, target.position.z);
      pathFinder.SetDestination(targetPosition);

      yield return new WaitForSeconds(refreshRate);
    }
  }
}
