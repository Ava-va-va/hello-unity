using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyPlayerShooting : MonoBehaviour
{
    public float timeBetweenBullets=0.15f;//子弹间隔时间
    private float time=0f;//计时器
    private float effectsDisplayTime=0.2f;//特效显示时间
    private AudioSource gunAudio;//枪声音频
    private Light gunLight;//枪光
    private LineRenderer gunLine; //枪线渲染器
    private ParticleSystem gunParticle;//枪口的粒子系统
    

    //开枪发射射线相关变量
    private Ray shootRay;//定义一条射线
    private RaycastHit shootHit;//传递的射线检测信息
    private int shootMask;//射线检测的图层掩码

    private void Awake()
    {
        gunAudio = GetComponent<AudioSource>();
        gunLight = GetComponent<Light>();//获取枪光的灯光组件
        gunLine = GetComponent<LineRenderer>();//获取线渲染器组件
        gunParticle = GetComponent<ParticleSystem>();//获取粒子系统组件
        
        //获取射线检测的图层
        shootMask = LayerMask.GetMask("Shootable");//获取名为Shootable的图层掩码


    }
    // Update is called once per frame 
    void Update()
    {
        time+=Time.deltaTime;//计时器累加
       // Debug.Log(time);
        //获取用户的开火键
        if (Input.GetButton("Fire1")&&time>=timeBetweenBullets)//满足开火间隔时间才能射击
        {
            Shoot(); 
            
        }
        if(time>=timeBetweenBullets*effectsDisplayTime)//时间到达，关闭特效
                                                       //这里的effectsDisplayTime是一个百分比系数，到达射击间隔的百分比时间就关闭特效
        {
            gunLight.enabled=false;
            gunLine.enabled=false;
        }




    }
    void Shoot()//射击
    {
        time = 0f;//重置计时器
        //启用灯光组件
        gunLight.enabled = true;
       
        //绘制线条
        gunLine.SetPosition(0, transform.position);//设置线渲染器的起点位置为枪口位置
      
        /*//不默认绘制终点了
        gunLine.SetPosition(1,transform.position+transform.forward*100f);//transform.forward表示物体的前方方向
                                                                         //设置线渲染器的终点位置为枪口前方100单位处；0和1的意思是线段的第一个点和第二个点
        */
        gunLine.enabled = true;//启用线渲染器组件

        //播放粒子组件
        gunParticle.Play();
        //Debug.Log(DateTime.Now.ToString("HH:mm:ss"));//输出当前时间（时：分：秒）
        gunAudio.Play();//播放枪声音频
       
        //发射射线，检测是否击中敌人
        
        //定义一个Ray，定义一个LayerMask，定义一个RaycasHit
        shootRay.origin=transform.position;//射线的起点为枪口位置
        shootRay.direction = transform.forward;//射线的方向为枪口的前方方向(局部方向)
        
        
        //如果已经命中，终点就是击打的位置
        if (Physics.Raycast(shootRay, out shootHit, 100, shootMask))//射线检测是对碰撞体进行检测的，layerMask是用来过滤碰撞体的图层的
        {
            gunLine.SetPosition(1, shootHit.point);//设置线渲染器的终点位置为击打点位置。
                                                   //这里不用.transform.position的原因是击打点可能在敌人身上，而不是敌人对象的中心位置
            MyEnemyHealth enemyHealth= shootHit.collider.GetComponent<MyEnemyHealth>();//获取敌人的血量脚本组件
            enemyHealth.TakeDamage(20f,shootHit.point);//调用敌人血量脚本的受伤方法，传递伤害值和击打点位置

        }
        //没有击中敌人时，设置线渲染器的终点位置为枪口前方100单位处
        else
        {
            
            gunLine.SetPosition(1, transform.position + transform.forward * 100f);
        }
    }
}
