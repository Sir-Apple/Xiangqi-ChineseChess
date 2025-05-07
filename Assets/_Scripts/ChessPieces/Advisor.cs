using System.Collections.Generic;
using UnityEngine;

public class Advisor : ChessPiece
{
	public override List<Vector2Int> GetAvailableMoves(ref ChessPiece[,] board, int tileCountX, int tileCountY)
	{
		List<Vector2Int> r = new List<Vector2Int>();

		Vector2Int[] moveOffsets = new Vector2Int[]
		{
			new Vector2Int(1, 1),
			new Vector2Int(1, -1),
			new Vector2Int(-1, 1),
			new Vector2Int(-1, -1)
		};

		foreach (var moveOffset in moveOffsets)
		{
			int targetX = currentX + moveOffset.x;
			int targetY = currentY + moveOffset.y;

			//check if inside palace
			if (!IsInsidePalace(targetX, targetY, team))
				continue;

			ChessPiece targetPiece = board[targetX, targetY];
			if (targetPiece == null || targetPiece.team != team)
				r.Add(new Vector2Int(targetX, targetY));
		}

		return r;
	}

	private bool IsInsidePalace(int x, int y, int team)
	{
		if(x < 3 || x > 5) return false;

		if (team == 0) // red team
			return y >= 0 && y <= 2;
		else // blue team
			return y >= 7 && y <= 9;
	}
}