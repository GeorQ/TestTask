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
    }

    private void OnTimerUpdated(float holdTime)
    {
        _predictor.PredictTrajectory(holdTime);
    }

    public void Dispose()
    {
        _timeCounter.TimerUpdated -= OnTimerUpdated;
    }
}