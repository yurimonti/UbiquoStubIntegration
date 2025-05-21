using Newtonsoft.Json;

namespace UbiquoStub.Models;

public record TestSuite
{
    [JsonProperty("classUnderTest")]
    public string ClassUnderTest {get; set;}

    [JsonProperty("mocksToIntegration")]
    public IEnumerable<MockAttribute> MocksToIntegration {get;set;}

    [JsonProperty("tests")]
    public IEnumerable<TestAttribute> Tests {get;set;}
}

public class TestAttribute
{
    [JsonProperty("methodName")]
    public string MethodName {get; set;}

    [JsonProperty("inputs")]
    public IEnumerable<TypeAndValue> Inputs {get; set;}

    [JsonProperty("output")]
    public TypeAndValue Output {get; set;}

    [JsonProperty("expected")]
    public TypeAndValue Expected {get; set;}

    [JsonProperty("mocks")]
    public IEnumerable<MockBehavior> Mocks {get; set;}
}

public class MockBehavior
{
    [JsonProperty("name")]
    public string Name {get; set;}
    
    [JsonProperty("method")]
    public MethodStructure Method {get; set;}
}

public class MethodStructure
{
    [JsonProperty("methodName")]
    public string MethodName {get; set;}

    [JsonProperty("methodInputs")]
    public IEnumerable<TypeAndValue> MethodInputs {get; set;}

    [JsonProperty("methodOutput")]
    public TypeAndValue MethodOutput {get; set;}
}

public class TypeAndValue
{
    [JsonProperty("type")]
    public string Type {get; set;}

    [JsonProperty("value")]
    public string Value {get; set;}
}

public class MockAttribute
{
    [JsonProperty("mockName")]
    public string MockName {get; set;}

    [JsonProperty("serviceName")]
    public string ServiceName {get; set;}
}