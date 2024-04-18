using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrajectoryPredictorMediator : IDisposable
{
    private TrajectoryPredictor _predictor;
    private TimeCounter _timeCounter;

    public TrajectoryPredictorMediator(TrajectoryPredictor predictor, TimeCounter timeCounter)
    {
        _predictor = predictor;
        _timeCounter = timeCounter;
        _timeCounter.TimerUpdated += OnTimerUpdated;
        _timeCounter.TimerEnd += OnTimerEnded;
        _timeCounter.TimerStarted += OnTimerStarted;
    }

    private void OnTimerUpdated(float holdTime)
    {
        _predictor.PredictTrajectory(holdTime);
    }

    private void OnTimerEnded(float holdTime)
    {
        _predictor.SetProjectoryActiveStatus(false);
    }

    private void OnTimerStarted()
    {
        _predictor.SetProjectoryActiveStatus(true);
    }

    public void Dispose()
    {
        _timeCounter.TimerStarted -= OnTimerStarted;
        _timeCounter.TimerUpdated -= OnTimerUpdated;
        _timeCounter.TimerEnd -= OnTimerEnded;
    }
}