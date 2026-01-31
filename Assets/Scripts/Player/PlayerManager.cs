using UnityEngine;
using UnityEngine.Events;

public class PlayerManager : MonoBehaviour
{
    #region Events

        public UnityAction OnMove;
        public UnityAction OnJump;
        public UnityAction OnDoubleJump;
        public UnityAction OnDash;

    #endregion
}