using System.Collections.Generic;
using UnityEngine;

public class General : ChessPiece
{
	public override List<Vector2Int> GetAvailableMoves(ref ChessPiece[,] board, int tileCountX, int tileCountY)
	{
		List<Vector2Int> r = new List<Vector2Int>();

		Vector2Int[] directions = new Vector2Int[]
		{
			Vector2Int.up,
			Vector2Int.down,
			Vector2Int.left,
			Vector2Int.right,
		};

		foreach (var direction in directions)
		{
			int targetX = currentX + direction.x;
			int targetY = currentY + direction.y;

			if (!IsInsidePalace(targetX, targetY, team))
				continue;

			ChessPiece targetPiece = board[targetX, targetY];
			if (targetPiece == null || targetPiece.team != team)
				r.Add(new Vector2Int(targetX, targetY));
		}

		return r;
	}
}