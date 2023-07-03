using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScoreAndHealthManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI healthCounter;
    [SerializeField] private Slider healthBar;
    [SerializeField] private TextMeshProUGUI coinsCounter;
    [SerializeField] private Slider coinsBar;

    private int _coins;
    private int _maxHealth = 10; 
    private int _health;

    public int coins { get => _coins; }
    public int health { get => _health; }
    public int maxHealth { get => _maxHealth; }

    void Start()
    {
        _coins = 0;
        _health = _maxHealth;

        healthBar.maxValue = _maxHealth;
        healthBar.value = _health;
        coinsBar.maxValue = 10;
        coinsBar.value = _coins;

        healthCounter.text = _health.ToString();
        coinsCounter.text = _coins.ToString();
    }

    public void collectCoin()
    {
        _coins++;
        coinsCounter.text = _coins.ToString();
        coinsBar.value = _coins;
        if (_coins >= coinsBar.maxValue)
            coinsBar.maxValue += 10;
    }

    public void getDamage()
    {
        _health--;
        healthBar.value = _health;
        healthCounter.text = _health.ToString();
        if (_health <= 0)
        {

        }
    }

    public void showHealth()
    {
        healthBar.value = _health;
        healthCounter.text = _health.ToString();
    }
}
