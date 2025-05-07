using System.Collections.Generic;
using UnityEngine;

public class Chariot : ChessPiece
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
			for (int i = 1; i < Mathf.Max(tileCountX, tileCountY); i++)
			{
				int newX = currentX + direction.x * i;
				int newY = currentY + direction.y * i;

				if (!IsInsideBoard(newX, newY, tileCountX, tileCountY))
					break;

				ChessPiece targetPiece = board[newX, newY];

				if (targetPiece == null)
				{
					r.Add(new Vector2Int(newX, newY));
				}
				else
				{
					if (targetPiece.team != team)
						r.Add(new Vector2Int(newX, newY));

					break;
				}
			}
		}

		return r;
	}
	private bool IsInsideBoard(int x, int y, int maxX, int maxY)
	{
		return x >= 0 && x < maxX && y >= 0 && y < maxY;
	}
}