using System.Collections.Generic;

namespace Core.Services
{
    public interface IMediaService
    {
        IMediaProvider Parent { get; }
    }
}