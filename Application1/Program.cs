using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

CancellationTokenSource cts = new();
Socket udp = new(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
udp.Bind(new IPEndPoint(IPAddress.Any, 7777));
async Task LogReceivedMessageAsync(CancellationToken token)
{
    IPEndPoint receiveFrom = new(IPAddress.Any, 0);
    while (!token.IsCancellationRequested)
    {
        var result = await udp.ReceiveFromAsync(Memory<byte>.Empty, SocketFlags.None, receiveFrom, token);
        _ = udp.SendToAsync(Encoding.ASCII.GetBytes(result.RemoteEndPoint.ToString()!), SocketFlags.None,
            result.RemoteEndPoint);
    }
}
Console.WriteLine($"server listening on {udp.LocalEndPoint}");
await LogReceivedMessageAsync(cts.Token);
