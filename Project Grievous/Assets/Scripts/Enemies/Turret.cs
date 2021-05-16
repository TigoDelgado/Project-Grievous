using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour
{
    TurretsManager m_TurretsManager;

    void Start()
    {
        m_TurretsManager = GameObject.FindObjectOfType<TurretsManager>();
        if (!m_TurretsManager.Turrets.Contains(this))
        {
            m_TurretsManager.Turrets.Add(this);
        }
    }

    void OnDestroy()
    {
        if (m_TurretsManager)
        {
            m_TurretsManager.Turrets.Remove(this);
        }
    }


}
