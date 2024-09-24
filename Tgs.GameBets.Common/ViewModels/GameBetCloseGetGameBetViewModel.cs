using System.ComponentModel;

using System.ComponentModel.DataAnnotations;

namespace Tgs.GameBets.Common.ViewModels;
public record GameBetCloseGetGameBetViewModel
{
	[Required]
	[DefaultValue(typeof(DateTimeOffset), "2024-09-12 09:55:12.737 +0800")]
	public DateTimeOffset ExternalCreatedOn { get; init; }
}
