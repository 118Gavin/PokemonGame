using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerController : MonoBehaviour
{
    Animator animator;

    public LayerMask solidObjedtLayer;
    public LayerMask longGrassLayer;

    public event Action onEnterBattleSystem;

    public float moveSpeed;
    private Vector2 input;
    private bool isMoving; //默认为false


    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    public void HandleUpdate()
    {
        if (!isMoving) //如果移动
        {
            input.x = Input.GetAxisRaw("Horizontal"); //返回的值在1,0，-1其中一个
            input.y = Input.GetAxisRaw("Vertical");

            if (input.x != 0) // 删除对角距
                input.y = 0;

            if (input != Vector2.zero) //如果获取用户的输入不为0
            {
                animator.SetFloat("MoveX", input.x);
                animator.SetFloat("MoveY", input.y);
                Vector2 getPos = transform.position;
                getPos.x += input.x;
                getPos.y += input.y;
                if (IsWalk(getPos))
                    StartCoroutine(Move(getPos));
            }
        }

        animator.SetBool("IsMoving", isMoving);
    }

    IEnumerator Move(Vector3 getPos)
    {
        isMoving = true;

        while ((getPos - transform.position).sqrMagnitude > Mathf.Epsilon) //因为moveTowards每次只能走0.几的位置所以要判断是否接近最小的浮点数作为判断
        {
            transform.position = Vector3.MoveTowards(transform.position, getPos, moveSpeed * Time.deltaTime);
            yield return null;
        }
        transform.position = getPos;
        isMoving = false;
        meetPokemonWild();
    }

    // 判定当前坐标的0.2f的范围内是否有指定layer的碰撞体，有则返回值，无则为空
    private bool IsWalk(Vector3 getPos)
    {
        // 如果有物体则返回false
        if (Physics2D.OverlapCircle(getPos, 0.2f, solidObjedtLayer) != null)
            return false;
        //没有物体返回true
        return true;
    }

    private void meetPokemonWild()
    {
        if (Physics2D.OverlapCircle(transform.position, 0.1f, longGrassLayer) != null)
        {
            if (Random.Range(1, 101) <= 10)
            {
                animator.SetBool("IsMoving", false);
                onEnterBattleSystem();
            }
        }
    }

}
