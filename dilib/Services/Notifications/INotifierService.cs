using System;
using System.Threading.Tasks;
namespace dataislandcommon.Services.Notifications
{
    public interface INotifierService
    {
        Task SendNotification(string notificatorName, string rendererName, string title, string iconCssClass, string notification, string ownerUsername);
    }
}
