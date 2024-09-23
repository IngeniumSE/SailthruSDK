namespace SailthruSDK;

using System;
using System.Collections.Generic;

/// <summary>
/// Represents a map.
/// </summary>
/// <typeparam name="TValue">The value type.</typeparam>
public class Map<TValue> : Dictionary<string, TValue>
{
	public Map() : base(StringComparer.OrdinalIgnoreCase) { }
}
