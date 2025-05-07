using System.Collections.Generic;
using UnityEngine;

public class Elephant : ChessPiece
{
	public override List<Vector2Int> GetAvailableMoves(ref ChessPiece[,] board, int tileCountX, int tileCountY)
	{
		List<Vector2Int> r = new List<Vector2Int>();

		Vector2Int[] moveOffsets = new Vector2Int[]
		{
			new Vector2Int(2, 2),
			new Vector2Int(-2, 2),
			new Vector2Int(2, -2),
			new Vector2Int(-2, -2)
		};

		foreach (var offset in moveOffsets)
		{
			int targetX = currentX + offset.x;
			int targetY = currentY + offset.y;

			//Stays in team's territory (can't cross the river)
			if (team == 0 && targetY > 4) continue; // red team
			if (team == 1 && targetY < 5) continue; // blue team

			if (!IsInsideBoard(targetX, targetY, tileCountX, tileCountY))
				continue;

			// check if "eye" is blocked
			int eyeX = currentX + offset.x / 2;
			int eyeY = currentY + offset.y / 2;
			if (board[eyeX, eyeY] != null)
				continue;

			// valid move
			ChessPiece targetPiece = board[targetX, targetY];
			if (targetPiece == null || targetPiece.team != team)
				r.Add(new Vector2Int(targetX, targetY));
		}

		return r;
	}
	private bool IsInsideBoard(int x, int y, int maxX, int maxY)
	{
		return x >= 0 && x < maxX && y >= 0 && y < maxY;
	}
}