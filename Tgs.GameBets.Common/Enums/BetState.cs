namespace Tgs.GameBets.Common.Enums;

/// <summary>
/// 注單狀態
/// </summary>
public enum BetState
{
	/// <summary>
	/// The waiting
	/// 等待中: 我們交易員因為可能因為賠率的轉換等因素，還未接受這張注單。
	/// </summary>
	Waiting = 0,

	/// <summary>
	/// The running
	/// 進行中: 此註單還沒有結算。 (注單還沒有結算的狀態有可能是這場比賽還沒有結算之類的情形。)
	/// </summary>
	Running = 1,

	/// <summary>
	/// The void
	/// 作廢: 在註單為 Running 的狀態下，玩家下註註金返回。原因可能為我們交易員對此場賽事有些疑慮。可與我們聯繫詢問發生什麼狀況。
	/// </summary>
	Void = 2,

	/// <summary>
	/// The refund
	/// 退款: 在註單為 Running 的狀態下，玩家下註金返回。 原因有可能是賽事取消或發生什麼意外。
	/// </summary>
	Refund = 3,

	/// <summary>
	/// The cancel
	/// 已取消: 在註單為 Waiting 的狀態下，玩家下註金返回。 可能狀況很多。
	/// </summary>
	Cancel = 4,

	/// <summary>
	/// The loss
	/// 輸: 此註單已結算且玩家輸了此註單。
	/// </summary>
	Loss = 5,

	/// <summary>
	/// The win
	/// 贏: 此註單已結算且玩家贏了此註單。
	/// </summary>
	Win = 6,

	/// <summary>
	/// The draw
	/// 和局: 此註單已結算且此註單為和局。
	/// </summary>
	Draw = 7,

	/// <summary>
	/// The half win
	/// 半贏: 此註單已結算且玩家贏了下注額一半。
	/// </summary>
	HalfWin = 8,

	/// <summary>
	/// The half loss
	/// 半輸: 此註單已結算且玩家輸了下注額一半。
	/// </summary>
	HalfLoss = 9,
}
