using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// 枚举游戏的状态
public enum GameState { FreeWalk, Battle };
public class GameController : MonoBehaviour
{
    [SerializeField] PlayerController playerController;
    [SerializeField] BattleSystem battleSystem;
    [SerializeField] Camera mainCamera;
    [SerializeField] PokemonParty playerPokemons;
    [SerializeField] MapArea wildPokemon;

    GameState gameState;


    private void Awake()
    {
        // 开始前初始化状态ID
        ConditionsDB.Init();
    }

    private void Start()
    {
        playerController.onEnterBattleSystem += StartBattle;
        battleSystem.EndBattle += EndBattle;
    }

    void EndBattle(bool won)
    {
        gameState = GameState.FreeWalk;
        mainCamera.gameObject.SetActive(true);
        battleSystem.gameObject.SetActive(false);
    }

    void StartBattle()
    {
        gameState = GameState.Battle;
        mainCamera.gameObject.SetActive(false);
        battleSystem.gameObject.SetActive(true);

        //var playerPokemons = playerController.GetComponent<PokemonParty>();
        // var wildPokemon = FindObjectOfType<MapArea>().GetComponent<MapArea>().GetRandomWildPokemon();

        battleSystem.StartBattle(playerPokemons, wildPokemon.GetRandomWildPokemon());
    }

    private void Update()
    {
        /*
         *如果是走路状态
         *则启动playerController的update
         */
        if (gameState == GameState.FreeWalk)
        {
            playerController.HandleUpdate();
        }
        /*
         * 如果是战斗状态
         * 则执行battleSystem中的update
         */
        else if (gameState == GameState.Battle)
        {
            battleSystem.HandleUpdate();
        }
    }

}
