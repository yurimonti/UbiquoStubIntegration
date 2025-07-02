using System.Text.Json.Nodes;

namespace UbiquoStub.Services
{
    public class ResponsesComparator
    {
        public async Task CompareResponses(HttpResponseMessage actual, HttpResponseMessage expected)
        {
            //CHECK STATUS CODE
            if (!actual.StatusCode.Equals(expected.StatusCode))
                throw new Exception($"Status Code \n{actual.StatusCode}\n\t, but \n{expected.StatusCode} \n\t was expected");
            //CHECK HEADERS
            //var actualHeader = JsonSerializer.Serialize(actual.Headers);
            //var expectedHeader = JsonSerializer.Serialize(expected.Headers);
            //if (!actual.Equals(expectedHeader))
            //    throw new Exception($"Header \n{actualHeader}\n\t, but \n{expectedHeader} \n\t was expected");
            //CHECK BODY
            string actualContent = await actual.Content.ReadAsStringAsync();
            string expectedContent = await expected.Content.ReadAsStringAsync();
            JsonNode actualBody = (actualContent == "" || actualContent == null) ? null : JsonNode.Parse(actualContent);
            JsonNode expectedBody = (expectedContent == "" || expectedContent == null) ? null : JsonNode.Parse(expectedContent);
            CompareJsonBody(actualBody, expectedBody);
        }

        private void CompareJsonBody(JsonNode actual, JsonNode expected)
        {
            if (!JsonNode.DeepEquals(actual, expected))
                throw new Exception($"Actual Content is: \n{actual}\n, but was expected: \n{expected}\n");
        }
    }
}
