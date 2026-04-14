namespace RepHack;
class Enemy : Entity
{
    public enum EnemyType { Goblin , Slime , Dragon};
    public EnemyType enemyType;

}

class Slime : Enemy
{
    public Slime()
    {
        MaxHp = 10;
        Attack = 3;
        Symbol = 'S';
    }
}

class Goblin : Enemy
{
    public Goblin()
    {
        MaxHp = 8;
        Attack = 4;
        Symbol = 'G';
    }
}