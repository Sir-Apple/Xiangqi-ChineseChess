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
}