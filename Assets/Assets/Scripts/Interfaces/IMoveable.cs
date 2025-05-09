using UnityEngine;

public interface IMoveable
{
    public void Move(ref Rigidbody2D rb, in Vector2 dir, in float speed);
    public void StopMovement(ref Rigidbody2D rb);
}
