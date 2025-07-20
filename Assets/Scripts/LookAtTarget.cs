using UnityEngine;

public class LookAtTarget : MonoBehaviour
{
    [Tooltip("The target to look at. If not assigned, will use the main camera.")]
    [SerializeField] private Transform target;

    [Tooltip("Look at mode: Normal = direct facing, Inverted = facing away from target")]
    [SerializeField] private LookAtMode lookAtMode = LookAtMode.Normal;

    [Tooltip("Only rotate around these axes (X, Y, Z)")]
    [SerializeField] private Vector3 rotationMask = new Vector3(1, 1, 1);

    [Tooltip("Additional rotation offset applied after looking at target")]
    [SerializeField] private Vector3 rotationOffset = Vector3.zero;

    [Tooltip("How quickly to interpolate rotation (0 = instant)")]
    [SerializeField] private float rotationSpeed = 0;

    [Tooltip("Up direction for the look rotation")]
    [SerializeField] private Vector3 upDirection = Vector3.up;

    public enum LookAtMode { Normal, Inverted }

    private void Start()
    {
        // Default to main camera if no target specified
        if (target == null && Camera.main != null)
        {
            target = Camera.main.transform;
        }
    }

    private void LateUpdate()
    {
        if (target == null) return;

        // Calculate the direction to look
        Vector3 direction = (target.position - transform.position).normalized;
        if (lookAtMode == LookAtMode.Inverted)
        {
            direction = -direction;
        }

        // Apply rotation mask
        if (rotationMask != Vector3.one)
        {
            Vector3 currentForward = transform.forward;
            direction.x = rotationMask.x != 0 ? direction.x : currentForward.x;
            direction.y = rotationMask.y != 0 ? direction.y : currentForward.y;
            direction.z = rotationMask.z != 0 ? direction.z : currentForward.z;
            direction = direction.normalized;
        }

        // Calculate the target rotation
        Quaternion targetRotation = Quaternion.LookRotation(direction, upDirection);
        
        // Apply offset
        targetRotation *= Quaternion.Euler(rotationOffset);

        // Apply rotation 
        if (rotationSpeed <= 0)
        {
            transform.rotation = targetRotation;
        }
        else
        {
            transform.rotation = Quaternion.Slerp(
                transform.rotation, 
                targetRotation, 
                rotationSpeed * Time.deltaTime
            );
        }
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }
}
