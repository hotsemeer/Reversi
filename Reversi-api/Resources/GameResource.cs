using Reversi_api.Models;

namespace Reversi_api.Resources;

public sealed record GameResource(int Id, Player? playerWhite, Player? playerBlack, string? description, List<List<int>> gameStatus);
