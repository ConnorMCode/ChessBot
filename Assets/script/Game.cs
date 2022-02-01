using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{

    [SerializeField] BoardManager boardManager;
    [SerializeField] ClickSquare _clickSquarePF;

    [SerializeField] BlackMoves bot;
    public GameObject ChessPiece;

    public GameObject movePlate;

    public bool wCheck;
    public bool bCheck;

    public bool bStale = false;
    public bool wStale = false;

    public bool mate;

    private GameObject[,] positions = new GameObject[8,8];

    public ClickSquare[,] clickSquares = new ClickSquare[8,8];
    private string[,] testBoard = new string[8,8] {{"x", "x", "x", "x", "x", "x", "x", "x"},
                                                   {"x", "x", "x", "x", "x", "x", "x", "x"},
                                                   {"x", "x", "x", "x", "x", "x", "x", "x"},
                                                   {"x", "x", "x", "x", "x", "x", "x", "x"},
                                                   {"x", "x", "x", "x", "x", "x", "x", "x"},
                                                   {"x", "x", "x", "x", "x", "x", "x", "x"},
                                                   {"x", "x", "x", "x", "x", "x", "x", "x"},
                                                   {"x", "x", "x", "x", "x", "x", "x", "x"}};

    private ChessPiece[] playerBlack = new ChessPiece[16];
    private List<ChessPiece> piecesOnBoard = new List<ChessPiece>();
    private ChessPiece[] playerWhite = new ChessPiece[16];

    private string currentPlayer = "white";

    private bool gameOver = false;

    private GameObject loader;

    void Start()
    {
        playerWhite = new ChessPiece[]{
            Create("wPawn",0,1), Create("wPawn",1,1), Create("wPawn",2,1), Create("wPawn",3,1), Create("wPawn",4,1), Create("wPawn",5,1), Create("wPawn",6,1), Create("wPawn",7,1), 
            Create("wRook",0,0), Create("wKnight",1,0), Create("wBishop",2,0), Create("wQueen",3,0), Create("wKing",4,0), Create("wBishop",5,0), Create("wKnight",6,0), Create("wRook",7,0)
        };
        playerBlack = new ChessPiece[]{
            Create("bPawn",0,6), Create("bPawn",1,6), Create("bPawn",2,6), Create("bPawn",3,6), Create("bPawn",4,6), Create("bPawn",5,6), Create("bPawn",6,6), Create("bPawn",7,6), 
            Create("bRook",0,7), Create("bKnight",1,7), Create("bBishop",2,7), Create("bQueen",3,7), Create("bKing",4,7), Create("bBishop",5,7), Create("bKnight",6,7), Create("bRook",7,7)
        };

        GameObject obj;

        for(int file = 0; file < 8; file++){
            for(int rank = 0; rank < 8; rank++){
                var spawnedClickSquare = Instantiate(_clickSquarePF, new Vector3(file,rank,-2),Quaternion.identity);
                clickSquares[file, rank] = spawnedClickSquare;
                obj = positions[file, rank];
                spawnedClickSquare.name = $"clickSquare {file}, {rank}";
            }
        }

        updateBoard();

        boardManager.GenerateBoard();
    }

    public void moveBlack(){
        if(!mate){
            bot.performMove();
        }
    }

    public List<ChessPiece> getPieces(){
        return piecesOnBoard;
    }

    public bool MPSpawn(int matrixX, int matrixY, ChessPiece piece){
        int x = piece.GetBoardFile();
        int y = piece.GetBoardRank();

        if(illegalMove(x, y, matrixX, matrixY, piece, false, false)){
            return false;
        }

        GameObject mp = Instantiate(movePlate, new Vector3(matrixX, matrixY, -1.0f), Quaternion.identity);
        mp.name = $"Move Plate {matrixX} {matrixY}";

        MovePlate mpScript = mp.GetComponent<MovePlate>();
        mpScript.SetReference(piece);
        mpScript.SetCoords(matrixX, matrixY);
        clickSquares[matrixX, matrixY].assignPlate(mpScript);

        return true;
        
    }

    public bool MPASpawn(int matrixX, int matrixY, ChessPiece piece){
        int x = piece.GetBoardFile();
        int y = piece.GetBoardRank();

        if(illegalMove(x, y, matrixX, matrixY, piece, true, false)){
            return false;
        }

        GameObject mp = Instantiate(movePlate, new Vector3(matrixX, matrixY, -1.0f), Quaternion.identity);
        mp.name = $"Move Plate {matrixX} {matrixY}";

        MovePlate mpScript = mp.GetComponent<MovePlate>();
        mpScript.attack = true;
        mpScript.SetReference(piece);
        mpScript.SetCoords(matrixX, matrixY);
        clickSquares[matrixX, matrixY].assignPlate(mpScript);
        return true;
    }

    public bool MPCSPawn(int matrixX, int matrixY, ChessPiece piece){
        int x = piece.GetBoardFile();
        int y = piece.GetBoardRank();

        if(illegalMove(x, y, matrixX, matrixY, piece, false, true)){
            return false;
        }
        GameObject mp = Instantiate(movePlate, new Vector3(matrixX, matrixY, -1.0f), Quaternion.identity);
        mp.name = $"Move Plate {matrixX} {matrixY}";

        MovePlate mpScript = mp.GetComponent<MovePlate>();
        mpScript.castle = true;
        mpScript.SetReference(piece);
        mpScript.SetCoords(matrixX, matrixY);
        clickSquares[matrixX, matrixY].assignPlate(mpScript);
        return true;
    }

    public bool illegalMove(int x, int y, int matrixX, int matrixY, ChessPiece piece, bool attack, bool castle){
        int count = 0;
        bool pinned = false;
        foreach(ChessPiece cp in piecesOnBoard){
            if(piece.getPlayer()){
                if(!cp.getPlayer() && cp.attackingKing){
                    count++;
                }
                if(!cp.getPlayer() && cp.pinning){
                    pinned = true;
                }
            }else{
                if(cp.getPlayer() && cp.attackingKing){
                    count++;
                }
                if(cp.getPlayer() && cp.pinning){
                    pinned = true;
                }
            }
        }
        if(count > 0){
            if(piece.name == "wKing"){
                if(!GetPosition(matrixX, matrixY).pbAttack && !castle){
                    return false;
                }else{
                    return true;
                }
            }else if(piece.name == "bKing"){
                if(!GetPosition(matrixX, matrixY).pwAttack && !castle){
                    return false;
                }else{
                    return true;
                }
            }
            if(count == 1){
                if(piece.getPlayer() && piece.name != "wKing"){
                    ChessPiece attacker = piecesOnBoard.Find(x => (!x.getPlayer() && x.attackingKing));
                    if(GetPosition(matrixX, matrixY).pbAttack){
                        return false;
                    }else if(GetPosition(matrixX,matrixY).getPiece() == attacker){
                        return false;
                    }else{
                        return true;
                    }
                }else if(!piece.getPlayer() && piece.name != "bKing"){
                    ChessPiece attacker = piecesOnBoard.Find(x => (x.getPlayer() && x.attackingKing));
                    if(GetPosition(matrixX, matrixY).pwAttack){
                        return false;
                    }else if(GetPosition(matrixX,matrixY).getPiece() == attacker){
                        return false;
                    }else{
                        return true;
                    }
                }
            }
        }else{
            if(piece.getPlayer() && pinned && GetPosition(x, y).pbAttack){
                if(GetPosition(matrixX, matrixY).pbAttack){
                    return false;
                }else{
                    return true;
                }
            }else if(piece.getPlayer()){
                return false;
            }
            if(!piece.getPlayer() && pinned && GetPosition(x, y).pwAttack){
                return true;
            }else if(!piece.getPlayer()){
                return false;
            }
        }
        if(piece.name == "wKing"){
            if(GetPosition(matrixX, matrixY).bAttack){
                return true;
            }else{
                return false;
            }
        }
        if(piece.name == "wKing"){
            if(GetPosition(matrixX, matrixY).wAttack){
                return true;
            }else{
                return false;
            }
        }
        return false;
    }

    public bool findLegalMoves(string side, bool test){
        updateBoard();
        if(side == "black"){
            foreach(ClickSquare cs in clickSquares){
                if(cs.getPiece() != null){
                    if(!cs.getPiece().getPlayer()){
                        cs.getPiece().InitiateMovePlates();
                    }
                }
            }
            List<GameObject> movePlates = new List<GameObject>(GameObject.FindGameObjectsWithTag("MovePlate"));
            movePlates.RemoveAll(obj => obj.GetComponent<MovePlate>().GetReference().getPlayer());
            if(movePlates.Count == 0){
                if(!bCheck){
                    bStale = true;
                }else if(test){
                    return true;
                }else{
                    checkMate();
                    return true;
                }
            }
            for(int i = 0; i < movePlates.Count; i++){
                Destroy(movePlates[i]);
            }
        }else if(side == "white"){
            foreach(ClickSquare cs in clickSquares){
                if(cs.getPiece() != null){
                    if(cs.getPiece().getPlayer()){
                        cs.getPiece().InitiateMovePlates();
                    }
                }
            }
            List<GameObject> movePlates = new List<GameObject>(GameObject.FindGameObjectsWithTag("MovePlate"));
            movePlates.RemoveAll(obj => !obj.GetComponent<MovePlate>().GetReference().getPlayer());
            if(movePlates.Count == 0){
                Debug.Log("here");
                if(!wCheck){
                    wStale = true;
                }else if(test){
                    return true;
                }else{
                    checkMate();
                    return true;
                }
            }
            for(int i = 0; i < movePlates.Count; i++){
                Destroy(movePlates[i]);
            }
        }
        return false;
    }

    public void printBoard(){
        Debug.Log(testBoard[0,7] + testBoard[1,7] + testBoard[2,7] + testBoard[3,7] + testBoard[4,7] + testBoard[5,7] + testBoard[6,7] + testBoard[7,7] + "\n" +
                  testBoard[0,6] + testBoard[1,6] + testBoard[2,6] + testBoard[3,6] + testBoard[4,6] + testBoard[5,6] + testBoard[6,6] + testBoard[7,6] + "\n" +
                  testBoard[0,5] + testBoard[1,5] + testBoard[2,5] + testBoard[3,5] + testBoard[4,5] + testBoard[5,5] + testBoard[6,5] + testBoard[7,5] + "\n" +
                  testBoard[0,4] + testBoard[1,4] + testBoard[2,4] + testBoard[3,4] + testBoard[4,4] + testBoard[5,4] + testBoard[6,4] + testBoard[7,4] + "\n" +
                  testBoard[0,3] + testBoard[1,3] + testBoard[2,3] + testBoard[3,3] + testBoard[4,3] + testBoard[5,3] + testBoard[6,3] + testBoard[7,3] + "\n" +
                  testBoard[0,2] + testBoard[1,2] + testBoard[2,2] + testBoard[3,2] + testBoard[4,2] + testBoard[5,2] + testBoard[6,2] + testBoard[7,2] + "\n" +
                  testBoard[0,1] + testBoard[1,1] + testBoard[2,1] + testBoard[3,1] + testBoard[4,1] + testBoard[5,1] + testBoard[6,1] + testBoard[7,1] + "\n" +
                  testBoard[0,0] + testBoard[1,0] + testBoard[2,0] + testBoard[3,0] + testBoard[4,0] + testBoard[5,0] + testBoard[6,0] + testBoard[7,0]);
    }

    public ChessPiece Create(string name, int x, int y){
        GameObject obj = Instantiate(ChessPiece, new Vector3(0,0,5), Quaternion.identity);

        ChessPiece cp = obj.GetComponent<ChessPiece>();

        cp.name = name;
        cp.SetBoardFile(x);
        cp.SetBoardRank(y);
        cp.Activate();

        piecesOnBoard.Add(cp);

        return cp;
    }

    public void updateAttacks(){
        foreach(ClickSquare cs in clickSquares){
            cs.GetComponent<SpriteRenderer>().color = new Color(0.0f, 0.0f, 0.0f, 0.0f);
            cs.wAttack = false;
            cs.bAttack = false;
            cs.pwAttack = false;
            cs.pbAttack = false;
        }
        foreach(ChessPiece cp in piecesOnBoard){
            cp.attackingKing = false;
            switch(cp.name){
            case "bQueen":
            case "wQueen":
                LineCheck(1,0, cp);
                LineCheck(0,1, cp);
                LineCheck(1,1, cp);
                LineCheck(-1,0, cp);
                LineCheck(0,-1, cp);
                LineCheck(-1,-1, cp);
                LineCheck(-1,1, cp);
                LineCheck(1,-1, cp);
                break;
            case "bKnight":
            case "wKnight":
                LCheck(cp);
                break;
            case "bBishop":
            case "wBishop":
                LineCheck(1,1, cp);
                LineCheck(-1,-1, cp);
                LineCheck(-1,1, cp);
                LineCheck(1,-1, cp);
                break;
            case "bKing":
            case "wKing":
                SurroundCheck(cp);
                break;
            case "bRook":
            case "wRook":
                LineCheck(1,0, cp);
                LineCheck(0,1, cp);
                LineCheck(-1,0, cp);
                LineCheck(0,-1, cp);
                break;
            case "bPawn":
                PawnCheck(cp.GetBoardFile(), cp.GetBoardRank() - 1, cp);
                break;
            case "wPawn":
                PawnCheck(cp.GetBoardFile(), cp.GetBoardRank() + 1, cp);
                break;
            }
        }
        checkCheck();
    }

    public void checkCheck(){
        GameObject obj = GameObject.Find("wKing");
        ChessPiece wk = obj.GetComponent<ChessPiece>();

        obj = GameObject.Find("bKing");
        ChessPiece bk = obj.GetComponent<ChessPiece>();

        if(GetPosition(bk.GetBoardFile(), bk.GetBoardRank()).wAttack){
            bCheck = true;
        }else{
            bCheck = false;
        }
        if(GetPosition(wk.GetBoardFile(), wk.GetBoardRank()).bAttack){
            wCheck = true;
        }else{
            wCheck = false;
        }
    }

    public void LineCheck(int xDir, int yDir, ChessPiece cp){
        int x = cp.GetBoardFile() + xDir;
        int y = cp.GetBoardRank() + yDir;
        int x2;
        int y2;
        bool pieceFound = false;
        bool secondPieceFound = false;

        while(PositionOnBoard(x,y)){
            if(cp.getPlayer() && !pieceFound){
                GetPosition(x,y).wAttack = true;
            }else if(!cp.getPlayer() && !pieceFound){
                GetPosition(x,y).bAttack = true;
            }
            if(pieceFound && GetPosition(x,y).getPiece() != null && !secondPieceFound){
                secondPieceFound = true;
                if(cp.getPlayer()){
                    if(GetPosition(x,y).getPiece() != null && GetPosition(x,y).getPiece().name == "bKing"){
                        cp.pinning = true;
                        x2 = x;
                        y2 = y;
                        if(PositionOnBoard((x2+xDir), (y2+yDir))){
                            x2 += xDir;
                            y2 += yDir;
                        }
                        while(x2 != cp.GetBoardFile() || y2 != cp.GetBoardRank()){
                            GetPosition(x2,y2).pwAttack = true;
                            x2 -= xDir;
                            y2 -= yDir;
                        }
                    }
                }else{
                    if(GetPosition(x,y).getPiece() != null && GetPosition(x,y).getPiece().name == "wKing"){
                        cp.pinning = true;
                        x2 = x;
                        y2 = y;
                        if(PositionOnBoard((x2+xDir), (y2+yDir))){
                            x2 += xDir;
                            y2 += yDir;
                        }
                        while(x2 != cp.GetBoardFile() || y2 != cp.GetBoardRank()){
                            GetPosition(x2,y2).pbAttack = true;
                            x2 -= xDir;
                            y2 -= yDir;
                        }
                    }
                }
            }
            if(GetPosition(x,y).getPiece() != null && !pieceFound){
                if(cp.getPlayer()){
                    GetPosition(x,y).wAttack = true;
                    if(GetPosition(x,y).getPiece().name == "bKing"){
                        cp.attackingKing = true;
                        x2 = x;
                        y2 = y;
                        if(PositionOnBoard((x2+xDir), (y2+yDir))){
                            x2 += xDir;
                            y2 += yDir;
                        }
                        while(x2 != cp.GetBoardFile() && y2 != cp.GetBoardRank()){
                            GetPosition(x2,y2).pwAttack = true;
                            x2 -= xDir;
                            y2 -= yDir;
                        }
                    }
                }else{
                    GetPosition(x,y).bAttack = true;
                    if(GetPosition(x,y).getPiece().name == "wKing"){
                        cp.attackingKing = true;
                        x2 = x;
                        y2 = y;
                        if(PositionOnBoard((x2+xDir), (y2+yDir))){
                            x2 += xDir;
                            y2 += yDir;
                        }
                        while(x2 != cp.GetBoardFile() || y2 != cp.GetBoardRank()){
                            GetPosition(x2,y2).pbAttack = true;
                            x2 -= xDir;
                            y2 -= yDir;
                        }
                    }
                }
                pieceFound = true;
            }
            if(cp.getPlayer() && !secondPieceFound){
                if(GetPosition(x,y).getPiece() != null && GetPosition(x,y).getPiece().name == "bKing"){
                    cp.attackingKing = true;
                    x2 = x;
                    y2 = y;
                    while(x2 != cp.GetBoardFile() || y2 != cp.GetBoardRank()){
                        GetPosition(x2,y2).pwAttack = true;
                        x2 -= xDir;
                        y2 -= yDir;
                    }
                }
            }else if(!secondPieceFound){
                if(GetPosition(x,y).getPiece() != null && GetPosition(x,y).getPiece().name == "wKing"){
                    cp.attackingKing = true;
                    x2 = x;
                    y2 = y;
                    while(x2 != cp.GetBoardFile() || y2 != cp.GetBoardRank()){
                        GetPosition(x2,y2).pbAttack = true;
                        x2 -= xDir;
                        y2 -= yDir;
                    }
                }
            }
            x += xDir;
            y += yDir;
        }
        if(PositionOnBoard(x,y)){
            if(cp.getPlayer()){
                GetPosition(x,y).wAttack = true;
                if(GetPosition(x,y).getPiece().name == "bKing"){
                        cp.attackingKing = true;
                    }
            }else{
                GetPosition(x,y).bAttack = true;
                if(GetPosition(x,y).getPiece().name == "wKing"){
                        cp.attackingKing = true;
                    }
            }
        }
    }

    public void LCheck(ChessPiece cp){
        PointCheck(cp.GetBoardFile() + 1, cp.GetBoardRank() + 2, cp);
        PointCheck(cp.GetBoardFile() - 1, cp.GetBoardRank() - 2, cp);
        PointCheck(cp.GetBoardFile() + 2, cp.GetBoardRank() + 1, cp);
        PointCheck(cp.GetBoardFile() + 2, cp.GetBoardRank() - 1, cp);
        PointCheck(cp.GetBoardFile() + 1, cp.GetBoardRank() - 2, cp);
        PointCheck(cp.GetBoardFile() - 1, cp.GetBoardRank() + 2, cp);
        PointCheck(cp.GetBoardFile() - 2, cp.GetBoardRank() + 1, cp);
        PointCheck(cp.GetBoardFile() - 2, cp.GetBoardRank() - 1, cp);
    }

    public void SurroundCheck(ChessPiece cp){
        PointCheck(cp.GetBoardFile(), cp.GetBoardRank() + 1, cp);
        PointCheck(cp.GetBoardFile(), cp.GetBoardRank() - 1, cp);
        PointCheck(cp.GetBoardFile() + 1, cp.GetBoardRank() + 1, cp);
        PointCheck(cp.GetBoardFile() + 1, cp.GetBoardRank() - 1, cp);
        PointCheck(cp.GetBoardFile() + 1, cp.GetBoardRank(), cp);
        PointCheck(cp.GetBoardFile() - 1, cp.GetBoardRank() + 1, cp);
        PointCheck(cp.GetBoardFile() - 1, cp.GetBoardRank() - 1, cp);
        PointCheck(cp.GetBoardFile() - 1, cp.GetBoardRank(), cp);
    }

    public void PointCheck(int x, int y, ChessPiece cp){
        if(PositionOnBoard(x,y)){
            if(cp.getPlayer()){
                GetPosition(x,y).wAttack = true;
            }else{
                GetPosition(x,y).bAttack = true;
            }
        }
    }

    public void PawnCheck(int x, int y, ChessPiece cp){
        if(PositionOnBoard(x+1, y)){
            if(cp.getPlayer()){
                GetPosition(x+1,y).wAttack = true;
            }else{
                GetPosition(x+1,y).bAttack = true;
            }
        }
        if(PositionOnBoard(x-1, y)){
            if(cp.getPlayer()){
                GetPosition(x-1,y).wAttack = true;
            }else{
                GetPosition(x-1,y).bAttack = true;
            }
        }
    }

    public void updateBoard(){
        foreach(ClickSquare cs in clickSquares){
            if(cs.getPiece() != null){
                cs.removePiece();
            }
        }
        foreach(ChessPiece cp in piecesOnBoard){
            clickSquares[cp.GetBoardFile(), cp.GetBoardRank()].Init(cp);
        }
    }

    public void AttackPosition(int x, int y){
        testBoard[x, y] = "x";
        piecesOnBoard.Remove(GetPosition(x,y).getPiece());
        clickSquares[x, y].removePiece();
    }

    public void removePiece(ChessPiece cp, bool destroy){
        piecesOnBoard.Remove(cp);
        if(destroy){
            Debug.Log("Queen Down");
            Destroy(cp.gameObject);
        }
    }

    public void addPiece(ChessPiece cp){
        if(piecesOnBoard.Contains(cp)){
            return;
        }
        piecesOnBoard.Add(cp);
    }

    public ClickSquare GetPosition(int x, int y){
        return clickSquares[x, y];
    }

    public bool PositionOnBoard(int x, int y){
        if(x < 0 || y < 0 || x >= clickSquares.GetLength(0) || y >= clickSquares.GetLength(1)) return false;
        return true; 
    }

    public int calculateScore(){
        int totalScore = 0;
        int whiteScore = 0;
        int blackScore = 0;
        int centerControl = 0;
        foreach(ChessPiece cp in piecesOnBoard){
            if(cp.getPlayer()){
                switch(cp.name){
                    case "wPawn":
                        whiteScore += 10;
                        break;
                    case "wRook":
                        whiteScore += 50;
                        break;
                    case "wKnight":
                        whiteScore += 30;
                        break;
                    case "wBishop":
                        whiteScore += 30;
                        break;
                    case "wQueen":
                        whiteScore += 100;
                        break;
                }
                centerControl = 10 - (Mathf.Abs(cp.GetBoardFile() - cp.GetBoardRank()));
                whiteScore += centerControl;
                if(GetPosition(cp.GetBoardFile(), cp.GetBoardRank()).bAttack){
                    if(GetPosition(cp.GetBoardFile(), cp.GetBoardRank()).wAttack){
                        if(cp.name == "wQueen"){
                            whiteScore -= 10; 
                        }
                    }else{
                        switch(cp.name){
                            case "wPawn":
                                whiteScore -= 5;
                                break;
                            case "wRook":
                                whiteScore -= 25;
                                break;
                            case "wKnight":
                                whiteScore -= 15;
                                break;
                            case "wBishop":
                                whiteScore -= 15;
                                break;
                            case "wQueen":
                                whiteScore -= 50;
                                break;
                        }
                    }
                }
            }
        }
        foreach(ChessPiece cp in piecesOnBoard){
            if(!cp.getPlayer()){
                switch(cp.name){
                    case "bPawn":
                        blackScore += 10;
                        break;
                    case "bRook":
                        blackScore += 50;
                        break;
                    case "bKnight":
                        blackScore += 30;
                        break;
                    case "bBishop":
                        blackScore += 30;
                        break;
                    case "bQueen":
                        blackScore += 100;
                        break;
                }
                centerControl = 10 - (Mathf.Abs(cp.GetBoardFile() - cp.GetBoardRank()));
                blackScore += centerControl;
                if(GetPosition(cp.GetBoardFile(), cp.GetBoardRank()).wAttack){
                    if(GetPosition(cp.GetBoardFile(), cp.GetBoardRank()).bAttack){
                        if(cp.name == "bQueen"){
                            blackScore -= 10; 
                        }
                    }else{
                        switch(cp.name){
                            case "bPawn":
                                blackScore -= 5;
                                break;
                            case "bRook":
                                blackScore -= 25;
                                break;
                            case "bKnight":
                                blackScore -= 15;
                                break;
                            case "bBishop":
                                blackScore -= 15;
                                break;
                            case "bQueen":
                                blackScore -= 50;
                                break;
                        }
                    }
                }
                
            }
        }
        if(wCheck){
            whiteScore -= 10;
        }
        if(bCheck){
            blackScore -= 10;
        }
        totalScore = blackScore - whiteScore;
        return totalScore;
    }

    public void SetPieceMemory(int slot){
        foreach(ChessPiece cp in piecesOnBoard){
            cp.SetMemory(slot);
        }
    }

    public void RefreshPieceMemory(int slot){
        foreach(ChessPiece cp in piecesOnBoard){
            cp.RefreshMemory(slot);
        }
        bStale = false;
        wStale = false;
        updateBoard();
        updateAttacks();
    }

    public void checkMate(){
        Debug.Log("Check Mate");
        mate = true;
    }
}
