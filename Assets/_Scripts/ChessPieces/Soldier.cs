using System.Collections.Generic;
using UnityEngine;

public class Soldier : ChessPiece
{
	public override List<Vector2Int> GetAvailableMoves(ref ChessPiece[,] board, int tileCountX, int tileCountY)
	{
		List<Vector2Int> r = new List<Vector2Int>();

		int direction = (team == 0) ? 1 : -1; //Red goes up and Blue goes down
		Vector2Int forward = new Vector2Int(currentX, currentY + direction);

		// Piece moves forward
		if(IsInsideBoard(forward.x, forward.y, tileCountX, tileCountY))
			r.Add(forward);

		// Crossed the river
		bool crossedRiver = (team == 0 && currentY >= 5) || (team == 1 && currentY <= 4);

		if (crossedRiver)
		{
			// Left and Right moves
			Vector2Int left = new Vector2Int(currentX - 1, currentY);
			Vector2Int right = new Vector2Int(currentX + 1, currentY);

			if (IsInsideBoard(left.x, left.y, tileCountX, tileCountY))
				r.Add(left);
			if (IsInsideBoard(right.x, right.y, tileCountX, tileCountY))
				r.Add(right);
		}

		return r;
	}
	 private bool IsInsideBoard(int x, int y, int maxX, int maxY)
	 {
		return x >= 0 && x < maxX && y >= 0 && y < maxY;
	 }
}
