using System;
using Unity.VisualScripting;
using UnityEngine;


public interface IInput
{
    Vector3 GetPointerDelta();
    event Action<Vector3> ClickDown;
    event Action<Vector3> ClickUp;
    event Action<Vector3> ClickHold;
}