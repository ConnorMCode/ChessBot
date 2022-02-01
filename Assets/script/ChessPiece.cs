using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChessPiece : MonoBehaviour
{
    public GameObject controller;
    public GameObject movePlate;

    private int boardRank = -1;
    private int boardFile = -1;

    public int turnCount = 0;

    private ChessPiece myself;

    private int memX1;
    private int memY1;
    private int memX2;
    private int memY2;

    public bool attackingKing = false;

    public bool pinning = false;

    private string player;

    public Sprite bQueen, bKnight, bBishop, bKing, bRook, bPawn;
    public Sprite wQueen, wKnight, wBishop, wKing, wRook, wPawn;

    int crudeScore;

    List<Move> myMoves = new List<Move>();

    public void Activate(){
        controller = GameObject.FindGameObjectWithTag("GameController");

        switch(this.name){
            case "bQueen" : this.GetComponent<SpriteRenderer>().sprite = bQueen; player = "black"; break;
            case "wQueen" : this.GetComponent<SpriteRenderer>().sprite = wQueen; player = "white"; break;
            case "bKing" : this.GetComponent<SpriteRenderer>().sprite = bKing; player = "black"; break;
            case "wKing" : this.GetComponent<SpriteRenderer>().sprite = wKing; player = "white"; break;
            case "bPawn" : this.GetComponent<SpriteRenderer>().sprite = bPawn; player = "black"; break;
            case "wPawn" : this.GetComponent<SpriteRenderer>().sprite = wPawn; player = "white"; break;
            case "bBishop" : this.GetComponent<SpriteRenderer>().sprite = bBishop; player = "black"; break;
            case "wBishop" : this.GetComponent<SpriteRenderer>().sprite = wBishop; player = "white"; break;
            case "bKnight" : this.GetComponent<SpriteRenderer>().sprite = bKnight; player = "black"; break;
            case "wKnight" : this.GetComponent<SpriteRenderer>().sprite = wKnight; player = "white"; break;
            case "bRook" : this.GetComponent<SpriteRenderer>().sprite = bRook; player = "black"; break;
            case "wRook" : this.GetComponent<SpriteRenderer>().sprite = wRook; player = "white"; break;
        }
        myself = gameObject.GetComponent<ChessPiece>();
    }

    public void turn(){
        turnCount++;
    }

    public void unturn(){
        if(turnCount > 0){
            turnCount--;
        }
    }

    public int getTurn(){
        return turnCount;
    }

    public int GetBoardFile(){
        return boardFile;
    }

    public int GetBoardRank(){
        return boardRank;
    }

    public void SetBoardFile(int x){
        boardFile = x;
    }

    public void SetBoardRank(int y){
        boardRank = y;
    }

    public List<Move> getMoves(){
        return myMoves;
    }

    public bool getPlayer(){
        if(player == "white"){
            return true;        //white
        }else{
            return false;       //black
        }
    }

    public void setPlayer(string color){
        player = color;
    }

    public void SetMemory(int slot){
        if(slot == 1){
            memX1 = GetBoardFile();
            memY1 = GetBoardRank();
        }
        if(slot == 2){
            memX2 = GetBoardFile();
            memY2 = GetBoardRank();
        }
        
    }

    public void RefreshMemory(int slot){
        if(slot == 1){
            boardFile = memX1;
            boardRank = memY1;
        }
        if(slot == 2){
            boardFile = memX2;
            boardRank = memY2;
        }
    }

    public void Click(){
        myMoves.Clear();
        crudeScore = 100;

        DestroyMovePlates();

        InitiateMovePlates();

    }

    public void DestroyMovePlates(){
        GameObject[] movePlates = GameObject.FindGameObjectsWithTag("MovePlate");
        for(int i = 0; i < movePlates.Length; i++){
            Destroy(movePlates[i]);
        }
        foreach(ClickSquare cs in controller.GetComponent<Game>().clickSquares){
            cs.unassignPlate();
        }
    }

    public void InitiateMovePlates(){
        switch(this.name){
            case "bQueen":
            case "wQueen":
                LineMovePlate(1,0);
                LineMovePlate(0,1);
                LineMovePlate(1,1);
                LineMovePlate(-1,0);
                LineMovePlate(0,-1);
                LineMovePlate(-1,-1);
                LineMovePlate(-1,1);
                LineMovePlate(1,-1);
                break;
            case "bKnight":
            case "wKnight":
                LMovePlate();
                break;
            case "bBishop":
            case "wBishop":
                LineMovePlate(1,1);
                LineMovePlate(-1,-1);
                LineMovePlate(-1,1);
                LineMovePlate(1,-1);
                break;
            case "bKing":
            case "wKing":
                SurroundMovePlate();
                break;
            case "bRook":
            case "wRook":
                LineMovePlate(1,0);
                LineMovePlate(0,1);
                LineMovePlate(-1,0);
                LineMovePlate(0,-1);
                break;
            case "bPawn":
                PawnMovePlate(boardFile, boardRank - 1);
                break;
            case "wPawn":
                PawnMovePlate(boardFile, boardRank + 1);
                break;
        }
    }

    public void LineMovePlate(int xDir, int yDir){
        Game sc = controller.GetComponent<Game>();

        int x = boardFile + xDir;
        int y = boardRank + yDir;

        while(sc.PositionOnBoard(x,y) && sc.GetPosition(x,y).getPiece() == null){
            MovePlateSpawn(x,y);
            x += xDir;
            y += yDir;
        }

        if(sc.PositionOnBoard(x,y) && sc.GetPosition(x,y).getPiece() != null && sc.GetPosition(x,y).getPiece().player != player){
            MovePlateAttackSpawn(x, y);
        }

    }

    public void LMovePlate(){
        PointMovePlate(boardFile + 1, boardRank + 2);
        PointMovePlate(boardFile - 1, boardRank - 2);
        PointMovePlate(boardFile + 2, boardRank + 1);
        PointMovePlate(boardFile + 2, boardRank - 1);
        PointMovePlate(boardFile + 1, boardRank - 2);
        PointMovePlate(boardFile - 1, boardRank + 2);
        PointMovePlate(boardFile - 2, boardRank + 1);
        PointMovePlate(boardFile - 2, boardRank - 1);
    }

    public void SurroundMovePlate(){
        Game sc = controller.GetComponent<Game>();
        if(turnCount == 0){
            if(this.name == "wKing"){
                if(sc.GetPosition(5, 0).getPiece() == null && sc.GetPosition(6, 0).getPiece() == null){
                    ClickSquare cp = sc.GetPosition(7, 0);
                    if(cp.getPiece() != null && cp.getPiece().getTurn() == 0){
                        Castle(6,0);
                    }
                }
                if(sc.GetPosition(3, 0).getPiece() == null && sc.GetPosition(2, 0).getPiece() == null && sc.GetPosition(1, 0).getPiece() == null){
                    ClickSquare cp = sc.GetPosition(0, 0);
                    if(cp.getPiece() != null && cp.getPiece().getTurn() == 0){
                        Castle(2,0);
                    }
                }
            }else if(this.name == "bKing"){
                if(sc.GetPosition(5, 7).getPiece() == null && sc.GetPosition(6, 7).getPiece() == null){
                    ClickSquare cp = sc.GetPosition(7, 7);
                    if(cp.getPiece() != null && cp.getPiece().getTurn() == 0){
                        Castle(6,7);
                    }
                }
                if(sc.GetPosition(3, 7).getPiece() == null && sc.GetPosition(2, 7).getPiece() == null && sc.GetPosition(1, 7).getPiece() == null){
                    ClickSquare cp = sc.GetPosition(0, 7);
                    if(cp.getPiece() != null && cp.getPiece().getTurn() == 0){
                        Castle(2,7);
                    }
                }
            }
        }
        PointMovePlate(boardFile, boardRank + 1);
        PointMovePlate(boardFile, boardRank - 1);
        PointMovePlate(boardFile + 1, boardRank + 1);
        PointMovePlate(boardFile + 1, boardRank - 1);
        PointMovePlate(boardFile + 1, boardRank);
        PointMovePlate(boardFile - 1, boardRank + 1);
        PointMovePlate(boardFile - 1, boardRank - 1);
        PointMovePlate(boardFile - 1, boardRank);
    }

    public void Castle(int x, int y){
        MovePlateCastleSpawn(x, y);
    }

    public void PointMovePlate(int x, int y){
        Game sc = controller.GetComponent<Game>();
        if(sc.PositionOnBoard(x,y)){
            ClickSquare cp = sc.GetPosition(x, y);

            if(cp.wAttack && this.name == "bKing"){
                return;
            }else if(cp.bAttack && this.name == "wKing"){
                return;
            }

            if(cp.getPiece() == null){
                MovePlateSpawn(x, y);
            }else if(cp.getPiece() != null && cp.getPiece().player != player){
                MovePlateAttackSpawn(x, y);
            }
        }
    }

    public void PawnMovePlate(int x, int y){
        Game sc = controller.GetComponent<Game>();
        if(turnCount == 0){
            if(this.name == "wPawn"){
                if(sc.PositionOnBoard(x, y)){
                    if(sc.GetPosition(x,y).getPiece() == null){
                        MovePlateSpawn(x,y);
                        if(sc.PositionOnBoard(x, y+1)){
                            if(sc.GetPosition(x,y+1).getPiece() == null){
                                MovePlateSpawn(x, y+1);
                            }
                        }
                    }
                }
            }else if(this.name == "bPawn"){
                if(sc.PositionOnBoard(x, y)){
                    if(sc.GetPosition(x,y).getPiece() == null){
                        MovePlateSpawn(x, y);
                        if(sc.PositionOnBoard(x, y-1)){
                            if(sc.GetPosition(x,y-1).getPiece() == null){
                                MovePlateSpawn(x,y-1);
                            }
                        }
                    }
                }
            }
            if(sc.PositionOnBoard(x + 1, y) && sc.GetPosition(x + 1, y) != null){
                    if(sc.GetPosition(x+1,y).getPiece() != null && sc.GetPosition(x+1,y).getPiece().player != player){
                        MovePlateAttackSpawn(x+1, y);
                    }
                    
                }

                if(sc.PositionOnBoard(x - 1, y) && sc.GetPosition(x - 1, y) != null){
                    if(sc.GetPosition(x-1,y).getPiece() != null && sc.GetPosition(x-1,y).getPiece().player != player){
                        MovePlateAttackSpawn(x-1, y);
                    }
                }
        }else{
            if(sc.PositionOnBoard(x, y)){
                if(sc.GetPosition(x,y).getPiece() == null){
                    MovePlateSpawn(x, y);
                }

                if(sc.PositionOnBoard(x + 1, y) && sc.GetPosition(x + 1, y) != null){
                    if(sc.GetPosition(x+1,y).getPiece() != null && sc.GetPosition(x+1,y).getPiece().player != player){
                        MovePlateAttackSpawn(x+1, y);
                    }
                    
                }

                if(sc.PositionOnBoard(x - 1, y) && sc.GetPosition(x - 1, y) != null){
                    if(sc.GetPosition(x-1,y).getPiece() != null && sc.GetPosition(x-1,y).getPiece().player != player){
                        MovePlateAttackSpawn(x-1, y);
                    }
                }
            }
        }
    }

    public void MovePlateSpawn(int matrixX, int matrixY){
        controller = GameObject.FindGameObjectWithTag("GameController");
        if(controller.GetComponent<Game>().MPSpawn(matrixX, matrixY, myself)){
            crudeScore += 10 - (Mathf.Abs(matrixX - matrixY));
            if(player == "white"){
                if(controller.GetComponent<Game>().GetPosition(matrixX, matrixY).bAttack){
                    switch(myself.name){
                        case "wPawn":
                            crudeScore -= 10;
                            break;
                        case "wRook":
                            crudeScore -= 50;
                            break;
                        case "wKnight":
                            crudeScore -= 30;
                            break;
                        case "wBishop":
                            crudeScore -= 30;
                            break;
                        case "wQueen":
                            crudeScore -= 100;
                            break;
                    }
                }
            }else if (player == "black"){
                if(controller.GetComponent<Game>().GetPosition(matrixX, matrixY).wAttack){
                    switch(myself.name){
                        case "bPawn":
                            crudeScore -= 10;
                            break;
                        case "bRook":
                            crudeScore -= 50;
                            break;
                        case "bKnight":
                            crudeScore -= 30;
                            break;
                        case "bBishop":
                            crudeScore -= 30;
                            break;
                        case "bQueen":
                            crudeScore -= 100;
                            break;
                    }
                }
            }
            Move move = new Move();
            move.piece = myself;
            move.moveX = matrixX;
            move.moveY = matrixY;
            move.pieceX = boardFile;
            move.pieceY = boardRank;
            move.crude = crudeScore;
            myMoves.Add(move);
        }
        crudeScore = 100;
    }

    public void MovePlateAttackSpawn(int matrixX, int matrixY){

        controller = GameObject.FindGameObjectWithTag("GameController");
        if(controller.GetComponent<Game>().MPASpawn(matrixX, matrixY, myself)){
            crudeScore += 10 - (Mathf.Abs(matrixX - matrixY));
            string opponent = controller.GetComponent<Game>().GetPosition(matrixX, matrixY).getPiece().name;
            attackScore(myself.name, opponent);
            Move move = new Move();
            move.piece = myself;
            move.moveX = matrixX;
            move.moveY = matrixY;
            move.pieceX = boardFile;
            move.pieceY = boardRank;
            move.crude = crudeScore;
            myMoves.Add(move);
        }
        crudeScore = 100;
    }

    public void MovePlateCastleSpawn(int x, int y){
        controller = GameObject.FindGameObjectWithTag("GameController");
        if(controller.GetComponent<Game>().MPCSPawn(x, y, myself)){
            crudeScore += 10 - (Mathf.Abs(x - y));
            Move move = new Move();
            move.piece = myself;
            move.moveX = x;
            move.moveY = y;
            move.pieceX = boardFile;
            move.pieceY = boardRank;
            move.crude = crudeScore;
            myMoves.Add(move);
        }
        crudeScore = 100;
    }

    public void attackScore(string myName, string opponent){
        switch(myName){
            case "bPawn":
                switch(opponent){
                    case "wPawn":
                        break;
                    case "wRook":
                        crudeScore += 40;
                        break;
                    case "wKnight":
                        crudeScore += 20;
                        break;
                    case "wBishop":
                        crudeScore += 20;
                        break;
                    case "wQueen":
                        crudeScore += 90;
                        break;
                }
            break;
            case "bRook":
                switch(opponent){
                    case "wPawn":
                        crudeScore -= 40;
                        break;
                    case "wRook":
                        break;
                    case "wKnight":
                        crudeScore -= 20;
                        break;
                    case "wBishop":
                        crudeScore -= 20;
                        break;
                    case "wQueen":
                        crudeScore += 30;
                        break;
                }
            break;
            case "bKnight":
                switch(opponent){
                    case "wPawn":
                        crudeScore -= 20;
                        break;
                    case "wRook":
                        crudeScore += 20;
                        break;
                    case "wKnight":
                        break;
                    case "wBishop":
                        break;
                    case "wQueen":
                        crudeScore += 50;
                        break;
                }
            break;
            case "bBishop":
                switch(opponent){
                    case "wPawn":
                        crudeScore -= 20;
                        break;
                    case "wRook":
                        crudeScore += 20;
                        break;
                    case "wKnight":
                        break;
                    case "wBishop":
                        break;
                    case "wQueen":
                        crudeScore += 50;
                        break;
                }
            break;
            case "bQueen":
                switch(opponent){
                    case "wPawn":
                        crudeScore -= 90;
                        break;
                    case "wRook":
                        crudeScore -= 30;
                        break;
                    case "wKnight":
                        crudeScore -= 50;
                        break;
                    case "wBishop":
                        crudeScore -= 50;
                        break;
                    case "wQueen":
                        break;
                }
            break;
            case "wPawn":
                switch(opponent){
                    case "bPawn":
                        break;
                    case "bRook":
                        crudeScore += 40;
                        break;
                    case "bKnight":
                        crudeScore += 20;
                        break;
                    case "bBishop":
                        crudeScore += 20;
                        break;
                    case "bQueen":
                        crudeScore += 90;
                        break;
                }
            break;
            case "wRook":
                switch(opponent){
                    case "bPawn":
                        crudeScore -= 40;
                        break;
                    case "bRook":
                        break;
                    case "bKnight":
                        crudeScore -= 20;
                        break;
                    case "bBishop":
                        crudeScore -= 20;
                        break;
                    case "bQueen":
                        crudeScore += 30;
                        break;
                }
            break;
            case "wKnight":
                switch(opponent){
                    case "bPawn":
                        crudeScore -= 20;
                        break;
                    case "bRook":
                        crudeScore += 20;
                        break;
                    case "bKnight":
                        break;
                    case "bBishop":
                        break;
                    case "bQueen":
                        crudeScore += 50;
                        break;
                }
            break;
            case "wBishop":
                switch(opponent){
                    case "bPawn":
                        crudeScore -= 20;
                        break;
                    case "bRook":
                        crudeScore += 20;
                        break;
                    case "bKnight":
                        break;
                    case "bBishop":
                        break;
                    case "bQueen":
                        crudeScore += 50;
                        break;
                }
            break;
            case "wQueen":
                switch(opponent){
                    case "bPawn":
                        crudeScore -= 90;
                        break;
                    case "bRook":
                        crudeScore -= 30;
                        break;
                    case "bKnight":
                        crudeScore -= 50;
                        break;
                    case "bBishop":
                        crudeScore -= 50;
                        break;
                    case "bQueen":
                        break;
                }
            break;
        }
    }
}
