using System.Threading.Tasks;
using Newtonsoft.Json;
using SPT.Common.Http;
using Vagabond.Common.Models;

namespace Vagabond.Client.Networking
{
    internal static class ApiClient
    {
        public static async Task<SyncStateResponse> HydrateVagabondState()
        {
            string payload = await RequestHandler.GetJsonAsync("/vagabond/sync");
            return JsonConvert.DeserializeObject<SyncStateResponse>(payload);
        }
    }
}
