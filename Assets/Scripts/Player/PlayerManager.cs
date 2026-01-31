using UnityEngine;
using UnityEngine.Events;

public class PlayerManager : MonoBehaviour
{
    #region Events

        public UnityAction OnMove;
        public UnityAction OnJump;
        public UnityAction OnDoubleJump;
        public UnityAction OnDash;
        public UnityAction OnAttack;
        public UnityAction OnHeavyAttack;
        public UnityAction OnComboAttack;
        public UnityAction OnDashAttack;
        public UnityAction OnJumpAttack;
        public UnityAction OnParry;

    #endregion
}