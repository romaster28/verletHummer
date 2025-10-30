using UnityEngine;

public class CharacterView : MonoBehaviour
{
    [SerializeField] private Transform _head;

    public Transform Head => _head;

    public void UpdatePosition(Vector3 position)
    {
        transform.position = position;
    }

    public void UpdateRotation(Quaternion rotation)
    {
        transform.rotation = rotation;
    }

    public void UpdateScale(Vector3 scale)
    {
        transform.localScale = scale;
    }
}