using System.Text;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;

namespace ChatLibrary;

public class JsonSchemaValidator
{
    public static JSchema MessageSchema = JSchema.Parse(@"
    {
        ""type"":""object"",
        ""properties"":
        {
            ""Sender"": 
            {
                ""type"":""string"",
                ""default"":""""
            },
            ""Message"": 
            {
                ""type"":""string"",
                ""default"":""""
            }
        },
        ""required"": [""Sender"", ""Message""],
        ""additionalProperties"": false
    }");

    public static (bool success, string errors) Validate(string json)
    {
        bool success = true;
        StringBuilder errs = new StringBuilder();
        try
        {
            JObject jobj = JObject.Parse(json);
            if (!jobj.IsValid(MessageSchema, out IList<ValidationError> errorMessages))
            {
                success = false;
                foreach (var error in errorMessages)
                {
                    errs.AppendLine($"Error: {error.Message} at: {error.Path}");
                }
            }

            return (success, errs.ToString());
        }
        catch (Exception ex)
        {
            return (false, "Invalid JSON: {ex.Message}");
        }
    }
}