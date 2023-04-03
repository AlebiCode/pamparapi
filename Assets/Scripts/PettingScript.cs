using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PettingScript : MonoBehaviour
{
    Touch touch = new Touch();
    RaycastHit2D hit = new RaycastHit2D();
    float pettingTimer;

    private void Update()
    {
        if (Input.touchCount >= 1)
        {
            touch = Input.GetTouch(0);
            if (true || hit.collider)
            {
                switch (touch.phase)
                {
                    case TouchPhase.Moved:
                        pettingTimer = 5f;
                        GameManager.instance.Love += Time.deltaTime;
                        break;
                    case TouchPhase.Stationary:
                        if (pettingTimer >= 0)
                        {
                            GameManager.instance.Love += Time.deltaTime;
                            pettingTimer -= Time.deltaTime;
                        }
                        break;
                }
            }
        }
    }

    private void FixedUpdate()
    {
        Ray ray = Camera.main.ScreenPointToRay(touch.position);
        hit = Physics2D.Raycast(ray.origin, ray.direction, 1 << 6);
    }

}
