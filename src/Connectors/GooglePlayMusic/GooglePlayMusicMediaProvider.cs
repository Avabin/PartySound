using System;
using System.Collections.Generic;
using System.Reactive.Subjects;
using Core;
using Core.Services;

namespace GooglePlayMusic
{
    public class GooglePlayMusicMediaProvider : IMediaProvider
    {
        private readonly ISubject<IError> _errorSubject;

        public IObservable<IError> ErrorObservable { get; }

        public GooglePlayMusicMediaProvider()
        {
            _errorSubject = new Subject<IError>();
            ErrorObservable = _errorSubject;
        }

        public void Initialize()
        {

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