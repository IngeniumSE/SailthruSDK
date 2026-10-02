# SailthruSDK

An opinionated .NET SDK built for the Sailthru API

[![.github/workflows/main.yml](https://github.com/IngeniumSE/SailthruSDK/actions/workflows/main.yml/badge.svg)](https://github.com/IngeniumSE/SailthruSDK/actions/workflows/main.yml) [![.github/workflows/release.yml](https://github.com/IngeniumSE/SailthruSDK/actions/workflows/release.yml/badge.svg)](https://github.com/IngeniumSE/SailthruSDK/actions/workflows/release.yml)

## Purchases

```csharp
var response = await client.Purchases.UpsertPurchaseAsync(
	new UpsertPurchaseRequest(email, items, messageId: sailthruBidCookie)
	{
		ExtId = order.Reference,   // purchase_keys.extid - Sailthru rejects duplicates
		Date = order.SaleOn        // optional, for purchases sent late (retries, backfill)
	});

if (!response.IsSuccess && response.Error?.IsDuplicateExtId != true)
{
	// response.Error.Code / response.Error.Message hold Sailthru's error and errormsg.
	// Or call response.EnsureSuccess() to throw a SailthruException.
}
```

An incomplete purchase (cart) cannot have an extid. Item images are sent as Sailthru's
`images: { full: { url }, thumb: { url } }` object.

