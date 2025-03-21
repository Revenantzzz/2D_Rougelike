using System;
using System.Globalization;
using UnityEngine;
using UnityEngine.InputSystem.Controls;

namespace Rougelike2D
{
    public abstract class Timer
    {
        protected float initialTime;
        protected float currentTime {get; set;}
        public bool IsRunning {get; protected set;}

        public float progress => currentTime/initialTime;

        public event Action OnStartTimer = delegate{};
        public event Action OnStopTimer = delegate{};

        public Timer(float initialTime)
        {
            this.initialTime = initialTime;
        }
        public void StartTimer()
        {
            currentTime = initialTime;
            if(!IsRunning)
            {
                IsRunning = true;
                OnStartTimer?.Invoke();
            }
        }
        public void StopTimer()
        {
            if(IsRunning)
            {
                IsRunning = false;
                OnStopTimer?.Invoke();
            }
        }

        public void Resume() => IsRunning = true;
        public void Pause() => IsRunning = false;
        
        public abstract void Tick(float deltaTime);

    }
    public class CountDownTimer : Timer
    {
        public CountDownTimer(float initialTime) : base(initialTime)
        {
        }

        public override void Tick(float deltaTime)
        {
            if(IsRunning && currentTime > 0)
            {
                currentTime -= deltaTime;
            }
            else if(IsRunning && currentTime <= 0)
            {
                StopTimer();
            }
        }

        public bool Finished => currentTime <= 0;
        public void Reset()
        {
            currentTime = initialTime;
        }
        public void Reset(float newTime)
        {
            initialTime = newTime;
            Reset();
        }
    }
    public class StopWatch : Timer
    {
        public StopWatch(float initialTime) : base(0)
        {
        }


        public override void Tick(float deltaTime)
        {
            if(IsRunning)
            {
                currentTime += deltaTime;
            }
        }
        public void Reset() => initialTime = 0;

        public float GetTime() => currentTime;
    }
}
