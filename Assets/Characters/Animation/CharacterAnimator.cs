namespace Characters.Animation {
  using UnityEngine;

  public class CharacterAnimator : MonoBehaviour {
    public bool Attacking;
    private Animator _animator;

    private void Start() {
      Attacking = false;
      _animator = GetComponent<Animator>();
    }

    private void Update() {
      if (!_animator.GetCurrentAnimatorStateInfo(0).IsName("Attack") && !_animator.IsInTransition(0)) {
        Attacking = false;
      }
    }

    public void Attack() {
      _animator.Play("Attack");
    }
  }
}