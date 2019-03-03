namespace Characters.NPCs.Controllers {
  using System.Collections.Generic;
  using UnityEngine;
  using UnityEngine.AI;
  using Lerocia.Characters;

  public class NPCController : MonoBehaviour {
    private float _lookRadius = 10f;
    private Transform _target;
    private NavMeshAgent _agent;
    public List<string> TargetTypes;

    private void Start() {
      _agent = GetComponent<NavMeshAgent>();
    }

    private void Update() {
      if (TargetTypes != null) {
        float closestDistance = float.MaxValue;
        bool foundTarget = false;
        foreach (Character character in ConnectedCharacters.Characters.Values) {
          if (TargetTypes.Contains(character.CharacterPersonality)) {
            float distance = Vector3.Distance(character.Avatar.transform.position, transform.position);

            if (distance < _lookRadius && distance < closestDistance) {
              closestDistance = distance;
              foundTarget = true;
              _target = character.Avatar.transform;
            }
          }
        }

        if (foundTarget) {
          _agent.SetDestination(_target.position);
          if (closestDistance <= _agent.stoppingDistance) {
            //TODO Attack target
            FaceTarget();
          }
        } else {
          _target = null;
        }
      }
    }

    private void FaceTarget() {
      Vector3 direction = (_target.position - transform.position).normalized;
      Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
      transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
    }

    public void Destroy() {
      Destroy(gameObject);
    }

    private void OnDrawGizmosSelected() {
      Gizmos.color = Color.red;
      Gizmos.DrawWireSphere(transform.position, _lookRadius);
    }
  }
}