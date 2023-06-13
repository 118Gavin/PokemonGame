using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;
using UnityEngine.UIElements;

/*
    start:开始状态
    PlayerAction:玩家选择攻击，还是逃跑状态
    PlayerSkill:玩家操作技能状态
    EnemySkill:敌人释放技能状态
    Busy: 表示玩家和敌人攻击时的状态
 */
public enum BattleState { Start, ActionSelection, MoveSelection, PerformMove, Busy, BattleOver, PartyScreen }

public class BattleSystem : MonoBehaviour
{
    [SerializeField] BattleUnit playerUnit;
    [SerializeField] BattleUnit enemyUnit;
    [SerializeField] BattleDialogBox battleDialogBox;
    [SerializeField] PartyScreen partyScreen;

    BattleState state; // 记录当前游戏的状态
    private int currentActionIndex; // 记录ActionSelector下标,默认是0
    private int currentSkillIndex; // 记录SkillSelector下标，默认是0
    private int currentMemberIndex; // 记录宝可梦列表的下标，默认是0

    public event Action<bool> EndBattle;

    // 定义玩家宝可梦和野生宝可梦
    PokemonParty playerPokemons;
    Pokemon wildPokemon;

    // 战斗开始时讲设定好的玩家宝可梦和野生宝可梦保存到创建的对象中
    public void StartBattle(PokemonParty playerPokemons, Pokemon wildPokemon)
    {
        this.playerPokemons = playerPokemons;
        this.wildPokemon = wildPokemon;

        StartCoroutine(HandleSetup());
    }

    // 初始化battle系统的所有对象
    public IEnumerator HandleSetup()
    {
        playerUnit.Setup(playerPokemons.GetHealthyPokemon());
        enemyUnit.Setup(wildPokemon);
        battleDialogBox.SetSkillNames(playerUnit.Pokemon.Skills); // 初始化技能

        yield return battleDialogBox.TypeDialog($"发现野生的{enemyUnit.Pokemon.Base.Name}!");
        yield return battleDialogBox.TypeDialog($"{playerUnit.Pokemon.Base.Name},就决定是你了！");

        ChooseFirstMove();
    }

    public void HandleUpdate()
    {
        if (state == BattleState.ActionSelection)
        {
            HandleActionSelector();
        }
        else if (state == BattleState.MoveSelection)
        {
            HandleSkillSelector();
        }
        else if (state == BattleState.PartyScreen)
        {
            HandlePartySelector();
        }
    }

    /*
     * 玩家使用技能后
     * 1、扣除PP
     * 2、扣除血量
     * 3、在场景中更新血条
     * 4、调用敌方攻击
     */
    private IEnumerator PlayerMove()
    {
        state = BattleState.PerformMove; // 当前处于选择技能状态

        var skill = playerUnit.Pokemon.Skills[currentSkillIndex];
        yield return RunMove(playerUnit, enemyUnit, skill);

        if (state == BattleState.PerformMove)
            StartCoroutine(EnemyMove());
    }

    // 敌方攻击
    private IEnumerator EnemyMove()
    {
        state = BattleState.PerformMove;

        var skill = enemyUnit.Pokemon.GetRandomSkill();
        yield return RunMove(enemyUnit, playerUnit, skill);

        if (state == BattleState.PerformMove)
            ActionSelection();
    }

    /*
     * sourceUnit: 表示攻击方
     * targetUnit: 表示攻击对象
     * skill : 表示使用的技能
     */
    IEnumerator RunMove(BattleUnit sourceUnit, BattleUnit targetUnit, Skill skill)
    {
        // 判定玩家宝可梦是否能行动
        bool canRun = sourceUnit.Pokemon.OnBeforeTurn();
        if (!canRun)
        {
            yield return ShowStatusMessage(sourceUnit.Pokemon);
            // 当自身使用技能陷入混乱状态，更新血量
            yield return sourceUnit.Hub.UpdateHP();
            yield break;
        }
        yield return ShowStatusMessage(sourceUnit.Pokemon);

        state = BattleState.PerformMove; // 当前处于选择技能状态
        yield return battleDialogBox.TypeDialog($"{sourceUnit.Pokemon.Base.Name}使用了{skill.SkillBase.Name}!");

        sourceUnit.AttackAnimation();
        targetUnit.HitAnimation();
        yield return new WaitForSeconds(1f);


        if (skill.SkillBase.Category == SkillCategory.Status)
        {
            yield return (RunSkillEffects(skill, sourceUnit, targetUnit));
        }
        else
        {
            var damageDetails = targetUnit.Pokemon.TakeDamage(skill, sourceUnit.Pokemon);
            yield return targetUnit.Hub.UpdateHP();
            yield return ShowDamageDetails(damageDetails);
        }

        /*
         *受攻击方倒下，显示倒下动画
         *否则敌方攻击
         */
        if (targetUnit.Pokemon.Hp <= 0)
        {
            yield return battleDialogBox.TypeDialog($"{targetUnit.Pokemon.Base.Name}倒下了！");
            targetUnit.DeadAnimation();
            yield return new WaitForSeconds(1f);
            CheckForBattleOver(targetUnit);
        }

        // 在双方攻击都结束后调用
        sourceUnit.Pokemon.OnAfterTurn();
        yield return ShowStatusMessage(sourceUnit.Pokemon);
        yield return targetUnit.Hub.UpdateHP();
        if (sourceUnit.Pokemon.Hp <= 0)
        {
            yield return battleDialogBox.TypeDialog($"{sourceUnit.Pokemon.Base.Name}倒下了！");
            sourceUnit.DeadAnimation();
            yield return new WaitForSeconds(1f);
            CheckForBattleOver(sourceUnit);
        }
    }

    void CheckForBattleOver(BattleUnit faintedUnit)
    {
        if (faintedUnit.IsPlayerUnit)
        {
            var nextPokemon = playerPokemons.GetHealthyPokemon();
            if (nextPokemon != null)
                OpenPartyScreen();
            else
                BattleOver(false); // false表示玩家输了，true表示玩家赢了！
        }
        else
            BattleOver(true);
    }



    /*
     * skill: 使用的技能
     * user: 使用当前技能的宝可梦
     * target： 当前技能的作用对象
     */
    IEnumerator RunSkillEffects(Skill skill, BattleUnit user, BattleUnit target)
    {
        var effects = skill.SkillBase.Effects;

        //   技能提升削弱
        if (effects.Boosts != null)
        {
            // 敌人削弱，自己加强
            if (skill.SkillBase.Target == SkillTarget.self)
                user.Pokemon.AppleBoost(effects.Boosts);
            else
                target.Pokemon.AppleBoost(effects.Boosts);
        }

        // 技能造成的状态
        if (effects.Status != ConditionID.空)
        {
            target.Pokemon.SetStatus(effects.Status);
        }

        //技能造成的不确定状态
        if (effects.VolatileStatus != ConditionID.空)
        {
            target.Pokemon.SetVolatileStatus(effects.VolatileStatus);
        }

        yield return ShowStatusMessage(user.Pokemon);
        yield return ShowStatusMessage(target.Pokemon);
    }

    /*
     * 显示技能的效果战斗信息
     */
    IEnumerator ShowDamageDetails(DamageDetails damageDetails)
    {
        if (damageDetails.TypeEffectiveness > 1f)
        {
            yield return battleDialogBox.TypeDialog("效果拔群！");
        }

        else if (damageDetails.TypeEffectiveness < 1f)
        {
            yield return battleDialogBox.TypeDialog("微不足道的效果！");
        }


        if (damageDetails.Critical > 1f)
        {
            yield return battleDialogBox.TypeDialog("会心一击！");
        }
    }

    /*
     * 根据宝可梦的速度判断哪个优先释放技能    
     */
    void ChooseFirstMove()
    {
        if (playerUnit.Pokemon.Speed >= enemyUnit.Pokemon.Speed)
            ActionSelection();
        else
            StartCoroutine(EnemyMove());
    }

    /*
     * 显示当前宝可梦队列中的战斗信息
     */
    IEnumerator ShowStatusMessage(Pokemon pokemon)
    {
        while (pokemon.StatusMessage.Count > 0)
        {
            yield return battleDialogBox.TypeDialog(pokemon.StatusMessage.Dequeue());
        }
    }

    void OpenPartyScreen()
    {
        state = BattleState.PartyScreen;
        partyScreen.gameObject.SetActive(true);
        partyScreen.init();
        partyScreen.SetPartyData(playerPokemons.Pokemons);
    }

    // 当玩家进入PlayerAction的时候调用
    private void ActionSelection()
    {
        state = BattleState.ActionSelection;
        battleDialogBox.SetDialogText("请选择一个操作！");
        battleDialogBox.EnableActionSelector(true);
    }

    /*
     * 执行玩家的操作
     * 关闭ActionSelector
     * 关闭dialogText
     * 打开SkillSelector和SkillDetails
     */
    private void MoveSelection()
    {
        state = BattleState.MoveSelection;
        battleDialogBox.EnableActionSelector(false);
        battleDialogBox.EnableDialogText(false);
        battleDialogBox.EnableSkillSelector(true);
    }

    /*
      * 处理ActionSelector的变化
      * 根据currentActionIndex，初始为0，场景中攻击背包宝可梦和逃跑
      * 默认变化就是0123之间的变化
      * 确认下标之后调用BattleDialogText中改变字体颜色的方法
      */
    private void HandleActionSelector()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow))
            currentActionIndex += 2;

        else if (Input.GetKeyDown(KeyCode.UpArrow))
            currentActionIndex -= 2;

        else if (Input.GetKeyDown(KeyCode.LeftArrow))
            currentActionIndex--;

        else if (Input.GetKeyDown(KeyCode.RightArrow))
            currentActionIndex++;

        currentActionIndex = Mathf.Clamp(currentActionIndex, 0, 3);

        // 选中的操作字体变颜色
        battleDialogBox.UpdateActionSelector(currentActionIndex);

        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            if (currentActionIndex == 0)
            {
                MoveSelection();
            }
            else if (currentActionIndex == 1)
            {
                // 背包
            }
            else if (currentActionIndex == 2)
            {
                OpenPartyScreen();
            }
            else if (currentActionIndex == 3)
            {
                // 逃跑
            }
        }
    }

    /*
    * 处理SkillSelector的变化
    * 根据currentSkillIndex，初始为0，场景中默认4个技能
    * 变化就是上下左右的变化
    * 根据pokemon中的skills保存的技能数量。
    * 如果下标是小于（技能数量-1），则可以+1
    * 下标大于0，则可以--
    * ###################
    * 如果下标是小于（技能数量-2）的，+2
    * 如果下标是大于1的，则可以-2
    * 横向变化是01,23之间的变化
    * 纵向是02,13之间的变化
    * 确认下标之后调用BattleDialogText中改变字体颜色的方法以及对应PP和Type的更新
    */
    private void HandleSkillSelector()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow))
            currentSkillIndex += 2;
        else if (Input.GetKeyDown(KeyCode.UpArrow))
            currentSkillIndex -= 2;
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
            currentSkillIndex -= 1;
        else if (Input.GetKeyDown(KeyCode.RightArrow))
            currentSkillIndex += 1;

        currentSkillIndex = Mathf.Clamp(currentSkillIndex, 0, playerUnit.Pokemon.Skills.Count - 1);


        battleDialogBox.UpdateSkillSelector(currentSkillIndex, playerUnit.Pokemon.Skills[currentSkillIndex]);

        if (Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Return))
        {
            if (playerUnit.IsHavePP(playerUnit.Pokemon.Skills[currentSkillIndex]))
            {
                battleDialogBox.EnableSkillSelector(false);
                battleDialogBox.EnableDialogText(true);
                StartCoroutine(PlayerMove());
            }
        }
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            battleDialogBox.EnableSkillSelector(false);
            battleDialogBox.EnableDialogText(true);
            ActionSelection();
        }
    }

    void HandlePartySelector()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow))
            currentMemberIndex += 2;
        else if (Input.GetKeyDown(KeyCode.UpArrow))
            currentMemberIndex -= 2;
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
            currentMemberIndex -= 1;
        else if (Input.GetKeyDown(KeyCode.RightArrow))
            currentMemberIndex += 1;

        currentMemberIndex = Mathf.Clamp(currentMemberIndex, 0, playerPokemons.Pokemons.Count - 1);
        partyScreen.UpdatePartyMember(currentMemberIndex);


        if (Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Return))
        {
            var selectedPokemon = partyScreen.Pokemons[currentMemberIndex];
            if (selectedPokemon.Hp <= 0)
            {
                partyScreen.SetMessage("不能选择已经晕倒的宝可梦！");
                return;
            }
            if (selectedPokemon == playerUnit.Pokemon)
            {
                partyScreen.SetMessage("不能选择正在场上的宝可梦！");
                return;
            }
            partyScreen.gameObject.SetActive(false);
            state = BattleState.Busy;
            StartCoroutine(SwitchPokemon(selectedPokemon));

        }
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            partyScreen.gameObject.SetActive(false);
            ActionSelection();
        }

    }

    void BattleOver(bool won)
    {
        state = BattleState.BattleOver;
        playerPokemons.Pokemons.ForEach(p => p.OnBattleOver());
        EndBattle(won);
    }

    IEnumerator SwitchPokemon(Pokemon newPokemon)
    {
        // 默认切换宝可梦是当前玩家宝可梦倒下
        bool currentPokemonFainted = true;
        if (playerUnit.Pokemon.Hp > 0)
        {
            currentPokemonFainted = false;
            yield return battleDialogBox.TypeDialog($"回来吧，{playerUnit.Pokemon.Base.Name}!");
            playerUnit.DeadAnimation();
            yield return new WaitForSeconds(1f);
        }

        playerUnit.Setup(newPokemon);
        battleDialogBox.SetSkillNames(newPokemon.Skills);
        yield return battleDialogBox.TypeDialog($"上吧，{playerUnit.Pokemon.Base.Name}!");

        // 如果是当前玩家宝可梦倒下，切换上场的宝可梦需要判定速度谁先攻击
        if (currentPokemonFainted)
            ChooseFirstMove();
        else
            //切换宝可梦消耗一次战斗机会,敌方宝可梦行动
            StartCoroutine(EnemyMove());
    }

}
