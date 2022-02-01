using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePlate : MonoBehaviour
{
    public GameObject controller;

    ChessPiece reference = null;

    public int matrixX;
    public int matrixY;

    int referenceRank;
    int referenceFile;

    public bool attack;
    
    public bool castle;

    public void Start(){
        if(attack){
            gameObject.GetComponent<SpriteRenderer>().color = new Color(1.0f, 0.0f, 0.0f, .4f);
        }
    }

    public void Click(){
        controller = GameObject.FindGameObjectWithTag("GameController");

        if(attack){
            
            controller.GetComponent<Game>().AttackPosition(matrixX, matrixY);

        }

        if(castle){
            ChessPiece cp;
            if(matrixX > reference.GetBoardFile()){
                cp = controller.GetComponent<Game>().GetPosition(matrixX + 1, matrixY).getPiece();
                controller.GetComponent<Game>().GetPosition(matrixX + 1, matrixY).removePiece();
                cp.SetBoardFile(reference.GetBoardFile() + 1);
            }else{
                cp = controller.GetComponent<Game>().GetPosition(matrixX - 2, matrixY).getPiece();
                controller.GetComponent<Game>().GetPosition(matrixX - 2, matrixY).removePiece();
                cp.SetBoardFile(reference.GetBoardFile() - 1);
            }
            cp.SetBoardRank(reference.GetBoardRank());
            cp.turn();
        }

        reference.SetBoardFile(matrixX);
        reference.SetBoardRank(matrixY);
        reference.turn();

        if(reference.name == "wPawn" && matrixY == 7){
            controller.GetComponent<Game>().removePiece(reference, false);
            Promotion();
        }else if(reference.name == "bPawn" && matrixY == 0){
            controller.GetComponent<Game>().removePiece(reference, false);
            Promotion();
        }

        controller.GetComponent<Game>().updateBoard();

        if(castle){
            
        }

        reference.DestroyMovePlates();

        if(reference.getPlayer()){
            controller.GetComponent<Game>().findLegalMoves("black", false);
        }else{
            controller.GetComponent<Game>().findLegalMoves("white", false);
        }
    }

    public void Promotion(){

        ChessPiece cp;

        if(reference.name == "wPawn"){
            cp = controller.GetComponent<Game>().Create("wQueen", matrixX, matrixY);
            Debug.Log("Queen Up");
        }else{
            cp = controller.GetComponent<Game>().Create("bQueen", matrixX, matrixY);
            Debug.Log("Queen Up");
        }
    }

    public void SetCoords(int x, int y){
        matrixX = x;
        matrixY = y;
    }

    public void SetReference(ChessPiece piece){
        reference = piece;
    }

    public ChessPiece GetReference(){
        return reference;
    }

}
