using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyTankMovement : MonoBehaviour
{
    public int  playerNumber = 1;
    public float speed=12;
    public float TurnSpeed=180;
    public AudioSource MovementAudio;//通过属性暴露的方式来获取音频组件，可以直接添加相关的音频文件
    public AudioClip EngineIdling;
    public AudioClip EngineDriving;
    public float PitchRange = 0.2f;//音调范围   

   

    private Rigidbody rb;
    private string MovementAxisName;
    private string TurnAxisName;

    private float MovementInputValue;//当前获取的轴线值
    private float TurnInputValue;

    private float OriginalPitch;//原始的音调

    // Start is called before the first frame update
    private void Awake()
    {

        rb = GetComponent<Rigidbody>();//获取当前的刚体组件
    }
    void Start()
    {
        //进行轴线变量赋值，这里的轴线名称需要和InputManager中的名称一致，根据这个名称来获取轴线（按键）信息
        MovementAxisName = "Vertical"+playerNumber;
        TurnAxisName = "Horizontal"+playerNumber;
        OriginalPitch = MovementAudio.pitch;//获取原始音调
    }
    

    // Update is called once per frame
    void Update()
    {
        //获取轴线信息，写到Update函数中，因为Update函数每帧都会调用，可以实时获取轴线信息
        MovementInputValue = Input.GetAxis(MovementAxisName);//用变量的值来获取轴线，（使用变量的好处是可以在编辑器中直接修改轴线名称，也方便代码的复用和修改）
        
        TurnInputValue = Input.GetAxis(TurnAxisName);//获取轴线信息

        EngineAudio();

    }
    void EngineAudio()
    {
        //播放声音
        //通过判断轴线值的绝对值来判断坦克是否在移动，此处用Mathf.Abs函数来获取绝对值然后与0.1（比较小的值）进行比较
        //tip：MovementInputValue的值在-1到1之间
        if (Mathf.Abs(MovementInputValue) < 0.1f && Mathf.Abs(TurnInputValue) < 0.1f)
        {
            if (MovementAudio.clip == EngineDriving)//判断是否静止，因为静止（怠速，或者低速）的时候就是EngineIdling在播放，需要检测是不是在播放运动的音频
                                                    //如果是静止的话就跳过，如果刚开始是运动音频的话就切换到静止的音频。
            {
                MovementAudio.clip = EngineIdling;//clp是AudioSource组件的属性，将音频文件设置给这个属性，相当于是将音频文件装入音频源，但是没有播放。
                                                  //一个AudioSource组件只能播放一个音频文件，这里是通过将MovementAudio这个音源的文件换成不同的音频文件来实现音频的切换

                MovementAudio.pitch = Random.Range(OriginalPitch - PitchRange, OriginalPitch + PitchRange);//设计一个随机的音调范围，在每帧中都会在这个范围中随机生成一个音调值。
                MovementAudio.Play();//播放音频
            }

        }
        else
        {
            if (MovementAudio.clip == EngineIdling)//如果当前播放的是怠速音频
            {
                MovementAudio.clip = EngineDriving;//切换到运动音频
                MovementAudio.pitch = Random.Range(OriginalPitch - PitchRange, OriginalPitch + PitchRange);
                MovementAudio.Play();//播放音频
            }
        }
    }
    private void FixedUpdate()
    {
        //在FixedUpdate函数中进行物理相关的操作，因为FixedUpdate函数的调用频率是固定的，更适合处理物理相关的操作
        Move();
        Turn();
    }
    void Move()
    {
        
        Vector3 movementV3 = transform.forward * MovementInputValue * speed * Time.deltaTime;
       
        rb.MovePosition(rb.position + movementV3);//moveposition是直接设置位置，不会受到物理引擎的影响,需要使用原始的位置加上移动向量来设置新的位置

    }
    void Turn()
    {

        //rb.angularVelocity = transform.up * TurnInputValue * 5;//这种方式会导致旋转不平滑，会因为碰撞而改变旋转速度


        float turn = TurnInputValue * TurnSpeed * Time.deltaTime;

       Quaternion quaterion = Quaternion.Euler(0, turn, 0);//为了应对旋转的需求，根据欧拉角创建四元数

        //转动一定的角度，用相对旋转
        rb.MoveRotation(rb.rotation * quaterion);
    }

}
