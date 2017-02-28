using UnityEngine;
using System.Collections;
using System;

public class RVBullet : MonoBehaviour
{
    new Light light;
    float liveTime = -1f;
    public Vector3 Dir = new Vector3(1, 0, 0);
    public float Speed = 2f;
    public int Att = 2;
    GameObject exploder;
    Color c;

    public void Init(Vector3 dir,float liveTime, float speed,int att,GameObject exploder, Color c)
    {
        this.c = c;
        this.Dir = dir;
        this.liveTime = liveTime;
        this.Speed = speed;
        this.Att = att;
        this.exploder = exploder;
        light = this.transform.FindChild("light").GetComponent<Light>();
        //this.GetComponent<Renderer>().material.color = c;
        this.GetComponent<MeshRenderer>().material.SetColor("_TintColor", c);
        light.color = c;
    }

    void Update()
    {
        if(liveTime > 0f)
        {
            liveTime -= Time.deltaTime;
            if(liveTime <=0)
            {
                EndByTime();
            }
        }

        this.transform.position += Dir * this.Speed * Time.deltaTime;
    }

    void OnTriggerEnter(Collider collision)
    {
        if(collision != null && collision.gameObject != null)
        {
            RVShooter other = collision.gameObject.GetComponent<RVShooter>();
            PlayExploderEffect();
            if (other != null)
            {
                other.Hit(this.Att);
            }

            this.gameObject.SetActive(false);
            GameObject.DestroyObject(this.gameObject);
        }
    }

    void EndByTime()
    {
        this.gameObject.SetActive(false);
        GameObject.Destroy(this.gameObject);
        GameObject.Destroy(this.exploder);
    }

    void PlayExploderEffect()
    {
        this.exploder.transform.position = this.transform.position;
        this.exploder.SetActive(true);
        ParticleSystem p = this.exploder.GetComponent<ParticleSystem>();
        p.startColor = c;
        p.Play();
        Light light = this.exploder.GetComponentInChildren<Light>();
        light.color = c;

        GameObject.DestroyObject(this.exploder, 0.8f);
    }
}
