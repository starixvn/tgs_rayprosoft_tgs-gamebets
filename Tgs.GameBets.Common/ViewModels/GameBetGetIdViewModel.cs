using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Tgs.GameBets.Common.ViewModels;
public record GameBetGetIdViewModel
{
	[DefaultValue(typeof(DateTimeOffset), "2024-09-12 09:55:12.737 +0800")]
	public DateTimeOffset? ExternalCreatedOn { get; init; }
}