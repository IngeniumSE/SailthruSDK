// This work is licensed under the terms of the MIT license.
// For a copy, see <https://opensource.org/licenses/MIT>.

namespace SailthruSDK.Api;

partial interface ISailthruApiClient
{
	/// <summary>
	/// Gets the user operations.
	/// </summary>
	IUserOperations Users { get; }
}

partial class SailthruApiClient
{
	Lazy<IUserOperations>? _users;
	public IUserOperations Users => (_users ??= Defer<IUserOperations>(
		c => new UserOperations(new("/user"), c))).Value;
}

public partial interface IUserOperations
{
	Task<SailthruResponse<User>> GetUserAsync(
		string id,
		string key = UserKeyType.Email,
		UserFields? fields = default,
		CancellationToken cancellationToken = default);

	Task<SailthruResponse> UpsertUserAsync(
			string id,
			string key = UserKeyType.Email,
			Map<string>? keys = default,
			KeyConflict keyConflict = KeyConflict.Merge,
			Map<string>? cookies = default,
			Map<bool>? lists = default,
			Map<bool>? templates = default,
			Map<string?>? vars = default,
			OptOutStatus? optOutEmailStatus = default,
			bool? optOutSms = default,
			UserFields? fields = default,
			CancellationToken cancellationToken = default);
}

public partial class UserOperations(
	PathString path,
	ApiClient client) : IUserOperations
{
	readonly PathString _path = path;
	readonly ApiClient _client = client;

	public async Task<SailthruResponse<User>> GetUserAsync(
		string id,
		string key = UserKeyType.Email,
		UserFields? fields = null,
		CancellationToken cancellationToken = default)
	{
		var model = new GetUserRequest(id, key, fields);
		var request = new SailthruRequest<GetUserRequest>(HttpMethod.Get, _path, model);

		return await _client.FetchAsync<GetUserRequest, User>(
			request,
			cancellationToken)
			.ConfigureAwait(false);
	}

	public async Task<SailthruResponse> UpsertUserAsync(
			string id,
			string key = UserKeyType.Email,
			Map<string>? keys = default,
			KeyConflict keyConflict = KeyConflict.Merge,
			Map<string>? cookies = default,
			Map<bool>? lists = default,
			Map<bool>? templates = default,
			Map<string?>? vars = default,
			OptOutStatus? optOutEmailStatus = default,
			bool? optOutSms = default,
			UserFields? fields = default,
			CancellationToken cancellationToken = default)
	{
		var model = new UpsertUserRequest(id, key, keys, keyConflict, cookies, lists, templates, vars, optOutEmailStatus, optOutSms, fields);
		var request = new SailthruRequest<UpsertUserRequest>(HttpMethod.Post, _path, model);

		return await _client.SendAsync(
			request,
			cancellationToken)
			.ConfigureAwait(false);
	}
}
