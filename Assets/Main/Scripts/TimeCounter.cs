using System;
using UnityEngine;


public class TimeCounter : IDisposable
{
    public event Action<float> TimerEnd;
    public event Action<float> TimerUpdated;
    public event Action TimerStarted;

    private IInput _input;
    private float _timer;


    public TimeCounter(IInput input)
    {
        _input = input;
        _input.ClickUp += OnClickUp;
        _input.ClickDown += OnClickDown;
        _input.ClickHold += OnClickHold;
    }

    private void OnClickDown(Vector3 mousePos)
    {
        _timer = 0;
        TimerStarted?.Invoke();
    }

    private void OnClickUp(Vector3 mousePos)
    {
        TimerEnd?.Invoke(_timer);
    }

    private void OnClickHold(Vector3 mousePos)
    {
        _timer += Time.deltaTime;
        TimerUpdated?.Invoke(_timer);
    }

    public void Dispose()
    {
        _input.ClickUp -= OnClickUp;
        _input.ClickDown -= OnClickDown;
        _input.ClickHold -= OnClickHold;
    }
}