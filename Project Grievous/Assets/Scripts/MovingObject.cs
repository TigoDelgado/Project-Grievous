using UnityEngine;

public class MovingObject : MonoBehaviour
{
    [SerializeField] private Transform _targetA, _targetB;
    [SerializeField] float _speed = 1.0f;
    private bool _switching = false;

    // Update is called once per frame
    void FixedUpdate()
    {
        if (_switching == false)
        {
            transform.position = Vector3.MoveTowards(transform.position, _targetB.position, _speed * Time.deltaTime);
        }
        else if (_switching)
        {
            transform.position = Vector3.MoveTowards(transform.position, _targetA.position, _speed * Time.deltaTime);
        }
        if (transform.position == _targetB.position)
        {
            _switching = true;
        }
        else if (transform.position == _targetA.position)
        {
            _switching = false;
        }
    }
}
