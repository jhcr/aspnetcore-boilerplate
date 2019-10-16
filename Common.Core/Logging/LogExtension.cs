using System.Net.Http;
using System.Threading.Tasks;

namespace Common.Core.Logging
{
    public static class LogExtension
    {
        /// <summary>
        /// Generate formated log message with request and resposne information
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        public static async Task<string> GetLogMessage(this HttpResponseMessage response)
        {
            var request = response?.RequestMessage;
            var requestMessage = request?.Content != null ? await request.Content.ReadAsStringAsync() : string.Empty;
            var responseMessage = response?.Content != null ? await response.Content.ReadAsStringAsync() : string.Empty;

            string message = $"[REQUEST] {request.Method} {request.RequestUri} {requestMessage}\n" +
                $"[RESPONSE] {(int)response.StatusCode} {response.StatusCode} {responseMessage}";

            return message;
        }
    }
}
