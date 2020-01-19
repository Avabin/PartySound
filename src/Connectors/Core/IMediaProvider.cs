using System;
using System.Collections.Generic;
using Core.Services;

namespace Core
{
    public interface IMediaProvider
    {
        IObservable<IError> ErrorObservable { get; }
        void Initialize();

        IMediaService GetMediaService<T>() where T : IMediaService;
        IEnumerable<IMediaService> GetUsableMediaServices();
    }
}