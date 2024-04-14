using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;

public class Slash : MonoBehaviour
{
    public Transform Holder;
    public Vector3 cameraPos = new Vector3(0, 0, 0);
    public float currDistance = 5.0f;
    public float xRotate = 250.0f;
    public float yRotate = 120.0f;
    public float yMinLimit = -20.0f;
    public float yMaxLimit = 80.0f;
    public float prevDistance;
    private float x = 0.0f;
    private float y = 0.0f;

    [Header("GUI")]
    private float windowDpi;
    public ParticleSystem Effect;
    public float activationTime;
    private bool enableGUI = true;

    public Animator animObject;
    // Start is called before the first frame update
    void Start()
    {
        if (Screen.dpi < 1) windowDpi = 1;
        if (Screen.dpi < 200) windowDpi = 1;
        else windowDpi = Screen.dpi / 200f;
        var angles = transform.eulerAngles;
        x = angles.y;
        y = angles.x;
        PlayEffectWithAnimation();
        animObject.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            enableGUI = !enableGUI;
        }
    }

    private void OnGUI()
    {
        if (enableGUI)
        {
            if (GUI.Button(new Rect(120 * windowDpi, 5 * windowDpi, 110 * windowDpi, 35 * windowDpi), "Play Effect"))
            {
                PlayEffectWithAnimation();
            }
        }
    }
    public bool useAnimation = false;

    void PlayEffectWithAnimation()
    {
        Invoke("Activate", activationTime);
        if (useAnimation == true)
        {
            animObject.SetTrigger("Play");
        }
    }

    void Activate()
    {
        Effect.Play();
    }

    void LateUpdate()
    {
        if (currDistance < 2)
        {
            currDistance = 2;
        }
        currDistance -= Input.GetAxis("Mouse ScrollWheel") * 2;
        if (Holder&&(Input.GetMouseButton(0) || Input.GetMouseButton(1)))
        {
            var pos = Input.mousePosition;
            float dpiScale = 1;
            if (Screen.dpi < 1) dpiScale = 1;
            if (Screen.dpi < 200) dpiScale = 1;
            else dpiScale = Screen.dpi / 200f;
            if (pos.x < 380 * dpiScale && Screen.height - pos.y < 250 * dpiScale) return;
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            x += (float)(Input.GetAxis("Mouse X") * xRotate * 0.02);
            y -= (float)(Input.GetAxis("Mouse Y") * yRotate * 0.02);
            y = ClampAngle(y, yMinLimit, yMaxLimit);
            var rotation = Quaternion.Euler(y, x, 0);
            var position = rotation * new Vector3(0, 0, -currDistance) + Holder.position + cameraPos;
            transform.rotation = rotation;
            transform.position = position;
        }
        else
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        if (prevDistance != currDistance)
        {
            prevDistance = currDistance;
            var rot = Quaternion.Euler(y, x, 0);
            var po = rot * new Vector3(0, 0, -currDistance) + Holder.position + cameraPos;
            transform.rotation = rot;
            transform.position = po;

        }
    }
    static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360)
        {
            angle += 360;
        }
        if (angle > 360)
        {
            angle -= 360;
        }
        return Mathf.Clamp(angle, min, max);
    }
}
