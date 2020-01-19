using System;

namespace Core.Services.Players
{
    public interface IPlayerMediaService
    {
        void Play();
        void Pause();
        void Stop();
        TimeSpan Forward(TimeSpan timeSpan);
    }
}