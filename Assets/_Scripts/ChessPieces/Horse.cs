using System.Collections.Generic;
using UnityEngine;

public class Horse : ChessPiece
{
	public override List<Vector2Int> GetAvailableMoves(ref ChessPiece[,] board, int tileCountX, int tileCountY)
	{
		List<Vector2Int> r = new List<Vector2Int>();

		//Each tuple: (target offset, leg offset)
		(Vector2Int move, Vector2Int leg)[] directions = new (Vector2Int, Vector2Int)[]
		{
			(new Vector2Int(1, 2), new Vector2Int(0 , 1)),
			(new Vector2Int(-1, 2), new Vector2Int(0 , 1)),
			(new Vector2Int(1, -2), new Vector2Int(0 , -1)),
			(new Vector2Int(-1, -2), new Vector2Int(0 , -1)),
			(new Vector2Int(2, 1), new Vector2Int(1 , 0)),
			(new Vector2Int(2, -1), new Vector2Int(1 , 0)),
			(new Vector2Int(-2, 1), new Vector2Int(-1 , 0)),
			(new Vector2Int(-2, -1), new Vector2Int(-1 , 0)),
		};

		foreach (var (moveOffset, legOffset) in directions)
		{
			int legX = currentX + legOffset.x;
			int legY = currentY + legOffset.y;

			int targetX = currentX + moveOffset.x;
			int targetY = currentY + moveOffset.y;

			//Leg blocking
			if (!IsInsideBoard(legX, legY, tileCountX, tileCountY))
				continue;

			if (board[legX, legY] != null)
				continue;

			//Valid move (not blocked)
			if (IsInsideBoard(targetX, targetY, tileCountX, tileCountY))
			{
				ChessPiece targetPiece = board[targetX, targetY];
				if (targetPiece == null || targetPiece.team != team)
					r.Add(new Vector2Int(targetX, targetY));
			}
		}

		return r;
	}
	private bool IsInsideBoard(int x, int y, int maxX, int maxY)
	{
		return x >= 0 && x < maxX && y >= 0 && y < maxY;
	}
}