using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackMoves : MonoBehaviour
{
    
    [SerializeField] Game controller;

    public Move bestMove;

    public bool turnBlack;

    public void performMove(){
        Search(2, true, 999);
        if(bestMove != null){
            bestMove.makeMove();
        }
    }

    public int Search(int depth, bool maxing, int best){
        if(depth == 0){
            return controller.GetComponent<Game>().calculateScore();
        }else{
            List<Move> moves = GenerateMoves(depth);
            if(moves.Count == 0){
                if(controller.GetComponent<Game>().bCheck){
                    return -999;
                }
                return 0;
            }

            float count = moves.Count;
            int best2 = -999;
            if(maxing){
                best = -999;
                for(int i = 0; i < (Mathf.Ceil(count * 0.66f)); i++){
                    Move move = moves[i];
                    move.makeMove();
                    int evaluation = Search(depth - 1, false, best);
                    move.unmakeMove();
                    if(evaluation > best2){
                        best2 = evaluation;
                        best = evaluation;
                        bestMove = move;
                    }
                }
                return best;
            }else{
                int worst = 999;
                for(int i = 0; i < (Mathf.Ceil(count * 0.66f)); i++){
                    Move move = moves[i];
                    move.makeMove();
                    int evaluation = Search(depth - 1, false, best);
                    if(evaluation < worst){
                        worst = evaluation;
                    }
                    move.unmakeMove();
                    if(worst < best){
                        break;
                    }
                }
                return worst;
            }
        }
    }

    public List<Move> GenerateMoves(int depth){
        List<Move> moves = new List<Move>();
        if(depth % 2 == 0){
            foreach(ChessPiece cp in controller.GetComponent<Game>().getPieces()){
                if(!cp.getPlayer()){
                    cp.Click();
                    foreach(Move move in cp.getMoves()){
                        moves.Add(move);
                        DestroyMovePlates();
                    }
                }
            }
            heapSort(moves, moves.Count);
            return moves;
        }else{
            foreach(ChessPiece cp in controller.GetComponent<Game>().getPieces()){
                if(cp.getPlayer()){
                    cp.Click();
                    foreach(Move move in cp.getMoves()){
                        moves.Add(move);
                        DestroyMovePlates();
                    }
                }
            }
            heapSort(moves, moves.Count);
            return moves;
        }
    }

    public void heapSort (List<Move> moves, int n){
        for (int i = n / 2 - 1; i >= 0; i--)
         heapify(moves, n, i);
         for (int i = n-1; i>=0; i--) {
            Move temp = moves[0];
            moves[0] = moves[i];
            moves[i] = temp;
            heapify(moves, i, 0);
         }
    }

    public void heapify(List<Move> moves, int n, int i) {
         int largest = i;
         int left = 2*i + 1;
         int right = 2*i + 2;
         if (left < n && moves[left].crude < moves[largest].crude)
         largest = left;
         if (right < n && moves[right].crude < moves[largest].crude)
         largest = right;
         if (largest != i) {
            Move swap = moves[i];
            moves[i] = moves[largest];
            moves[largest] = swap;
            heapify(moves, n, largest);
         }
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
}
