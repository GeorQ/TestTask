using UnityEngine;


public interface ILaunch
{
    Transform StartPoint { get; }
    void Launch(float holdTime);
}