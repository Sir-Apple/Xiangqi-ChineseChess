using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class Chessboard : MonoBehaviour
{
	[Header("Art stuff")]
	[SerializeField] private Material tileMaterial;
	[SerializeField] private Material checkHighlightMaterial;
	private Material originalTileMaterial;
	private Vector2Int? checkedGeneralPosition = null;
	[SerializeField] private float tileSize = 1.0f;
	[SerializeField] private float yOffset = 0.2f;
	[SerializeField] private Vector3 boardCenter = Vector3.zero;

	[Header("Prefabs && Materials")]
	[SerializeField] private GameObject[] prefabs;

	private ChessPiece[,] chessPieces;
	private ChessPiece currentlyDragging; //Drag
	private List<ChessPiece> deadReds = new List<ChessPiece>();
	private List<ChessPiece> deadBlues = new List<ChessPiece>();
	private const int TILE_COUNT_X = 9;
	private const int TILE_COUNT_Y = 10;
	private GameObject[,] tiles;
	private Camera currentCamera;
	private int currentTurn = 0; // 0 = Red, 1 = Blue
	private Vector2Int currentHover;
	private Vector3 bounds;

	private void Awake()
	{
		GenerateAllTiles(tileSize, TILE_COUNT_X, TILE_COUNT_Y);
		SpawnAllPieces();
		PositionAllPieces();
	}
	private void Update()
	{
		if (!currentCamera)
		{
			currentCamera = Camera.main;
			return;
		}

		RaycastHit info;
		Ray ray = currentCamera.ScreenPointToRay(Input.mousePosition);
		if (Physics.Raycast(ray, out info, 100, LayerMask.GetMask("Tile", "Hover")))
		{
			Vector2Int hitPosition = LookupTileIndex(info.transform.gameObject);

			if (currentHover == -Vector2Int.one)
			{
				currentHover = hitPosition;
				tiles[hitPosition.x, hitPosition.y].layer = LayerMask.NameToLayer("Hover");
			}

			if (currentHover != hitPosition)
			{
				tiles[currentHover.x, currentHover.y].layer = LayerMask.NameToLayer("Tile");
				currentHover = hitPosition;
				tiles[hitPosition.x, hitPosition.y].layer = LayerMask.NameToLayer("Hover");
			}

			if (Input.GetMouseButtonDown(0))
			{
				ChessPiece clickedPiece = chessPieces[hitPosition.x, hitPosition.y];

				if (clickedPiece != null && clickedPiece.team == currentTurn)
				{
					currentlyDragging = clickedPiece;
				}
				else if (currentlyDragging != null)
				{
					Vector2Int previousPosition = new Vector2Int(currentlyDragging.currentX, currentlyDragging.currentY);
					List<Vector2Int> availableMoves = currentlyDragging.GetAvailableMoves(ref chessPieces, TILE_COUNT_X, TILE_COUNT_Y);

					availableMoves = availableMoves.FindAll(move => !DoesMoveExposeGeneral(currentlyDragging, move));

					if (availableMoves.Contains(hitPosition))
					{
						bool validMove = MoveTo(currentlyDragging, hitPosition.x, hitPosition.y);
						if (validMove)
						{
							currentlyDragging = null;

							int opponent = 1 - currentTurn;
							if (IsCheckmate(opponent))
							{
								Debug.Log((currentTurn == 0 ? "Red" : "Blue") + "wins by checkmate");

								// disable further interaction
								enabled = false;
							}
							else
							{
								currentTurn = opponent;
							}
							//currentTurn = 1 - currentTurn;
						}
					}
				}
			}
		}
		else
		{
			if (currentHover != -Vector2Int.one)
			{
				tiles[currentHover.x, currentHover.y].layer = LayerMask.NameToLayer("Tile");
				currentHover = -Vector2Int.one;
			}
			if(currentlyDragging && Input.GetMouseButtonDown(0))
			{
				currentlyDragging.SetPosition(GetTileCenter(currentlyDragging.currentX, currentlyDragging.currentY));
				currentlyDragging = null;
			}
		}
		HighlightGeneralIfInCheck();
	}

	private void GenerateAllTiles(float tileSize, int tileCountX, int tileCountY)
	{
		yOffset += transform.position.y;
		bounds = new Vector3((tileCountX / 2) * tileSize, 0, (tileCountX / 2) * tileSize) + boardCenter;

		tiles = new GameObject[tileCountX, tileCountY];
		for (int x = 0; x < tileCountX; x++)
			for (int y = 0; y < tileCountY; y++)
				tiles[x, y] = GenerateSingleTile(tileSize, x, y);
	}
	private GameObject GenerateSingleTile(float tileSize, int x, int y)
	{
		GameObject tileObject = new GameObject(string.Format("X:{0}, Y:{1}", x, y));
		tileObject.transform.parent = transform;

		Mesh mesh = new Mesh();
		tileObject.AddComponent<MeshFilter>().mesh = mesh;
		tileObject.AddComponent<MeshRenderer>().material = tileMaterial;

		Vector3[] verticies = new Vector3[4];
		verticies[0] = new Vector3(x * tileSize, yOffset, y * tileSize) - bounds;
		verticies[1] = new Vector3(x * tileSize, yOffset, (y+1) * tileSize) - bounds;
		verticies[2] = new Vector3((x + 1) * tileSize, yOffset, y * tileSize) - bounds;
		verticies[3] = new Vector3((x + 1) * tileSize, yOffset, (y + 1) * tileSize) - bounds;

		int[] tris = new int[] { 0, 1, 2, 1, 3, 2 };

		mesh.vertices = verticies;
		mesh.triangles = tris;

		mesh.RecalculateNormals();

		tileObject.layer = LayerMask.NameToLayer("Tile");
		tileObject.AddComponent<BoxCollider>();

		return tileObject;
	}

	private void SpawnAllPieces()
	{
		chessPieces = new ChessPiece[TILE_COUNT_X, TILE_COUNT_Y];

		int redTeam = 0, blueTeam = 1;

		//Red
		chessPieces[0, 0] = SpawnSinglePiece(ChessPieceType.Red_Chariot, redTeam);
		chessPieces[1, 0] = SpawnSinglePiece(ChessPieceType.Red_Horse, redTeam);
		chessPieces[2, 0] = SpawnSinglePiece(ChessPieceType.Red_Elephant, redTeam);
		chessPieces[3, 0] = SpawnSinglePiece(ChessPieceType.Red_Advisor, redTeam);
		chessPieces[4, 0] = SpawnSinglePiece(ChessPieceType.Red_General, redTeam);
		chessPieces[5, 0] = SpawnSinglePiece(ChessPieceType.Red_Advisor, redTeam);
		chessPieces[6, 0] = SpawnSinglePiece(ChessPieceType.Red_Elephant, redTeam);
		chessPieces[7, 0] = SpawnSinglePiece(ChessPieceType.Red_Horse, redTeam);
		chessPieces[8, 0] = SpawnSinglePiece(ChessPieceType.Red_Chariot, redTeam);
		chessPieces[1, 2] = SpawnSinglePiece(ChessPieceType.Red_Cannon, redTeam);
		chessPieces[7, 2] = SpawnSinglePiece(ChessPieceType.Red_Cannon, redTeam);
		for (int x = 0; x <= 8; x += 2)
		{
			chessPieces[x, 3] = SpawnSinglePiece(ChessPieceType.Red_Soldier, redTeam);
		}

		//Blue
		chessPieces[0, 9] = SpawnSinglePiece(ChessPieceType.Blue_Chariot, blueTeam);
		chessPieces[1, 9] = SpawnSinglePiece(ChessPieceType.Blue_Horse, blueTeam);
		chessPieces[2, 9] = SpawnSinglePiece(ChessPieceType.Blue_Elephant, blueTeam);
		chessPieces[3, 9] = SpawnSinglePiece(ChessPieceType.Blue_Advisor, blueTeam);
		chessPieces[4, 9] = SpawnSinglePiece(ChessPieceType.Blue_General, blueTeam);
		chessPieces[5, 9] = SpawnSinglePiece(ChessPieceType.Blue_Advisor, blueTeam);
		chessPieces[6, 9] = SpawnSinglePiece(ChessPieceType.Blue_Elephant, blueTeam);
		chessPieces[7, 9] = SpawnSinglePiece(ChessPieceType.Blue_Horse, blueTeam);
		chessPieces[8, 9] = SpawnSinglePiece(ChessPieceType.Blue_Chariot, blueTeam);
		chessPieces[1, 7] = SpawnSinglePiece(ChessPieceType.Blue_Cannon, blueTeam);
		chessPieces[7, 7] = SpawnSinglePiece(ChessPieceType.Blue_Cannon, blueTeam);
		for (int x = 0; x <= 8; x += 2)
		{
			chessPieces[x, 6] = SpawnSinglePiece(ChessPieceType.Blue_Soldier, blueTeam);
		}
	}
	private ChessPiece SpawnSinglePiece(ChessPieceType type, int team)
	{
		ChessPiece cp = Instantiate(prefabs[(int)type - 1], transform).GetComponent<ChessPiece>();

		cp.type = type;
		cp.team = team;

		return cp;
	}

	private void PositionAllPieces()
	{
		for (int x = 0; x < TILE_COUNT_X; x++)
			for(int y = 0;  y < TILE_COUNT_Y; y++)
				if (chessPieces[x, y] != null)
					PositionSinglePiece(x, y, true);
	}
	private void PositionSinglePiece(int x, int y, bool force = false)
	{
		chessPieces[x, y].currentX = x;
		chessPieces[x, y].currentY = y;
		bool isInitialPlacement = force;
		chessPieces[x, y].SetPosition(GetTileCenter(x, y), force);
	}

	private Vector3 GetTileCenter(int x, int y)
	{
		return new Vector3(x * tileSize, yOffset, y * tileSize) - bounds + new Vector3(tileSize / 2, 0, tileSize / 2);
	}

	private bool MoveTo(ChessPiece cp, int x, int y)
	{
		Vector2Int previousPosition = new Vector2Int(cp.currentX, cp.currentY);
		ChessPiece targetPiece = chessPieces[x, y];
		if (targetPiece != null && targetPiece.team == cp.team)
			return false;

		chessPieces[previousPosition.x, previousPosition.y] = null;
		ChessPiece captured = chessPieces[x, y];
		chessPieces[x, y] = cp;

		cp.currentX = x;
		cp.currentY = y;

		if (IsFlyingGeneralExposed())
		{
			chessPieces[previousPosition.x, previousPosition.y] = cp;
			chessPieces[x, y] = captured;
			cp.currentX = previousPosition.x;
			cp.currentY = previousPosition.y;
			return false;
		}

		if (captured != null)
			StartCoroutine(AnimateDeathAndDestroy(captured));

		PositionSinglePiece(x, y, false);
		return true;
	}
	private IEnumerator AnimateDeathAndDestroy(ChessPiece piece)
	{
		piece.SetScale(Vector3.zero);

		yield return new WaitForSeconds(0.5f);

		Destroy(piece.gameObject);
	}
	private Vector2Int LookupTileIndex(GameObject hitInfo)
	{
		for (int x = 0; x < TILE_COUNT_X; x++)
			for (int y = 0; y < TILE_COUNT_Y; y++)
				if (tiles[x, y] == hitInfo)
					return new Vector2Int(x, y);

		return -Vector2Int.one;

	}

	private bool IsTeamInCheck(int team)
	{
		Vector2Int generalPos = FindGeneralPosition(team);

		for (int x = 0; x < TILE_COUNT_X; x++)
		{
			for (int y = 0; y < TILE_COUNT_Y; y++)
			{
				ChessPiece piece = chessPieces[x, y];
				if (piece != null && piece.team != team)
				{
					var enemyMoves = piece.GetAvailableMoves(ref chessPieces, TILE_COUNT_X, TILE_COUNT_Y);
					if (enemyMoves.Contains(generalPos))
						return true;
				}
			}
		}
		return false;

	}

	private Vector2Int FindGeneralPosition(int team)
	{
		for (int x = 0; x < TILE_COUNT_X; x++)
		{
			for (int y = 0; y < TILE_COUNT_Y; y++)
			{
				ChessPiece piece = chessPieces[x, y];
				if (piece is General && piece.team == team)
				{
					return new Vector2Int(x, y);
				}
			}
		}
		return -Vector2Int.one;
	}

	private bool DoesMoveExposeGeneral(ChessPiece cp, Vector2Int target)
	{
		Vector2Int originalPos = new Vector2Int(cp.currentX, cp.currentY);
		ChessPiece targetPiece = chessPieces[target.x, target.y];

		chessPieces[originalPos.x, originalPos.y] = null;
		chessPieces[target.x, target.y] = cp;
		cp.currentX = target.x;
		cp.currentY = target.y;

		bool stillInCheck = IsTeamInCheck(cp.team);

		chessPieces[originalPos.x, originalPos.y] = cp;
		chessPieces[target.x, target.y] = targetPiece;
		cp.currentX = originalPos.x;
		cp.currentY = originalPos.y;

		return stillInCheck;
	}

	private void HighlightGeneralIfInCheck()
	{
		if (checkedGeneralPosition.HasValue)
		{
			Vector2Int prev = checkedGeneralPosition.Value;
			tiles[prev.x, prev.y].GetComponent<MeshRenderer>().material = originalTileMaterial;
			checkedGeneralPosition = null;
		}
		Vector2Int generalPos = FindGeneralPosition(currentTurn);
		if(IsTeamInCheck(currentTurn))
		{
			originalTileMaterial = tiles[generalPos.x, generalPos.y].GetComponent<MeshRenderer>().material;
			tiles[generalPos.x, generalPos.y].GetComponent<MeshRenderer>().material = checkHighlightMaterial;
			checkedGeneralPosition = generalPos;
		}
	}

	private bool IsFlyingGeneralExposed()
	{
		int generalX = -1;
		int redY = -1, blueY = -1;

		for (int x = 0; x < TILE_COUNT_X; x++)
		{
			int found = 0;
			for (int y = 0; y < TILE_COUNT_Y; y++)
			{
				var piece = chessPieces[x, y];
				if (piece is General)
				{
					if (piece.team == 0)
					{
						redY = y;
					}
					else
					{
						blueY = y;
					}
					generalX = x;
					found++;
				}
			}
			if (found == 2 && generalX != -1)
			{
				// check if any piece between 2 generals
				int minY = Mathf.Min(redY, blueY);
				int maxY = Mathf.Max(redY, blueY);
				for(int y = minY + 1; y < maxY; y++)
				{
					if (chessPieces[generalX, y] != null)
						return false; // not exposed
				}
				return true; // exposed (illegal)
			}
		}
		return false; 
	}

	private bool IsCheckmate(int team)
	{
		if (!IsTeamInCheck(team)) 
			return false;

		for (int x = 0;x < TILE_COUNT_X; x++)
		{
			for(int y = 0; y < TILE_COUNT_Y; y++)
			{
				ChessPiece piece = chessPieces[x, y];
				if (piece != null && piece.team == team)
				{
					List<Vector2Int> moves = piece.GetAvailableMoves(ref chessPieces, TILE_COUNT_X, TILE_COUNT_Y);
					foreach (var move in moves)
					{
						if (!DoesMoveExposeGeneral(piece, move))
							return false; // no one legal move to avoid check
					}
				}
			}
		}
		return true;
	}
}