using System.Threading.Tasks;
using Newtonsoft.Json;
using SPT.Common.Http;
using UnityEngine;
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
        
        public static async Task<PlaceHideoutResponse> EstablishHideoutExtract(PlaceHideoutRequest body)
        {
            string payload = await RequestHandler.PostJsonAsync("/vagabond/place-hideout", JsonConvert.SerializeObject(body));
            return JsonConvert.DeserializeObject<PlaceHideoutResponse>(payload);
        }
    }
}