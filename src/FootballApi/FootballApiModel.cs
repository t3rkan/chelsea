using System.Text;
using System.Text.Json;

namespace FootballApi
{
    public class FootballApiModel
    {
        public JsonElement Get { get; set; }
        public JsonElement Parameters { get; set; }
        public JsonElement Errors { get; set; }
        public JsonElement Results { get; set; }
        public JsonElement Paging { get; set; }
        public JsonElement Response { get; set; }
        public int? RequestsRemaining { get; set; }

        public override string ToString()
        {
            StringBuilder strBuilder = new StringBuilder();

            strBuilder.AppendLine($"ENDPOINT: {this.Get.ToString()}");
            strBuilder.AppendLine($"PARAMETERS: {this.Parameters.ToString()}");
            strBuilder.AppendLine($"ERRORS: {this.Errors.ToString()}");
            strBuilder.AppendLine($"RESULTS: {this.Results.ToString()}");
            strBuilder.AppendLine($"PAGING: {this.Paging.ToString()}");
            strBuilder.AppendLine($"RESPONSE: {this.Response.ToString()}");
            strBuilder.AppendLine($"REQUESTS REMAINING: {this.RequestsRemaining}");

            return strBuilder.ToString();
        }
    }
}
