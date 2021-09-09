using System;
using System.Threading.Tasks;

namespace UDPConnection
{
    /// <summary>
    ///     Listens on a port for UDP traffic and can send UDP data to arbitrary endpoints.
    /// </summary>
    public interface IUdpSocketReceiver : IDisposable
    {
       
        Task StartListeningAsync(int port, ICommsInterface listenOn);

       
        Task StopListeningAsync();

        
        Task SendToAsync(byte[] data, string address, int port);

       
        Task SendToAsync(byte[] data, int length, string address, int port);

    
        event EventHandler<UdpSocketMessageReceivedEventArgs> MessageReceived;
    }
}