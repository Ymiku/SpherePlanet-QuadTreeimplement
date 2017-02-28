using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class RVShooter : MonoBehaviour
{
    public const int Const = 10;
    public static int Static= 10;
    public static readonly int Static_Readonly = 10;

    [SerializeField]
    Image hpBar = null;
    [SerializeField]
    RVShooter target = null;

    Transform point;
    Transform bullet;
    Transform bulletExploder;
    Transform barPoint;

    int maxHP = 100;
    int nowHP = 100;
    
    float minMoveTime = 0.5f;
    float maxMoveTime = 1.3f;
    float minStopMoveTime = 0.3f;
    float maxStopMoveTime = 0.6f;
    float maxMoveDis = 6f;
    float minSpeed = 4;
    float maxSpeed = 6;
    float nowSpeed;
    float rotLerp = 0.03f;

    Countdown moveCD;
    Vector3 nowMoveDir = new Vector3(1, 0, 0);
    Vector3 startDir;

    public Vector3 Position { get { return this.transform.position; } }
    public Quaternion Rotation { get { return this.transform.rotation; } }

    //RV test datas
    Dictionary<int, Color> bulletDataDic = new Dictionary<int, Color>();
    List<Vector3> list_Vector3 = new List<Vector3>();
    List<Countdown> list_class = new List<Countdown>();
    List<int> list_int = new List<int>();

    void Start()
    {
        point = this.transform.FindChild("point");
        Transform b = this.transform.parent.FindChild("b");
        bullet = b.transform.FindChild("bullet");
        bulletExploder = b.transform.FindChild("bulletExploder");
        barPoint = this.transform.FindChild("barPoint");
        bullet.gameObject.SetActive(false);
        startDir = this.transform.forward;

        moveCD = new Countdown(GetStopMoveTime(), OnEnd_StopMove,OnUpdate_StopMove);
        moveCD.Start(true);

        for (int i = 0; i < 3; i++)
        {
            bulletDataDic.Add(i*3,GetColor());
            list_Vector3.Add(Vector3.one * i);
            list_class.Add(new Countdown(i*3));
            list_int.Add(i*3);
        }
    }

    void Update()
    {
        if(hpBar != null)
        {
            this.hpBar.transform.parent.position = this.barPoint.position;
            this.hpBar.fillAmount = (float)nowHP / (float)maxHP;
        }

        if (moveCD != null)
            moveCD.Update();
    }

    public void Hit(int Att)
    {
        this.nowHP -= Att;
        if(this.nowHP <= 0)
        {
            this.nowHP = this.maxHP;
        }
    }

    void OnShoot()
    {
        RVBullet b = CreateBullet();
        GameObject exploder = GameObject.Instantiate(bulletExploder.gameObject) as GameObject;
        exploder.transform.parent = this.bulletExploder.parent;
        b.Init(this.point.forward, 1f, 20f, Random.Range(8, 20), exploder, GetColor());
    }

    RVBullet CreateBullet()
    {
        RVBullet b = GameObject.Instantiate(bullet).GetComponent<RVBullet>();
        b.transform.parent = bullet.transform.parent;
        b.transform.position = this.point.position;
        b.gameObject.SetActive(true);

        return b;
    }

    void OnEnd_Move()
    {
        moveCD = new Countdown(GetStopMoveTime(), OnEnd_StopMove, OnUpdate_StopMove,0.9f, OnPer_StopMove);
        moveCD.Start(true);
    }

    void OnUpdate_Move()
    {
        if (this.transform.position.x > maxMoveDis)
        {
            this.nowMoveDir.x = -1f;
        }
        else if (this.transform.position.x < 0)
        {
            this.nowMoveDir.x = 1f;
        }

        this.transform.position += nowMoveDir * nowSpeed * Time.deltaTime;
        this.transform.forward = Vector3.Lerp(this.transform.forward, startDir, rotLerp);
    }

    void OnEnd_StopMove()
    {
        moveCD = new Countdown(GetMoveTime(), OnEnd_Move, OnUpdate_Move);
        moveCD.Start(true);
        this.nowSpeed = GetSpeed();
    }

    void OnUpdate_StopMove()
    {
        Vector3 targetDir = this.target.transform.position - this.transform.position;
        targetDir.y = 0f;
        this.transform.forward = Vector3.Lerp(this.transform.forward, targetDir, rotLerp);
    }

    void OnPer_StopMove()
    {
        OnShoot();
    }

    float GetMoveTime()
    {
        return Random.Range(minMoveTime, maxMoveTime);
    }

    float GetStopMoveTime()
    {
        return Random.Range(minStopMoveTime, maxStopMoveTime);
    }

    float GetSpeed()
    {
        return Random.Range(minSpeed, maxSpeed);
    }

    Color GetColor()
    {
        return new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f), 1f);
    }
}
