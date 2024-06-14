using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class EnemyHP : MonoBehaviour
{
    public Image enemyText;
    public Image enemyHP_Fill;
    public Image enemyHP_BG;

    private PlayerController player;
    private EnemyController enemy;

    private bool healthShown = false;
    private bool nameShown = false;
    private float nameTime = 0f;

    private void Awake()
    {
        player = FindObjectOfType<PlayerController>();
        enemy = gameObject.transform.parent.transform.parent.gameObject.GetComponent<EnemyController>();
    }

    private void Update()
    {
        BillBoard();

        if (nameShown && !healthShown)
        {
            nameTime += Time.deltaTime;
            if (nameTime >= 0.5f)
            {
                HideName();
            }
        }
    }

    public void BillBoard()
    {
        transform.LookAt(transform.position + Camera.main.transform.rotation * -Vector3.back,
            Camera.main.transform.rotation * -Vector3.down);
    }

    public void ShowHealth()
    {
        healthShown = true;
        enemyHP_Fill.gameObject.SetActive(true);
        enemyHP_BG.gameObject.SetActive(true);
    }

    public void HideHealth()
    {
        healthShown = false;
        enemyHP_Fill.gameObject.SetActive(false);
        enemyHP_BG.gameObject.SetActive(false);
    }

    public void ShowName()
    {
        nameShown = true;
        nameTime = 0f;
        enemyText.gameObject.SetActive(true);
    }

    public void HideName()
    {
        if (healthShown) return;
        nameShown = false;
        enemyText.gameObject.SetActive(false);
    }

    public void UpdateHealthBar()
    {
        enemyHP_Fill.fillAmount = (float)enemy.CurrentHealth / enemy.MaxHealth;
    }

    public void ResetBar()
    {
        HideHealth();
        HideName();
    }
}
