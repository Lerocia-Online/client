namespace Players.Animation {
  using UnityEngine;

  public class PlayerAnimator : MonoBehaviour {
    public bool Attacking;
    public bool Charging;
    private Animator _animator;

    private void Start() {
      Attacking = false;
      Charging = false;
      _animator = GetComponent<Animator>();
    }

    private void Update() {
      if (!_animator.GetCurrentAnimatorStateInfo(0).IsName("Attack") && !_animator.IsInTransition(0)) {
        Attacking = false;
      }
    }

    public void Attack() {
      Charging = false;
      Attacking = true;
      _animator.SetTrigger("Attack");
    }

    public void Charge() {
      Charging = true;
      _animator.Play("Charge");
    }
  }
}