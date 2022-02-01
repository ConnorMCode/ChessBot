using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move
{
    public ChessPiece piece;
    public ChessPiece secondPiece;
    public int moveX;
    public int moveY;

    public bool castle;

    public int pieceX;
    public int pieceY;

    public int crude;
    GameObject controller;

    public void setPiece(ChessPiece _piece){
        piece = _piece;
        pieceX = piece.GetBoardFile();
        pieceY = piece.GetBoardRank();
    }

    public void setX(int x){
        moveX = x;
    }
    public void setY(int y){
        moveY = y;
    }

    public void makeMove(){
        piece.Click();

        controller = GameObject.FindGameObjectWithTag("GameController");

        castle = controller.GetComponent<Game>().GetPosition(moveX, moveY).checkCastle();

        if(controller.GetComponent<Game>().GetPosition(moveX, moveY).checkPlate()){
            secondPiece = controller.GetComponent<Game>().GetPosition(moveX, moveY).returnAttack();
        }
        controller.GetComponent<Game>().GetPosition(moveX, moveY).clickPlate();
    }

    public void unmakeMove(){

        piece.DestroyMovePlates();

        controller = GameObject.FindGameObjectWithTag("GameController");
        piece.SetBoardFile(pieceX);
        piece.SetBoardRank(pieceY);
        piece.unturn();
        controller.GetComponent<Game>().GetPosition(pieceX, pieceY).Init(piece);

        if(secondPiece != null){
            if(castle){
                if(moveX > pieceX){
                    moveX += 1;
                }else{
                    moveX -= 2;
                }
            }
            secondPiece.SetBoardFile(moveX);
            secondPiece.SetBoardRank(moveY);
            secondPiece.unturn();
            controller.GetComponent<Game>().addPiece(secondPiece);
            controller.GetComponent<Game>().GetPosition(moveX, moveY).Init(secondPiece);

        }

        controller.GetComponent<Game>().addPiece(piece);

        controller.GetComponent<Game>().updateBoard();

    }
}
