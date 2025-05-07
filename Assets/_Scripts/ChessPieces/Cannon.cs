using System.Collections.Generic;
using UnityEngine;

public class Cannon : ChessPiece
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
			bool hasJumped = false;

			for (int i = 1; i < Mathf.Max(tileCountX, tileCountY); i++)
			{
				int newX = currentX + direction.x * i;
				int newY = currentY + direction.y * i;

				if (!IsInsideBoard(newX, newY, tileCountX, tileCountY))
					break;

				ChessPiece piece = board[newX, newY];

				if (!hasJumped)
				{
					if (piece == null)
					{
						r.Add(new Vector2Int(newX, newY));
					}
					else
					{
						hasJumped = true;
					}
				}
				else
				{
					if (piece != null)
					{
						if (piece.team != team)
							r.Add(new Vector2Int(newX, newY));
						break;
					}
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