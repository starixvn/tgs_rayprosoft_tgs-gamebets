using System.ComponentModel.DataAnnotations;

namespace Tgs.GameBets.Common.ViewModels;

public record GameBetByExternalBetViewMode
{
	[Required]
	public Guid GameProviderId { get; init; }
	[Required]
	public int GameId { get; init; }
	[Required]
	public Guid WalletId { get; init; }
	[Required]
	public DateTimeOffset ExternalCreatedOn { get; init; }
}
