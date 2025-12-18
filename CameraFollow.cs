using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public float Smoothing = 5f;
    private GameObject player;
    private Vector3 offset;
   
    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");//找到玩家的标签对象

    }
    private void Start()
    {
        offset = transform.position - player.transform.position;

    }
    private void FixedUpdate()
    {
        //通过FixedUpdate函数的每帧调用的性质，通过插值函数Lerp实现相机平滑跟随玩家
        //当 float  t>1 时，直接到达目标位置,通过FixedUpdate的每帧调用，实现 t随时间的变化
        transform.position = Vector3.Lerp(transform.position, player.transform.position+offset,Smoothing*Time.deltaTime);
    }

}
