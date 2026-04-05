using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;
using Microsoft.AspNetCore.Http;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.Models.Utils;
using SPTarkov.Server.Core.Servers.Ws;

namespace Vagabond.Server.Services;

[Injectable(InjectionType = InjectionType.Singleton)]
public class VagabondSocketHandler(
    ISptLogger<VagabondSocketHandler> logger) : IWebSocketConnectionHandler
{
    private readonly ConcurrentDictionary<string, WebSocket> _clients = new();

    public string GetHookUrl() => "/vagabond/socket/";
    public string GetSocketId() => "Vagabond";

    public Task OnConnection(WebSocket ws, HttpContext context, string sessionIdContext)
    {
        _clients[sessionIdContext] = ws;
        logger.Info($"Vagabond socket connected: {sessionIdContext}");
        return Task.CompletedTask;
    }

    public Task OnMessage(byte[] rawData, WebSocketMessageType messageType, WebSocket ws, HttpContext context)
    {
        return Task.CompletedTask;
    }

    public Task OnClose(WebSocket ws, HttpContext context, string sessionIdContext)
    {
        _clients.TryRemove(sessionIdContext, out _);
        return Task.CompletedTask;
    }

    public async Task BroadcastExfilRefresh()
    {
        var bytes = Encoding.UTF8.GetBytes("{\"type\":\"vagabond-exfil-refresh\"}");

        foreach (var kvp in _clients.ToArray())
        {
            var ws = kvp.Value;
            if (ws.State != WebSocketState.Open)
            {
                continue;
            }

            await ws.SendAsync(bytes, WebSocketMessageType.Text, true, CancellationToken.None);
        }
    }
}