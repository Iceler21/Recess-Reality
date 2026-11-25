using UnityEngine;

public class Rotator : MonoBehaviour
{
    public float rotationSpeed = 50f;

        private void Update()
    {
        transform.Rotate(rotationSpeed * Time.deltaTime, rotationSpeed * Time.deltaTime, 0);
    }
}