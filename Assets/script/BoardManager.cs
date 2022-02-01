using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{

    [SerializeField] private Square _squarePF;
    [SerializeField] private Transform _cam;

    public void GenerateBoard(){
        for(int file = 0; file < 8; file++){
            for(int rank = 0; rank < 8; rank++){
                var spawnedSquare = Instantiate(_squarePF, new Vector3(file,rank),Quaternion.identity);
                spawnedSquare.name = $"Square {file} {rank}";

                bool darkSquare = (file % 2 == 0 && rank % 2 != 0) || (file % 2 != 0 && rank % 2 == 0);
                spawnedSquare.Init(darkSquare);
            }
        }

        _cam.transform.position = new Vector3((float)4 - .5f, (float)4 - .5f, -10);
    }
}
