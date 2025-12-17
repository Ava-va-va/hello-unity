using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MyTankShooting : MonoBehaviour
{
    public int PlayerNumber = 1;//第一个玩家
    public GameObject FireTransform;//先获取发射的位置点，然后再通过这个位置点来生成炮弹模型
    public GameObject Shell;//在场景中将要创建的炮弹模型

    public Slider AimSlider;//添加瞄准的滑动条

    public AudioSource ShootingAudio;
    public AudioClip ChargingClip;//充能音效
    public AudioClip FireClip;//发射音效

    public float MinLaunchForce = 15f;//最小的发射力
    public float MaxLaunchForce = 30f;//最大发射力
    public float ChargeTime = 0.75f;//炮弹充能时间


    private string FireButton;
    private float CurrentLaunchForce;//当前充能的发射力
    private float ChargeSpeed;//充能速度
   
    
    private bool isCharging;
    private bool isIncreaing;
    private bool Fired;

    // Start is called before the first frame update
    void Start()
    {
        FireButton = "Fire" + PlayerNumber;//第一个玩家的开火俺就就是Fire1，通过字符串的组合来设置不同玩家的开火按钮
        ChargeSpeed = (MaxLaunchForce - MinLaunchForce) / ChargeTime;//定义充能速度
       
    }

    // Update is called once per frame
    void Update()
    {



        //炮弹蓄力发射，通过检测每帧的开火按钮的状态是触发还是按住还是回弹，来控制炮弹的发射情况
        //Update是每帧运行一次，第一帧触发第一个if，后面的不触发，如果按钮是按住的时候，第二帧就重新判断if和else if的结构，这时触发else if
        if (Input.GetButtonDown(FireButton))//按下开火按钮的时候，要么是蓄力发射，要么是按钮弹起就发射
        {
            CurrentLaunchForce = MinLaunchForce;//在按下开火按钮的时候，发射的力为最小值
                                                //就是按下开火按钮的时候发射力从最小值开始增大
           
            //按下按钮的时候就直接播放充能音频
            //一直播放到发射触发的时候，切换到发射音频，然后再次检测按钮按下的时候再重新播放发射音频
            ShootingAudio.clip = ChargingClip;
            ShootingAudio.Play();
         
            Fired = false;//按下开火按钮时，表示没有发射，可以进行发射选择，是蓄力还是不蓄力。
        }
        else if (Input.GetButton(FireButton)&&!Fired)
        {
            CurrentLaunchForce += ChargeSpeed * Time.deltaTime;//发射力每帧累加
                                                               //相当于CurrentLaunchForce=CurrentLaunchForce+ChargeSpeed*Time.deltaTime
           AimSlider.value = CurrentLaunchForce;//将当前发射力在滑动条上显示出来
            if (CurrentLaunchForce >= MaxLaunchForce)
            {
                //CurrentLaunchForce = MaxLaunchForce;//以最大发射力连射
                Fire();

            }
            /*
             * 充能音频播放优化
             * 优化代码，将其放在GetButtonDown里，按下发射按钮的时候就直接开始充能
             * 
            if (ShootingAudio.clip == FireClip)//加了这个判断是为了防止每帧都重新播放充能音频，可以保证充能时的后续帧都继续播放充能音频
                                               //如果不判断是否为发射音频，每帧就的充能音频就会重新播放，就会一直播放充能音频的第一帧，无法播放完整
            {
                ShootingAudio.clip = ChargingClip;
                ShootingAudio.Play();
            }     
            */


        }
        else if (Input.GetButtonUp(FireButton)&&!Fired)//Fied为true的时候不发射，防止同时触发两种蓄力和不蓄力的发射。
        {
            Fire();
        }
        /*
         //炮弹按下开火按钮开始充能，再按一次开火按钮发射
         if (Input.GetButtonDown(FireButton))
         {
             if (!isCharging)
             {
                 StartCharing();
             }
             else
             {
                 Shoot();
             }
         }
         Charging();
     */

    }
    void StartCharing()//充能初始化
    {
        isCharging = true;
        isIncreaing = true;
        CurrentLaunchForce = MinLaunchForce;
    }
    void Charging()//来回充能
    {
        if (isCharging)
        {
            if (isIncreaing)
            {
                CurrentLaunchForce += ChargeSpeed * Time.deltaTime;
                if (CurrentLaunchForce >= MaxLaunchForce)
                {
                    CurrentLaunchForce = MaxLaunchForce;
                    isIncreaing = false;
                }
            }
            else
            {
                CurrentLaunchForce-=ChargeSpeed* Time.deltaTime;
                if (CurrentLaunchForce <=MinLaunchForce)
                {
                    CurrentLaunchForce = MinLaunchForce;
                    isIncreaing = true;
                }
            }
           
        }
      
    }
    void Shoot()//充能发射
        {
        GameObject gameObjectInstance = Instantiate(Shell, FireTransform.transform.position, FireTransform.transform.rotation);//将炮弹模型实例化
        Rigidbody rigidbody = gameObjectInstance.GetComponent<Rigidbody>();
        rigidbody.velocity = FireTransform.transform.forward * CurrentLaunchForce;//松开时以当前发射力发射
        isCharging = false;
        isIncreaing= false;

    }
    void Fire()
    {
        //发射炮弹
        /*
         //按一下发射一次，检测的是：是否按下开火按钮
         if (Input.GetButtonDown(FireButton))
         {
             GameObject gameObjectInstance = Instantiate(Shell, FireTransform.transform.position, FireTransform.transform.rotation);//将炮弹模型实例化
             Rigidbody rigidbody = gameObjectInstance.GetComponent<Rigidbody>();//要先获取实例化后的炮弹模型的刚体组件，再进行炮弹属性的设置
             rigidbody.velocity = FireTransform.transform.forward * 15;//forward是z轴正方向，right是x轴正方向，up是y轴正方向
                                                                       //这里是给炮弹一个发射方向的初速度
         }
       */
        Fired = true;
        AimSlider.value = MinLaunchForce;//将滑动条重置为最小值
        GameObject gameObjectInstance = Instantiate(Shell, FireTransform.transform.position, FireTransform.transform.rotation);//将炮弹模型实例化
        Rigidbody rigidbody = gameObjectInstance.GetComponent<Rigidbody>();
        if (rigidbody != null)
        {
            rigidbody.velocity = FireTransform.transform.forward * CurrentLaunchForce;//松开时以当前发射力发射}
        }
       
        CurrentLaunchForce = MinLaunchForce;//发射完之后将发射力重置为最小值
        ShootingAudio.clip= FireClip;
        ShootingAudio.Play();
    }
    }

