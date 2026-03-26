using System.Threading.Tasks;
using Newtonsoft.Json;
using SPT.Common.Http;
using Vagabond.Client.Models;

namespace Vagabond.Client.Networking
{
    internal static class ApiClient
    {
        public static async Task<StateResponse> HydrateVagabondState()
        {
            string payload = await RequestHandler.GetJsonAsync("/vagabond/sync");
            return JsonConvert.DeserializeObject<StateResponse>(payload);
        }
    }
}
