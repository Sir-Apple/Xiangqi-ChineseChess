using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class Chessboard1 : MonoBehaviour
{
	[Header("Art stuff")]
	[SerializeField] private Material tileMaterial;
	[SerializeField] private float tileSize = 1.0f;
	[SerializeField] private float yOffset = 0.2f;
	[SerializeField] private Vector3 boardCenter = Vector3.zero;

	[Header("Prefabs && Materials")]
	[SerializeField] private GameObject[] prefabs;
	//	[SerializeField] private Material[] teamMaterials;

	private ChessPiece[,] chessPieces;
	private ChessPiece currentlyDragging; //Drag
	private List<Vector2Int> availableMoves = new List<Vector2Int>();
	private const int TILE_COUNT_X = 9;
	private const int TILE_COUNT_Y = 10;
	private GameObject[,] tiles;
	private Camera currentCamera;
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
				if (chessPieces[hitPosition.x, hitPosition.y] != null)
				{
					//turn switch
					if (true)
					{
						currentlyDragging = chessPieces[hitPosition.x, hitPosition.y];
						availableMoves = currentlyDragging.GetAvailableMoves(ref chessPieces, TILE_COUNT_X, TILE_COUNT_Y);
						HighlightTiles();
					}
				}
			}
			if (currentlyDragging != null && Input.GetMouseButtonUp(0))
			{
				Vector2Int previousPosition = new Vector2Int(currentlyDragging.currentX, currentlyDragging.currentY);
				RemoveHighlightTiles();
				bool validMove = MoveTo(currentlyDragging, hitPosition.x, hitPosition.y);
				if (!validMove)
				{
					currentlyDragging.SetPosition(GetTileCenter(previousPosition.x, previousPosition.y));
					currentlyDragging = null;
				}
				else
				{
					currentlyDragging = null;
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
			if (currentlyDragging && Input.GetMouseButtonDown(0))
			{
				currentlyDragging.SetPosition(GetTileCenter(currentlyDragging.currentX, currentlyDragging.currentY));
				currentlyDragging = null;
				RemoveHighlightTiles();
			}
		}
	}

	//Board
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
		verticies[1] = new Vector3(x * tileSize, yOffset, (y + 1) * tileSize) - bounds;
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

	//Spawning chess pieces
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
		chessPieces[0, 3] = SpawnSinglePiece(ChessPieceType.Red_Soldier, redTeam);
		chessPieces[2, 3] = SpawnSinglePiece(ChessPieceType.Red_Soldier, redTeam);
		chessPieces[4, 3] = SpawnSinglePiece(ChessPieceType.Red_Soldier, redTeam);
		chessPieces[6, 3] = SpawnSinglePiece(ChessPieceType.Red_Soldier, redTeam);
		chessPieces[8, 3] = SpawnSinglePiece(ChessPieceType.Red_Soldier, redTeam);

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
		chessPieces[0, 6] = SpawnSinglePiece(ChessPieceType.Blue_Soldier, blueTeam);
		chessPieces[2, 6] = SpawnSinglePiece(ChessPieceType.Blue_Soldier, blueTeam);
		chessPieces[4, 6] = SpawnSinglePiece(ChessPieceType.Blue_Soldier, blueTeam);
		chessPieces[6, 6] = SpawnSinglePiece(ChessPieceType.Blue_Soldier, blueTeam);
		chessPieces[8, 6] = SpawnSinglePiece(ChessPieceType.Blue_Soldier, blueTeam);
	}
	private ChessPiece SpawnSinglePiece(ChessPieceType type, int team)
	{
		ChessPiece cp = Instantiate(prefabs[(int)type - 1], transform).GetComponent<ChessPiece>();

		cp.type = type;
		cp.team = team;
		//		cp.GetComponent<MeshRenderer>().material = teamMaterials[team];

		return cp;
	}

	//Chess Positions
	private void PositionAllPieces()
	{
		for (int x = 0; x < TILE_COUNT_X; x++)
			for (int y = 0; y < TILE_COUNT_Y; y++)
				if (chessPieces[x, y] != null)
					PositionSinglePiece(x, y, true);
	}
	private void PositionSinglePiece(int x, int y, bool force = false)
	{
		chessPieces[x, y].currentX = x;
		chessPieces[x, y].currentY = y;
		chessPieces[x, y].SetPosition(GetTileCenter(x, y), force);
	}
	private Vector3 GetTileCenter(int x, int y)
	{
		return new Vector3(x * tileSize, yOffset, y * tileSize) - bounds + new Vector3(tileSize / 2, 0, tileSize / 2);
	}

	//Highlight Tiles
	private void HighlightTiles()
	{
		for (int i = 0; i < availableMoves.Count; i++)
			tiles[availableMoves[i].x, availableMoves[i].y].layer = LayerMask.NameToLayer("Highlight");
	}
	private void RemoveHighlightTiles()
	{
		for (int i = 0; i < availableMoves.Count; i++)
			tiles[availableMoves[i].x, availableMoves[i].y].layer = LayerMask.NameToLayer("Tile");

		availableMoves.Clear();
	}

	//Operations
	private bool MoveTo(ChessPiece cp, int x, int y)
	{
		Vector2Int previousPosition = new Vector2Int(cp.currentX, cp.currentY);

		if (chessPieces[x, y] != null)
		{
			ChessPiece ocp = chessPieces[x, y];

			if (cp.team == ocp.team)
				return false;
		}

		chessPieces[x, y] = cp;
		chessPieces[previousPosition.x, previousPosition.y] = null;

		PositionSinglePiece(x, y);

		return true;
	}
	private Vector2Int LookupTileIndex(GameObject hitInfo)
	{
		for (int x = 0; x < TILE_COUNT_X; x++)
			for (int y = 0; y < TILE_COUNT_Y; y++)
				if (tiles[x, y] == hitInfo)
					return new Vector2Int(x, y);

		return -Vector2Int.one;

	}

}