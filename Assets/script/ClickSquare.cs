using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickSquare : MonoBehaviour
{

    private ChessPiece _piece;
    private MovePlate _movePlate;
    public GameObject controller;

    private ChessPiece castle;

    public bool wAttack;
    public bool bAttack;

    public bool plateWhite;

    public bool pwAttack = false;
    public bool pbAttack = false;

    public void Init(ChessPiece piece){
        _piece = piece;
        float x = piece.GetBoardFile();
        float y = piece.GetBoardRank();

        _piece.transform.position = new Vector3(x,y, -1.0f);
    }

    public ChessPiece getPiece(){
        return _piece;
    }

    public void removePiece(){
        _piece.transform.position = new Vector3(-1,-1, -1.0f);
        _piece = null;
    }

    public void assignPlate(MovePlate plate){
        _movePlate = plate;
        controller = GameObject.FindGameObjectWithTag("GameController");
        if(plate.castle){
            if(plate.matrixX > plate.GetReference().GetBoardFile()){
                castle = controller.GetComponent<Game>().GetPosition(plate.matrixX + 1, plate.matrixY).getPiece();
            }else{
                castle = controller.GetComponent<Game>().GetPosition(plate.matrixX - 2, plate.matrixY).getPiece();
            }
        }
        if(plate.GetReference().getPlayer()){
            plateWhite = true;
        }else{
            plateWhite = false;
        }
    }

    public void unassignPlate(){
        castle = null;
        _movePlate = null;
    }

    public bool checkCastle(){
        if(_movePlate != null && _movePlate.castle){
            return true;
        }else{
            return false;
        }
    }

    void OnMouseUp() {
        controller = GameObject.FindGameObjectWithTag("GameController");
        if(_movePlate != null){
            _movePlate.Click();
            controller.GetComponent<Game>().updateBoard();
        }else if(_piece != null){
            _piece.Click();
        }
        controller.GetComponent<Game>().updateAttacks();
    }

    public bool checkPlate(){
        bool attack;
        if(_movePlate != null && _movePlate.attack){
            attack = true;
        }else if(_movePlate != null && _movePlate.castle){
            attack = true;
        }else{
            attack = false;
        }
        return attack;
    }

    public ChessPiece returnAttack(){
        controller = GameObject.FindGameObjectWithTag("GameController");
        if(_movePlate.castle){
            return castle;
        }else{
            return(controller.GetComponent<Game>().GetPosition(_movePlate.matrixX, _movePlate.matrixY).getPiece());
        }
        
    }

    public void clickPlate(){
        controller = GameObject.FindGameObjectWithTag("GameController");
        if(_movePlate != null){
            _movePlate.Click();
        }
        controller.GetComponent<Game>().updateAttacks();
    }
}
