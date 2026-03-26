using System.Threading.Tasks;
using HardmodeChallenge.Client.Models;
using Newtonsoft.Json;
using SPT.Common.Http;

namespace HardmodeChallenge.Client.Networking
{
    internal static class ApiClient
    {
        public static async Task<StateResponse> HydrateHardmodeState()
        {
            string payload = await RequestHandler.GetJsonAsync("/hardmode-challenge/sync");
            return JsonConvert.DeserializeObject<StateResponse>(payload);
        }
    }
}
