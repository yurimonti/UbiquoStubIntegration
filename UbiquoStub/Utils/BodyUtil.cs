using System;
using System.Text.Json.Nodes;

namespace UbiquoStub.Utils;

public static class BodyUtil
{

    public static bool GptIsJsonSubset(JsonNode? subset, JsonNode? superset)
    {
        if (subset == null || superset == null)
            return subset == superset;

        if (subset is JsonObject subsetObj && superset is JsonObject supersetObj)
        {
            foreach (var prop in subsetObj)
            {
                if (!supersetObj.TryGetPropertyValue(prop.Key, out var superValue))
                    return false;

                if (!GptIsJsonSubset(prop.Value, superValue))
                    return false;
            }
            return true;
        }
        else if (subset is JsonArray subsetArray && superset is JsonArray supersetArray)
        {
            foreach (var item in subsetArray)
            {
                bool matchFound = supersetArray.Any(superItem => GptIsJsonSubset(item, superItem));
                if (!matchFound)
                    return false;
            }
            return true;
        }
        else
        {
            return subset.ToJsonString() == superset.ToJsonString();
        }
    }
    // public static bool LeoIsSubset(JsonNode subset, JsonNode superset)
    // {
    //     if (subset is JsonObject subsetObj && superset is JsonObject supersetObj)
    //     {
    //         foreach (var property in subsetObj)
    //         {
    //             bool containsKey = supersetObj.ContainsKey(property.Key);
    //             bool sameValue = JsonObject.DeepEquals(supersetObj[property.Key], property.Value);
    //             if (!containsKey || !sameValue)
    //             {
    //                 return false;
    //             }
    //         }
    //         return true;
    //     }
    //     else return false;
    // }

    // public static bool GeminiIsSubsetOf(JsonNode? subset, JsonNode? superset)
    // {
    //     // 1. Handle null nodes
    //     // If subset is null, it's only a subset if superset is also null.
    //     if (subset == null)
    //     {
    //         return superset == null;
    //     }
    //     // A non-null subset cannot be a subset of a null superset.
    //     if (superset == null)
    //     {
    //         return false;
    //     }

    //     // Use pattern matching for different JsonNode types
    //     switch (subset)
    //     {
    //         // 2. Case: Subset is a JsonValue (primitive like string, number, bool, or JSON null)
    //         case JsonValue subsetValue:
    //             // Superset must also be a JsonValue and their JSON representations must be identical.
    //             // Using ToJsonString() is a reliable way to compare values, including nulls.
    //             return superset is JsonValue supersetValue &&
    //                    subsetValue.ToJsonString() == supersetValue.ToJsonString();

    //         // 3. Case: Subset is a JsonObject
    //         case JsonObject subsetObject:
    //             // Superset must also be a JsonObject
    //             if (superset is not JsonObject supersetObject)
    //             {
    //                 return false; // Type mismatch
    //             }

    //             // Check every key-value pair in the subset object
    //             foreach (var kvp in subsetObject)
    //             {
    //                 string key = kvp.Key;
    //                 JsonNode? subsetValueNode = kvp.Value;

    //                 // The key must exist in the superset object
    //                 if (!supersetObject.ContainsKey(key))
    //                 {
    //                     return false; // Key missing in superset
    //                 }

    //                 JsonNode? supersetValueNode = supersetObject[key];

    //                 // Recursively check if the value in the subset is a subset
    //                 // of the corresponding value in the superset.
    //                 if (!GeminiIsSubsetOf(subsetValueNode, supersetValueNode)) // Recursive call
    //                 {
    //                     return false; // Values don't match recursively
    //                 }
    //             }
    //             // If all keys/values in the subset exist and match recursively in the superset, it's a subset.
    //             return true;

    //         // 4. Case: Subset is a JsonArray
    //         case JsonArray subsetArray:
    //             // Superset must also be a JsonArray
    //             if (superset is not JsonArray supersetArray)
    //             {
    //                 return false; // Type mismatch
    //             }

    //             // Every element in the subset array must find a corresponding element
    //             // in the superset array for which it is a subset (recursively).
    //             // This implementation does NOT require order to be preserved
    //             // and allows duplicates in the superset.
    //             foreach (var subsetElement in subsetArray)
    //             {
    //                 // Check if *any* element in the superset array is a superset
    //                 // of the current subset element.
    //                 bool foundMatch = supersetArray.Any(supersetElement => GeminiIsSubsetOf(subsetElement, supersetElement)); // Recursive call within Any()

    //                 if (!foundMatch)
    //                 {
    //                     // If any subset element doesn't find a match in the superset, it's not a subset.
    //                     return false;
    //                 }
    //             }
    //             // If all subset elements found a suitable match in the superset, it's a subset.
    //             return true;

    //         // Should not be reached with valid JsonNode types
    //         default:
    //             // Consider throwing an exception for unexpected node types
    //             // throw new NotSupportedException($"Unsupported JsonNode type: {subset.GetType()}");
    //             return false;
    //     }
    // }
}
