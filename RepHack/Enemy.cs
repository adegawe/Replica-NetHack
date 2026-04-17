namespace RepHack;
class Enemy : Entity
{
    public enum EnemyType { Goblin , Slime , Dragon};
    public EnemyType enemyType;

    public virtual void Act(){}
}

class Slime : Enemy
{
    public Slime()
    {
        MaxHp = 10;
        Attack = 3;
        Symbol = 'S';
    }
    public override void Act()
    {
        //단순추격
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
    public override void Act()
    {
        //공격 후 도주
    }
}