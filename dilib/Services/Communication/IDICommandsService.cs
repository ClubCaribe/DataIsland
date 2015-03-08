using System;
namespace dilib.Services.Communication
{
    public interface IDICommandsService
    {
        dynamic User(string senderId, string user);
    }
}
