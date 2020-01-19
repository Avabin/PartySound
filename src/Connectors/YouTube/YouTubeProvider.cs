using System;
using System.Collections.Generic;
using System.Reactive.Subjects;
using Core;
using Core.Services;

namespace YouTube
{
    public class YouTubeProvider : IMediaProvider
    {
        private readonly ISubject<IError> _errorSubject;

        public YouTubeProvider()
        {
            _errorSubject = new Subject<IError>();
            ErrorObservable = _errorSubject;
        }

        public IObservable<IError> ErrorObservable { get; }
        public void Initialize()
        {
            throw new NotImplementedException();
        }

        public IMediaService GetMediaService<T>() where T : IMediaService
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IMediaService> GetUsableMediaServices()
        {
            throw new NotImplementedException();
        }
    }
}