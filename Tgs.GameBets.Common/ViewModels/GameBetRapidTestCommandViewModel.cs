using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Tgs.GameBets.Common.ViewModels;
public record GameBetRapidTestCommandViewModel
{
	[Required]
	[DefaultValue(typeof(string), "KY")]
	public string GameProviderCode { get; init; } = default!;
	[Required]
	[DefaultValue(typeof(string), "KY")]
	public string SubGameProviderCode { get; init; } = default!;
	[Required]
	[DefaultValue(typeof(int), "6700018")]
	public int GameId { get; init; }
	[Required]
	[DefaultValue(typeof(string), "TGS")]
	public string ResellerCode { get; init; } = default!;
	[Required]
	[DefaultValue(typeof(string), "TGS")]
	public string LicenseeCode { get; init; } = default!;
	[Required]
	[DefaultValue(typeof(string), "GPI")]
	public string BrandCode { get; init; } = default!;
	[Required]
	[DefaultValue(typeof(string), "4ucmqplxb029")]
	public string PlayerCode { get; init; } = default!;
	[Required]
	[DefaultValue(typeof(string), "4ucmqplxb029")]
	public string Username { get; init; } = default!;
	[Required]
	[DefaultValue(typeof(string), "CNY")]
	public string CurrencyCode { get; init; } = default!;
	[Required]
	[DefaultValue(typeof(Guid), "8bff49a2-66ff-4c66-81f6-c9b242f2b868")]
	public Guid WalletId { get; init; }
	[Required]
	[DefaultValue(typeof(string), "6323442723")]
	public string ExternalRoundId { get; init; } = default!;
	[Required]
	[DefaultValue(typeof(string), "6323442723")]
	public string ExternalBetId { get; init; } = default!;
	[Required]
	[DefaultValue(typeof(string), "bacc")]
	public string ExternalGameId { get; init; } = default!;
	[Required]
	[DefaultValue(typeof(DateTimeOffset), "2024-09-12 09:55:12.737 +0800")]
	public DateTimeOffset ExternalCreatedOn { get; init; }
	[Required]
	[DefaultValue(typeof(DateTimeOffset), "2024-09-12 09:55:15.555 +0800")]
	public DateTimeOffset ExternalClosedOn { get; init; }
	[Required]
	[DefaultValue(typeof(DateTimeOffset), "2024-09-12 09:55:15.555 +0800")]
	public DateTimeOffset ExternalPayoffOn { get; init; }
	[Required]
	[DefaultValue(typeof(DateTimeOffset), "1970-01-01 08:00:00.000 +0800")]
	public DateTimeOffset ExternalResettleOn { get; init; }
	[Required]
	[DefaultValue(typeof(string), "Win")]
	public Enums.BetState BetState { get; init; }
	[Required]
	[DefaultValue(typeof(string), "Normal")]
	public Enums.BetType BetType { get; init; }
	[Required]
	[DefaultValue(typeof(string), "None")]
	public Enums.FreeGameState FreeGameState { get; init; }
	[Required]
	[DefaultValue(typeof(string), "None")]
	public Enums.GambleState GambleState { get; init; }
	[Required]
	[DefaultValue(typeof(string), "None")]
	public Enums.CashoutState CashoutState { get; init; }
	[Required]
	[DefaultValue(typeof(string), "None")]
	public Enums.ResettleState ResettleState { get; init; }
	[Required]
	[DefaultValue(typeof(string), "None")]
	public Enums.JackpotState JackpotState { get; init; }
	[Required]
	[DefaultValue(typeof(decimal), "1.0")]
	public decimal CurrencyRatio { get; init; }
	[Required]
	[Range(minimum: 0, maximum: double.MaxValue)]
	[DefaultValue(typeof(decimal), "0.8")]
	public decimal Bet { get; init; }
	[Required]
	[Range(minimum: 0, maximum: double.MaxValue)]
	[DefaultValue(typeof(decimal), "0.8")]
	public decimal ValidBet { get; init; }
	[Required]
	[Range(minimum: 0, maximum: double.MaxValue)]
	[DefaultValue(typeof(decimal), "1.2")]
	public decimal Settle { get; init; }
	[Required]
	[DefaultValue(typeof(decimal), "0.4")]
	public decimal WinLoss { get; init; }
	[Required]
	[Range(minimum: 0, maximum: double.MaxValue)]
	[DefaultValue(typeof(decimal), "0.8")]
	public decimal Turnover { get; init; }
	[Required]
	[Range(minimum: 0, maximum: double.MaxValue)]
	[DefaultValue(typeof(decimal), "0.01")]
	public decimal Commission { get; init; }
	[Required]
	[Range(minimum: 0, maximum: double.MaxValue)]
	[DefaultValue(typeof(decimal), "0.001")]
	public decimal Contribute { get; init; }
	[Required]
	[Range(minimum: 0, maximum: double.MaxValue)]
	[DefaultValue(typeof(decimal), "0.0")]
	public decimal Jackpot { get; init; }
}
