using System.ComponentModel;

using System.ComponentModel.DataAnnotations;

namespace Tgs.GameBets.Common.ViewModels;
public record GameBetCloseGetExternalViewModel
{
	[Required]
	[DefaultValue(typeof(DateTimeOffset), "2024-09-12 09:55:12.737 +0800")]
	public DateTimeOffset ExternalCreatedOn { get; init; }
	[Required]
	[DefaultValue(typeof(Guid), "147cef56-4fcd-4ee3-b748-87ad398f2871")]
	public Guid GameProviderId { get; init; }
	[Required]
	[DefaultValue(typeof(int), "6700018")]
	public int GameId { get; init; }
	[Required]
	[DefaultValue(typeof(Guid), "8bff49a2-66ff-4c66-81f6-c9b242f2b868")]
	public Guid WalletId { get; init; }
	[Required]
	[DefaultValue(typeof(string), "6323442723")]
	public string ExternalBetId { get; init; } = default!;
}
