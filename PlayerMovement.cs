using System.Net;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
   public float speed =6f;
   private Rigidbody rb;
    private Animator anim;
    /*
     *全局变量写法，方法不需要传参
     private float h;
     private float v;
     */
    private void Awake()
    {
        rb=GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
    }
    /*
     *全局变量写法，Update函数中获取轴线值
    private void Update()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

    }*/
    private void FixedUpdate()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        Move(h,v);
        Turning();
        Animating(h,v); 

    }
    void Move(float h,float v)
    {
        // Vector3 movementV3 = new Vector3(h, 0f, v).normalized * speed * Time.deltaTime;
       //分布写
        Vector3 movementV3 = new Vector3(h, 0, v);
        movementV3 = movementV3.normalized * speed * Time.deltaTime;
        rb.MovePosition(rb.position + movementV3);
    }
    void Turning()
    {
        //在鼠标的位置创建一条相机射线(调用相机类的方法）
        //Camera.main：返回主相机；最好保持主相机只存在一个
        //ScreenPointToRay 返回从摄像机通过屏幕点的光线，从摄像机的近平面开始，并通过屏幕上位置的 (x,y) 像素坐标
        Ray cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        int floorLayer = LayerMask.GetMask("Floor");//获得图层的掩码，掩码根图层的索引不一样，索引是名字前面的数字；层掩码是索引的2的幂次方
        RaycastHit floorHit;
        //Debug.Log(floorLayer);
        //射线检测
        bool isTouchFloor = Physics.Raycast(cameraRay, out floorHit, 100, floorLayer);//如果是True(检测到了)，返回穿透对象 floorHit（hitInfo)
        if (isTouchFloor)
        {
            Vector3 v3 = floorHit.point - transform.position;//计算点到角色的向量
            v3.y = 0f;//Y是不动的，只希望朝Z方向移动（只有水平向量），只希望在平面上旋转
            //物体正方向（正前Z和正上Y）
            Quaternion quaternion = Quaternion.LookRotation(v3);//根据一个方向向量计算旋转角度；从正方向到目标方向的旋转
            //鼠标控制旋转，面朝某个方向，用绝对旋转
            rb.MoveRotation(quaternion);//刚体旋转
        }
    }
       void Animating(float h,float v)
        {
           /*
            bool isW = false;
            if (h != 0 || v != 0)
                isW = true;
            */
           bool isW= h!=0 || v!= 0;//简写
            anim.SetBool("isWalking", isW);
        }

    }
        
    

