using UnityEngine;

[ExecuteInEditMode]
public class PendulumPreview3D : MonoBehaviour
{
    [Header("Geometry")]
    [SerializeField] private Transform _pivot;          // точка подвеса (неподвижна)
    [SerializeField] private Transform _bob;            // подвижная точка (груз)
    [SerializeField] private float _ropeLength = 10f;   // длина верёвки

    [Header("Physics")]
    [SerializeField] private float _gravity = 9.81f;    // g
    [SerializeField] private float _damping = 0.1f;     // коэффициент затухания

    [Header("Initial state")]
    [SerializeField] private float _initialAngleDeg = 30f; // градусов от вертикали

    [Header("External control")]
    [Tooltip("Угловая скорость, которую можно менять извне (рад/с).")]
    [SerializeField] private float _velocity = 0f;     // наш новый параметр

    [Header("Optional input (joystick/keyboard)")]
    [SerializeField] private bool _useInput = true;
    [SerializeField] private string _inputAxis = "Horizontal";
    [SerializeField] private float _inputForce = 8f;   // как сильно ввод влияет на _velocity

    // Внутренние переменные
    private float _theta;   // текущий угол от вертикали (рад)
    private float _omega;   // текущая угловая скорость (рад/с)

    // Плоскость качания (по умолчанию XZ → качание влево‑вправо)
    [SerializeField] private Vector3 _planeNormal = Vector3.forward;

    private void Reset()
    {
        // Автоматически заполняем поля, если они пусты
        if (_pivot == null) _pivot = transform;
        if (_bob == null)
        {
            GameObject go = new GameObject("Bob");
            go.transform.SetParent(transform);
            _bob = go.transform;
        }
    }

    private void OnValidate()
    {
        // Гарантируем корректную длину при изменении в инспекторе
        _ropeLength = Mathf.Max(0.1f, _ropeLength);
    }

    private void Start()
    {
        InitPendulum();
    }

    private void InitPendulum()
    {
        // Начальный угол
        _theta = _initialAngleDeg * Mathf.Deg2Rad;

        // Если bob уже где‑то находится – пересчитаем угол из реального положения
        if (_pivot && _bob)
        {
            Vector3 toBob = _bob.position - _pivot.position;
            float vertical = -toBob.y;                     // положительная вниз
            float horizontal = Vector3.ProjectOnPlane(toBob, _planeNormal).magnitude;
            if (horizontal > 0.001f)
                _theta = Mathf.Atan2(horizontal, vertical);
            else
                _theta = 0f;
        }

        _omega = 0f;
        UpdateBobPosition();
    }

    private void FixedUpdate()
    {
        // 1. Внешний ввод (джойстик/клавиатура)
        if (_useInput)
        {
            float axis = Input.GetAxis(_inputAxis);
            _velocity += axis * _inputForce * Time.fixedDeltaTime;
        }

        // 2. Добавляем внешнюю скорость к текущей угловой скорости
        _omega += _velocity * Time.fixedDeltaTime;

        // 3. Обычная физика маятника
        float sinTheta = Mathf.Sin(_theta);
        float alpha = -(_gravity / _ropeLength) * sinTheta - _damping * _omega;

        _omega += alpha * Time.fixedDeltaTime;
        _theta += _omega * Time.fixedDeltaTime;

        // Ограничиваем угол, чтобы маятник не переворачивался
        _theta = Mathf.Clamp(_theta, -Mathf.PI * 0.99f, Mathf.PI * 0.99f);

        UpdateBobPosition();

        // Сбрасываем внешнюю скорость (она «импульсная», а не постоянная)
        _velocity = 0f;
    }

    /// <summary>
    /// Перемещает _bob в правильную позицию согласно текущему углу.
    /// </summary>
    private void UpdateBobPosition()
    {
        if (!_pivot || !_bob) return;

        // Направление оси качания (в плоскости, перпендикулярно нормали)
        Vector3 swingAxis = Vector3.Cross(_planeNormal, Vector3.up).normalized;

        // Поворачиваем ось на угол _theta вокруг нормали плоскости
        Vector3 dir = Quaternion.AngleAxis(_theta * Mathf.Rad2Deg, _planeNormal) * swingAxis;

        // Вычисляем смещение от вертикали
        float x = _ropeLength * Mathf.Sin(_theta);
        float y = -_ropeLength * Mathf.Cos(_theta); // вниз

        Vector3 offset = dir * x + Vector3.up * y;
        _bob.position = _pivot.position + offset;
    }

    // --------------------------------------------------------------------
    // Визуализация в редакторе
    // --------------------------------------------------------------------
    private void OnDrawGizmosSelected()
    {
        if (_pivot && _bob)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(_pivot.position, _bob.position);

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(_pivot.position, 0.3f);
            Gizmos.DrawWireSphere(_bob.position, 0.3f);
        }
    }

    // --------------------------------------------------------------------
    // Публичные методы – удобно вызывать из других скриптов
    // --------------------------------------------------------------------
    /// <summary>
    /// Добавляет мгновенный импульс угловой скорости.
    /// </summary>
    public void AddVelocity(float radPerSec)
    {
        _velocity += radPerSec;
    }

    /// <summary>
    /// Сбрасывает маятник в начальное состояние.
    /// </summary>
    public void ResetPendulum()
    {
        InitPendulum();
    }
}