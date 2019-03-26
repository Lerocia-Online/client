namespace Characters.Animation {
  using UnityEngine;

  public class CharacterAnimator : MonoBehaviour {
    public GameObject CharacterObject;
    private Animator _animator;
    public bool Attacking;

    private void Awake() {
      _animator = CharacterObject.GetComponent<Animator>();
    }

    private void Update() {
      if (!_animator.GetCurrentAnimatorStateInfo(0).IsName("Attack") && !_animator.IsInTransition(0)) {
        Attacking = false;
      }
    }

    public void Attack() {
      Attacking = true;
      _animator.SetTrigger("Attack");
    }

    public void Die() {
      _animator.SetTrigger("Die");
    }

    public void Running(bool running) {
      _animator.SetBool("Running", running);
    }
  }
}