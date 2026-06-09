using UnityEngine;

[CreateAssetMenu(fileName = "NewEnemyData", menuName = "Gamedata/EnemyData")]
public class EnemyData : ScriptableObject
{
    public string enemyName = "Punk";
    public int maxHealth = 3;
    public float speed = 3f;
    public Color debugColor = Color.white;
}