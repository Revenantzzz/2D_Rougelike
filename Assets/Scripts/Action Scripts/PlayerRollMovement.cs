using UnityEngine;

namespace Rougelike2D
{
    public class PlayerRollMovement : BaseRollMovement
    {
        PlayerController _playerController;
        PlayerMovementStats _movementStats;
        public PlayerRollMovement(PlayerController controller, Rigidbody2D rb, PlayerMovementStats movementStats) : base(rb)
        {
            _movementStats = movementStats;
            _playerController = controller;
        }

        public override void HandleRoll(float dir)
        {
            _playerController.RollPressTimer.StopTimer();
            var currentGravity = _rb.gravityScale;
            _rb.gravityScale = 0;

            float force = _movementStats.RollAmount;
            _rb.AddForce(Vector2.right * dir * force, ForceMode2D.Impulse);
        }
        #region Roll
        public override void RollCheck()
        {
            if (_playerController.RollPressTimer.IsRunning && !_playerController.RollTimer.IsRunning && !_playerController.RollCoolDownTimer.IsRunning)
            {
                _playerController.RollTimer.StartTimer();
            }
        }
        #endregion
    }

}
