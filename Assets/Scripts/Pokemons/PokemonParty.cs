using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PokemonParty : MonoBehaviour
{
    [SerializeField] List<Pokemon> pokemons;

    public List<Pokemon> Pokemons
    {
        get
        {
            return pokemons;
        }
    }

    // 游戏开始时，初始化玩家的宝可梦列表
    private void Start()
    {
        foreach (var pokemon in pokemons)
        {
            pokemon.Init();
        }
    }

    public Pokemon GetHealthyPokemon()
    {
        return pokemons.Where(x => x.Hp > 0).FirstOrDefault();
    }
}
