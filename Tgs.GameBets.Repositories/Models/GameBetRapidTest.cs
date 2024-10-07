using System.ComponentModel.DataAnnotations.Schema;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Tgs.GameBets.Repositories.Models;

[Table("game_bet_rapid_tests")]
public record GameBetRapidTest
{
	/// <summary>
	/// The identifier
	/// </summary>
	[BsonId]
	public ObjectId Id { get; set; } = default!;
	/// <summary>
	/// The game provider code
	/// </summary>
	[BsonElement("game_provider_code")]
	[BsonRepresentation(BsonType.String)]
	public string GameProviderCode { get; set; } = default!;
	/// <summary>
	/// The sub game provider code
	/// </summary>
	[BsonElement("sub_game_provider_code")]
	[BsonRepresentation(BsonType.String)]
	public string SubGameProviderCode { get; set; } = default!;
	/// <summary>
	/// The game identifier
	/// </summary>
	[BsonElement("game_id")]
	[BsonRepresentation(BsonType.Int32)]
	public int GameId { get; set; }
	/// <summary>
	/// The reseller code
	/// </summary>
	[BsonElement("reseller_code")]
	[BsonRepresentation(BsonType.String)]
	public string ResellerCode { get; set; } = default!;
	/// <summary>
	/// The licensee code
	/// </summary>
	[BsonElement("licensee_code")]
	[BsonRepresentation(BsonType.String)]
	public string LicenseeCode { get; set; } = default!;
	/// <summary>
	/// The brand code
	/// </summary>
	[BsonElement("brand_code")]
	[BsonRepresentation(BsonType.String)]
	public string BrandCode { get; set; } = default!;
	/// <summary>
	/// The player code
	/// </summary>
	[BsonElement("player_code")]
	[BsonRepresentation(BsonType.String)]
	public string PlayerCode { get; set; } = default!;
	/// <summary>
	/// The username
	/// </summary>
	[BsonElement("username")]
	[BsonRepresentation(BsonType.String)]
	public string Username { get; set; } = default!;
	/// <summary>
	/// The currency code
	/// </summary>
	[BsonElement("currency_code")]
	[BsonRepresentation(BsonType.String)]
	public string CurrencyCode { get; set; } = default!;
	/// <summary>
	/// The wallet identifier
	/// </summary>
	[BsonElement("wallet_id")]
	[BsonRepresentation(BsonType.String)]
	public Guid WalletId { get; set; }
	/// <summary>
	/// The external round identifier
	/// </summary>
	[BsonElement("external_round_id")]
	[BsonRepresentation(BsonType.String)]
	public string ExternalRoundId { get; set; } = default!;
	/// <summary>
	/// The external bet identifier
	/// </summary>
	[BsonElement("external_bet_id")]
	[BsonRepresentation(BsonType.String)]
	public string ExternalBetId { get; set; } = default!;
	/// <summary>
	/// The external game identifier
	/// </summary>
	[BsonElement("external_game_id")]
	[BsonRepresentation(BsonType.String)]
	public string ExternalGameId { get; set; } = default!;
	/// <summary>
	/// The bet state
	/// </summary>
	[BsonElement("bet_state")]
	[BsonRepresentation(BsonType.String)]
	public Common.Enums.BetState BetState { get; set; }
	/// <summary>
	/// The bet type
	/// </summary>
	[BsonElement("bet_type")]
	[BsonRepresentation(BsonType.String)]
	public Common.Enums.BetType BetType { get; set; }
	/// <summary>
	/// The free game state
	/// </summary>
	[BsonElement("free_game_state")]
	[BsonRepresentation(BsonType.String)]
	public Common.Enums.FreeGameState FreeGameState { get; set; }
	/// <summary>
	/// The gamble state
	/// </summary>
	[BsonElement("gamble_state")]
	[BsonRepresentation(BsonType.String)]
	public Common.Enums.GambleState GambleState { get; set; }
	/// <summary>
	/// The cashout state
	/// </summary>
	[BsonElement("cashout_state")]
	[BsonRepresentation(BsonType.String)]
	public Common.Enums.CashoutState CashoutState { get; set; }
	/// <summary>
	/// The resettle state
	/// </summary>
	[BsonElement("resettle_state")]
	[BsonRepresentation(BsonType.String)]
	public Common.Enums.ResettleState ResettleState { get; set; }
	/// <summary>
	/// The jackpot state
	/// </summary>
	[BsonElement("jackpot_state")]
	[BsonRepresentation(BsonType.String)]
	public Common.Enums.JackpotState JackpotState { get; set; }
	/// <summary>
	/// The currency ratio
	/// </summary>
	[BsonElement("currency_ratio")]
	[BsonRepresentation(BsonType.Decimal128)]
	[BsonDefaultValue(0)]
	public decimal CurrencyRatio;
	/// <summary>
	/// The bet
	/// </summary>
	[BsonElement("bet")]
	[BsonRepresentation(BsonType.Decimal128)]
	[BsonDefaultValue(0)]
	public decimal Bet { get; set; }
	/// <summary>
	/// The valid bet
	/// </summary>
	[BsonElement("valid_bet")]
	[BsonRepresentation(BsonType.Decimal128)]
	[BsonDefaultValue(0)]
	public decimal ValidBet { get; set; }
	/// <summary>
	/// The settle
	/// </summary>
	[BsonElement("settle")]
	[BsonRepresentation(BsonType.Decimal128)]
	[BsonDefaultValue(0)]
	public decimal Settle { get; set; }
	/// <summary>
	/// The commission
	/// </summary>
	[BsonElement("commission")]
	[BsonRepresentation(BsonType.Decimal128)]
	[BsonDefaultValue(0)]
	public decimal Commission { get; set; }
	/// <summary>
	/// The turnover
	/// </summary>
	[BsonElement("turnover")]
	[BsonRepresentation(BsonType.Decimal128)]
	[BsonDefaultValue(0)]
	public decimal Turnover { get; set; }
	/// <summary>
	/// The win loss
	/// </summary>
	[BsonElement("win_loss")]
	[BsonRepresentation(BsonType.Decimal128)]
	[BsonDefaultValue(0)]
	public decimal WinLoss { get; set; }
	/// <summary>
	/// The jackpot
	/// </summary>
	[BsonElement("jackpot")]
	[BsonRepresentation(BsonType.Decimal128)]
	[BsonDefaultValue(0)]
	public decimal Jackpot { get; set; }
	/// <summary>
	/// The additional data
	/// </summary>
	[BsonElement("additional_data")]
	[BsonRepresentation(BsonType.String)]
	public string AdditionalData { get; set; } = default!;
	/// <summary>
	/// The rapid test on
	/// </summary>
	[BsonElement("rapid_test_on")]
	[BsonIgnoreIfNull]
	public DateTime RapidTestOn { get; set; }
	/// <summary>
	/// The expire on
	/// </summary>
	[BsonElement("expire_on")]
	[BsonIgnoreIfNull]
	public DateTime ExpireOn { get; set; }
}
